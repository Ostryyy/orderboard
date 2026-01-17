using Microsoft.AspNetCore.Mvc;
using OrderBoard.Core.Abstractions;
using OrderBoard.Core.Contracts.Orders;
using OrderBoard.Core.Domain.Orders;
using Microsoft.AspNetCore.SignalR;
using OrderBoard.Api.Hubs;
using OrderBoard.Api.Realtime;

namespace OrderBoard.Api.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController(IOrderRepository repo, IHubContext<OrdersHub> hub) : ControllerBase
{
    private readonly IOrderRepository _repo = repo;
    private readonly IHubContext<OrdersHub> _hub = hub;

    [HttpPost]
    public async Task<ActionResult<OrderResponse>> Create([FromBody] CreateOrderRequest request, CancellationToken ct)
    {
        var items = request.Items.Select(i => new OrderItem(i.Name, i.Quantity)).ToList();
        var boardId = string.IsNullOrWhiteSpace(request.BoardId) ? "main" : request.BoardId;
        var order = new Order(Guid.NewGuid(), request.CustomerName, boardId, items);

        await _repo.AddAsync(order, ct);

        var response = Map(order);

        await _hub.Clients.Group(GroupName(order.BoardId))
            .SendAsync(OrderEvents.OrderCreated, response, ct);

        return CreatedAtAction(nameof(GetById), new { id = order.Id }, response);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderResponse>>> Get([FromQuery] bool active = true, CancellationToken ct = default)
    {
        var orders = active
            ? await _repo.GetActiveAsync(ct)
            : throw new NotSupportedException("Only active=true is supported for now.");

        return Ok(orders.Select(Map).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderResponse>> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var order = await _repo.GetByIdAsync(id, ct);
        if (order is null) return NotFound();

        return Ok(Map(order));
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<OrderResponse>> UpdateStatus([FromRoute] Guid id, [FromBody] UpdateOrderStatusRequest request, CancellationToken ct)
    {
        var order = await _repo.GetByIdAsync(id, ct);
        if (order is null) return NotFound();

        order.ChangeStatus(request.Status);
        await _repo.UpdateAsync(order, ct);

        var response = Map(order);

        await _hub.Clients.Group(GroupName(order.BoardId))
            .SendAsync(OrderEvents.OrderUpdated, response, ct);

        return Ok(response);
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<ActionResult<OrderResponse>> Cancel([FromRoute] Guid id, CancellationToken ct)
    {
        var order = await _repo.GetByIdAsync(id, ct);
        if (order is null) return NotFound();

        order.Cancel();
        await _repo.UpdateAsync(order, ct);

        var response = Map(order);

        await _hub.Clients.Group(GroupName(order.BoardId))
            .SendAsync(OrderEvents.OrderCanceled, response, ct);

        return Ok(response);
    }

    private static OrderResponse Map(Order order) =>
        new(
            order.Id,
            order.CustomerName,
            order.BoardId,
            order.Status,
            order.CreatedAt,
            order.Items.Select(i => new OrderItemDto(i.Name, i.Quantity)).ToList()
        );
    
    private static string GroupName(string boardId)
        => $"board:{boardId.Trim().ToLowerInvariant()}";
}