using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PUA4.Observabilities.Options;

namespace PUA4.Observabilities.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly FailOptions _options;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IOptions<FailOptions> options)
    {
        _logger = logger;
        _options = options.Value ?? throw new ArgumentException("Options {options}", nameof(options));
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        string correlationId;
        if (Request.Headers.TryGetValue(Constants.CorrelationIdHeaderKey, out var correlationIds))
        {
            correlationId = correlationIds.FirstOrDefault(k =>  k.Equals(Constants.CorrelationIdHeaderKey)) 
                            ?? Guid.NewGuid().ToString("N");
            _logger.LogDebug("took correlation Id: {correlationId}", correlationId);
        }
        else
        {
            correlationId = Guid.NewGuid().ToString("N");
            _logger.LogDebug("generated correlation Id: {correlationId}", correlationId);
        }
        
        using (_logger.BeginScope(
                   new Dictionary<string, object>()
               {
                   [nameof(correlationId)] = correlationId,
                   ["Operation_Id"] = correlationId,
                   ["Id"] = Guid.NewGuid(),
               }))
        {
            if (_options.Enable && (Random.Shared.NextInt64() % 3) == 0)
            {
                _logger.LogCritical("random error {correlationId}", correlationId);
                throw new ApplicationException($"fails sometimes happened, try again {correlationId}");
            }

            if (_options.PerformanceEnable && (Random.Shared.NextInt64() % 3) == 0)
            {
                var delay = Random.Shared.Next(1000, (10 * 1_000)); //get from 1000 milliseconds to 12 s
                await Task.Delay(delay);
            }
            _logger.LogInformation("Fetching weather {correlationId}", correlationId);

            var range =  Enumerable.Range(1, 5).Select(index =>
                {
                    var day = DateTime.Now.AddDays(index);
                    _logger.LogTrace("Fetching weather for {day} - {correlationId}", day.ToString("d") ,correlationId);
                    var forecast = new WeatherForecast
                    {
                        CorrelationId = correlationId,
                        Date = day,
                        TemperatureC = Random.Shared.Next(-20, 55),
                        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                    };
                    _logger.LogTrace("Fetched weather for {day} - {correlationId}", day.ToString("d"), correlationId);
                    
                    return forecast;
                })
                .ToArray();
            _logger.LogInformation("Fetched weather {correlationId}", correlationId);
            
            return range;
        }
    }
}