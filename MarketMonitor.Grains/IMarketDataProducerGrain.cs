
namespace MarketMonitor.Grains;

[Alias(nameof(IMarketDataProducerGrain))]
public interface IMarketDataProducerGrain : IGrainWithStringKey
{
    [Alias(nameof(Start))]
    Task Start();
}