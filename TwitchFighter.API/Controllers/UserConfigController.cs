using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwitchFighter.API.Data;

namespace TwitchFighter.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserConfigController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<UserConfigController> _logger;

    public UserConfigController(AppDbContext context, ILogger<UserConfigController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get user's display config (for OBS overlay to fetch without URL params)
    /// </summary>
    [HttpGet("{twitchId}")]
    public async Task<IActionResult> GetUserConfig(string twitchId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.TwitchId == twitchId);

        if (user == null)
        {
            // Return default config if user not found
            return Ok(new
            {
                layoutMode = "full",
                showDamageNumbers = true,
                showHUD = true,
                heroSkin = "hero_1"
            });
        }

        return Ok(new
        {
            layoutMode = user.LayoutMode ?? "full",
            showDamageNumbers = user.ShowDamageNumbers,
            showHUD = user.ShowHUD,
            heroSkin = user.HeroSkin ?? "hero_1"
        });
    }

    /// <summary>
    /// Save user's display config
    /// </summary>
    [HttpPost("{twitchId}")]
    public async Task<IActionResult> SaveUserConfig(string twitchId, [FromBody] UserConfigRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.TwitchId == twitchId);

        if (user == null)
        {
            return NotFound("User not found");
        }

        user.LayoutMode = request.LayoutMode;
        user.ShowDamageNumbers = request.ShowDamageNumbers;
        user.ShowHUD = request.ShowHUD;
        user.HeroSkin = request.HeroSkin;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Saved display config for user {TwitchId}", twitchId);

        return Ok(new { message = "Config saved" });
    }
}

public class UserConfigRequest
{
    public string LayoutMode { get; set; } = "full";
    public bool ShowDamageNumbers { get; set; } = true;
    public bool ShowHUD { get; set; } = true;
    public string HeroSkin { get; set; } = "hero_1";
}

