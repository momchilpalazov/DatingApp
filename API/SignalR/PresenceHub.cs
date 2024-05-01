using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
        public class PresenceHub : Hub
    {
        private readonly PresentTracker _tracker;
        public PresenceHub(PresentTracker tracker)
        {
            _tracker = tracker;
        }

        public override async Task OnConnectedAsync()
        {
            var isOnline = await _tracker.UserConnected(Context.User.GetUserName(), Context.ConnectionId);
            if (isOnline)
            {
               await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUserName());
           }
                                         

            var currentUsers = await _tracker.GetOnLineUsers();
            await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var isOffline= await _tracker.UserDisconnected(Context.User.GetUserName(), Context.ConnectionId);
            if (isOffline)
            {
                await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUserName());

            }             

            await base.OnDisconnectedAsync(exception);
        }
    }
}


