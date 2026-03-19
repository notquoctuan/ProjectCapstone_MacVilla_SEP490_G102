using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Application.Services;

public class QuotationService : IQuotationService
{
    private readonly IQuotationRepository _quotationRepository;
    private readonly IRfqRepository _rfqRepository;
    private readonly IOrderRepository _orderRepository;

    public QuotationService(
        IQuotationRepository quotationRepository, 
        IRfqRepository rfqRepository,
        IOrderRepository orderRepository)
    {
        _quotationRepository = quotationRepository;
        _rfqRepository = rfqRepository;
        _orderRepository = orderRepository;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    // ════════════════════════════════════════════════════════
    // ADMIN / SALE
    // ════════════════════════════════════════════════════════

    public async Task<QuotationDetailResponse> CreateQuotationAsync(
        CreateQuotationRequest request, long createdByUserId)
    {
        // 1. Kiểm tra RFQ tồn tại và đang ở trạng thái hợp lệ
        var rfq = await _rfqRepository.GetRfqDetailByIdAsync(request.RfqId);
        if (rfq == null)
            throw new KeyNotFoundException($"Không tìm thấy RFQ #{request.RfqId}.");

        if (rfq.Status is "Closed" or "Cancelled")
            throw new InvalidOperationException(
                $"Không thể tạo báo giá cho RFQ có trạng thái '{rfq.Status}'.");

        // 2. Tự sinh mã Quotation
        var year = DateTime.UtcNow.Year;
        var seq = await _quotationRepository.GetNextSequenceAsync(year);
        var quotationCode = $"QT-{year}-{seq:D3}";

        // 3. Tính toán tài chính
        var items = BuildQuotationItems(request.Items);
        var (subTotal, discountTotal) = CalculateFinancials(items);
        var vatAmount = Math.Round(subTotal * request.VatRate / 100, 0);
        var totalAmount = subTotal - discountTotal + vatAmount;

        // 4. Tạo entity
        var quotation = new Quotation
        {
            QuotationCode = quotationCode,
            RfqId = request.RfqId,
            CreatedBy = createdByUserId,
            Status = "Draft",
            SubTotal = subTotal,
            DiscountTotal = discountTotal,
            VatRate = request.VatRate,
            VatAmount = vatAmount,
            TotalAmount = totalAmount,
            ValidUntil = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(request.ValidDays)),
            Notes = request.Notes,
            InternalNote = request.InternalNote,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            QuotationItems = items
        };

        var created = await _quotationRepository.CreateQuotationAsync(quotation);

        // 5. Tự động cập nhật RFQ sang Quoted nếu chưa phải
        if (rfq.Status == "Processing" || rfq.Status == "Pending")
        {
            rfq.Status = "Quoted";
            rfq.UpdatedAt = DateTime.UtcNow;
            await _rfqRepository.UpdateRfqAsync(rfq);
        }

        var detail = await _quotationRepository.GetQuotationDetailByIdAsync(created.QuotationId);
        return MapToDetailResponse(detail!);
    }

    public async Task<QuotationDetailResponse> UpdateQuotationAsync(
        long quotationId, UpdateQuotationRequest request)
    {
        var quotation = await _quotationRepository.GetQuotationDetailByIdAsync(quotationId);
        if (quotation == null)
            throw new KeyNotFoundException($"Không tìm thấy báo giá #{quotationId}.");

        if (quotation.Status != "Draft")
            throw new InvalidOperationException(
                "Chỉ có thể chỉnh sửa báo giá ở trạng thái Draft.");

        // Xóa items cũ và tạo lại
        quotation.QuotationItems.Clear();
        var items = BuildQuotationItems(request.Items);
        var (subTotal, discountTotal) = CalculateFinancials(items);
        var vatAmount = Math.Round(subTotal * request.VatRate / 100, 0);
        var totalAmount = subTotal - discountTotal + vatAmount;

        quotation.QuotationItems = items;
        quotation.SubTotal = subTotal;
        quotation.DiscountTotal = discountTotal;
        quotation.VatRate = request.VatRate;
        quotation.VatAmount = vatAmount;
        quotation.TotalAmount = totalAmount;
        quotation.ValidUntil = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(request.ValidDays));
        quotation.Notes = request.Notes;
        quotation.InternalNote = request.InternalNote;
        quotation.UpdatedAt = DateTime.UtcNow;

