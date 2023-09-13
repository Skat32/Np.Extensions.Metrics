namespace Np.Extensions.Metrics.Settings;

/// <summary>
/// Настройки для метрик
/// </summary>
public class MetricsSettings
{
    /// <summary>
    /// Включен ли сбор метрик для приложения
    /// </summary>
    public bool EnableMetrics { get; set; } = true;
    
    /// <summary>
    /// Использовать ли только стандартные метрики
    /// </summary>
    public bool UseDefaultMetrics { get; set; } = false;
    
    /// <summary>
    /// Исопльзовать ли метрики для отладки
    /// </summary>
    /// <remarks>
    /// Включение отладки приведет к появлению двух показателей:
    /// 1. dotnet_debug_events_total - отслеживает объем событий, обрабатываемых каждым statscollector
    /// 2. dotnet_debug_cpu_seconds_total - отслеживает (примерно) количество процессора, потребляемого каждым сборщиком статистики.
    /// </remarks>
    public bool UseDebuggingMetrics { get; set; } = false;
}