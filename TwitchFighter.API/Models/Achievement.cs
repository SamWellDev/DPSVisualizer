namespace TwitchFighter.API.Models;

public class Achievement
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty; // e.g., "first_kill", "wave_100"
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconPath { get; set; } = string.Empty;
    public string? Hint { get; set; }
    
    // Navigation
    public ICollection<UserAchievement> UserAchievements { get; set; } = [];
}

public class UserAchievement
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int AchievementId { get; set; }
    public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;
    public bool IsNew { get; set; } = true;
    
    // Navigation
    public User User { get; set; } = null!;
    public Achievement Achievement { get; set; } = null!;
}
