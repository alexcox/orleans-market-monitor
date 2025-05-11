using System.Threading.Channels;

namespace MarketDataClient;

public interface IMarketDataClient
{
    ChannelReader<Quote> Subscribe(CancellationToken cancellationToken = default);
}