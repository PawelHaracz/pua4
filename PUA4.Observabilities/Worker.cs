using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Options;
using PUA4.Observabilities.Options;

namespace PUA4.Observabilities;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly TelemetryClient _telemetryClient;
    private static readonly HttpClient HttpClient = new();
    private readonly WorkerOptions _options;

    public Worker(ILogger<Worker> logger, TelemetryClient tc, IOptions<WorkerOptions> options)
    {
        _options = options.Value ?? throw new ArgumentNullException(nameof(IOptions<WorkerOptions>));
        _logger = logger;
        _telemetryClient = tc;
        
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Enabled)
        {
            await base.StopAsync(stoppingToken);
            return;
        }
        
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            using (_telemetryClient.StartOperation<RequestTelemetry>("operation"))
            {
                _logger.LogWarning("A sample warning message. By default, logs with severity Warning or higher is captured by Application Insights");
                _logger.LogInformation("Calling bing.com");
                var res = await HttpClient.GetAsync("https://bing.com", stoppingToken);
                _logger.LogInformation("Calling bing completed with status: {statusCode}", res.StatusCode);
                _telemetryClient.TrackEvent("Bing call event completed");
            }
            await Task.Delay(1000, stoppingToken);
        }
    }
}