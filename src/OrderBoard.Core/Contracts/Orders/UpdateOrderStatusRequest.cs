using OrderBoard.Core.Domain.Orders;

namespace OrderBoard.Core.Contracts.Orders;

public sealed record UpdateOrderStatusRequest(OrderStatus Status);