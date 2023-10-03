using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Prometheus.Client.HttpClient.MessageHandlers
{
    public sealed class HttpClientInProgressHandler : HttpClientMessageHandlerBase<IMetricFamily<IGauge, ValueTuple<string, string, string, string>>, IGauge>
    {
        public HttpClientInProgressHandler(IMetricFactory metricFactory, string clientName)
            : base(metricFactory, clientName)
        {
        }

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

        protected override IMetricFamily<IGauge, ValueTuple<string, string, string, string>> CreateMetricInstance() => MetricFactory.CreateGauge(Constants.InProgresMetricName, Constants.InProgresMetricHelp, (Constants.Host, Constants.Client, Constants.Method, Constants.StatusCode));
    }
}
