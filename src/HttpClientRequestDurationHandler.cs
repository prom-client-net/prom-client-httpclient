using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Prometheus.Client.HttpClient;

public class HttpClientRequestDurationHandler(IMetricFactory metricFactory, string client) : DelegatingHandler
{
    internal const string MetricName = "httpclient_request_duration_seconds";
    internal const string MetricHelp = "Duration histogram of HTTP requests performed by an HttpClient.";

    private readonly IMetricFamily<IHistogram, (string method, string host, string client, string statusCode)> _requestDuration =
        metricFactory.CreateHistogram(MetricName, MetricHelp, (LabelNames.Method, LabelNames.Host, LabelNames.Client, LabelNames.StatusCode));

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var stopWatch = Stopwatch.StartNew();
        HttpResponseMessage response = null;
        var method = request.Method.Method;
        var host = request.RequestUri?.Host ?? string.Empty;

        try
        {
            response = await base.SendAsync(request, cancellationToken);
            return response;
        }
        finally
        {
            stopWatch.Stop();

            var statusCode = response != null ? ((int)response.StatusCode).ToString() : string.Empty;
            _requestDuration
                .WithLabels((method, host, client, statusCode))
                .Observe(stopWatch.Elapsed.TotalSeconds);
        }
    }
}
