using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Prometheus.Client.HttpClient;

public class HttpClientInProgressHandler(IMetricFactory metricFactory, string client) : DelegatingHandler
{
    internal const string MetricName = "httpclient_requests_in_progress";
    internal const string MetricHelp = "Number of requests currently being executed by an HttpClient.";

    private readonly IMetricFamily<IGauge, (string method, string host, string client)> _inProgress =
        metricFactory.CreateGauge(MetricName, MetricHelp, (LabelNames.Method, LabelNames.Host, LabelNames.Client));

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var method = request.Method.Method;
        var host = request.RequestUri?.Host ?? string.Empty;
        var gauge = _inProgress.WithLabels((method, host, client));

        gauge.Inc();

        try
        {
            return await base.SendAsync(request, cancellationToken);
        }
        finally
        {
            gauge.Dec();
        }
    }
}
