namespace OrderBoard.Core.Contracts.Orders;

public sealed record OrderItemRequest(string Name, int Quantity);