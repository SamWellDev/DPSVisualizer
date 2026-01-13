using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwitchFighter.API.Data;
using TwitchFighter.API.Services;

namespace TwitchFighter.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<StatsController> _logger;
    private readonly GameConfigService _configService;
    
    // Buff values (same as frontend)
    private static readonly Dictionary<string, object> BuffConfig = new()
    {
        ["follow"] = new { critChance = 2 },
        ["sub1"] = new { spd = 0.5m },
        ["sub2"] = new { spd = 1.0m },
        ["sub3"] = new { spd = 1.5m, atk = 10 },
        ["donate5"] = new { atk = 5 },
        ["donate10"] = new { atk = 20, critDamage = 15 },
        ["bits"] = new { critDamage = 0.01m } // per bit
    };
    
    public StatsController(AppDbContext context, ILogger<StatsController> logger, GameConfigService configService)
    {
        _context = context;
        _logger = logger;
        _configService = configService;
    }
    
    /// <summary>
    /// Get user stats
    /// </summary>
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetStats(int userId)
    {
        var stats = await _context.Stats.FirstOrDefaultAsync(s => s.UserId == userId);
        
        if (stats == null)
        {
            return NotFound();
        }
        
        return Ok(new
        {
            atk = stats.Atk,
            spd = stats.Spd,
            critChance = stats.CritChance,
            critDamage = stats.CritDamage,
            counters = new
            {
                follows = stats.TotalFollows,
                subs = stats.TotalSubs,
                bits = stats.TotalBits,
                donations = stats.TotalDonations
            }
        });
    }
    
    /// <summary>
    /// Apply a buff from a Twitch event
    /// </summary>
    [HttpPost("{userId}/buff")]
    public async Task<IActionResult> ApplyBuff(int userId, [FromBody] ApplyBuffRequest request)
    {
        var stats = await _context.Stats.FirstOrDefaultAsync(s => s.UserId == userId);
        
        if (stats == null)
        {
            return NotFound();
        }
        
        // Apply buff based on event type
        switch (request.EventType.ToLower())
        {
            case "follow":
                stats.CritChance += 2;
                stats.TotalFollows++;
                stats.MonthFollows++;
                break;
                
            case "sub1":
                stats.Spd += 0.5m;
                stats.TotalSubs++;
                stats.MonthSubs++;
                break;
                
            case "sub2":
                stats.Spd += 1.0m;
                stats.TotalSubs++;
                stats.MonthSubs++;
                break;
                
            case "sub3":
                stats.Spd += 1.5m;
                stats.Atk += 10;
                stats.TotalSubs++;
                stats.MonthSubs++;
                break;
                
            case "donate5":
                stats.Atk += 5;
                stats.TotalDonations += 5;
                stats.MonthDonations += 5;
                break;
                
            case "donate10":
                stats.Atk += 20;
                stats.CritDamage += 15;
                stats.TotalDonations += 10;
                stats.MonthDonations += 10;
                break;
                
            case "bits":
                var bitsAmount = request.Amount ?? 100;
                stats.CritDamage += (int)(bitsAmount * 0.01);
                stats.TotalBits += bitsAmount;
                stats.MonthBits += bitsAmount;
                break;
                
            default:
                return BadRequest($"Unknown event type: {request.EventType}");
        }
        
        stats.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Applied buff {EventType} to user {UserId}", request.EventType, userId);
        
        return Ok(new
        {
            atk = stats.Atk,
            spd = stats.Spd,
            critChance = stats.CritChance,
            critDamage = stats.CritDamage
        });
    }
    
    /// <summary>
    /// Reset stats for a new month
    /// </summary>
    [HttpPost("{userId}/reset-monthly")]
    public async Task<IActionResult> ResetMonthly(int userId)
    {
        var stats = await _context.Stats.FirstOrDefaultAsync(s => s.UserId == userId);
        
        if (stats == null)
        {
            return NotFound();
        }
        
        // Reset to base stats
        stats.Atk = 10;
        stats.Spd = 1.0m;
        stats.CritChance = 5;
        stats.CritDamage = 150;
        stats.MonthFollows = 0;
        stats.MonthSubs = 0;
        stats.MonthBits = 0;
        stats.MonthDonations = 0;
        stats.UpdatedAt = DateTime.UtcNow;
        
        // Also reset progress (wave) to 1 and monster HP to base (50)
        var progress = await _context.Progresses.FirstOrDefaultAsync(p => p.UserId == userId);
        if (progress != null)
        {
            progress.CurrentWave = 1;
            progress.MonsterCurrentHp = _configService.Config.Monster.BaseHp;
            progress.UpdatedAt = DateTime.UtcNow;
        }
        
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Reset stats and wave for user {UserId}", userId);
        
        return Ok(new { message = "Stats and progress reset for new month" });
    }
}

public class ApplyBuffRequest
{
    public string EventType { get; set; } = string.Empty;
    public int? Amount { get; set; }
    public string? Username { get; set; }
}
