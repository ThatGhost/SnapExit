namespace SnapExit.Example.Services.Interfaces;

public interface IWeatherForecastService
{
    public WeatherForecast GetWeatherForecast();
    public Task UpdateWeatherForecast(WeatherForecast weatherForecast);
}
