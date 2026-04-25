using BE_API.Database;
using BE_API.Domain;
using BE_API.Dto.Admin;
using BE_API.Dto.Common;
using BE_API.Entities;
using BE_API.Service.IService;
using Microsoft.EntityFrameworkCore;

namespace BE_API.Service;

public class AdminReturnService(BeContext db) : IAdminReturnService
{
    public async Task<PagedResultDto<AdminReturnListItemDto>> GetPagedAsync(
        int page,
        int pageSize,
        string? status = null,
        string? type = null,
        int? customerId = null,
        int? orderId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = db.ReturnExchangeTickets
            .AsNoTracking()
            .Include(t => t.Customer)
            .Include(t => t.Order)
            .Include(t => t.ManagerApproved)
            .Include(t => t.Items)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(t => t.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(type))
        {
            query = query.Where(t => t.Type == type);
        }

        if (customerId.HasValue)
        {
            query = query.Where(t => t.CustomerId == customerId.Value);
        }

        if (orderId.HasValue)
        {
            query = query.Where(t => t.OrderId == orderId.Value);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(t => t.CreatedAt >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            var endDate = toDate.Value.Date.AddDays(1);
            query = query.Where(t => t.CreatedAt < endDate);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower().Trim();
            query = query.Where(t =>
                t.TicketNumber.ToLower().Contains(searchLower) ||
                t.Customer.FullName.ToLower().Contains(searchLower) ||
                (t.Customer.Phone != null && t.Customer.Phone.Contains(searchLower)) ||
                t.Order.OrderCode.ToLower().Contains(searchLower));
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new AdminReturnListItemDto
            {
                Id = t.Id,
                TicketNumber = t.TicketNumber,
                Type = t.Type,
                Status = t.Status,
                Reason = t.Reason,
                RefundAmount = t.RefundAmount,
                ItemCount = t.Items.Count,
                CreatedAt = t.CreatedAt,
                ApprovedAt = t.ApprovedAt,
                CompletedAt = t.CompletedAt,
                CustomerId = t.CustomerId,
                CustomerName = t.Customer.FullName,
                CustomerPhone = t.Customer.Phone,
                CustomerEmail = t.Customer.Email,
                OrderId = t.OrderId,
                OrderCode = t.Order.OrderCode,
                ManagerIdApproved = t.ManagerIdApproved,
                ManagerName = t.ManagerApproved != null ? t.ManagerApproved.FullName : null
            })
            .ToListAsync(cancellationToken);

        return new PagedResultDto<AdminReturnListItemDto>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<AdminReturnDetailDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var ticket = await GetTicketWithDetailsAsync(t => t.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Không tìm thấy phiếu đổi/trả với ID {id}");

        return MapToDetailDto(ticket);
    }

    public async Task<AdminReturnDetailDto> GetByTicketNumberAsync(string ticketNumber, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(ticketNumber))
            throw new ArgumentException("Mã phiếu đổi/trả không được để trống");

        var numberLower = ticketNumber.Trim().ToLower();
        var ticket = await GetTicketWithDetailsAsync(t => t.TicketNumber.ToLower() == numberLower, cancellationToken)
            ?? throw new KeyNotFoundException($"Không tìm thấy phiếu đổi/trả với mã {ticketNumber}");

        return MapToDetailDto(ticket);
    }

    public async Task<AdminReturnDetailDto> CreateAsync(
        AdminReturnCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        var order = await db.CustomerOrders
            .AsNoTracking()
            .Include(o => o.Customer)
            .Include(o => o.Items)
                .ThenInclude(i => i.Variant)
                    .ThenInclude(v => v.Product)
            .FirstOrDefaultAsync(o => o.Id == dto.OrderId, cancellationToken)
            ?? throw new KeyNotFoundException("Không tìm thấy đơn hàng");

        if (order.OrderStatus != OrderStatuses.Delivered && order.OrderStatus != OrderStatuses.Completed)
            throw new InvalidOperationException("Chỉ có thể tạo phiếu đổi/trả cho đơn hàng đã giao hoặc hoàn thành");

        if (!ReturnTypes.IsValid(dto.Type))
            throw new ArgumentException($"Loại '{dto.Type}' không hợp lệ. Phải là: {string.Join(", ", ReturnTypes.All)}");

        if (dto.Items == null || dto.Items.Count == 0)
            throw new ArgumentException("Phải có ít nhất 1 sản phẩm cần đổi/trả");

        var orderItemVariantIds = order.Items.Select(i => i.VariantId).ToHashSet();
        foreach (var item in dto.Items)
        {
            if (!orderItemVariantIds.Contains(item.VariantIdReturned))
                throw new ArgumentException($"Sản phẩm VariantId {item.VariantIdReturned} không có trong đơn hàng");

            var orderItem = order.Items.First(i => i.VariantId == item.VariantIdReturned);
            if (item.Quantity <= 0 || item.Quantity > orderItem.Quantity)
                throw new ArgumentException($"Số lượng trả cho VariantId {item.VariantIdReturned} không hợp lệ (tối đa {orderItem.Quantity})");

            if (dto.Type == ReturnTypes.Exchange && item.VariantIdExchanged.HasValue)
            {
                var exchangeVariant = await db.ProductVariants.AsNoTracking()
                    .FirstOrDefaultAsync(v => v.Id == item.VariantIdExchanged.Value, cancellationToken)
                    ?? throw new KeyNotFoundException($"Không tìm thấy sản phẩm đổi VariantId {item.VariantIdExchanged.Value}");
            }
        }

        var ticketNumber = await ReturnTicketCodes.GenerateUniqueAsync(db.ReturnExchangeTickets, cancellationToken);
        var now = DateTime.UtcNow;

        var ticket = new ReturnExchangeTicket
        {
            TicketNumber = ticketNumber,
            OrderId = dto.OrderId,
            CustomerId = order.CustomerId,
            Type = dto.Type,
            Reason = dto.Reason,
            CustomerNote = dto.CustomerNote,
            InternalNote = dto.InternalNote,
            Status = ReturnTicketStatuses.Requested,
            RefundAmount = 0,
            CreatedAt = now
        };

        await db.ReturnExchangeTickets.AddAsync(ticket, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        foreach (var itemDto in dto.Items)
        {
            var returnItem = new ReturnItem
            {
                TicketId = ticket.Id,
                VariantIdReturned = itemDto.VariantIdReturned,
                VariantIdExchanged = dto.Type == ReturnTypes.Exchange ? itemDto.VariantIdExchanged : null,
                Quantity = itemDto.Quantity,
                InventoryAction = null
            };
            await db.ReturnItems.AddAsync(returnItem, cancellationToken);
        }

        await db.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(ticket.Id, cancellationToken);
    }

    public async Task<AdminReturnDetailDto> ApproveAsync(
        int id,
        int managerId,
        AdminReturnApproveDto dto,
        CancellationToken cancellationToken = default)
    {
        var ticket = await db.ReturnExchangeTickets
            .Include(t => t.Items)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Không tìm thấy phiếu đổi/trả với ID {id}");

        if (!ReturnTicketStatuses.CanApprove(ticket.Status))
            throw new InvalidOperationException($"Phiếu ở trạng thái '{ticket.Status}' không thể duyệt");

        var manager = await db.AppUsers.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == managerId, cancellationToken)
            ?? throw new KeyNotFoundException("Không tìm thấy Manager");

        ticket.Status = ReturnTicketStatuses.Approved;
        ticket.ManagerIdApproved = managerId;
        ticket.ApprovedAt = DateTime.UtcNow;
        ticket.RefundAmount = dto.RefundAmount;

        if (!string.IsNullOrWhiteSpace(dto.InternalNote))
        {
            ticket.InternalNote = string.IsNullOrWhiteSpace(ticket.InternalNote)
                ? dto.InternalNote
                : $"{ticket.InternalNote}\n[Duyệt] {dto.InternalNote}";
        }

        await db.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<AdminReturnDetailDto> RejectAsync(
        int id,
        int managerId,
        AdminReturnRejectDto dto,
        CancellationToken cancellationToken = default)
    {
        var ticket = await db.ReturnExchangeTickets
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Không tìm thấy phiếu đổi/trả với ID {id}");

        if (!ReturnTicketStatuses.CanReject(ticket.Status))
            throw new InvalidOperationException($"Phiếu ở trạng thái '{ticket.Status}' không thể từ chối");

        var manager = await db.AppUsers.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == managerId, cancellationToken)
            ?? throw new KeyNotFoundException("Không tìm thấy Manager");

        ticket.Status = ReturnTicketStatuses.Rejected;
        ticket.ManagerIdApproved = managerId;
        ticket.ApprovedAt = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(dto.RejectReason))
        {
            ticket.InternalNote = string.IsNullOrWhiteSpace(ticket.InternalNote)
                ? $"[Từ chối] {dto.RejectReason}"
                : $"{ticket.InternalNote}\n[Từ chối] {dto.RejectReason}";
        }

        await db.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<AdminReturnDetailDto> CompleteAsync(
        int id,
        int stockManagerId,
        AdminReturnCompleteDto dto,
        CancellationToken cancellationToken = default)
    {
        var ticket = await db.ReturnExchangeTickets
            .Include(t => t.Items)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Không tìm thấy phiếu đổi/trả với ID {id}");

        if (!ReturnTicketStatuses.CanComplete(ticket.Status))
            throw new InvalidOperationException($"Phiếu ở trạng thái '{ticket.Status}' không thể hoàn thành. Cần ở trạng thái '{ReturnTicketStatuses.ItemsReceived}'");

        var stockManager = await db.AppUsers.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == stockManagerId, cancellationToken)
            ?? throw new KeyNotFoundException("Không tìm thấy Stock Manager");

        foreach (var itemDto in dto.Items)
        {
            var returnItem = ticket.Items.FirstOrDefault(i => i.Id == itemDto.ReturnItemId)
                ?? throw new KeyNotFoundException($"Không tìm thấy ReturnItem với ID {itemDto.ReturnItemId}");

            if (!InventoryActions.IsValid(itemDto.InventoryAction))
                throw new ArgumentException($"Hành động '{itemDto.InventoryAction}' không hợp lệ. Phải là: {string.Join(", ", InventoryActions.All)}");

            returnItem.InventoryAction = itemDto.InventoryAction;

            if (itemDto.InventoryAction == InventoryActions.Restock)
            {
                var inventory = await db.Inventories
                    .FirstOrDefaultAsync(i => i.VariantId == returnItem.VariantIdReturned, cancellationToken);

                if (inventory != null)
                {
                    inventory.QuantityOnHand += returnItem.Quantity;
                }

                var transaction = new InventoryTransaction
                {
                    VariantId = returnItem.VariantIdReturned,
                    TransactionType = TransactionTypes.In,
                    Quantity = returnItem.Quantity,
                    ReferenceType = "ReturnExchangeTicket",
                    ReferenceId = ticket.TicketNumber,
                    Notes = $"Nhập lại kho từ phiếu đổi/trả {ticket.TicketNumber}",
                    ManagerIdApproved = stockManagerId,
                    Timestamp = DateTime.UtcNow
                };
                await db.InventoryTransactions.AddAsync(transaction, cancellationToken);
            }
        }

        ticket.Status = ReturnTicketStatuses.Completed;
        ticket.StockManagerId = stockManagerId;
        ticket.CompletedAt = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(dto.InternalNote))
        {
            ticket.InternalNote = string.IsNullOrWhiteSpace(ticket.InternalNote)
                ? $"[Hoàn thành] {dto.InternalNote}"
                : $"{ticket.InternalNote}\n[Hoàn thành] {dto.InternalNote}";
        }

