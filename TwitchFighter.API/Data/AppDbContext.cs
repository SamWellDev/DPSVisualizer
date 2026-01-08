using Microsoft.EntityFrameworkCore;
using TwitchFighter.API.Models;

namespace TwitchFighter.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<User> Users => Set<User>();
    public DbSet<Progress> Progresses => Set<Progress>();
    public DbSet<Stats> Stats => Set<Stats>();
    public DbSet<Achievement> Achievements => Set<Achievement>();
    public DbSet<UserAchievement> UserAchievements => Set<UserAchievement>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.TwitchId).IsUnique();
            entity.HasOne(e => e.Progress)
                  .WithOne(e => e.User)
                  .HasForeignKey<Progress>(e => e.UserId);
            entity.HasOne(e => e.Stats)
                  .WithOne(e => e.User)
                  .HasForeignKey<Stats>(e => e.UserId);
        });
        
        // Achievement
        modelBuilder.Entity<Achievement>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique();
        });
        
        // UserAchievement
        modelBuilder.Entity<UserAchievement>(entity =>
        {
            entity.HasIndex(e => new { e.UserId, e.AchievementId }).IsUnique();
        });
        
        // Seed default achievements
        modelBuilder.Entity<Achievement>().HasData(
            new Achievement { Id = 1, Code = "first_kill", Name = "First Blood", Description = "Defeat your first monster", IconPath = "/cheevos/first_kill.png", Hint = "Defeat a monster" },
            new Achievement { Id = 2, Code = "wave_10", Name = "Getting Started", Description = "Reach wave 10", IconPath = "/cheevos/first_kill.png", Hint = "Keep fighting!" },
            new Achievement { Id = 3, Code = "wave_50", Name = "Veteran", Description = "Reach wave 50", IconPath = "/cheevos/first_kill.png", Hint = "Half way there!" },
            new Achievement { Id = 4, Code = "wave_100", Name = "Centurion", Description = "Reach wave 100", IconPath = "/cheevos/first_kill.png", Hint = "The century awaits!" },
            new Achievement { Id = 5, Code = "first_sub", Name = "Community Support", Description = "Receive your first subscription", IconPath = "/cheevos/first_kill.png", Hint = "Get a subscriber" },
            new Achievement { Id = 6, Code = "crit_master", Name = "Critical Master", Description = "Reach 50% critical chance", IconPath = "/cheevos/first_kill.png", Hint = "Followers boost crit!" },
            new Achievement { Id = 7, Code = "speed_demon", Name = "Speed Demon", Description = "Reach 5.0 attack speed", IconPath = "/cheevos/first_kill.png", Hint = "Subs boost speed!" },
            new Achievement { Id = 8, Code = "whale", Name = "Whale Watcher", Description = "Receive 10,000 bits total", IconPath = "/cheevos/first_kill.png", Hint = "Bits add up!" }
        );
    }
}
