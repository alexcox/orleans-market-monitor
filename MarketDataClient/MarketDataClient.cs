using System.Threading.Channels;
using Microsoft.Extensions.Options;

namespace MarketDataClient;

internal sealed class MarketDataClient : IMarketDataClient
{
    private readonly string[] _identifiers;

    private static readonly BoundedChannelOptions _channelOptions = new(3)
    {
        AllowSynchronousContinuations = false
    };
    
    public MarketDataClient(IOptions<MarketDataClientOptions> options)
    {
        _identifiers = [.. Enumerable.Range(1, options.Value.SubscriptionCount).Select(i => i.ToString())];
    }
    
    public ChannelReader<Quote> Subscribe(CancellationToken cancellationToken = default)
    {
        var channel = Channel.CreateBounded<Quote>(_channelOptions);
        var writer = channel.Writer;

        foreach (var identifier in _identifiers)
        {
            Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var value = (decimal)Random.Shared.NextDouble() * 100;
                    Quote quote = new(identifier, DateTime.UtcNow, value);
                    await writer.WriteAsync(quote, cancellationToken).ConfigureAwait(false);
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken).ConfigureAwait(false);
                }
            }, cancellationToken);
        }
        
        return channel.Reader;
    }
}