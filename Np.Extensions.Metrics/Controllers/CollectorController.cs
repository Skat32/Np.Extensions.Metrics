using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Np.Extensions.Metrics.Settings;
using Serilog;
using Serilog.Core;

namespace Np.Extensions.Metrics.Controllers;

/// <summary>
/// Управление сборщиком статистики
/// </summary>
[Route("api-collector")]
[ApiController]
public abstract class CollectorController : ControllerBase
{
    private readonly ILogger _logger =
        Log.Logger.ForContext(Constants.SourceContextPropertyName, nameof(CollectorController));
    private readonly MetricsSettings _settings;
    
    /// ctor
    protected CollectorController(IOptions<MetricsSettings> options)
    {
        _settings = options.Value;
    }
    
    /// <summary>
    /// Включить сбрщик статистики
    /// </summary>
    [HttpGet("enable")]
    public async Task<JsonResult> EnableAsync()
    {
        if (MetricsExtensions.Collector != null)
            return new JsonResult(new { Status = "Failed - already enabled"}) { StatusCode = (int)HttpStatusCode.InternalServerError};

        MetricsExtensions.CreateCollector(_logger, _settings);
            
        return new JsonResult(new { Status = "Ok- started and assigned collector"});
    }
        
    /// <summary>
    /// Выключить сбрщик статистики
    /// </summary>
    [HttpGet("disable")]
    public async Task<JsonResult> DisableAsync()
    {
        if (MetricsExtensions.Collector == null)
            return new JsonResult(new { Status = "Failed - already disable"}) { StatusCode = (int)HttpStatusCode.InternalServerError};

        MetricsExtensions.Collector.Dispose();
        MetricsExtensions.Collector = null;
            
        return new JsonResult(new { Status = "Ok- stopped the collector"});
    }
}