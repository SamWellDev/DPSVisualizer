using Microsoft.AspNetCore.Mvc;
using TwitchLib.Api;

namespace TwitchFighter.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StreamController : ControllerBase
{
    private readonly ILogger<StreamController> _logger;
    private readonly TwitchAPI _twitchApi;

    public StreamController(ILogger<StreamController> logger, IConfiguration config)
    {
        _logger = logger;

        // Initialize Twitch API
        _twitchApi = new TwitchAPI();
        _twitchApi.Settings.ClientId = config["Twitch:ClientId"];
        _twitchApi.Settings.Secret = config["Twitch:ClientSecret"];
    }

    /// <summary>
    /// Check if a Twitch channel is currently live
    /// </summary>
    [HttpGet("status/{twitchId}")]
    public async Task<IActionResult> GetStreamStatus(string twitchId)
    {
        try
        {
            // Get App Access Token if not set
            if (string.IsNullOrEmpty(_twitchApi.Settings.AccessToken))
            {
                var accessToken = await _twitchApi.Auth.GetAccessTokenAsync();
                _twitchApi.Settings.AccessToken = accessToken;
            }

            // Query Twitch Helix API for stream status
            var streams = await _twitchApi.Helix.Streams.GetStreamsAsync(userIds: new List<string> { twitchId });

            var isLive = streams.Streams.Length > 0;
            var streamInfo = streams.Streams.FirstOrDefault();

            _logger.LogInformation("Stream status check for {TwitchId}: {IsLive}", twitchId, isLive);

            return Ok(new
            {
                isLive,
                viewerCount = streamInfo?.ViewerCount ?? 0,
                title = streamInfo?.Title,
                gameName = streamInfo?.GameName,
                startedAt = streamInfo?.StartedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check stream status for {TwitchId}", twitchId);
            return StatusCode(500, new { error = "Failed to check stream status" });
        }
    }
}
