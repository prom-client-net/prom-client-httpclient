using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Prometheus.Client.Collectors;
using Prometheus.Client.HttpClient.Options;

namespace Prometheus.Client.HttpClient.MessageHandlers
{
    public abstract class HttpClientMessageHandlerBase<TMetricFamily, TMetric> : DelegatingHandler
        where TMetricFamily: IMetricFamily<TMetric>
        where TMetric : IMetric
    {
        private readonly string _clientName;
        private readonly TMetricFamily _metric;
        protected readonly IMetricFactory MetricFactory;
        protected abstract string[] Labels { get; }
        protected abstract TMetricFamily CreateMetricInstance(string[] labels);
        protected HttpClientMessageHandlerBase(HttpClientMetricsOptions options,IMetricFactory metricFactory, string clientName)
        {
            _clientName = clientName;
            MetricFactory = metricFactory;
            _metric = CreateMetricInstance(Labels);
        }

        protected TMetric CreateMetric(HttpRequestMessage httpRequest, HttpResponseMessage httpResponse)
        {
            if (!_metric.LabelNames.Any())
                return _metric.Unlabelled;

            var labelValues = new string[_metric.LabelNames.Count];

            for (int i = 0; i < labelValues.Length; i++)
            {
                switch (_metric.LabelNames[i])
                {
                    case Constants.Host:
                        labelValues[i] = httpRequest?.RequestUri?.Host;
                        break;
                    case Constants.Method:
                        labelValues[i] = httpRequest?.Method.Method;
                        break;
                    case Constants.Client:
                        labelValues[i] = _clientName;
                        break;
                    case Constants.StatusCode:
                        labelValues[i] = Convert.ToString(httpResponse?.StatusCode) ?? string.Empty;
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }

            return _metric.WithLabels();
        }
    }
}
