using PUA4.Observabilities;
using PUA4.Observabilities.Options;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationInsightsTelemetry();
//builder.Services.AddServiceProfiler();


builder.Services.AddOptions<FailOptions>()
    .Configure<IConfiguration>(
        (options, configuration) 
            => configuration.GetSection("Fails").Bind(options));

builder.Services.AddOptions<WorkerOptions>()
    .Configure<IConfiguration>(
        (options, configuration) 
            => configuration.GetSection("Worker").Bind(options));

builder.Services.AddHostedService<Worker>();
builder.Services.AddApplicationInsightsTelemetryWorkerService();
builder.Logging.AddConsole();
builder.Services.AddHealthChecks();
builder.Configuration.AddEnvironmentVariables();

builder.Host.UseSerilog((ctx, lc) =>
{
    var minimumLvl = ctx.Configuration.GetSection("Logging:Serilog:LogLevel").GetValue<LogEventLevel>("Default");
    lc
        .MinimumLevel.Is(minimumLvl)
        .WriteTo.Console()
        .WriteTo.AzureAnalytics(
            ctx.Configuration.GetSection("AzureAnalytics").GetValue<string>("workspaceId"),
            ctx.Configuration.GetSection("AzureAnalytics").GetValue<string>("authenticationId"),
            "pua4", minimumLvl);
});
var app = builder.Build();
app.UseMiddleware<CorrelationIdMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapHealthChecks("/healthz");
app.UseSerilogRequestLogging();


app.MapControllers();

app.Run();