using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
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
        public static IHttpClientBuilder UseHttpClientMetrics(this IHttpClientBuilder builder, IApplicationBuilder applicationBuilder, Action<HttpClientMetricsOptions> configure)
        {
            var options = new HttpClientMetricsOptions();

            configure?.Invoke(options);

            builder.UseHttpClientMetrics(applicationBuilder, options);

            return builder;
        }

        /// <summary>
        /// Configures the HttpClient pipeline to collect Prometheus metrics.
        /// </summary>
        public static IHttpClientBuilder UseHttpClientMetrics(this IHttpClientBuilder builder, IApplicationBuilder applicationBuilder,  HttpClientMetricsOptions options = null)
        {
            options ??= new HttpClientMetricsOptions();
            options.CollectorRegistry ??= (ICollectorRegistry)applicationBuilder.ApplicationServices.GetService(typeof(ICollectorRegistry))
                    ?? Metrics.DefaultCollectorRegistry;

            var metricFactory = new MetricFactory(options.CollectorRegistry);
            
            builder = builder.AddHttpMessageHandler(x => new HttpClientInProgressHandler(metricFactory, builder.Name));
            builder = builder.AddHttpMessageHandler(x => new HttpClientRequestDurationHandler(metricFactory, builder.Name));
            
            return builder;
        }
    }
}
