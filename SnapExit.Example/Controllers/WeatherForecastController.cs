using Microsoft.AspNetCore.Mvc;
using SnapExit.Example.Services.Interfaces;

namespace SnapExit.Example.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
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
    public void Update([FromBody] WeatherForecast weatherForecast)
    {
        _weatherForecastService.UpdateWeatherForecast(weatherForecast);
    }
}
