using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Prometheus.Client.HttpClient.MessageHandlers;
using Prometheus.Client.HttpClient.Options;

namespace Prometheus.Client.HttpClient
{
    public static class HttpClientMetricsExtensions
    {
        /// <summary>
        /// Configures the HttpClient pipeline to collect Prometheus metrics.
        /// </summary>
        public static IHttpClientBuilder UseHttpClientMetrics(this IHttpClientBuilder builder, Action<HttpClientMetricsOptions> configure)
        {
            var options = new HttpClientMetricsOptions();

            configure?.Invoke(options);

            builder.UseHttpClientMetrics(options);

            return builder;
        }

        /// <summary>
        /// Configures the HttpClient pipeline to collect Prometheus metrics.
        /// </summary>
        public static IHttpClientBuilder UseHttpClientMetrics(this IHttpClientBuilder builder, HttpClientMetricsOptions options = null)
        {
            options ??= new HttpClientMetricsOptions();

            var metricFactory = new MetricFactory(options.CollectorRegistry ?? new Collectors.CollectorRegistry());
            
            //builder = builder.AddHttpMessageHandler(x => new HttpClientRequestCountHandler(options, metricFactory, builder.Name));
            
            return builder;
        }
    }
}
