using OrderBoard.Core.Exceptions;

namespace OrderBoard.Core.Domain.Orders;

public sealed class OrderItem
{
    public string Name { get; }
    public int Quantity { get; }

    public OrderItem(string name, int quantity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Item name cannot be empty.");

        if (quantity <= 0)
            throw new DomainException("Item quantity must be greater than zero.");

        Name = name.Trim();
        Quantity = quantity;
    }
}