        await _quotationRepository.UpdateQuotationAsync(quotation);

        var updated = await _quotationRepository.GetQuotationDetailByIdAsync(quotationId);
        return MapToDetailResponse(updated!);
    }

    public async Task<PagedResponse<QuotationListResponse>> GetAllQuotationsAsync(QuotationFilterRequest request)
    {
        var (items, total) = await _quotationRepository.SearchQuotationsAsync(
            request.Status, request.Keyword, request.CreatedBy,
            request.StartDate, request.EndDate,
            request.PageNumber, request.PageSize);

        return new PagedResponse<QuotationListResponse>
        {
            Data = items.Select(MapToListResponse).ToList(),
            TotalCount = total,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)total / request.PageSize)
        };
    }

    public async Task<QuotationDetailResponse?> GetQuotationDetailAsync(long quotationId)
    {
        var q = await _quotationRepository.GetQuotationDetailByIdAsync(quotationId);
        return q == null ? null : MapToDetailResponse(q);
    }

    public async Task<bool> SendQuotationAsync(long quotationId)
    {
        var quotation = await _quotationRepository.GetByIdAsync(quotationId);
        if (quotation == null) return false;

        if (quotation.Status != "Draft")
            throw new InvalidOperationException(
                $"Chỉ có thể gửi báo giá đang ở trạng thái Draft. Trạng thái hiện tại: {quotation.Status}.");

        quotation.Status = "SentToCustomer";
        quotation.SentAt = DateTime.UtcNow;
        quotation.UpdatedAt = DateTime.UtcNow;
        await _quotationRepository.UpdateQuotationAsync(quotation);
        return true;
    }

    public async Task<byte[]> ExportPdfAsync(long quotationId)
    {
        var q = await _quotationRepository.GetQuotationDetailByIdAsync(quotationId);
        if (q == null)
            throw new KeyNotFoundException($"Không tìm thấy báo giá #{quotationId}.");

        return GeneratePdf(q);
    }

    // ════════════════════════════════════════════════════════
    // CUSTOMER
    // ════════════════════════════════════════════════════════

    public async Task<PagedResponse<QuotationListResponse>> GetMyQuotationsAsync(
        long userId, string? status, int pageNumber, int pageSize)
    {
        var (items, total) = await _quotationRepository.GetByRfqUserIdAsync(userId, status, pageNumber, pageSize);
        return new PagedResponse<QuotationListResponse>
        {
            Data = items.Select(MapToListResponse).ToList(),
            TotalCount = total,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)total / pageSize)
        };
    }

    public async Task<QuotationDetailResponse?> GetMyQuotationDetailAsync(long quotationId, long userId)
    {
        var q = await _quotationRepository.GetByIdAndUserIdAsync(quotationId, userId);
        return q == null ? null : MapToDetailResponse(q);
    }

    public async Task<bool> ApproveQuotationAsync(long quotationId, long userId)
    {
        var quotation = await _quotationRepository.GetByIdAndUserIdAsync(quotationId, userId);
        if (quotation == null) return false;

        if (quotation.Status != "SentToCustomer")
            throw new InvalidOperationException(
                "Chỉ có thể chấp nhận báo giá đã được gửi. " +
                $"Trạng thái hiện tại: {quotation.Status}.");

        // Kiểm tra hạn hiệu lực
        if (quotation.ValidUntil.HasValue && quotation.ValidUntil.Value < DateOnly.FromDateTime(DateTime.UtcNow))
            throw new InvalidOperationException("Báo giá này đã hết hạn hiệu lực.");

        quotation.Status = "Approved";
        quotation.UpdatedAt = DateTime.UtcNow;

        // Đóng RFQ tương ứng
        if (quotation.Rfq != null)
        {
            quotation.Rfq.Status = "Closed";
            quotation.Rfq.UpdatedAt = DateTime.UtcNow;
            await _rfqRepository.UpdateRfqAsync(quotation.Rfq);
        }

        await _quotationRepository.UpdateQuotationAsync(quotation);
        return true;
    }

    public async Task<bool> RejectQuotationAsync(long quotationId, long userId, RejectQuotationRequest request)
    {
        var quotation = await _quotationRepository.GetByIdAndUserIdAsync(quotationId, userId);
        if (quotation == null) return false;

        if (quotation.Status != "SentToCustomer")
            throw new InvalidOperationException(
                "Chỉ có thể từ chối báo giá đã được gửi. " +
                $"Trạng thái hiện tại: {quotation.Status}.");

        quotation.Status = "Rejected";
        quotation.RejectReason = request.Reason;
        quotation.UpdatedAt = DateTime.UtcNow;
        await _quotationRepository.UpdateQuotationAsync(quotation);
        return true;
    }

    // ════════════════════════════════════════════════════════
    // ADVANCED BUSINESS LOGICS (Cancel, Revise, Order Conversion, Stats)
    // ════════════════════════════════════════════════════════

    public async Task<bool> CancelQuotationAsync(long quotationId)
    {
        var quotation = await _quotationRepository.GetByIdAsync(quotationId);
        if (quotation == null) return false;

        if (quotation.Status == "Approved" || quotation.Status == "Rejected")
            throw new InvalidOperationException($"Không thể hủy báo giá đã được phản hồi. Cần thu hồi trạng thái hiện tại: {quotation.Status}");

        quotation.Status = "Cancelled";
        quotation.UpdatedAt = DateTime.UtcNow;
        await _quotationRepository.UpdateQuotationAsync(quotation);
        return true;
    }

    public async Task<QuotationDetailResponse> ReviseQuotationAsync(long quotationId, long createdByUserId)
    {
        var oldQ = await _quotationRepository.GetQuotationDetailByIdAsync(quotationId);
        if (oldQ == null) throw new KeyNotFoundException($"Không tìm thấy báo giá #{quotationId}.");

        // Clone Items
        var newItems = oldQ.QuotationItems.Select(i => new QuotationItemRequest
        {
            ProductId = i.ProductId,
            Sku = i.Sku,
            ProductName = i.ProductName ?? "",
            Quantity = i.Quantity ?? 0,
            Unit = i.Unit,
            UnitPrice = i.UnitPrice ?? 0,
            DiscountPercent = i.DiscountPercent ?? 0
        }).ToList();

        // Create new request mapping old data
        var request = new CreateQuotationRequest
        {
            RfqId = oldQ.RfqId ?? 0,
            VatRate = oldQ.VatRate ?? 10,
            ValidDays = 30, // Default for revise
            Notes = oldQ.Notes,
            InternalNote = "Bản điều chỉnh từ báo giá: " + oldQ.QuotationCode,
            Items = newItems
        };

        // Reuse CreateLogic
        return await CreateQuotationAsync(request, createdByUserId);
    }

    public async Task<long> CreateOrderFromQuotationAsync(long quotationId, long createdByUserId)
    {
        var q = await _quotationRepository.GetQuotationDetailByIdAsync(quotationId);
        if (q == null) throw new KeyNotFoundException($"Không tìm thấy báo giá #{quotationId}.");

        if (q.Status != "Approved")
            throw new InvalidOperationException($"Chỉ có thể tạo đơn từ báo giá đã được khách hàng duyệt (Approved). Hiện tại: {q.Status}");

        var orderItems = q.QuotationItems.Select(qi => new OrderItem
        {
            ProductId = qi.ProductId,
            Quantity = qi.Quantity,
            UnitPrice = Math.Round((qi.UnitPrice ?? 0) * (1 - (qi.DiscountPercent ?? 0)/100), 0)
        }).ToList();

        var order = new Order
        {
            UserId = q.Rfq?.UserId, // Same customer as RFQ
            TotalAmount = q.TotalAmount,
            Status = "Pending", 
            Note = $"Đơn hàng được sinh tự động từ Báo giá: {q.QuotationCode}",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            OrderItems = orderItems
        };

        var created = await _orderRepository.CreateOrderAsync(order);
        return created.OrderId;
    }

    public async Task<RfqDashboardStatsResponse> GetDashboardStatsAsync()
    {
        // Tính năng này dùng count của repository bằng pageSize=1 trick
        var (_, tRfq) = await _rfqRepository.SearchRfqsAsync(null, null, null, null, null, 1, 1);
        var (_, pRfq) = await _rfqRepository.SearchRfqsAsync("Pending", null, null, null, null, 1, 1);
        var (_, prRfq) = await _rfqRepository.SearchRfqsAsync("Processing", null, null, null, null, 1, 1);
        var (_, qRfq) = await _rfqRepository.SearchRfqsAsync("Quoted", null, null, null, null, 1, 1);

        var (_, tQT) = await _quotationRepository.SearchQuotationsAsync(null, null, null, null, null, 1, 1);
        var (_, aQT) = await _quotationRepository.SearchQuotationsAsync("Approved", null, null, null, null, 1, 1);
        var (_, rQT) = await _quotationRepository.SearchQuotationsAsync("Rejected", null, null, null, null, 1, 1);

        // Fetch just the Approved ones accurately to calculate total revenue
        // (If there are many, ideally this needs a custom aggregate in repository, but for standard scenarios, get all approved)
        // Here we just pull first 1000 since it is generic
        var (approvedList, _) = await _quotationRepository.SearchQuotationsAsync("Approved", null, null, null, null, 1, 1000);
        decimal sumRevenue = approvedList.Sum(q => q.TotalAmount ?? 0);

        return new RfqDashboardStatsResponse
        {
            TotalRfqs = tRfq,
            PendingRfqs = pRfq,
            ProcessingRfqs = prRfq,
            QuotedRfqs = qRfq,
            TotalQuotations = tQT,
            ApprovedQuotations = aQT,
            RejectedQuotations = rQT,
            TotalRevenueFromApproved = sumRevenue
        };
    }

    // ════════════════════════════════════════════════════════
    // PRIVATE HELPERS
    // ════════════════════════════════════════════════════════

    private static List<QuotationItem> BuildQuotationItems(List<QuotationItemRequest> requests)
    {
        return requests.Select(i =>
        {
            var lineTotal = Math.Round(i.Quantity * i.UnitPrice * (1 - i.DiscountPercent / 100), 0);
            return new QuotationItem
            {
                ProductId = i.ProductId,
                Sku = i.Sku,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                Unit = i.Unit,
                UnitPrice = i.UnitPrice,
                DiscountPercent = i.DiscountPercent,
                LineTotal = lineTotal
            };
        }).ToList();
    }

    private static (decimal SubTotal, decimal DiscountTotal) CalculateFinancials(List<QuotationItem> items)
    {
        decimal subTotal = 0;
        decimal discountTotal = 0;
        foreach (var item in items)
        {
            var grossLine = (item.Quantity ?? 0) * (item.UnitPrice ?? 0);
            var discountLine = Math.Round(grossLine * (item.DiscountPercent ?? 0) / 100, 0);
            subTotal += Math.Round(grossLine - discountLine, 0);
            discountTotal += discountLine;
        }
        return (subTotal, discountTotal);
    }

    private static QuotationListResponse MapToListResponse(Quotation q) => new()
    {
        QuotationId = q.QuotationId,
        QuotationCode = q.QuotationCode,
        RfqId = q.RfqId,
        RfqCode = q.Rfq?.RfqCode,
        CustomerName = q.Rfq?.CustomerName,
        CompanyName = q.Rfq?.CompanyName,
        ProjectName = q.Rfq?.ProjectName,
        Status = q.Status,
        TotalAmount = q.TotalAmount,
        ValidUntil = q.ValidUntil,
        CreatedByName = q.CreatedByUser?.FullName,
        CreatedAt = q.CreatedAt,
        SentAt = q.SentAt
    };

    internal static QuotationDetailResponse MapToDetailResponse(Quotation q) => new()
    {
        QuotationId = q.QuotationId,
        QuotationCode = q.QuotationCode,
        Status = q.Status,
        RfqId = q.RfqId,
        RfqCode = q.Rfq?.RfqCode,
        CustomerName = q.Rfq?.CustomerName,
        CompanyName = q.Rfq?.CompanyName,
        Phone = q.Rfq?.Phone,
        Email = q.Rfq?.Email,
        Address = q.Rfq?.Address,
        ProjectName = q.Rfq?.ProjectName,
        CreatedBy = q.CreatedBy,
        CreatedByName = q.CreatedByUser?.FullName,
        Financial = new QuotationFinancialSummary
        {
            SubTotal = q.SubTotal ?? 0,
            DiscountTotal = q.DiscountTotal ?? 0,
            VatRate = q.VatRate ?? 0,
            VatAmount = q.VatAmount ?? 0,
            TotalAmount = q.TotalAmount ?? 0
        },
        ValidUntil = q.ValidUntil,
        Notes = q.Notes,
        InternalNote = q.InternalNote,
        RejectReason = q.RejectReason,
        CreatedAt = q.CreatedAt,
        UpdatedAt = q.UpdatedAt,
        SentAt = q.SentAt,
        Items = q.QuotationItems?.Select(i => new QuotationItemResponse
        {
            QuotationItemId = i.QuotationItemId,
            ProductId = i.ProductId,
            Sku = i.Sku,
            ProductName = i.ProductName,
            ProductImageUrl = i.Product?.ProductImages?
                .OrderByDescending(img => img.IsMain)
                .FirstOrDefault()?.ImageUrl,
            Quantity = i.Quantity,
            Unit = i.Unit,
            UnitPrice = i.UnitPrice,
            DiscountPercent = i.DiscountPercent,
            LineTotal = i.LineTotal
        }).ToList() ?? new()
    };

    // ════════════════════════════════════════════════════════
    // PDF GENERATION — QuestPDF
    // ════════════════════════════════════════════════════════

    private static byte[] GeneratePdf(Quotation q)
    {
        var detail = MapToDetailResponse(q);

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30, Unit.Point);
                page.DefaultTextStyle(x => x.FontSize(10));

                // ── HEADER ──────────────────────────────────────
                page.Header().Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(inner =>
                        {
                            inner.Item().Text("MACVILLA").Bold().FontSize(22).FontColor("#1a3c6d");
                            inner.Item().Text("Công ty CP MacVilla").FontSize(9).FontColor(Colors.Grey.Darken2);
                        });
                        row.RelativeItem().AlignRight().Column(inner =>
                        {
                            inner.Item().Text("BÁO GIÁ").Bold().FontSize(18).FontColor("#1a3c6d");
                            inner.Item().Text($"Mã: {detail.QuotationCode}").Bold().FontSize(11);
                            inner.Item().Text($"Ngày: {detail.CreatedAt:dd/MM/yyyy}").FontSize(9);
                            inner.Item().Text($"Hiệu lực đến: {detail.ValidUntil:dd/MM/yyyy}").FontSize(9).FontColor(Colors.Red.Medium);
                        });
                    });
                    col.Item().PaddingTop(4).LineHorizontal(1.5f).LineColor("#1a3c6d");
                });

                // ── CONTENT ─────────────────────────────────────
                page.Content().PaddingTop(10).Column(col =>
                {
                    // Thông tin khách hàng & người tạo
                    col.Item().Row(row =>
                    {
                        // Khách hàng
                        row.RelativeItem().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(8).Column(inner =>
                        {
                            inner.Item().Text("KÍNH GỬI:").Bold().FontSize(9).FontColor(Colors.Grey.Darken2);
                            inner.Item().PaddingTop(2).Text(detail.CustomerName ?? "").Bold().FontSize(11);
                            if (!string.IsNullOrWhiteSpace(detail.CompanyName))
                                inner.Item().Text(detail.CompanyName).FontSize(10);
                            if (!string.IsNullOrWhiteSpace(detail.Phone))
                                inner.Item().Text($"ĐT: {detail.Phone}").FontSize(9);
                            if (!string.IsNullOrWhiteSpace(detail.Email))
                                inner.Item().Text($"Email: {detail.Email}").FontSize(9);
                            if (!string.IsNullOrWhiteSpace(detail.Address))
                                inner.Item().Text(detail.Address).FontSize(9);
                        });

                        row.ConstantItem(10);

                        // Thông tin dự án
                        row.RelativeItem().Border(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(8).Column(inner =>
                        {
                            inner.Item().Text("THÔNG TIN DỰ ÁN").Bold().FontSize(9).FontColor(Colors.Grey.Darken2);
                            if (!string.IsNullOrWhiteSpace(detail.ProjectName))
                            {
                                inner.Item().PaddingTop(2).Text(detail.ProjectName).Bold().FontSize(10);
                            }
                            inner.Item().PaddingTop(4).Text($"Người báo giá: {detail.CreatedByName ?? "MacVilla"}").FontSize(9);
                            inner.Item().Text($"Mã RFQ: {detail.RfqCode ?? "-"}").FontSize(9);
                        });
                    });

                    col.Item().PaddingTop(14).Text("BẢNG BÁO GIÁ SẢN PHẨM").Bold().FontSize(12).FontColor("#1a3c6d");
                    col.Item().PaddingTop(6);

                    // ── Bảng sản phẩm ───────────────────────────────
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.ConstantColumn(25);   // STT
                            cols.RelativeColumn(3);    // Tên SP
                            cols.ConstantColumn(50);   // SKU
                            cols.ConstantColumn(40);   // SL
                            cols.ConstantColumn(40);   // ĐVT
                            cols.ConstantColumn(80);   // Đơn giá
                            cols.ConstantColumn(45);   // CK%
                            cols.ConstantColumn(85);   // Thành tiền
                        });

                        // Header
                        static IContainer HeaderCell(IContainer container) =>
                            container.Background("#1a3c6d").PaddingVertical(5).PaddingHorizontal(4);

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderCell).AlignCenter().Text("STT").Bold().FontColor(Colors.White).FontSize(9);
                            header.Cell().Element(HeaderCell).Text("Tên sản phẩm / SKU").Bold().FontColor(Colors.White).FontSize(9);
                            header.Cell().Element(HeaderCell).AlignCenter().Text("SKU").Bold().FontColor(Colors.White).FontSize(9);
                            header.Cell().Element(HeaderCell).AlignCenter().Text("SL").Bold().FontColor(Colors.White).FontSize(9);
                            header.Cell().Element(HeaderCell).AlignCenter().Text("ĐVT").Bold().FontColor(Colors.White).FontSize(9);
                            header.Cell().Element(HeaderCell).AlignRight().Text("Đơn giá (đ)").Bold().FontColor(Colors.White).FontSize(9);
                            header.Cell().Element(HeaderCell).AlignCenter().Text("CK%").Bold().FontColor(Colors.White).FontSize(9);
                            header.Cell().Element(HeaderCell).AlignRight().Text("Thành tiền (đ)").Bold().FontColor(Colors.White).FontSize(9);
                        });

                        // Rows
                        for (int idx = 0; idx < detail.Items.Count; idx++)
                        {
                            var item = detail.Items[idx];
                            string bg = idx % 2 == 0 ? "#FFFFFF" : "#f5f7fa";

                            static IContainer DataCell(IContainer c, string bg) =>
                                c.Background(bg).BorderBottom(0.3f).BorderColor(Colors.Grey.Lighten2)
                                 .PaddingVertical(5).PaddingHorizontal(4);

                            table.Cell().Element(c => DataCell(c, bg)).AlignCenter().Text((idx + 1).ToString()).FontSize(9);
                            table.Cell().Element(c => DataCell(c, bg)).Text(item.ProductName ?? "").FontSize(9);
                            table.Cell().Element(c => DataCell(c, bg)).AlignCenter().Text(item.Sku ?? "-").FontSize(9);
                            table.Cell().Element(c => DataCell(c, bg)).AlignCenter().Text(item.Quantity?.ToString() ?? "0").FontSize(9);
                            table.Cell().Element(c => DataCell(c, bg)).AlignCenter().Text(item.Unit ?? "-").FontSize(9);
                            table.Cell().Element(c => DataCell(c, bg)).AlignRight().Text(FormatCurrency(item.UnitPrice)).FontSize(9);
                            table.Cell().Element(c => DataCell(c, bg)).AlignCenter().Text($"{item.DiscountPercent:0}%").FontSize(9);
                            table.Cell().Element(c => DataCell(c, bg)).AlignRight().Text(FormatCurrency(item.LineTotal)).Bold().FontSize(9);
                        }
                    });

                    // ── Tổng kết tài chính ───────────────────────────
                    col.Item().PaddingTop(10).AlignRight().Width(260).Table(tbl =>
                    {
                        tbl.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn();
                            cols.ConstantColumn(120);
                        });

                        static IContainer SumRow(IContainer c) =>
                            c.PaddingVertical(3).PaddingHorizontal(6);

                        tbl.Cell().Element(SumRow).Text("Tạm tính:").FontSize(10);
                        tbl.Cell().Element(SumRow).AlignRight().Text(FormatCurrency(detail.Financial.SubTotal)).FontSize(10);

                        tbl.Cell().Element(SumRow).Text("Tổng chiết khấu:").FontColor(Colors.Orange.Medium).FontSize(10);
                        tbl.Cell().Element(SumRow).AlignRight().Text($"-{FormatCurrency(detail.Financial.DiscountTotal)}").FontColor(Colors.Orange.Medium).FontSize(10);

                        tbl.Cell().Element(SumRow).Text($"Thuế VAT ({detail.Financial.VatRate:0}%):").FontSize(10);
                        tbl.Cell().Element(SumRow).AlignRight().Text(FormatCurrency(detail.Financial.VatAmount)).FontSize(10);

                        tbl.Cell().Background("#1a3c6d").Padding(6).Text("TỔNG CỘNG:").Bold().FontColor(Colors.White).FontSize(12);
                        tbl.Cell().Background("#1a3c6d").Padding(6).AlignRight().Text(FormatCurrency(detail.Financial.TotalAmount)).Bold().FontColor(Colors.White).FontSize(12);
                    });

                    // ── Ghi chú ─────────────────────────────────────
                    if (!string.IsNullOrWhiteSpace(detail.Notes))
                    {
                        col.Item().PaddingTop(16).Column(inner =>
                        {
                            inner.Item().Text("Điều kiện & ghi chú:").Bold().FontSize(10).FontColor(Colors.Grey.Darken2);
                            inner.Item().PaddingTop(4).Text(detail.Notes).FontSize(9).FontColor(Colors.Grey.Darken1);
                        });
                    }
                });

                // ── FOOTER ──────────────────────────────────────────
                page.Footer().AlignCenter().Text(txt =>
                {
                    txt.Span("MacVilla — ").FontSize(8).FontColor(Colors.Grey.Medium);
                    txt.Span("Cảm ơn quý khách đã tin tưởng!").FontSize(8).FontColor(Colors.Grey.Medium);
                    txt.Span("  |  Trang ").FontSize(8).FontColor(Colors.Grey.Medium);
                    txt.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Medium);
                    txt.Span(" / ").FontSize(8).FontColor(Colors.Grey.Medium);
                    txt.TotalPages().FontSize(8).FontColor(Colors.Grey.Medium);
                });
            });
        }).GeneratePdf();
    }

    private static string FormatCurrency(decimal? value) =>
        value.HasValue ? $"{value.Value:N0}đ" : "0đ";
}
