using System;
using Microsoft.Extensions.DependencyInjection;
using Prometheus.Client.Collectors;

namespace Prometheus.Client.HttpClient;

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
    public static IHttpClientBuilder AddHttpClientMetrics(this IHttpClientBuilder builder, HttpClientMetricsOptions options = null)
    {
        options ??= new HttpClientMetricsOptions();

        builder.AddHttpMessageHandler(sp =>
            new HttpClientInProgressHandler(new MetricFactory(ResolveRegistry(options, sp)), builder.Name));
        builder.AddHttpMessageHandler(sp =>
            new HttpClientRequestDurationHandler(new MetricFactory(ResolveRegistry(options, sp)), builder.Name));

        return builder;
    }

    private static ICollectorRegistry ResolveRegistry(HttpClientMetricsOptions options, IServiceProvider sp)
        => options.CollectorRegistry ??= sp.GetService<ICollectorRegistry>() ?? Metrics.DefaultCollectorRegistry;
}
