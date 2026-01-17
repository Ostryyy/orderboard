using System.Net;
using System.Net.Http.Json;
using OrderBoard.Api.IntegrationTests.Infrastructure;
using OrderBoard.Core.Contracts.Orders;

namespace OrderBoard.Api.IntegrationTests;

public sealed class OrdersEndpointsTests(ApiFixture fixture) : IClassFixture<ApiFixture>
{
    private readonly HttpClient _client = fixture.Client;

    [Fact]
    public async Task CreateOrder_ShouldPersistAndAppearInActiveList()
    {
        var req = new CreateOrderRequest(
            CustomerName: "John Smith",
            Items: [new OrderItemRequest("Burger", 2), new OrderItemRequest("Cola", 1)],
            Note: "No onions",
            BoardId: "main"
        );

        var create = await _client.PostAsJsonAsync("/api/orders", req);
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var created = await create.Content.ReadFromJsonAsync<OrderResponse>(Json.Options);
        Assert.NotNull(created);
        Assert.Equal("New", created!.Status.ToString());

        var active = await _client.GetFromJsonAsync<List<OrderResponse>>("/api/orders?active=true", Json.Options);

        Assert.NotNull(active);
        Assert.Contains(active!, o => o.Id == created.Id);
    }

    [Fact]
    public async Task InvalidStatusTransition_ShouldReturn400ProblemDetails()
    {
        var req = new CreateOrderRequest(
            "John",
            [new OrderItemRequest("Burger", 1)],
            null,
            "main"
        );

        var create = await _client.PostAsJsonAsync("/api/orders", req);
        var created = await create.Content.ReadFromJsonAsync<OrderResponse>(Json.Options);
        Assert.NotNull(created);

        var patch = await _client.PatchAsJsonAsync($"/api/orders/{created!.Id}/status", new UpdateOrderStatusRequest(Core.Domain.Orders.OrderStatus.Ready), Json.Options);

        Assert.Equal(HttpStatusCode.BadRequest, patch.StatusCode);
    }
}
