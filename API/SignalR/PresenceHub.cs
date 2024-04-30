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
            await _tracker.UserConnected(Context.User.GetUserName(), Context.ConnectionId)   ;
           
            
                await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUserName());
            

            var currentUsers = await _tracker.GetOnLineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await _tracker.UserDisconnected(Context.User.GetUserName(), Context.ConnectionId);
                
            
                await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUserName());

            var currentUsers = await _tracker.GetOnLineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);           

            await base.OnDisconnectedAsync(exception);
        }
    }
}


