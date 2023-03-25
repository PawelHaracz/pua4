using PUA4.Observabilities;
using PUA4.Observabilities.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationInsightsTelemetry();
//builder.Services.AddServiceProfiler();
builder.Services.AddLogging(builder =>
{
    builder.AddApplicationInsights( options =>
    {
        options.IncludeScopes = true;
        options.FlushOnDispose = true;
        options.TrackExceptionsAsExceptionTelemetry = false;
    });
});

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

builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();
app.UseMiddleware<CorrelationIdMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();