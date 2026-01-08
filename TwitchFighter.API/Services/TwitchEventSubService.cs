using Microsoft.AspNetCore.SignalR;
using TwitchFighter.API.Hubs;
using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;
using TwitchLib.EventSub.Websockets;
using TwitchLib.EventSub.Websockets.Core.EventArgs;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;

namespace TwitchFighter.API.Services;

public class TwitchEventSubService : IHostedService
{
    private readonly ILogger<TwitchEventSubService> _logger;
    private readonly IHubContext<GameHub> _hubContext;
    private readonly IConfiguration _config;
    private readonly EventSubWebsocketClient _eventSubClient;
    private readonly TwitchAPI _twitchApi;
    private string? _broadcasterId;

    public TwitchEventSubService(
        ILogger<TwitchEventSubService> logger,
        IHubContext<GameHub> hubContext,
        IConfiguration config,
        EventSubWebsocketClient eventSubClient)
    {
        _logger = logger;
        _hubContext = hubContext;
        _config = config;
        _eventSubClient = eventSubClient;

        // Initialize Twitch API
        _twitchApi = new TwitchAPI();
        _twitchApi.Settings.ClientId = _config["Twitch:ClientId"];
        _twitchApi.Settings.Secret = _config["Twitch:ClientSecret"];

        // Wire up events
        _eventSubClient.WebsocketConnected += OnWebsocketConnected;
        _eventSubClient.WebsocketDisconnected += OnWebsocketDisconnected;
        _eventSubClient.WebsocketReconnected += OnWebsocketReconnected;
        _eventSubClient.ErrorOccurred += OnErrorOccurred;

        // Channel events
        _eventSubClient.ChannelFollow += OnChannelFollow;
        _eventSubClient.ChannelSubscribe += OnChannelSubscribe;
        _eventSubClient.ChannelCheer += OnChannelCheer;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting Twitch EventSub WebSocket service...");

        // Get App Access Token
        var tokenResponse = await _twitchApi.Auth.GetAccessTokenAsync();
        if (tokenResponse != null)
        {
            _twitchApi.Settings.AccessToken = tokenResponse.AccessToken;
            _logger.LogInformation("Obtained Twitch App Access Token");
        }

        await _eventSubClient.ConnectAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _eventSubClient.DisconnectAsync();
        _logger.LogInformation("Twitch EventSub WebSocket disconnected.");
    }

    public void SetBroadcasterId(string broadcasterId)
    {
        _broadcasterId = broadcasterId;
        _logger.LogInformation("Broadcaster ID set to: {BroadcasterId}", broadcasterId);
    }

    private async Task OnWebsocketConnected(object? sender, WebsocketConnectedArgs args)
    {
        _logger.LogInformation("EventSub WebSocket connected. Session ID: {SessionId}", 
            _eventSubClient.SessionId);

        if (!args.IsRequestedReconnect && !string.IsNullOrEmpty(_broadcasterId))
        {
            await SubscribeToEvents();
        }
    }

    private async Task SubscribeToEvents()
    {
        if (string.IsNullOrEmpty(_broadcasterId))
        {
            _logger.LogWarning("Cannot subscribe to events - Broadcaster ID not set");
            return;
        }

        try
        {
            var sessionId = _eventSubClient.SessionId;

            // Subscribe to follows (requires moderator:read:followers scope)
            var followCondition = new Dictionary<string, string>
            {
                { "broadcaster_user_id", _broadcasterId },
                { "moderator_user_id", _broadcasterId }
            };
            await _twitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync(
                "channel.follow", "2", followCondition, 
                EventSubTransportMethod.Websocket, sessionId);
            _logger.LogInformation("Subscribed to channel.follow events");

            // Subscribe to subscriptions
            var subCondition = new Dictionary<string, string>
            {
                { "broadcaster_user_id", _broadcasterId }
            };
            await _twitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync(
                "channel.subscribe", "1", subCondition,
                EventSubTransportMethod.Websocket, sessionId);
            _logger.LogInformation("Subscribed to channel.subscribe events");

            // Subscribe to cheers
            await _twitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync(
                "channel.cheer", "1", subCondition,
                EventSubTransportMethod.Websocket, sessionId);
            _logger.LogInformation("Subscribed to channel.cheer events");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to subscribe to EventSub events");
        }
    }

    private async Task OnWebsocketDisconnected(object? sender, EventArgs args)
    {
        _logger.LogWarning("EventSub WebSocket disconnected. Attempting to reconnect...");

        while (!await _eventSubClient.ReconnectAsync())
        {
            _logger.LogError("Websocket reconnect failed!");
            await Task.Delay(5000);
        }
    }

    private async Task OnWebsocketReconnected(object? sender, EventArgs args)
    {
        _logger.LogInformation("EventSub WebSocket reconnected");
    }

    private async Task OnErrorOccurred(object? sender, ErrorOccuredArgs args)
    {
        _logger.LogError(args.Exception, "EventSub error: {Message}", args.Message);
    }

    private async Task OnChannelFollow(object? sender, ChannelFollowArgs args)
    {
        var follow = args.Notification.Payload.Event;

        _logger.LogInformation("New follow from {UserName} on channel {BroadcasterName}",
            follow.UserName, follow.BroadcasterUserName);

        // Send to all connected clients for this broadcaster
        await _hubContext.Clients.Group(follow.BroadcasterUserId).SendAsync("TwitchEvent", new
        {
            Type = "follow",
            UserName = follow.UserName,
            UserId = follow.UserId,
            Buff = new { Type = "atk", Value = 5 }
        });
    }

    private async Task OnChannelSubscribe(object? sender, ChannelSubscribeArgs args)
    {
        var sub = args.Notification.Payload.Event;

        _logger.LogInformation("New subscription from {UserName} - Tier: {Tier}",
            sub.UserName, sub.Tier);

        // Calculate buff based on tier
        int buffValue = sub.Tier switch
        {
            "1000" => 10,  // Tier 1
            "2000" => 25,  // Tier 2
            "3000" => 50,  // Tier 3
            _ => 10
        };

        await _hubContext.Clients.Group(sub.BroadcasterUserId).SendAsync("TwitchEvent", new
        {
            Type = "subscription",
            UserName = sub.UserName,
            UserId = sub.UserId,
            Tier = sub.Tier,
            Buff = new { Type = "spd", Value = buffValue }
        });
    }

    private async Task OnChannelCheer(object? sender, ChannelCheerArgs args)
    {
        var cheer = args.Notification.Payload.Event;

        _logger.LogInformation("Cheer from {UserName}: {Bits} bits",
            cheer.UserName ?? "Anonymous", cheer.Bits);

        // 1 bit = 0.1% crit chance (100 bits = 10%)
        double critBonus = cheer.Bits * 0.1;

        await _hubContext.Clients.Group(cheer.BroadcasterUserId).SendAsync("TwitchEvent", new
        {
            Type = "bits",
            UserName = cheer.UserName ?? "Anonymous",
            Bits = cheer.Bits,
            Message = cheer.Message,
            Buff = new { Type = "crit_chance", Value = critBonus }
        });
    }
}
