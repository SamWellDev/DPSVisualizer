using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwitchFighter.API.Data;
using TwitchFighter.API.Models;
using TwitchFighter.API.Services;
using TwitchLib.Api;

namespace TwitchFighter.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthController> _logger;
    private readonly TwitchEventSubService _eventSubService;
    
    public AuthController(
        AppDbContext context, 
        IConfiguration config, 
        ILogger<AuthController> logger,
        TwitchEventSubService eventSubService)
    {
        _context = context;
        _config = config;
        _logger = logger;
        _eventSubService = eventSubService;
    }
    
    /// <summary>
    /// Redirects user to Twitch OAuth login
    /// </summary>
    [HttpGet("login")]
    public IActionResult Login()
    {
        var clientId = _config["Twitch:ClientId"];
        var redirectUri = _config["Twitch:RedirectUri"];
        var scopes = "user:read:email channel:read:subscriptions bits:read moderator:read:followers";
        
        var authUrl = $"https://id.twitch.tv/oauth2/authorize?client_id={clientId}&redirect_uri={Uri.EscapeDataString(redirectUri!)}&response_type=code&scope={Uri.EscapeDataString(scopes)}";
        
        return Redirect(authUrl);
    }
    
    /// <summary>
    /// Handles OAuth callback from Twitch
    /// </summary>
    [HttpGet("callback")]
    public async Task<IActionResult> Callback([FromQuery] string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return BadRequest("Authorization code is required");
        }
        
        try
        {
            var clientId = _config["Twitch:ClientId"]!;
            var clientSecret = _config["Twitch:ClientSecret"]!;
            var redirectUri = _config["Twitch:RedirectUri"]!;
            
            // Exchange code for token
            using var httpClient = new HttpClient();
            var tokenResponse = await httpClient.PostAsync("https://id.twitch.tv/oauth2/token", 
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["client_id"] = clientId,
                    ["client_secret"] = clientSecret,
                    ["code"] = code,
                    ["grant_type"] = "authorization_code",
                    ["redirect_uri"] = redirectUri
                }));
            
            if (!tokenResponse.IsSuccessStatusCode)
            {
                var errorBody = await tokenResponse.Content.ReadAsStringAsync();
                _logger.LogError("Failed to exchange code for token. Status: {Status}, Body: {Body}", 
                    tokenResponse.StatusCode, errorBody);
                return BadRequest($"Failed to authenticate with Twitch: {errorBody}");
            }
            
            var tokenData = await tokenResponse.Content.ReadFromJsonAsync<TwitchTokenResponse>();
            
            // Get user info from Twitch
            var api = new TwitchAPI();
            api.Settings.ClientId = clientId;
            api.Settings.AccessToken = tokenData!.AccessToken;
            
            var users = await api.Helix.Users.GetUsersAsync();
            var twitchUser = users.Users.FirstOrDefault();
            
            if (twitchUser == null)
            {
                return BadRequest("Could not get Twitch user info");
            }
            
            // Find or create user in database
            var user = await _context.Users
                .Include(u => u.Progress)
                .Include(u => u.Stats)
                .FirstOrDefaultAsync(u => u.TwitchId == twitchUser.Id);
            
            if (user == null)
            {
                user = new User
                {
                    TwitchId = twitchUser.Id,
                    Username = twitchUser.Login,
                    DisplayName = twitchUser.DisplayName,
                    AvatarUrl = twitchUser.ProfileImageUrl,
                    AccessToken = tokenData.AccessToken,
                    RefreshToken = tokenData.RefreshToken,
                    CreatedAt = DateTime.UtcNow
                };
                
                // Create default progress and stats
                user.Progress = new Progress
                {
                    CurrentWave = 1,
                    BestWave = 1,
                    CurrentMonth = DateTime.UtcNow.Month,
                    CurrentYear = DateTime.UtcNow.Year
                };
                
                user.Stats = new Stats();
                
                _context.Users.Add(user);
                _logger.LogInformation("Created new user: {Username}", user.Username);
            }
            else
            {
                // Update existing user
                user.AccessToken = tokenData.AccessToken;
                user.RefreshToken = tokenData.RefreshToken;
                user.AvatarUrl = twitchUser.ProfileImageUrl;
                user.DisplayName = twitchUser.DisplayName;
                user.LastLoginAt = DateTime.UtcNow;
                _logger.LogInformation("User logged in: {Username}", user.Username);
            }
            
            await _context.SaveChangesAsync();
            
            // Set broadcaster ID for EventSub subscriptions
            await _eventSubService.SetBroadcasterId(twitchUser.Id);
            _logger.LogInformation("Broadcaster ID set for EventSub: {BroadcasterId}", twitchUser.Id);
            
            // Redirect back to frontend with user data
            var frontendUrl = $"http://localhost:3000/dashboard?token={tokenData.AccessToken}&userId={user.Id}&twitchId={twitchUser.Id}";
            return Redirect(frontendUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during OAuth callback");
            return StatusCode(500, "Authentication failed");
        }
    }
    
    /// <summary>
    /// Validates a Twitch access token
    /// </summary>
    [HttpGet("validate")]
    public async Task<IActionResult> ValidateToken([FromHeader(Name = "Authorization")] string? authorization)
    {
        if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer "))
        {
            return Unauthorized();
        }
        
        var token = authorization.Substring(7);
        
        try
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"OAuth {token}");
            
            var response = await httpClient.GetAsync("https://id.twitch.tv/oauth2/validate");
            
            if (!response.IsSuccessStatusCode)
            {
                return Unauthorized();
            }
            
            return Ok(await response.Content.ReadFromJsonAsync<object>());
        }
        catch
        {
            return Unauthorized();
        }
    }
}

public class TwitchTokenResponse
{
    [System.Text.Json.Serialization.JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;
    
    [System.Text.Json.Serialization.JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; } = string.Empty;
    
    [System.Text.Json.Serialization.JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("scope")]
    public string[] Scope { get; set; } = [];
    
    [System.Text.Json.Serialization.JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;
}
