namespace OrderBoard.Core.Contracts.Orders;

public sealed record CreateOrderRequest(string CustomerName, List<OrderItemRequest> Items, string? Note);