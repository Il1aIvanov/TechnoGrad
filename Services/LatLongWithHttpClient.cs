using System.Net.Http.Json;
using System.Text.Json;
using TechnoGrad.Models;
using TechnoGrad.Services.Interfaces;

namespace TechnoGrad.Services;

public class LatLongWithHttpClient : IMapLocationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _apiKey;
    
    public LatLongWithHttpClient(IHttpClientFactory httpClientFactory, string apiKey)
    {
        _httpClientFactory = httpClientFactory;
        _apiKey = apiKey;
    }
    
    public async Task<CustomMapPoint> GetLatLongFromAddressAsync(string address)
    {
        var httpClient = _httpClientFactory.CreateClient("YandexMapsClient");
        var relativeUri = $"?geocode={Uri.EscapeDataString(address)}&format=json&apikey={_apiKey}";
        
        var response = await httpClient.GetAsync(relativeUri);
        response.EnsureSuccessStatusCode();
        
        var root = await response.Content.ReadFromJsonAsync<JsonElement>();

        try
        {
            // Навигация по структуре JSON-ответа Яндекса
            var pos = root
                .GetProperty("response")
                .GetProperty("GeoObjectCollection")
                .GetProperty("featureMember")[0]
                .GetProperty("GeoObject")
                .GetProperty("Point")
                .GetProperty("pos")
                .GetString();

            if (string.IsNullOrEmpty(pos))
            {
                throw new Exception("Coordinates not found.");
            }

            var coordinates = pos.Split(' ');
            double longitude = double.Parse(coordinates[0], System.Globalization.CultureInfo.InvariantCulture);
            double latitude = double.Parse(coordinates[1], System.Globalization.CultureInfo.InvariantCulture);

            return new CustomMapPoint
            {
                Latitude = latitude,
                Longitude = longitude
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving coordinates: {ex.Message}");
        }
    }
}