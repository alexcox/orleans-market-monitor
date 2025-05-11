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
        return AddMarketDataClient(services, options);
    }

    public static IServiceCollection AddMarketDataClient(
        this IServiceCollection services,
        Func<MarketDataClientOptions, MarketDataClientOptions>? configureOptions = null)
    {
        var options = new MarketDataClientOptions();
        if (configureOptions is not null)
        {
            options = configureOptions(options);
        }
        return AddMarketDataClient(services, options);
    }

    private static IServiceCollection AddMarketDataClient(IServiceCollection services, MarketDataClientOptions options)
    {
        services.AddSingleton(Options.Create(options));
        services.AddSingleton<IMarketDataClient, MarketDataClient>();
        return services;
    }
}