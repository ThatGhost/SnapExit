﻿using SnapExit.Example.Services.Interfaces;
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

    public async Task UpdateWeatherForecast(WeatherForecast weatherForecast)
    {
        if (weatherForecast.TemperatureC < -20) await assertionService.Forbidden("Temp too low", "abc123def456");
        if (weatherForecast.TemperatureC > 50) await assertionService.Teapot("Temp too high");
        if (weatherForecast.Date < DateOnly.FromDateTime(DateTime.Now)) await assertionService.NotFound();

        _weatherForecast = weatherForecast;
    }
}
