using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcJwt.Hubs
{
    public class EchoHub : Hub
    {
        public Task Echo(string message)
        {
            var userName = Context.User?.Identity?.Name;
            Clients.Client(Context.ConnectionId).SendAsync("OnEcho", $"{userName}:{message}");
            return Task.CompletedTask;
        }

        public override Task OnConnectedAsync()
        {
            Clients.All.SendAsync("OnBroadcast", $"{Context.ConnectionId} Connected.");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Clients.All.SendAsync("OnBroadcast", $"{Context.ConnectionId} Disconnected.");
            return base.OnDisconnectedAsync(exception);
        }
    }
}
