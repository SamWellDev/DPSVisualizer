using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwitchFighter.API.Data;

namespace TwitchFighter.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RankingsController : ControllerBase
{
    private readonly AppDbContext _context;
    
    public RankingsController(AppDbContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Get global rankings (top 100 by best wave)
    /// </summary>
    [HttpGet("global")]
    public async Task<IActionResult> GetGlobalRankings([FromQuery] int limit = 100)
    {
        var now = DateTime.UtcNow;
        
        var rankings = await _context.Users
            .Include(u => u.Progress)
            .Where(u => u.Progress != null && 
                        u.Progress.CurrentMonth == now.Month && 
                        u.Progress.CurrentYear == now.Year)
            .OrderByDescending(u => u.Progress!.BestWave)
            .Take(limit)
            .Select(u => new
            {
                name = u.DisplayName ?? u.Username,
                wave = u.Progress!.BestWave,
                avatar = u.AvatarUrl
            })
            .ToListAsync();
        
        return Ok(rankings);
    }
    
    /// <summary>
    /// Get all-time rankings
    /// </summary>
    [HttpGet("alltime")]
    public async Task<IActionResult> GetAllTimeRankings([FromQuery] int limit = 100)
    {
        var rankings = await _context.Users
            .Include(u => u.Progress)
            .Where(u => u.Progress != null)
            .OrderByDescending(u => u.Progress!.BestWave)
            .Take(limit)
            .Select(u => new
            {
                name = u.DisplayName ?? u.Username,
                wave = u.Progress!.BestWave,
                avatar = u.AvatarUrl
            })
            .ToListAsync();
        
        return Ok(rankings);
    }
    
    /// <summary>
    /// Get user's rank position
    /// </summary>
    [HttpGet("rank/{userId}")]
    public async Task<IActionResult> GetUserRank(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Progress)
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        if (user?.Progress == null)
        {
            return NotFound();
        }
        
        var now = DateTime.UtcNow;
        
        // Count users with higher waves this month
        var rank = await _context.Users
            .Include(u => u.Progress)
            .Where(u => u.Progress != null && 
                        u.Progress.CurrentMonth == now.Month && 
                        u.Progress.CurrentYear == now.Year &&
                        u.Progress.BestWave > user.Progress.BestWave)
            .CountAsync() + 1;
        
        return Ok(new
        {
            rank = rank,
            wave = user.Progress.BestWave,
            avatar = user.AvatarUrl
        });
    }
}
