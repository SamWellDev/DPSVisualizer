using Microsoft.AspNetCore.SignalR;

namespace TwitchFighter.API.Hubs;

public class GameHub : Hub
{
    private readonly ILogger<GameHub> _logger;
    
    public GameHub(ILogger<GameHub> logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Join the game room for a specific Twitch user
    /// </summary>
    public async Task JoinGame(string twitchId, string role = "viewer")
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, twitchId);
        _logger.LogInformation("User {TwitchId} joined game hub as {Role}", twitchId, role);
        await Clients.Caller.SendAsync("Joined", new { TwitchId = twitchId, ConnectionId = Context.ConnectionId, Role = role });
    }
    
    /// <summary>
    /// Dashboard broadcasts full game state to all overlays in the same room
    /// </summary>
    public async Task BroadcastGameState(string twitchId, object gameState)
    {
        // Send to all clients in the group EXCEPT the sender (dashboard)
        await Clients.OthersInGroup(twitchId).SendAsync("GameStateUpdate", gameState);
    }
    
    /// <summary>
    /// Dashboard broadcasts a damage number to display
    /// </summary>
    public async Task BroadcastDamage(string twitchId, object damageData)
    {
        await Clients.OthersInGroup(twitchId).SendAsync("DamageDealt", damageData);
    }
    
    /// <summary>
    /// Dashboard broadcasts a buff notification
    /// </summary>
    public async Task BroadcastBuff(string twitchId, object buffData)
    {
        await Clients.OthersInGroup(twitchId).SendAsync("BuffApplied", buffData);
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

