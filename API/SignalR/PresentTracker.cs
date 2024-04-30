using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

public class PresentTracker
{
    private static readonly Dictionary<string, List<string>> onlineUsers = new Dictionary<string, List<string>>();


    public async Task UserConnected(string username, string connectionId)
    {
        lock (onlineUsers)
        {
            if (!onlineUsers.ContainsKey(username))
            {
                onlineUsers[username].Add(connectionId);
            }

            else
            {
                onlineUsers.Add(username, new List<string> { connectionId });
            }

           
        }

         await Task.CompletedTask;

    }

    public async Task UserDisconnected(string username, string connectionId)
    {
        lock (onlineUsers)
        {
            if (!onlineUsers.ContainsKey(username))
            {
                onlineUsers[username].Remove(connectionId);
            }

            if (onlineUsers[username].Count == 0)
            {
                onlineUsers.Remove(username);
            }
        }

        await Task.CompletedTask;
    }


  public async Task<string[]> GetOnLineUsers()
    {
        string[] users;
        lock (onlineUsers)
        {
            users = onlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
        }

       return await Task.FromResult(users);
    }

    public async Task<string[]> GetConnectionForUser(string username)
    {
        string[] connectionIds;
        lock (onlineUsers)
        {
            connectionIds = onlineUsers.GetValueOrDefault(username).ToArray();
        }

        return await Task.FromResult(connectionIds);
    }


   

   

}
