using Microsoft.AspNetCore.Builder;
using Np.Extensions.Metrics.Settings;
using Prometheus;
using Prometheus.DotNetRuntime;
using Serilog;

namespace Np.Extensions.Metrics;

/// <summary>
/// Класс расширений для добавление метрик в приложение
/// </summary>
public static class MetricsExtensions
{
    /// <summary>
    /// Сборщик событий
    /// </summary>
    public static IDisposable? Collector;

    /// <summary>
    /// Добавление метрик на получаемые запросы в приложение
    /// </summary>
    /// <param name="app"></param>
    public static void UseInHttpMetrics(this IApplicationBuilder app)
    {
        app.UseMetricServer();
        app.UseHttpMetrics();
        app.UseMiddleware<ResponseMetricMiddleware>();
    }
    
    /// <summary>
    /// Создать сборщик событий
    /// </summary>
    public static void CreateCollector(ILogger logger, MetricsSettings settings)
    {
        var builder = DotNetRuntimeStatsBuilder.Default();
            
        if (!settings.UseDefaultMetrics)
        {
            builder = DotNetRuntimeStatsBuilder.Customize()
                .WithContentionStats(CaptureLevel.Informational)
                .WithGcStats(CaptureLevel.Verbose)
                .WithThreadPoolStats(CaptureLevel.Informational)
                .WithExceptionStats(CaptureLevel.Errors)
                .WithJitStats();
        }
            
        builder 
            .WithErrorHandler(ex => logger.Error(ex, "Unexpected exception occurred in prometheus-net.DotNetRuntime"));

        if (settings.UseDebuggingMetrics)
        {
            logger.Information("Using debugging metrics");
            builder.WithDebuggingMetrics(true);
        }

        logger.Information("Starting prometheus-net.DotNetRuntime...");
            
        Collector = builder
            .StartCollecting();
    }
}