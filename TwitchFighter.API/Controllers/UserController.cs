using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwitchFighter.API.Data;

namespace TwitchFighter.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;
    
    public UserController(AppDbContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Get current user profile by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _context.Users
            .Include(u => u.Progress)
            .Include(u => u.Stats)
            .Include(u => u.Achievements)
                .ThenInclude(ua => ua.Achievement)
            .FirstOrDefaultAsync(u => u.Id == id);
        
        if (user == null)
        {
            return NotFound();
        }
        
        return Ok(new
        {
            id = user.Id,
            twitchId = user.TwitchId,
            username = user.Username,
            displayName = user.DisplayName,
            avatarUrl = user.AvatarUrl,
            progress = user.Progress == null ? null : new
            {
                currentWave = user.Progress.CurrentWave,
                bestWave = user.Progress.BestWave
            },
            stats = user.Stats == null ? null : new
            {
                atk = user.Stats.Atk,
                spd = user.Stats.Spd,
                critChance = user.Stats.CritChance,
                critDamage = user.Stats.CritDamage,
                totalFollows = user.Stats.TotalFollows,
                totalSubs = user.Stats.TotalSubs,
                totalBits = user.Stats.TotalBits,
                totalDonations = user.Stats.TotalDonations
            },
            achievements = user.Achievements.Select(ua => new
            {
                id = ua.Achievement.Code,
                name = ua.Achievement.Name,
                description = ua.Achievement.Description,
                icon = ua.Achievement.IconPath,
                unlocked = true,
                unlockedAt = ua.UnlockedAt.ToString("yyyy-MM-dd"),
                isNew = ua.IsNew
            })
        });
    }
    
    /// <summary>
    /// Get user by Twitch ID
    /// </summary>
    [HttpGet("twitch/{twitchId}")]
    public async Task<IActionResult> GetUserByTwitchId(string twitchId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.TwitchId == twitchId);
        
        if (user == null)
        {
            return NotFound();
        }
        
        return await GetUser(user.Id);
    }
}
