namespace TwitchFighter.API.Models;

public class User
{
    public int Id { get; set; }
    public string TwitchId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    
    // Display config (synced to overlay)
    public string LayoutMode { get; set; } = "full";
    public bool ShowDamageNumbers { get; set; } = true;
    public bool ShowHUD { get; set; } = true;
    public string HeroSkin { get; set; } = "hero_1";
    
    // Navigation
    public Progress? Progress { get; set; }
    public Stats? Stats { get; set; }
    public ICollection<UserAchievement> Achievements { get; set; } = [];
}

