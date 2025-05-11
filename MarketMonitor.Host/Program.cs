using MarketDataClient;
using MarketMonitor.Grains;
using MarketMonitor.Host;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddOrleans(sb =>
{
    sb.UseLocalhostClustering();
    sb.AddMemoryStreams(Constants.StreamNamespace);
    sb.AddMemoryGrainStorage(Constants.GrainStorageName);
    sb.UseDashboard(o => o.HostSelf = true);
});

builder.Services.AddMarketDataClient(o => o with { SubscriptionCount = 10 });

builder.Services.AddHostedService<MarketDataProducerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Map("/dashboard", b => b.UseOrleansDashboard());

app.MapGet("/quotes/{id}", (string id, IGrainFactory grainFactory, CancellationToken cancellationToken) =>
{
    var grain = grainFactory.GetGrain<IMarketDataConsumerGrain>(id);
    return TypedResults.ServerSentEvents(grain.Subscribe(), eventType: "quote");
});

app.Run();
