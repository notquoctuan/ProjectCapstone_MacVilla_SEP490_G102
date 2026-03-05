# Order Management System Implementation

## Overview
This document describes the order management functionality implemented for the MacVilla admin system following clean architecture principles.

## Architecture Layers

### 1. Domain Layer (`Domain/`)
- **Interfaces:**
  - `IOrderRepository.cs` - Order repository interface
  - `IInventoryRepository.cs` - Inventory repository interface

### 2. Application Layer (`Application/`)
- **Interfaces:**
  - `IOrderService.cs` - Order service interface
- **Services:**
  - `OrderService.cs` - Order business logic with inventory management
- **DTOs:**
  - `OrderDetailResponse.cs` - Detailed order information
  - `OrderListResponse.cs` - Order list item
  - `OrderSearchRequest.cs` - Search/filter parameters
  - `UpdateOrderStatusRequest.cs` - Status update request
  - `OrderTrackingResponse.cs` - Order tracking information

### 3. Persistence Layer (`Persistence/`)
- **Repositories:**
  - `OrderRepository.cs` - Order data access
  - `InventoryRepository.cs` - Inventory management with history tracking

### 4. Presentation Layer (`Presentation/`)
- **Controllers:**
  - `OrderController.cs` - Admin order management API endpoints

## Features Implemented

### 1. View Order Detail (`GET /api/admin/order/{id}`)
Returns comprehensive order information including:
- Order basic info (ID, status, total amount, created date)
- Customer information (name, email, phone)
- Order items with product details (name, image, category, quantity, price)
- Payment information (method, status, paid date)
- Shipping information (address, method, fee, delivery date)

### 2. Order Tracking (`GET /api/admin/order/{id}/tracking`)
Provides order tracking timeline showing:
- Order placement status
- Payment received status
- Processing status
- Shipping status
- Delivery status
- Cancellation status (if applicable)

### 3. Search/List Orders (`GET /api/admin/order`)
Supports filtering and pagination:
- Filter by status (Pending, Processing, Shipped, Completed, Cancelled)
- Filter by customer (userId)
- Filter by date range (startDate, endDate)
- Pagination (pageNumber, pageSize)

### 4. Update Order Status (`PUT /api/admin/order/{id}/status`)
Updates order status with validation:
- Valid statuses: Pending, Processing, Shipped, Completed, Cancelled
- Status transition validation (prevents invalid transitions)
- **Automatic inventory management:**
  - When status changes to "Processing": Reduces inventory for all order items
  - When status changes from "Processing"/"Shipped" to "Cancelled": Restores inventory
  - Tracks inventory changes in InventoryHistory table

### 5. Cancel Order (`POST /api/admin/order/{id}/cancel`)
Cancels an order:
- Prevents cancellation of completed orders
- Restores inventory if order was Processing or Shipped
- Updates order status to "Cancelled"

## Order Status Workflow

```
Pending → Processing → Shipped → Completed
   ↓         ↓           ↓
Cancelled  Cancelled  Cancelled
```

### Valid Status Transitions:
- **Pending** → Processing, Cancelled
- **Processing** → Shipped, Cancelled
- **Shipped** → Completed, Cancelled
- **Completed** → (Final status, no transitions allowed)
- **Cancelled** → (Final status, no transitions allowed)

## Inventory Management

The system automatically manages inventory when order status changes:

1. **Order Processing**: When an order moves to "Processing" status:
   - Inventory is reduced for each product in the order
   - Inventory history is recorded with reason "Order {orderId} - Status changed to Processing"
   - Validates sufficient stock before processing

2. **Order Cancellation**: When an order is cancelled:
   - If order was Processing or Shipped, inventory is restored
   - Inventory history is recorded with reason "Order {orderId} - Order cancelled"

3. **Inventory History**: All inventory changes are tracked in the `InventoryHistory` table with:
   - Change quantity (positive for restores, negative for reductions)
   - Reason for the change
   - Timestamp

## API Endpoints Summary

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/admin/order` | List orders with filters and pagination |
| GET | `/api/admin/order/{id}` | Get order detail |
| GET | `/api/admin/order/{id}/tracking` | Get order tracking information |
| PUT | `/api/admin/order/{id}/status` | Update order status |
| POST | `/api/admin/order/{id}/cancel` | Cancel order |

## Data Flow

### Order Detail Flow:
1. Controller receives request → `OrderController.GetOrderDetail()`
2. Service layer → `OrderService.GetOrderDetailAsync()`
3. Repository layer → `OrderRepository.GetOrderDetailByIdAsync()`
4. Database query with includes (User, OrderItems, Products, Payments, Shippings)
5. Mapping to DTO → `OrderDetailResponse`
6. Return to client

### Status Update Flow:
1. Controller receives request → `OrderController.UpdateOrderStatus()`
2. Service validates status transition → `OrderService.UpdateOrderStatusAsync()`
3. Service handles inventory → `OrderService.HandleInventoryOnStatusChange()`
4. Inventory repository updates stock → `InventoryRepository.ReduceInventoryAsync()` or `RestoreInventoryAsync()`
5. Inventory history recorded → `InventoryRepository.AddInventoryHistoryAsync()`
6. Order status updated → `OrderRepository.UpdateOrderAsync()`
7. Return success response

## Error Handling

The system includes comprehensive error handling:
- **Invalid Status**: Returns 400 Bad Request with error message
- **Invalid Transition**: Returns 400 Bad Request explaining valid transitions
- **Order Not Found**: Returns 404 Not Found
- **Insufficient Inventory**: Throws exception preventing order processing
- **Cannot Cancel Completed Order**: Returns 400 Bad Request

## Best Practices Implemented

1. **Clean Architecture**: Separation of concerns across layers
2. **Repository Pattern**: Data access abstraction
3. **DTO Pattern**: Data transfer objects for API responses
4. **Dependency Injection**: Services registered in Program.cs
5. **Inventory Tracking**: All inventory changes are audited
6. **Status Validation**: Prevents invalid state transitions
7. **Comprehensive Error Handling**: Clear error messages for all scenarios
8. **Eager Loading**: Efficient data loading with Include() for related entities

## Testing Recommendations

1. Test order status transitions
2. Test inventory reduction/restoration
3. Test order cancellation scenarios
4. Test search and filtering
5. Test pagination
6. Test error cases (invalid status, insufficient inventory, etc.)

## Future Enhancements

1. Add order status history table for better tracking
2. Add email notifications on status changes
3. Add order notes/comments functionality
4. Add bulk status update capability
5. Add order export functionality
6. Add order analytics and reporting
