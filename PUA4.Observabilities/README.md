# PUA Application Insight app

## Build the app

```bash
docker build -t pua4:1.0 .
```

## How run this app
```bash
## run basic app
docker run -p 8080:80 -e ApplicationInsights__ConnectionString=<Connection-String-AI> pawelharcz/pua4:1.0
```

### Settings

- *ApplicationInsights__ConnectionString* : Application Insight connection string `string`
- *Logging__Console__LogLevel__Default* : Logging level on the console `enum log level`
- *Logging__ApplicationInsightsLoggerProvider__LogLevel__Default*: Logging level of Application Insight `enum log level`
- *Fails__Enable* : Enable random dynamic fail over  `boolean  true|false default false`
- *Fails__PerformanceEnable* : Enable random performance issues `boolean  true|false default false`  
- *Worker__Enable* : Enable background worker : values: `boolean true|false default true`

Log level supported values:
- Information
- Trace
- Debug
- Warning
- Error 
- Critical

## Endpoints:

- `/healthz` - Health check endpoint
- `/WeatherForecast` - dummy endpoint for testing logging 