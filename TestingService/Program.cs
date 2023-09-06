using System;
using System.Text.Json;
using System.Threading.Tasks;
using OpenWeatherMap.Configuration;
using OpenWeatherMap.Services;

namespace TestingService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var service = new Service(new Config());
            try
            {
                 // var result = await service.GetCurrentWeatherByCityNameAsync("London", units: "metric");
                   var result = await service.GetCurrentWeatherByGeoCoordinatesAsync(57, -2, units: "metric");
                 //var result = await service.GetForecastByZipCodeAsync("94040");
                 // var result = await service.GetCurrentWeatherByCityIdAsync(2172797);
                Console.WriteLine(JsonSerializer.Serialize(result));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}