using Asp.Versioning;
using MetalFlowScheduler.Api.Application.Services;
using MetalFlowScheduler.Api.Infrastructure.Mocks;
using MetalFlowScheduler.Api.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication.ExtendedProtection;
using System.Threading.Tasks;

namespace MetalFlowScheduler.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class WeatherForecastController : ControllerBase
{

    public readonly IProductionSolverService _productionSolverService;

    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IProductionSolverService productionSolverService)
    {
        _logger = logger;
        _productionSolverService = productionSolverService;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IActionResult> Get()
    {
        var planScenario = MockDataFactory.GetScenarioInputById(1);

        if (planScenario == null) { return BadRequest(); }


        var lines = await _productionSolverService.PlanProductionAsync(planScenario);

        var xx = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();

        return Ok(xx);
    }
}
