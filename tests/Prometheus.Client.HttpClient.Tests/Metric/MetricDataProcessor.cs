using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prometheus.Client.Collectors;

namespace Prometheus.Client.HttpClient.Tests.Metric
{
    internal static class MetricDataProcessor
    {
        public static MetricData Parse(string data)
        {
            if (string.IsNullOrEmpty(data))
                throw new ArgumentException("Data is null or empty");

            var metricData = new MetricData();

            var result = Fennel.CSharp.Prometheus.ParseText(data).ToList();

            if (result.Count >= 3)
            {
                var metric = result[2] as Fennel.CSharp.Metric;

                metricData.MetricName = metric.MetricName;
                metricData.Lables = metric.Labels.ToDictionary(item => item.Key, item => item.Value);
            }
            else
            {
                throw new ArgumentException("Failed to parse data");
            }
               
            return metricData;
        }

        public async static Task<string> GetMetricsDataAsync(CollectorRegistry collectorRegistry)
        {
            string data = string.Empty;

            using (var stream = new MemoryStream())
            {
                await ScrapeHandler.ProcessAsync(collectorRegistry, stream);
                data = Encoding.UTF8.GetString(stream.ToArray());
            }

            return data;
        }
    }
}
