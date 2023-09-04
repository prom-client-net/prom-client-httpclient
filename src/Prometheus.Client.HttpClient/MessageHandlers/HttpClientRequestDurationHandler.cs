using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Prometheus.Client.HttpClient.Options;

namespace Prometheus.Client.HttpClient.MessageHandlers
{
    public sealed class HttpClientRequestDurationHandler : HttpClientMessageHandlerBase<IMetricFamily<IHistogram>, IHistogram>
    {
        public HttpClientRequestDurationHandler(HttpClientMetricsOptions options, IMetricFactory metricFactory, string clientName)
            : base(options, metricFactory, clientName)
        {
        }

        protected override string[] Labels => Constants.PreResponseLabels;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var stopWatch = Stopwatch.StartNew();

            HttpResponseMessage response = null;

            try
            {
                response = await base.SendAsync(request, cancellationToken);
                return response;
            }
            finally
            {
                stopWatch.Stop();

                WithLabels(request, response).Observe(stopWatch.Elapsed.TotalSeconds);
            }
        }

        protected override IMetricFamily<IHistogram> CreateMetricInstance(string[] labels) => MetricFactory.CreateHistogram(Constants.RequestDurationMetricName, Constants.RequestDurationMetricHelp, labels);
    }
}
