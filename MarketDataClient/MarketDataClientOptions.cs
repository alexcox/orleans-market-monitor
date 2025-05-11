namespace MarketDataClient;

public sealed record MarketDataClientOptions
{
    public int SubscriptionCount { get; init; } = 1_000;
}