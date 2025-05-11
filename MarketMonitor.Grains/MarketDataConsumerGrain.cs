using System.Threading.Channels;
using MarketDataClient;
using Orleans.Streams;
using Orleans.Streams.Core;

namespace MarketMonitor.Grains;

[ImplicitStreamSubscription(Constants.StreamNamespace)]
public sealed class MarketDataConsumerGrain : Grain, IStreamSubscriptionObserver, IMarketDataConsumerGrain
{
    private static readonly BoundedChannelOptions _channelOptions = new(3)
    {
        SingleReader = true,
        SingleWriter = true,
    };

    private readonly Channel<Quote> _channel = Channel.CreateBounded<Quote>(_channelOptions);

    private QuoteObserver? _observer;

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _observer = new QuoteObserver(_channel.Writer);
        return base.OnActivateAsync(cancellationToken);
    }

    private sealed class QuoteObserver(ChannelWriter<Quote> writer) : IAsyncObserver<Quote>
    {
        public Task OnErrorAsync(Exception ex)
        {
            writer.TryComplete(ex);
            return Task.CompletedTask;
        }

        public async Task OnNextAsync(Quote item, StreamSequenceToken? token = null)
        {
            await writer.WriteAsync(item);
        }
    }

    public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
    {
        var handle = handleFactory.Create<Quote>();
        await handle.ResumeAsync(_observer!);
    }

    public async IAsyncEnumerable<Quote> Subscribe()
    {
        await foreach (var quote in _channel.Reader.ReadAllAsync())
        {
            yield return quote;
        }
    }
}