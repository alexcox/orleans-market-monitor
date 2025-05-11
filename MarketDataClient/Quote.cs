namespace MarketDataClient;

public sealed record Quote(
    string Identifier,
    DateTime TimestampUtc,
    decimal Value);