using Example.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Example.Controllers;
[ApiController]
[Route("[controller]/[action]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IMediator _mediator;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpGet]
    public async Task<IActionResult> TestCmdErrorValidation()
    {
        var commandRs = await _mediator.Send(new TestCommandRq(), default);

        return Ok(commandRs);
    }

    [HttpGet]
    public async Task<IActionResult> TestCmdCache()
    {
        var commandRs = await _mediator.Send(new TestCommandRq { Amount = 10000 }, default);

        return Ok(commandRs);
    }

    [HttpGet]
    public async Task<IActionResult> TestRateLimit()
    {
        var commandRs = await _mediator.Send(new GetUserByRateLimitCommandReq(), default);

        return Ok(commandRs);
    }
}
