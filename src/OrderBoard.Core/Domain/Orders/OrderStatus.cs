namespace OrderBoard.Core.Domain.Orders;

public enum OrderStatus
{
    New = 0,
    Preparing = 1,
    Ready = 2,
    Completed = 3,
    Canceled = 4
}