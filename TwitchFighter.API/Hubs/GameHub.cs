using Microsoft.AspNetCore.SignalR;

namespace TwitchFighter.API.Hubs;

public class GameHub : Hub
{
    private readonly ILogger<GameHub> _logger;
    
    public GameHub(ILogger<GameHub> logger)
    {
        _logger = logger;
    }
    
    public async Task JoinGame(string twitchId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, twitchId);
        _logger.LogInformation("User {TwitchId} joined game hub", twitchId);
        await Clients.Caller.SendAsync("Joined", new { TwitchId = twitchId, ConnectionId = Context.ConnectionId });
    }
    
    public async Task LeaveGame(string twitchId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, twitchId);
        _logger.LogInformation("User {TwitchId} left game hub", twitchId);
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}
