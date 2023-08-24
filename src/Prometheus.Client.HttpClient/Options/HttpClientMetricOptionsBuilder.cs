using System;
using System.Collections.Generic;
using System.Text;

namespace Prometheus.Client.HttpClient.Options
{
    public sealed class HttpClientMetricOptionsBuilder
    {
        public HttpClientRequestCountOptions RequestCount { get; set; } = new HttpClientRequestCountOptions();
    }
}
