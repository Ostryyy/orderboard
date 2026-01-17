using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderBoard.Core.Abstractions;
using OrderBoard.Infrastructure.Persistence;
using OrderBoard.Infrastructure.Repositories;

namespace OrderBoard.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var connStr = config.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException("Missing connection string: SqlServer");

        services.AddDbContext<OrderBoardDbContext>(opt =>
            opt.UseSqlServer(connStr));

        services.AddScoped<IOrderRepository, EfOrderRepository>();

        return services;
    }
}