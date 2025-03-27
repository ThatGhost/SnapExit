using Microsoft.AspNetCore.Mvc;
using SnapExit.Example.Services.Interfaces;

namespace SnapExit.Example.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class WeatherForecastController : ControllerBase
{
    private readonly IWeatherForecastService _weatherForecastService;

    public WeatherForecastController(IWeatherForecastService weatherForecastService)
    {
        _weatherForecastService = weatherForecastService;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public Task<WeatherForecast> Get()
    {
        return Task.FromResult(_weatherForecastService.GetWeatherForecast());
    }

    [HttpPut(Name = "UpdateWeatherForecast")]
    public async Task Update([FromBody] WeatherForecast weatherForecast)
    {
        await _weatherForecastService.UpdateWeatherForecast(weatherForecast);
    }
}
