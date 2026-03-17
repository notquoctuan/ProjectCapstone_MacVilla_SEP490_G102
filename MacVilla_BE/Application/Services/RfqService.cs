using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class RfqService : IRfqService
{
    private readonly IRfqRepository _rfqRepository;

    private static readonly string[] ValidStatuses =
        ["Pending", "Processing", "Quoted", "Closed", "Cancelled"];

    public RfqService(IRfqRepository rfqRepository)
    {
        _rfqRepository = rfqRepository;
    }

    // ════════════════════════════════════════════════════════
    // CUSTOMER
    // ════════════════════════════════════════════════════════

    public async Task<RfqDetailResponse> CreateRfqAsync(CreateRfqRequest request, long? userId)
    {
        var year = DateTime.UtcNow.Year;
        var seq = await _rfqRepository.GetNextSequenceAsync(year);
        var rfqCode = $"RFQ-{year}-{seq:D3}";

        var rfq = new Rfq
        {
            RfqCode = rfqCode,
            UserId = userId,
            CustomerName = request.CustomerName,
            CompanyName = request.CompanyName,
            Phone = request.Phone,
            Email = request.Email,
            Address = request.Address,
            ProjectName = request.ProjectName,
            ExpectedDeliveryDate = request.ExpectedDeliveryDate,
            Priority = request.Priority ?? "Normal",
            Description = request.Description,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        rfq.RfqItems = request.Items.Select(i => new RfqItem
        {
            ProductId = i.ProductId,
            Sku = i.Sku,
            ProductName = i.ProductName,
            Quantity = i.Quantity,
            Unit = i.Unit,
            Note = i.Note
        }).ToList();

        var created = await _rfqRepository.CreateRfqAsync(rfq);
        var detail = await _rfqRepository.GetRfqDetailByIdAsync(created.RfqId);
        return MapToDetailResponse(detail!);
    }

    public async Task<PagedResponse<RfqListResponse>> GetMyRfqsAsync(
        long userId, string? status, int pageNumber, int pageSize)
    {
        var (items, total) = await _rfqRepository.GetByUserIdAsync(userId, status, pageNumber, pageSize);
        return BuildPagedResponse(items, total, pageNumber, pageSize);
    }

    public async Task<RfqDetailResponse?> GetMyRfqDetailAsync(long rfqId, long userId)
    {
        var rfq = await _rfqRepository.GetByIdAndUserIdAsync(rfqId, userId);
        return rfq == null ? null : MapToDetailResponse(rfq);
    }

    public async Task<bool> CancelRfqAsync(long rfqId, long userId)
    {
        var rfq = await _rfqRepository.GetByIdAndUserIdAsync(rfqId, userId);
        if (rfq == null) return false;

        if (rfq.Status != "Pending")
            throw new InvalidOperationException(
                $"Chỉ có thể huỷ yêu cầu khi ở trạng thái Pending. Trạng thái hiện tại: {rfq.Status}.");

        rfq.Status = "Cancelled";
        rfq.UpdatedAt = DateTime.UtcNow;
        await _rfqRepository.UpdateRfqAsync(rfq);
        return true;
    }

    // ════════════════════════════════════════════════════════
    // ADMIN / SALE
    // ════════════════════════════════════════════════════════

    public async Task<PagedResponse<RfqListResponse>> GetAllRfqsAsync(RfqFilterRequest request)
    {
        var (items, total) = await _rfqRepository.SearchRfqsAsync(
            request.Status, request.Keyword, request.AssignedSaleId,
            request.StartDate, request.EndDate,
            request.PageNumber, request.PageSize);

        return BuildPagedResponse(items, total, request.PageNumber, request.PageSize);
    }

    public async Task<RfqDetailResponse?> GetRfqDetailAsync(long rfqId)
    {
        var rfq = await _rfqRepository.GetRfqDetailByIdAsync(rfqId);
        return rfq == null ? null : MapToDetailResponse(rfq);
    }

    public async Task<bool> UpdateRfqStatusAsync(long rfqId, UpdateRfqStatusRequest request)
    {
        if (!ValidStatuses.Contains(request.Status))
            throw new ArgumentException(
                $"Trạng thái '{request.Status}' không hợp lệ. Hợp lệ: {string.Join(", ", ValidStatuses)}");

        var rfq = await _rfqRepository.GetRfqDetailByIdAsync(rfqId);
        if (rfq == null) return false;

        if (!IsValidRfqTransition(rfq.Status, request.Status))
            throw new InvalidOperationException(
                $"Không thể chuyển từ '{rfq.Status}' sang '{request.Status}'.");

        rfq.Status = request.Status;
        rfq.UpdatedAt = DateTime.UtcNow;
        await _rfqRepository.UpdateRfqAsync(rfq);
        return true;
    }

    public async Task<bool> AssignSaleAsync(long rfqId, AssignSaleRequest request)
    {
        var rfq = await _rfqRepository.GetRfqDetailByIdAsync(rfqId);
        if (rfq == null) return false;

        rfq.AssignedSaleId = request.AssignedSaleId;
        rfq.UpdatedAt = DateTime.UtcNow;

        // Tự động chuyển sang Processing nếu còn Pending
        if (rfq.Status == "Pending")
            rfq.Status = "Processing";

        await _rfqRepository.UpdateRfqAsync(rfq);
        return true;
    }

    public async Task<bool> UpdateInternalNoteAsync(long rfqId, UpdateRfqInternalNoteRequest request)
    {
        var rfq = await _rfqRepository.GetRfqDetailByIdAsync(rfqId);
        if (rfq == null) return false;

        rfq.InternalNote = request.InternalNote;
        rfq.UpdatedAt = DateTime.UtcNow;
        await _rfqRepository.UpdateRfqAsync(rfq);
        return true;
    }

    // ════════════════════════════════════════════════════════
    // PRIVATE HELPERS
    // ════════════════════════════════════════════════════════

    private static bool IsValidRfqTransition(string? current, string next) =>
        current switch
        {
            "Pending" => next is "Processing" or "Cancelled",
            "Processing" => next is "Quoted" or "Closed" or "Cancelled",
            "Quoted" => next is "Closed",
            _ => false
        };

    private static PagedResponse<RfqListResponse> BuildPagedResponse(
        IEnumerable<Rfq> items, int total, int pageNumber, int pageSize)
    {
        return new PagedResponse<RfqListResponse>
        {
            Data = items.Select(MapToListResponse).ToList(),
            TotalCount = total,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)total / pageSize)
        };
    }

    private static RfqListResponse MapToListResponse(Rfq rfq) => new()
    {
        RfqId = rfq.RfqId,
        RfqCode = rfq.RfqCode,
        CustomerName = rfq.CustomerName,
        CompanyName = rfq.CompanyName,
        Phone = rfq.Phone,
        ProjectName = rfq.ProjectName,
        ItemCount = rfq.RfqItems?.Count ?? 0,
        Priority = rfq.Priority,
        Status = rfq.Status,
        AssignedSaleName = rfq.AssignedSale?.FullName,
        ExpectedDeliveryDate = rfq.ExpectedDeliveryDate,
        CreatedAt = rfq.CreatedAt,
        QuotationCount = rfq.Quotations?.Count ?? 0
    };

    internal static RfqDetailResponse MapToDetailResponse(Rfq rfq) => new()
    {
        RfqId = rfq.RfqId,
        RfqCode = rfq.RfqCode,
        UserId = rfq.UserId,
        CustomerName = rfq.CustomerName,
        CompanyName = rfq.CompanyName,
        Phone = rfq.Phone,
        Email = rfq.Email,
        Address = rfq.Address,
        ProjectName = rfq.ProjectName,
        ExpectedDeliveryDate = rfq.ExpectedDeliveryDate,
        Priority = rfq.Priority,
        Description = rfq.Description,
        InternalNote = rfq.InternalNote,
        Status = rfq.Status,
        AssignedSaleId = rfq.AssignedSaleId,
        AssignedSaleName = rfq.AssignedSale?.FullName,
        CreatedAt = rfq.CreatedAt,
        UpdatedAt = rfq.UpdatedAt,
        Items = rfq.RfqItems?.Select(i => new RfqItemResponse
        {
            RfqItemId = i.RfqItemId,
            ProductId = i.ProductId,
            Sku = i.Sku,
            ProductName = i.ProductName,
            ProductImageUrl = i.Product?.ProductImages?
                .OrderByDescending(img => img.IsMain)
                .FirstOrDefault()?.ImageUrl,
            Quantity = i.Quantity,
            Unit = i.Unit,
            Note = i.Note
        }).ToList() ?? new(),
        Quotations = rfq.Quotations?.Select(q => new QuotationSummaryInRfqDto
        {
            QuotationId = q.QuotationId,
            QuotationCode = q.QuotationCode,
            Status = q.Status,
            TotalAmount = q.TotalAmount,
            ValidUntil = q.ValidUntil,
            CreatedAt = q.CreatedAt
        }).ToList() ?? new()
    };
}
