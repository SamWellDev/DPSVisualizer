namespace TwitchFighter.API.Models;

public class Progress
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CurrentWave { get; set; } = 1;
    public int BestWave { get; set; } = 1;
    public int CurrentMonth { get; set; }
    public int CurrentYear { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public User User { get; set; } = null!;
}
