using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatData.Hubs;

[Authorize]
public class ChatHub : Hub
{
    [Authorize]
    public override async Task OnConnectedAsync()
    {
        var connectionId = Context.ConnectionId;
        var userId = Context.User!.FindFirstValue(ClaimTypes.NameIdentifier);

        UserConnectionIdService.ConnectionIds.Add(new(Guid.Parse(userId), connectionId));
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;
        var item = UserConnectionIdService.ConnectionIds.First(c => c.Item2 == connectionId);
        UserConnectionIdService.ConnectionIds.Remove(item);
    }
}

public static class UserConnectionIdService
{
    public static List<Tuple<Guid, string>> ConnectionIds = new();
}
