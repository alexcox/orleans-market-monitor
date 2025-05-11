using MarketMonitor.Grains;

namespace MarketMonitor.Host;

public sealed class MarketDataProducerService(IGrainFactory grainFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var grain = grainFactory.GetGrain<IMarketDataProducerGrain>("default");
        await grain.Start();
    }
}