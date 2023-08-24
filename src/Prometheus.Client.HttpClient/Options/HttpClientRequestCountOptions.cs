using System;
using System.Collections.Generic;
using System.Text;

namespace Prometheus.Client.HttpClient.Options
{
    public sealed class HttpClientRequestCountOptions : HttpClientMetricsOptionsBase
    {
        /// <summary>
        /// Set this to use a custom counter metric instead of default one.
        /// </summary>
        public IMetricFamily<ICounter> Counter { get; set; }
    }
}
