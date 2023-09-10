using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Prometheus.Client.HttpClient.Options;

namespace Prometheus.Client.HttpClient.MessageHandlers
{
    public sealed class HttpClientInProgressHandler : HttpClientMessageHandlerBase<IMetricFamily<IGauge>, IGauge>
    {
        public HttpClientInProgressHandler(HttpClientMetricsOptions options, IMetricFactory metricFactory, string clientName)
            : base(options, metricFactory, clientName)
        {
        }

        protected override string[] Labels => Constants.PreResponseLabels;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;
            var metric = CreateMetric(request, null);

            metric.Inc();

            try
            {
                response = await base.SendAsync(request, cancellationToken);
            }
            finally 
            {
                metric.Dec();
            }

            return response;
        }

        protected override IMetricFamily<IGauge> CreateMetricInstance(string[] labels) => MetricFactory.CreateGauge(Constants.InProgresMetricName, Constants.InProgresMetricHelp, labels);
    }
}
