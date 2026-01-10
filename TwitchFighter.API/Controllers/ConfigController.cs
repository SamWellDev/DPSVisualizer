using Microsoft.AspNetCore.Mvc;

namespace TwitchFighter.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConfigController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IWebHostEnvironment _env;
    
    public ConfigController(IConfiguration config, IWebHostEnvironment env)
    {
        _config = config;
        _env = env;
    }
    
    /// <summary>
    /// Returns the game configuration for the frontend
    /// </summary>
    [HttpGet]
    public IActionResult GetConfig()
    {
        var configPath = Path.Combine(_env.ContentRootPath, "gameconfig.json");
        
        if (!System.IO.File.Exists(configPath))
        {
            return NotFound("Game config not found");
        }
        
        var json = System.IO.File.ReadAllText(configPath);
        return Content(json, "application/json");
    }
}
