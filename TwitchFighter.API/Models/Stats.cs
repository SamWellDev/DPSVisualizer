namespace TwitchFighter.API.Models;

public class Stats
{
    public int Id { get; set; }
    public int UserId { get; set; }
    
    // Combat stats
    public int Atk { get; set; } = 10;
    public decimal Spd { get; set; } = 1.0m;
    public int CritChance { get; set; } = 5;
    public int CritDamage { get; set; } = 150;
    
    // Event counters (lifetime)
    public int TotalFollows { get; set; }
    public int TotalSubs { get; set; }
    public int TotalBits { get; set; }
    public decimal TotalDonations { get; set; }
    
    // This month counters
    public int MonthFollows { get; set; }
    public int MonthSubs { get; set; }
    public int MonthBits { get; set; }
    public decimal MonthDonations { get; set; }
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public User User { get; set; } = null!;
}
