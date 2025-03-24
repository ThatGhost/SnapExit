using SnapExit.Example.Services.Interfaces;
using System;

namespace SnapExit.Example.Services;

public sealed class WeatherForecastService : IWeatherForecastService
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
    private WeatherForecast _weatherForecast;

    private readonly IAssertionService assertionService;

    public WeatherForecastService(IAssertionService assertionService)
    {
        _weatherForecast = new WeatherForecast()
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(Random.Shared.Next(20))),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        };
        this.assertionService = assertionService;
    }

    public WeatherForecast GetWeatherForecast()
    {
        return _weatherForecast;
    }

    public void UpdateWeatherForecast(WeatherForecast weatherForecast)
    {
        if (weatherForecast.TemperatureC < -20) assertionService.Forbidden("Wow i hate it here", "abc123def456");
        if (weatherForecast.TemperatureC > 50) assertionService.Teapot("Wow i hate it here");
        if (weatherForecast.Date < DateOnly.FromDateTime(DateTime.Now)) assertionService.NotFound();

        _weatherForecast = weatherForecast;
    }
}
