using Prometheus;

namespace Np.Extensions.Metrics.MediatR;

/// <summary>
/// Промежуточный слой для сбора показателей запроса
/// </summary>
public class HttpOutMetricsHandler : DelegatingHandler
{
    private static readonly Counter HttpRequestsSentTotal = Prometheus.Metrics.CreateCounter(
        "http_requests_sent_total",
        "http_requests_sent_total счетчик для HTTP запросов для внешних сервисов.",
        new CounterConfiguration {LabelNames = new[] {"code", "method", "host", "local_path", "timeout"}});

    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpResponseMessage response;
        try
        {
            response = await base.SendAsync(request, cancellationToken);
            HttpRequestsSentTotal.Labels(((int) response.StatusCode).ToString(), request.Method.Method,
                    request.RequestUri!.Host, request.RequestUri.LocalPath, "false")
                .Inc();
        }
        catch
        {
            
            HttpRequestsSentTotal.Labels("500", request.Method.Method,
                    request.RequestUri!.Host, request.RequestUri.LocalPath, "true")
                .Inc();
            throw;
        }

        return response;
    }
}
