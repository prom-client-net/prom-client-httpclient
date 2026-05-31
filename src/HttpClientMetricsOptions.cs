using Prometheus.Client.Collectors;

namespace Prometheus.Client.HttpClient;

public class HttpClientMetricsOptions
{
    /// <summary>
    /// The <see cref="ICollectorRegistry"/> instance to use for metric collection.
    /// </summary>
    public ICollectorRegistry CollectorRegistry { get; set; }
}
