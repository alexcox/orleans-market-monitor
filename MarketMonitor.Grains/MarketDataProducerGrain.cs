using System.Collections.Concurrent;
using MarketDataClient;
using Orleans.Streams;

namespace MarketMonitor.Grains;

public sealed class MarketDataProducerGrain(IMarketDataClient marketDataClient) : Grain
{
    private ConcurrentDictionary<string, IAsyncStream<Quote>> _streams = [];
    
    public Task Start(string ns, CancellationToken cancellationToken = default)
    {
        Task.Factory.StartNew(async () =>
        {
            var reader = marketDataClient.Subscribe(cancellationToken);
            await foreach (var quote in reader.ReadAllAsync(cancellationToken))
            {
                if (!_streams.TryGetValue(quote.Identifier, out var stream))
                {
                    var streamId = StreamId.Create(ns, quote.Identifier);
                    stream = this.GetStreamProvider(Constants.StreamNamespace)
                        .GetStream<Quote>(streamId);
                    _streams.TryAdd(quote.Identifier, stream);
                }
                
                await stream.OnNextAsync(quote);
            }
        }, cancellationToken);
        return Task.CompletedTask;
    }
}