using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Globalization;

namespace QuickstartWeatherServer.Tools;

[McpServerToolType]
public static class WeatherTools
{
    [McpServerTool, Description("Get weather forecast for a location.")]
    public static async Task<string> GetForecast(
        HttpClient client,
        [Description("Latitude of the location.")] double latitude,
        [Description("Longitude of the location.")] double longitude)
    {
        try
        {
            var pointUrl = string.Create(CultureInfo.InvariantCulture, $"locationforecast/2.0/compact?lat={latitude}&lon={longitude}");
            using var jsonDocument = await client.ReadJsonDocumentAsync(pointUrl);
            var forecastUrl = jsonDocument.RootElement.GetProperty("properties").GetProperty("forecast").GetString()
                ?? throw new Exception($"No forecast URL provided by {client.BaseAddress}/locationforecast/2.0/compact?lat={latitude}&lon={longitude}");

            using var forecastDocument = await client.ReadJsonDocumentAsync(forecastUrl);
            var periods1 = forecastDocument.RootElement.GetProperty("properties");
            var periods = forecastDocument.RootElement.GetProperty("properties").GetProperty("timeseries").EnumerateArray();

            var test = periods.Select(period =>
                    period.GetProperty("data").GetProperty("instant").GetProperty("details").GetProperty("air_temperature").ToString());

            var test1 = periods.Select(period =>
                    period.GetProperty("data").GetProperty("instant").GetProperty("details").GetProperty("wind_speed").ToString());

            var test2 = periods.Select(period =>
                    period.GetProperty("data").GetProperty("next_1_hours").GetProperty("summary").GetProperty("symbol_code").ToString());

            return string.Join("\n---\n", periods.Select(period => $"""
                Temperature: {period.GetProperty("data").GetProperty("instant").GetProperty("details").GetProperty("air_temperature").ToString()}°C
                """));
        } catch(Exception ex)
        {
            return ex.Message + " " + client.BaseAddress + $"/locationforecast/2.0/compact?lat={latitude}&lon={longitude}";
        }
        
    }
}