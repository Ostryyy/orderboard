using OrderBoard.Core.Domain.Orders;
using OrderBoard.Core.Exceptions;

namespace OrderBoard.Core.Tests.Domain.Orders;

public class OrderStatusTransitionRulesTests
{
    [Theory]
    [InlineData(OrderStatus.New, OrderStatus.Preparing)]
    [InlineData(OrderStatus.New, OrderStatus.Canceled)]
    [InlineData(OrderStatus.Preparing, OrderStatus.Ready)]
    [InlineData(OrderStatus.Ready, OrderStatus.Completed)]
    [InlineData(OrderStatus.Ready, OrderStatus.Canceled)]
    public void Allowed_transition_should_pass(OrderStatus from, OrderStatus to)
    {
        OrderStatusTransitionRules.EnsureCanTransition(from, to);
    }

    [Theory]
    [InlineData(OrderStatus.New, OrderStatus.Ready)]
    [InlineData(OrderStatus.Completed, OrderStatus.Preparing)]
    [InlineData(OrderStatus.Canceled, OrderStatus.New)]
    public void Invalid_transition_should_throw(OrderStatus from, OrderStatus to)
    {
        var ex = Assert.Throws<DomainException>(() =>
            OrderStatusTransitionRules.EnsureCanTransition(from, to));

        Assert.Contains("Invalid status transition", ex.Message);
    }
}