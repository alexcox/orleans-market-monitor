using System.Collections.Concurrent;
using MarketDataClient;
using Orleans.Streams;

namespace MarketMonitor.Grains;

public sealed class MarketDataProducerGrain(IMarketDataClient marketDataClient) : Grain, IMarketDataProducerGrain
{
    private ConcurrentDictionary<string, IAsyncStream<Quote>> _streams = [];

    public async Task Start()
    {
        var streamProvider = this.GetStreamProvider(Constants.StreamNamespace);

        var reader = marketDataClient.Subscribe();

        await foreach (var quote in reader.ReadAllAsync())
        {

            if (!_streams.TryGetValue(quote.Identifier, out var stream))
            {
                var streamId = StreamId.Create(Constants.StreamNamespace, quote.Identifier);
                stream = streamProvider.GetStream<Quote>(streamId);
                _streams.TryAdd(quote.Identifier, stream);
            }

            await stream.OnNextAsync(quote);
        }
    }
}
