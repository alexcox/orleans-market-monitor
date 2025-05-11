using MarketDataClient;

namespace MarketMonitor.Grains;

[Alias(nameof(IMarketDataConsumerGrain))]
public interface IMarketDataConsumerGrain : IGrainWithStringKey
{
    [Alias(nameof(Subscribe))]
    IAsyncEnumerable<Quote> Subscribe();
}