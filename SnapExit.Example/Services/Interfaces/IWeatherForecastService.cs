namespace SnapExit.Example.Services.Interfaces
{
    public interface IWeatherForecastService
    {
        public WeatherForecast GetWeatherForecast();
        public void UpdateWeatherForecast(WeatherForecast weatherForecast);
    }
}
