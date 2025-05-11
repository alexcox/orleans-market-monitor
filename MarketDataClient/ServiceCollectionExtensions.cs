using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MarketDataClient;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMarketDataClient(
        this IServiceCollection services,
        Action<MarketDataClientOptions>? configureOptions = null)
    {
        var options = new MarketDataClientOptions();
        configureOptions?.Invoke(options);
        services.AddSingleton(Options.Create(options));
        services.AddSingleton<IMarketDataClient, MarketDataClient>();
        return services;
    }
}