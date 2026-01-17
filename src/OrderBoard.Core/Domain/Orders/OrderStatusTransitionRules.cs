using OrderBoard.Core.Exceptions;

namespace OrderBoard.Core.Domain.Orders;

public static class OrderStatusTransitionRules
{
    private static readonly Dictionary<OrderStatus, HashSet<OrderStatus>> Allowed = new()
    {
        [OrderStatus.New] = [OrderStatus.Preparing, OrderStatus.Canceled],
        [OrderStatus.Preparing] = [OrderStatus.Ready, OrderStatus.Canceled],
        [OrderStatus.Ready] = [OrderStatus.Completed, OrderStatus.Canceled],
        [OrderStatus.Completed] = [],
        [OrderStatus.Canceled] = []
    };

    public static void EnsureCanTransition(OrderStatus from, OrderStatus to)
    {
        if (!Allowed.TryGetValue(from, out var allowedTargets) || !allowedTargets.Contains(to))
            throw new DomainException($"Invalid status transition: {from} -> {to}");
    }
}
