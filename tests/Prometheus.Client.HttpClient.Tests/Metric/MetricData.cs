using System;
using System.Collections.Generic;
using System.Text;

namespace Prometheus.Client.HttpClient.Tests.Metric
{
    internal class MetricData
    {
        public string MetricName { get; set; }
        public Dictionary<string, string> Lables { get; set; }
    }
}
