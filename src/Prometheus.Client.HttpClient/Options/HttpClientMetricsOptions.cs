using System;
using System.Collections.Generic;
using System.Text;
using Prometheus.Client.Collectors;

namespace Prometheus.Client.HttpClient.Options
{
    public class HttpClientMetricsOptions
    {
        /// <summary>
        /// Allows you to override the registry for default metric.
        /// Value will be ignored if a custom metric is set.
        /// </summary>
        public ICollectorRegistry CollectorRegistry { get; set; }
    }
}
