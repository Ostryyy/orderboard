using System.Collections.Concurrent;
using OrderBoard.Core.Abstractions;
using OrderBoard.Core.Domain.Orders;

namespace OrderBoard.Infrastructure.Repositories;

public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<Guid, Order> _store = new();

    public Task AddAsync(Order order, CancellationToken ct)
    {
        _store[order.Id] = order;
        return Task.CompletedTask;
    }

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        _store.TryGetValue(id, out var order);
        return Task.FromResult(order);
    }

    public Task<IReadOnlyList<Order>> GetActiveAsync(CancellationToken ct)
    {
        var active = _store.Values
            .Where(o => o.Status is not OrderStatus.Completed and not OrderStatus.Canceled)
            .OrderByDescending(o => o.CreatedAt)
            .ToList()
            .AsReadOnly();

        return Task.FromResult((IReadOnlyList<Order>)active);
    }

    public Task UpdateAsync(Order order, CancellationToken ct)
    {
        _store[order.Id] = order;
        return Task.CompletedTask;
    }
}