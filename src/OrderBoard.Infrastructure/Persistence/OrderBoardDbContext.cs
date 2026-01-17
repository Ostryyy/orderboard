using Microsoft.EntityFrameworkCore;
using OrderBoard.Core.Domain.Orders;

namespace OrderBoard.Infrastructure.Persistence;

public sealed class OrderBoardDbContext(DbContextOptions<OrderBoardDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderBoardDbContext).Assembly);
    }
}
