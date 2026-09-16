using System.Collections.Concurrent;

namespace BookStoreApi.Services;

public class OnlineUsersService
{
    private readonly ConcurrentDictionary<
        string,
        HashSet<string>
    > _users = new();

    public void AddConnection(
        string userId,
        string connectionId)
    {
        var connections =
            _users.GetOrAdd(
                userId,
                _ => new HashSet<string>());

        lock (connections)
        {
            connections.Add(connectionId);
        }
    }

    public void RemoveConnection(
        string userId,
        string connectionId)
    {
        if (!_users.TryGetValue(
                userId,
                out var connections))
        {
            return;
        }

        lock (connections)
        {
            connections.Remove(connectionId);

            if (connections.Count == 0)
            {
                _users.TryRemove(
                    userId,
                    out _);
            }
        }
    }

    public List<string> GetOnlineUsers()
    {
        return _users.Keys.ToList();
    }
}