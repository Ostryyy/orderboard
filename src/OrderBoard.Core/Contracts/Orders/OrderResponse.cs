using OrderBoard.Core.Domain.Orders;

namespace OrderBoard.Core.Contracts.Orders;

public sealed record OrderResponse(
    Guid Id,
    string CustomerName,
    string BoardId,
    OrderStatus Status,
    DateTimeOffset CreatedAt,
    IReadOnlyCollection<OrderItemDto> Items
);

public sealed record OrderItemDto(string Name, int Quantity);