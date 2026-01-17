using Microsoft.Extensions.DependencyInjection;
using OrderBoard.Core.Abstractions;
using OrderBoard.Infrastructure.Repositories;

namespace OrderBoard.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
        return services;
    }
}