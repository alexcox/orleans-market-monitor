using MarketMonitor.Grains;

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Map("/dashboard", b => b.UseOrleansDashboard());

app.Run();