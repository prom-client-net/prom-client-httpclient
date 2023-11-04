using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Prometheus.Client.HttpClient.MessageHandlers
{
    public class HttpClientRequestDurationHandler : HttpClientMessageHandlerBase<IMetricFamily<IHistogram, ValueTuple<string,string,string,string>>, IHistogram>
    {
        public HttpClientRequestDurationHandler(IMetricFactory metricFactory, string clientName)
            : base(metricFactory, clientName)
        {
        }

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

        protected override IMetricFamily<IHistogram, ValueTuple<string, string, string, string>> CreateMetricInstance() => MetricFactory.CreateHistogram(Constants.RequestDurationMetricName, Constants.RequestDurationMetricHelp, (Constants.Host, Constants.Client, Constants.Method, Constants.StatusCode));
    }
}
