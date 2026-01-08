using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwitchFighter.API.Data;

namespace TwitchFighter.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProgressController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<ProgressController> _logger;
    
    public ProgressController(AppDbContext context, ILogger<ProgressController> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    /// <summary>
    /// Get user's current progress
    /// </summary>
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetProgress(int userId)
    {
        var progress = await _context.Progresses
            .FirstOrDefaultAsync(p => p.UserId == userId);
        
        if (progress == null)
        {
            return NotFound();
        }
        
        return Ok(new
        {
            currentWave = progress.CurrentWave,
            bestWave = progress.BestWave,
            currentMonth = progress.CurrentMonth,
            currentYear = progress.CurrentYear,
            updatedAt = progress.UpdatedAt
        });
    }
    
    /// <summary>
    /// Update current wave
    /// </summary>
    [HttpPost("{userId}/wave")]
    public async Task<IActionResult> UpdateWave(int userId, [FromBody] UpdateWaveRequest request)
    {
        var progress = await _context.Progresses
            .FirstOrDefaultAsync(p => p.UserId == userId);
        
        if (progress == null)
        {
            return NotFound();
        }
        
        // Check if month changed, reset if needed
        var now = DateTime.UtcNow;
        if (progress.CurrentMonth != now.Month || progress.CurrentYear != now.Year)
        {
            progress.CurrentWave = 1;
            progress.CurrentMonth = now.Month;
            progress.CurrentYear = now.Year;
            _logger.LogInformation("Monthly reset for user {UserId}", userId);
        }
        
        progress.CurrentWave = request.Wave;
        
        if (request.Wave > progress.BestWave)
        {
            progress.BestWave = request.Wave;
            _logger.LogInformation("User {UserId} achieved new best wave: {Wave}", userId, request.Wave);
        }
        
        progress.UpdatedAt = now;
        await _context.SaveChangesAsync();
        
        return Ok(new
        {
            currentWave = progress.CurrentWave,
            bestWave = progress.BestWave,
            isNewBest = request.Wave >= progress.BestWave
        });
    }
    
    /// <summary>
    /// Record a monster defeat and check achievements
    /// </summary>
    [HttpPost("{userId}/defeat")]
    public async Task<IActionResult> RecordDefeat(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Progress)
            .Include(u => u.Achievements)
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        if (user?.Progress == null)
        {
            return NotFound();
        }
        
        user.Progress.CurrentWave++;
        user.Progress.UpdatedAt = DateTime.UtcNow;
        
        if (user.Progress.CurrentWave > user.Progress.BestWave)
        {
            user.Progress.BestWave = user.Progress.CurrentWave;
        }
        
        // Check for achievements
        var newAchievements = new List<string>();
        
        // First Kill
        if (user.Progress.CurrentWave >= 1)
        {
            await TryUnlockAchievement(user, "first_kill", newAchievements);
        }
        
        // Wave milestones
        if (user.Progress.CurrentWave >= 10)
        {
            await TryUnlockAchievement(user, "wave_10", newAchievements);
        }
        if (user.Progress.CurrentWave >= 50)
        {
            await TryUnlockAchievement(user, "wave_50", newAchievements);
        }
        if (user.Progress.CurrentWave >= 100)
        {
            await TryUnlockAchievement(user, "wave_100", newAchievements);
        }
        
        await _context.SaveChangesAsync();
        
        return Ok(new
        {
            currentWave = user.Progress.CurrentWave,
            bestWave = user.Progress.BestWave,
            newAchievements = newAchievements
        });
    }
    
    private async Task TryUnlockAchievement(Models.User user, string code, List<string> newAchievements)
    {
        if (user.Achievements.Any(a => a.Achievement.Code == code))
        {
            return; // Already has it
        }
        
        var achievement = await _context.Achievements.FirstOrDefaultAsync(a => a.Code == code);
        if (achievement != null)
        {
            user.Achievements.Add(new Models.UserAchievement
            {
                AchievementId = achievement.Id,
                UserId = user.Id,
                IsNew = true
            });
            newAchievements.Add(code);
            _logger.LogInformation("User {UserId} unlocked achievement: {Code}", user.Id, code);
        }
    }
}

public class UpdateWaveRequest
{
    public int Wave { get; set; }
}
