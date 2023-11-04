using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Prometheus.Client.Collectors;
using Prometheus.Client.HttpClient.MessageHandlers;

namespace Prometheus.Client.HttpClient
{
    public static class HttpClientMetricsExtensions
    {
        /// <summary>
        /// Configures the HttpClient pipeline to collect Prometheus metrics.
        /// </summary>
        public static IHttpClientBuilder AddHttpClientMetrics(this IHttpClientBuilder builder, Action<HttpClientMetricsOptions> configure)
        {
            var options = new HttpClientMetricsOptions();

            configure?.Invoke(options);

            builder.AddHttpClientMetrics(options);

            return builder;
        }

        /// <summary>
        /// Configures the HttpClient pipeline to collect Prometheus metrics.
        /// </summary>
        public static IHttpClientBuilder AddHttpClientMetrics(this IHttpClientBuilder builder,  HttpClientMetricsOptions options = null)
        {
            options ??= new HttpClientMetricsOptions();
            
            builder = builder.AddHttpMessageHandler(sp =>
            {
                options.CollectorRegistry ??= (ICollectorRegistry)sp.GetService(typeof(ICollectorRegistry)) ?? Metrics.DefaultCollectorRegistry;
                return new HttpClientInProgressHandler(new MetricFactory(options.CollectorRegistry), builder.Name);
            });
            builder = builder.AddHttpMessageHandler(sp =>
            {
                options.CollectorRegistry ??= (ICollectorRegistry)sp.GetService(typeof(ICollectorRegistry)) ?? Metrics.DefaultCollectorRegistry;
                return new HttpClientRequestDurationHandler(new MetricFactory(options.CollectorRegistry), builder.Name);
            });
            
            return builder;
        }
    }
}
