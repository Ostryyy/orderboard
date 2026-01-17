using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderBoard.Infrastructure.Persistence;

namespace OrderBoard.Api.IntegrationTests.Infrastructure;

public sealed class ApiFixture : IAsyncLifetime
{
    public TestDatabase Db { get; } = new();
    public CustomWebApplicationFactory Factory { get; private set; } = default!;
    public HttpClient Client { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        await Db.CreateAsync();

        Factory = new CustomWebApplicationFactory(Db.ConnectionString);
        Client = Factory.CreateClient();

        using var scope = Factory.Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<OrderBoardDbContext>();
        await ctx.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        Client.Dispose();
        Factory.Dispose();
        await Db.DropAsync();
    }
}
