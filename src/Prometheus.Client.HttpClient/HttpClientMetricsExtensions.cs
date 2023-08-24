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
        public static IHttpClientBuilder UseHttpClientMetrics(this IHttpClientBuilder builder, Action<HttpClientMetricOptionsBuilder> configure)
        {
            var options = new HttpClientMetricOptionsBuilder();

            configure?.Invoke(options);

            builder.UseHttpClientMetrics(options);

            return builder;
        }

        /// <summary>
        /// Configures the HttpClient pipeline to collect Prometheus metrics.
        /// </summary>
        public static IHttpClientBuilder UseHttpClientMetrics(this IHttpClientBuilder builder, HttpClientMetricOptionsBuilder options = null)
        {
            options ??= new HttpClientMetricOptionsBuilder();

            if (options.RequestCount.Enabled)
            {
                builder = builder.AddHttpMessageHandler(x => new HttpClientRequestCountHandler(options.RequestCount, options.RequestCount.Counter, builder.Name));
            }

            return builder;
        }
    }
}
