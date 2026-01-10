using System.Text.Json;

namespace TwitchFighter.API.Services;

public class GameConfigService
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<GameConfigService> _logger;
    private GameConfig? _config;
    private DateTime _lastLoaded;

    public GameConfigService(IWebHostEnvironment env, ILogger<GameConfigService> logger)
    {
        _env = env;
        _logger = logger;
        LoadConfig();
    }

    public GameConfig Config => _config ?? LoadConfig();

    public GameConfig LoadConfig()
    {
        var configPath = Path.Combine(_env.ContentRootPath, "gameconfig.json");
        
        try
        {
            var json = File.ReadAllText(configPath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _config = JsonSerializer.Deserialize<GameConfig>(json, options) ?? new GameConfig();
            _lastLoaded = DateTime.UtcNow;
            _logger.LogInformation("Game config loaded successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load game config, using defaults");
            _config = new GameConfig();
        }

        return _config;
    }

    // Refresh config if file changed (call periodically or on demand)
    public void RefreshIfNeeded()
    {
        if ((DateTime.UtcNow - _lastLoaded).TotalMinutes > 1)
        {
            LoadConfig();
        }
    }
}

public class GameConfig
{
    public BuffsConfig Buffs { get; set; } = new();
    public MonsterConfig Monster { get; set; } = new();
    public HeroConfig Hero { get; set; } = new();
}

public class BuffsConfig
{
    public BuffValue Follow { get; set; } = new() { Type = "crit_chance", Value = 2 };
    public BuffValue SubTier1 { get; set; } = new() { Type = "spd", Value = 0.5 };
    public BuffValue SubTier2 { get; set; } = new() { Type = "spd", Value = 1.0 };
    public SubTier3Buff SubTier3 { get; set; } = new();
    public BuffValue BitsPerHundred { get; set; } = new() { Type = "crit_damage", Value = 10 };
}

public class BuffValue
{
    public string Type { get; set; } = "";
    public double Value { get; set; }
}

public class SubTier3Buff : BuffValue
{
    public double BonusAtk { get; set; } = 10;

    public SubTier3Buff()
    {
        Type = "spd";
        Value = 1.5;
    }
}

public class MonsterConfig
{
    public int BaseHp { get; set; } = 618;
    public double HpMultiplierPerWave { get; set; } = 1.5;
    public string[] Names { get; set; } = { "Slime", "Goblin", "Orc", "Troll", "Demon", "Dragon", "Titan", "Ancient One" };
}

public class HeroConfig
{
    public int BaseAtk { get; set; } = 10;
    public double BaseSpd { get; set; } = 1.0;
    public int BaseCritChance { get; set; } = 5;
    public int BaseCritDamage { get; set; } = 150;
    public double MaxSpd { get; set; } = 10;
    public int MaxCritChance { get; set; } = 100;
}
