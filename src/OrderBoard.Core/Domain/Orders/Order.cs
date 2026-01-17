using OrderBoard.Core.Exceptions;

namespace OrderBoard.Core.Domain.Orders;

public sealed class Order
{
    public Guid Id { get; }
    public string CustomerName { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; }

    private readonly List<OrderItem> _items = [];
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public string BoardId { get; private set; }

    public Order(Guid id, string customerName, string boardId, IEnumerable<OrderItem> items)
    {
        if (id == Guid.Empty) throw new DomainException("Order id cannot be empty.");
        if (string.IsNullOrWhiteSpace(customerName)) throw new DomainException("Customer name cannot be empty.");

        if (string.IsNullOrWhiteSpace(boardId))
            throw new DomainException("BoardId cannot be empty.");
        BoardId = boardId.Trim();

        var itemList = items?.ToList() ?? throw new DomainException("Order items cannot be null.");
        if (itemList.Count == 0) throw new DomainException("Order must contain at least one item.");

        Id = id;
        CustomerName = customerName.Trim();
        CreatedAt = DateTimeOffset.UtcNow;
        Status = OrderStatus.New;

        _items.AddRange(itemList);
    }

    public void ChangeStatus(OrderStatus nextStatus)
    {
        OrderStatusTransitionRules.EnsureCanTransition(Status, nextStatus);
        Status = nextStatus;
    }

    public void Cancel()
    {
        if (Status is OrderStatus.Completed or OrderStatus.Canceled)
            throw new DomainException($"Cannot cancel an order in status: {Status}");

        Status = OrderStatus.Canceled;
    }
}