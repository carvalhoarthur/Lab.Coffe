using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

namespace Lab.Coffe.Infrastructure.Logging;

public static class SerilogExtensions
{
    public static void ConfigureSerilog(
        IConfiguration configuration,
        string environment)
    {
        var elasticsearchUrl = configuration["Serilog:Elasticsearch:Uri"] ?? "http://localhost:9200";
        var indexFormat = configuration["Serilog:Elasticsearch:IndexFormat"] ?? "lab-coffe-logs-{0:yyyy.MM.dd}";

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Environment", environment)
            .Enrich.WithProperty("Application", "Lab.Coffe")
            .WriteTo.Console()
            .WriteTo.File("logs/lab-coffe-.log", rollingInterval: RollingInterval.Day)
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticsearchUrl))
            {
                AutoRegisterTemplate = true,
                IndexFormat = indexFormat,
                NumberOfShards = 2,
                NumberOfReplicas = 1
            })
            .CreateLogger();
    }
}
