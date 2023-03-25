# PUA Application Insight app

## Build the app

```bash
docker build -t pua4:1.0 .
```

## How run this app
```bash
## run basic app
docker run -e ApplicationInsights__ConnectionString=<Connection-String-AI> pua4:1.0
```

### Settings

- *ApplicationInsights__ConnectionString* : Application Insight connection string
- *Logging__Console__LogLevel__Default* : Logging level on the console
- *Logging__ApplicationInsightsLoggerProvider__LogLevel__Default*: Logging level of Application Insight
- *Fails__Enable* : Enable random dynamic fail over 
- *Fails__PerformanceEnable* : Enable random performance issues
- *Worker__Enable* : Enable background worker

Log level supported values:
- Information
- Trace
- Debug
- Warning
- Error 
- Critical