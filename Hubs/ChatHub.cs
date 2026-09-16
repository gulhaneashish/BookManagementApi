using BookStoreApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using BookStoreApi.Data;
using BookStoreApi.Models;
using Microsoft.EntityFrameworkCore;
namespace YourProject.Hubs;

[Authorize(AuthenticationSchemes = "Bearer")]
public class ChatHub : Hub
{

    private readonly OnlineUsersService _onlineUsersService;
    private readonly BookStoreDbContext _context;


    public ChatHub(
     BookStoreDbContext context,
     OnlineUsersService onlineUsersService)
    {
        _context = context;

        _onlineUsersService =
            onlineUsersService;
    }

    public async Task JoinRoom(
        string roomName,
        string user)
    {
        // Check whether the connection is already in a room
        if (Context.Items.TryGetValue(
                "CurrentRoom",
                out var currentRoom))
        {
            if (currentRoom is string oldRoom &&
                !string.Equals(
                    oldRoom,
                    roomName,
                    StringComparison.OrdinalIgnoreCase))
            {
                await Groups.RemoveFromGroupAsync(
                    Context.ConnectionId,
                    oldRoom
                );
            }
        }

        // Add connection to the new room
        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            roomName
        );

        // Store current room for this connection
        Context.Items["CurrentRoom"] = roomName;

        // Notify other users in the room
        await Clients.OthersInGroup(roomName)
            .SendAsync(
                "ReceiveMessage",
                "System",
                $"{user} joined the room."
            );
    }

    public async Task SendMessage(
        string roomName,
        string user,
        string message)
    {
        await Clients.Group(roomName)
            .SendAsync(
                "ReceiveMessage",
                user,
                message
            );
    }

    public async Task LeaveRoom(
        string roomName)
    {
        await Groups.RemoveFromGroupAsync(
            Context.ConnectionId,
            roomName
        );

        Context.Items.Remove("CurrentRoom");
    }

    // Temporary method to verify JWT user identity
    public Task<string?> GetMyUserId()
    {
        return Task.FromResult(
            Context.UserIdentifier
        );
    }

    // Private message
    public async Task SendPrivateMessage(
      string receiverUserId,
      string message)
    {
        var senderUserId =
            Context.UserIdentifier;

        if (string.IsNullOrEmpty(senderUserId))
        {
            throw new HubException(
                "User is not authenticated.");
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new HubException(
                "Message cannot be empty.");
        }

        // 1. Create message
        var chatMessage = new ChatMessage
        {
            SenderUserId = senderUserId,
            ReceiverUserId = receiverUserId,
            Message = message,
            SentAt = DateTime.UtcNow,
            IsRead = false
        };

        // 2. Save to database
        _context.ChatMessages.Add(chatMessage);

        await _context.SaveChangesAsync();

        // 3. Send real-time message
        await Clients.User(receiverUserId)
            .SendAsync(
                "ReceivePrivateMessage",
                senderUserId,
                message
            );

        // 4. Send notification
        await Clients.User(receiverUserId)
            .SendAsync(
                "ReceiveNotification",
                new
                {
                    type = "PrivateMessage",
                    fromUserId = senderUserId,
                    message =
                        $"New message from User {senderUserId}"
                }
            );
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;

        if (userId != null)
        {
            _onlineUsersService.AddConnection(
                userId,
                Context.ConnectionId
            );

            var onlineUsers =
                _onlineUsersService
                    .GetOnlineUsers();

            await Clients.All.SendAsync(
                "OnlineUsersUpdated",
                onlineUsers
            );
        }

        Console.WriteLine(
            $"User {userId} connected. " +
            $"ConnectionId: {Context.ConnectionId}"
        );

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(
    Exception? exception)
    {
        var userId = Context.UserIdentifier;

        if (userId != null)
        {
            _onlineUsersService.RemoveConnection(
                userId,
                Context.ConnectionId
            );

            var onlineUsers =
                _onlineUsersService
                    .GetOnlineUsers();

            await Clients.All.SendAsync(
                "OnlineUsersUpdated",
                onlineUsers
            );
        }

        Console.WriteLine(
            $"User {userId} disconnected. " +
            $"ConnectionId: {Context.ConnectionId}"
        );

        await base.OnDisconnectedAsync(exception);
    }
    public Task<List<string>> GetOnlineUsers()
    {
        return Task.FromResult(
            _onlineUsersService.GetOnlineUsers()
        );
    }

    public async Task UserTyping(
    string receiverUserId)
    {
        var senderUserId =
            Context.UserIdentifier;

        if (string.IsNullOrEmpty(senderUserId))
        {
            return;
        }

        await Clients.User(receiverUserId)
            .SendAsync(
                "UserTyping",
                senderUserId);
    }

    public async Task MarkMessagesAsRead(
    string senderUserId)
    {
        var receiverUserId =
            Context.UserIdentifier;

        if (string.IsNullOrEmpty(receiverUserId))
        {
            return;
        }

        var messages =
            await _context.ChatMessages
                .Where(x =>
                    x.SenderUserId == senderUserId &&
                    x.ReceiverUserId == receiverUserId &&
                    !x.IsRead)
                .ToListAsync();

        foreach (var message in messages)
        {
            message.IsRead = true;
        }

        await _context.SaveChangesAsync();
    }
}