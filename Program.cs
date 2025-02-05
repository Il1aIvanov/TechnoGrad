using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechnoGrad.Services;

namespace TechnoGrad;

public class Program
{
    static async Task Main()
    {
        var builder = new ConfigurationBuilder()
            .AddEnvironmentVariables(); // Добавляем поддержку переменных окружения

        var configuration = builder.Build();

        // Чтение API-ключа из переменной окружения
        var apiKey = configuration["YANDEX_API_KEY"];
        if (string.IsNullOrEmpty(apiKey))
        {
            Console.WriteLine("API key not found.");
            return;
        }
        
        var services = new ServiceCollection();

        services.AddHttpClient("YandexMapsClient", client =>
        {
            client.BaseAddress = new Uri("https://geocode-maps.yandex.ru/1.x/");
        });

        services.AddSingleton<LatLongWithHttpClient>(provider =>
            new LatLongWithHttpClient(provider.GetRequiredService<IHttpClientFactory>(), apiKey));

        var serviceProvider = services.BuildServiceProvider();
        while (true)
        {
            var latLongWithHttpClient = serviceProvider.GetRequiredService<LatLongWithHttpClient>();
            Console.Write("Введите адрес: ");
            var address = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(address))
            {
                break;
            }
            try
            {
                var coordinatesHttpClient = await latLongWithHttpClient.GetLatLongFromAddressAsync(address);
                Console.WriteLine($"Address ({address}) is at {coordinatesHttpClient.Longitude}, {coordinatesHttpClient.Latitude}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка: {e.Message}\n");
            }
        }
    }
}