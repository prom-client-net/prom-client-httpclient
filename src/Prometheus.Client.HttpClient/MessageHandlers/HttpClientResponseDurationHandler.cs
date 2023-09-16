using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Prometheus.Client.HttpClient.Options;

namespace Prometheus.Client.HttpClient.MessageHandlers
{
    public sealed class HttpClientResponseDurationHandler : HttpClientMessageHandlerBase<IMetricFamily<IHistogram>, IHistogram>
    {
        public HttpClientResponseDurationHandler(HttpClientMetricsOptions options, IMetricFactory metricFactory, string clientName)
            : base(options, metricFactory, clientName)
        {
        }

        protected override string[] Labels => Constants.AllLabels;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var stopWatch = Stopwatch.StartNew();

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            var stream = await response.Content.ReadAsStreamAsync();
            response.Content = new StreamContent(new CustomStream(stream, () =>
            {
                stopWatch.Stop();
                CreateMetric(request, response).Observe(stopWatch.Elapsed.TotalSeconds);
            }));
            
            return response;
        }

        protected override IMetricFamily<IHistogram> CreateMetricInstance(string[] labels) => MetricFactory.CreateHistogram(Constants.RequestDurationMetricName, Constants.RequestDurationMetricHelp, labels);
    }
}
