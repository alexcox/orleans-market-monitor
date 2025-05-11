namespace MarketDataClient;

[GenerateSerializer, Immutable, Alias(nameof(Quote))]
public sealed record Quote(
    string Identifier,
    DateTime TimestampUtc,
    decimal Value);