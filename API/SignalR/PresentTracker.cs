namespace API.SignalR;

public class PresentTracker
{
    private static readonly Dictionary<string, List<string>> onlineUsers = new Dictionary<string, List<string>>();


    public Task<bool> UserConnected(string username, string connectionId)
    {

        bool isOnline = false;
        lock (onlineUsers)
        {
            if (onlineUsers.ContainsKey(username))
            {
                onlineUsers[username].Add(connectionId);
            }

            else
            {
                onlineUsers.Add(username, new List<string> { connectionId });
                isOnline = true;
            }

           
        }
             return Task.FromResult(isOnline);

    }

    public  Task<bool> UserDisconnected(string username, string connectionId)
    {
        bool isOffline = false;
        lock (onlineUsers)
        {
            if (!onlineUsers.ContainsKey(username)) return Task.FromResult(isOffline);
            
                onlineUsers[username].Remove(connectionId);
            
         

            if (onlineUsers[username].Count == 0)
            {
                onlineUsers.Remove(username);
                isOffline = true;
            }
        }

        return Task.FromResult(isOffline);
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

    public static async Task<string[]> GetConnectionForUser(string username)
    {
        string[] connectionIds;
        lock (onlineUsers)
        {
            connectionIds = onlineUsers.GetValueOrDefault(username).ToArray();
        }

        return await Task.FromResult(connectionIds);
    }


    public async Task<string[]> GetConnectionForUsers(string[] usernames)
    {
        List<string> connectionIds = new List<string>();
        lock (onlineUsers)
        {
            foreach (var username in usernames)
            {
                var connectionId = onlineUsers.GetValueOrDefault(username);
                if (connectionId != null)
                {
                    connectionIds.AddRange(connectionId);
                }
            }
        }

        return await Task.FromResult(connectionIds.ToArray());
    }


   

   

}
