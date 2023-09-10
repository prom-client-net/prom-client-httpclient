using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Prometheus.Client.HttpClient.Options;

namespace Prometheus.Client.HttpClient.MessageHandlers
{
    public sealed class HttpClientRequestCountHandler : HttpClientMessageHandlerBase<IMetricFamily<ICounter>,  ICounter>
    {
        public HttpClientRequestCountHandler(HttpClientMetricsOptions options, IMetricFactory metricFactory, string clientName)
            : base(options, metricFactory, clientName)
        {
        }

        protected override string[] Labels => Constants.AllLabels;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;

            try
            {
                response = await base.SendAsync(request, cancellationToken);
                return response;
            }
            finally
            {
                CreateMetric(request, response).Inc();
            }
        }

        protected override IMetricFamily<ICounter> CreateMetricInstance(string[] labels) => MetricFactory.CreateCounter(Constants.CountMetricName, Constants.CountMetricHelp, labels);
    }
}
