using Microsoft.EntityFrameworkCore;
using OrderBoard.Core.Abstractions;
using OrderBoard.Core.Domain.Orders;
using OrderBoard.Infrastructure.Persistence;

namespace OrderBoard.Infrastructure.Repositories;

public sealed class EfOrderRepository(OrderBoardDbContext db) : IOrderRepository
{
    private readonly OrderBoardDbContext _db = db;

    public async Task AddAsync(Order order, CancellationToken ct)
    {
        _db.Orders.Add(order);
        await _db.SaveChangesAsync(ct);
    }

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken ct)
        => _db.Orders
            .Include("_items")
            .FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task<IReadOnlyList<Order>> GetActiveAsync(CancellationToken ct)
    {
        return await _db.Orders
            .Include("_items")
            .Where(o => o.Status != OrderStatus.Completed && o.Status != OrderStatus.Canceled)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task UpdateAsync(Order order, CancellationToken ct)
    {
        _db.Orders.Update(order);
        await _db.SaveChangesAsync(ct);
    }
}