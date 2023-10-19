using System;
using System.Net.Http;

namespace Prometheus.Client.HttpClient.MessageHandlers
{
    public abstract class HttpClientMessageHandlerBase<TMetricFamily, TMetric> : DelegatingHandler
        where TMetricFamily: IMetricFamily<TMetric, ValueTuple<string, string, string, string>>
        where TMetric : IMetric
    {
        private readonly string _clientName;
        private readonly TMetricFamily _metric;
        protected readonly IMetricFactory MetricFactory;
        protected abstract TMetricFamily CreateMetricInstance();
        protected HttpClientMessageHandlerBase(IMetricFactory metricFactory, string clientName)
        {
            _clientName = clientName;
            MetricFactory = metricFactory;
            _metric = CreateMetricInstance();
        }

        protected TMetric CreateMetric(HttpRequestMessage httpRequest, HttpResponseMessage httpResponse)
        {
            var host = httpRequest?.RequestUri?.Host ?? string.Empty;
            var method = httpRequest?.Method?.Method ?? string.Empty;
            var statusCode = GetStatusCode(httpResponse);

            return _metric.WithLabels((host, _clientName, method, statusCode));
        }

        private string GetStatusCode(HttpResponseMessage httpResponse)
        {
            if (httpResponse == null)
                return string.Empty;

            return Convert.ToString((int)httpResponse.StatusCode);
        }
    }
}
