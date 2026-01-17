using Microsoft.AspNetCore.SignalR;

namespace OrderBoard.Api.Hubs;

public sealed class OrdersHub : Hub
{
    public Task JoinBoard(string boardId)
        => Groups.AddToGroupAsync(Context.ConnectionId, GroupName(boardId));

    public Task LeaveBoard(string boardId)
        => Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(boardId));

    private static string GroupName(string boardId)
        => $"board:{boardId.Trim().ToLowerInvariant()}";
}