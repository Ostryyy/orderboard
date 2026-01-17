using Microsoft.AspNetCore.Mvc;
using OrderBoard.Core.Abstractions;
using OrderBoard.Core.Contracts.Orders;
using OrderBoard.Core.Domain.Orders;

namespace OrderBoard.Api.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly IOrderRepository _repo;

    public OrdersController(IOrderRepository repo)
    {
        _repo = repo;
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponse>> Create([FromBody] CreateOrderRequest request, CancellationToken ct)
    {
        var items = request.Items.Select(i => new OrderItem(i.Name, i.Quantity)).ToList();
        var order = new Order(Guid.NewGuid(), request.CustomerName, items);

        await _repo.AddAsync(order, ct);

        var response = Map(order);
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

        return Ok(Map(order));
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<ActionResult<OrderResponse>> Cancel([FromRoute] Guid id, CancellationToken ct)
    {
        var order = await _repo.GetByIdAsync(id, ct);
        if (order is null) return NotFound();

        order.Cancel();
        await _repo.UpdateAsync(order, ct);

        return Ok(Map(order));
    }

    private static OrderResponse Map(Order order) =>
        new(
            order.Id,
            order.CustomerName,
            order.Status,
            order.CreatedAt,
            order.Items.Select(i => new OrderItemDto(i.Name, i.Quantity)).ToList()
        );
}