        await db.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    private async Task<ReturnExchangeTicket?> GetTicketWithDetailsAsync(
        System.Linq.Expressions.Expression<Func<ReturnExchangeTicket, bool>> predicate,
        CancellationToken cancellationToken)
    {
        return await db.ReturnExchangeTickets
            .AsNoTracking()
            .Include(t => t.Customer)
            .Include(t => t.Order)
            .Include(t => t.ManagerApproved)
            .Include(t => t.StockManager)
            .Include(t => t.Items)
                .ThenInclude(i => i.VariantReturned)
                    .ThenInclude(v => v.Product)
            .Include(t => t.Items)
                .ThenInclude(i => i.VariantExchanged)
                    .ThenInclude(v => v!.Product)
            .FirstOrDefaultAsync(predicate, cancellationToken);
    }

    private AdminReturnDetailDto MapToDetailDto(ReturnExchangeTicket ticket)
    {
        return new AdminReturnDetailDto
        {
            Id = ticket.Id,
            TicketNumber = ticket.TicketNumber,
            Type = ticket.Type,
            Status = ticket.Status,
            Reason = ticket.Reason,
            CustomerNote = ticket.CustomerNote,
            InternalNote = ticket.InternalNote,
            RefundAmount = ticket.RefundAmount,
            CreatedAt = ticket.CreatedAt,
            ApprovedAt = ticket.ApprovedAt,
            CompletedAt = ticket.CompletedAt,
            Customer = new AdminReturnCustomerDto
            {
                Id = ticket.Customer.Id,
                FullName = ticket.Customer.FullName,
                Email = ticket.Customer.Email,
                Phone = ticket.Customer.Phone,
                CustomerType = ticket.Customer.CustomerType,
                CompanyName = ticket.Customer.CompanyName
            },
            Order = new AdminReturnOrderDto
            {
                Id = ticket.Order.Id,
                OrderCode = ticket.Order.OrderCode,
                CreatedAt = ticket.Order.CreatedAt,
                OrderStatus = ticket.Order.OrderStatus,
                PaymentStatus = ticket.Order.PaymentStatus,
                PayableTotal = ticket.Order.PayableTotal
            },
            ManagerIdApproved = ticket.ManagerIdApproved,
            ManagerName = ticket.ManagerApproved?.FullName,
            StockManagerId = ticket.StockManagerId,
            StockManagerName = ticket.StockManager?.FullName,
            Items = ticket.Items.Select(i => new AdminReturnItemDto
            {
                Id = i.Id,
                VariantIdReturned = i.VariantIdReturned,
                SkuReturned = i.VariantReturned.Sku,
                VariantNameReturned = i.VariantReturned.VariantName,
                ProductNameReturned = i.VariantReturned.Product.Name,
                ImageUrlReturned = i.VariantReturned.ImageUrl,
                UnitPriceReturned = i.VariantReturned.RetailPrice,
                VariantIdExchanged = i.VariantIdExchanged,
                SkuExchanged = i.VariantExchanged?.Sku,
                VariantNameExchanged = i.VariantExchanged?.VariantName,
                ProductNameExchanged = i.VariantExchanged?.Product.Name,
                ImageUrlExchanged = i.VariantExchanged?.ImageUrl,
                Quantity = i.Quantity,
                InventoryAction = i.InventoryAction
            }).ToList()
        };
    }
}
