using LoggingAuto.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoggingAuto.Controllers;

[ApiController]
[Route("[controller]")]
public class HomeController : ControllerBase
{
    private static readonly Serilog.ILogger _log = Serilog.Log.ForContext(typeof(HomeController));
    private readonly IUserService _userService;

    public HomeController(IUserService userService)
    {
        this._userService = userService;
    }
    [HttpGet("index")]
    public IActionResult Index(int id, string name)
    {
        _log.Information("Do action");

        Item item = new() { Id = _userService.Add(id, 5), Name = name, Description = $"Description {name}" };
        return Ok(item);
    }
    [HttpGet("indexno")]
    [NoIOLogging]
    public IActionResult Index2(int id, string name)
    {
        _log.Information("Do action 2");
        Item item = new() { Id = _userService.Subtract(id, 5), Name = name, Description = $"Description {name}" };
        return Ok(item);
    }
    [HttpGet("indexnoin")]
    [NoIOLogging]
    public IActionResult Index3(int id, string name)
    {
        _log.Information("Do action 3");
        Item item = new() { Id = _userService.Subtract(id, 5), Name = name, Description = $"Description {name}" };
        return Ok(item);
    }
    [HttpGet("indexitem")]
    [NoIOLogging]
    public IActionResult Index4(int id, string name)
    {
        _log.Information("Do action 4");
        (Item, int) ans = _userService.Hentai(new Item(){Id = -3, Name = "giang"},5);
        return Ok(ans.Item1);
    }
    [HttpPost("create")]
    public IActionResult Create(Item model)
    {
        _log.Information("Do action Create");
        Item a = new()
        {
            Id = model?.Id ?? 0,
            Name = model?.Name,
            Description = model?.Description
        };
        return Ok(model);
    }
}
