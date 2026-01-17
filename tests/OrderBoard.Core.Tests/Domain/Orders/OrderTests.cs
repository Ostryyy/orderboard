using OrderBoard.Core.Domain.Orders;
using OrderBoard.Core.Exceptions;

namespace OrderBoard.Core.Tests.Domain.Orders;

public class OrderTests
{
    [Fact]
    public void New_order_starts_with_status_new()
    {
        var order = CreateSampleOrder();
        Assert.Equal(OrderStatus.New, order.Status);
    }

    [Fact]
    public void Can_move_through_valid_status_flow()
    {
        var order = CreateSampleOrder();

        order.ChangeStatus(OrderStatus.Preparing);
        order.ChangeStatus(OrderStatus.Ready);
        order.ChangeStatus(OrderStatus.Completed);

        Assert.Equal(OrderStatus.Completed, order.Status);
    }

    [Fact]
    public void Invalid_status_change_should_throw()
    {
        var order = CreateSampleOrder();

        Assert.Throws<DomainException>(() => order.ChangeStatus(OrderStatus.Ready));
    }

    [Fact]
    public void Cancel_sets_status_to_canceled()
    {
        var order = CreateSampleOrder();
        order.Cancel();

        Assert.Equal(OrderStatus.Canceled, order.Status);
    }

    [Fact]
    public void Cannot_cancel_completed_order()
    {
        var order = CreateSampleOrder();
        order.ChangeStatus(OrderStatus.Preparing);
        order.ChangeStatus(OrderStatus.Ready);
        order.ChangeStatus(OrderStatus.Completed);

        Assert.Throws<DomainException>(() => order.Cancel());
    }

    private static Order CreateSampleOrder()
    {
        return new Order(
            Guid.NewGuid(),
            "John",
            "main",
            [new OrderItem("Burger", 2)]
        );
    }
}