using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Prometheus.Client.Collectors;
using Xunit;

namespace Prometheus.Client.HttpClient.Tests;

public class HttpClientMetricsExtensionsTests
{
    private const string _client = "test";

    // Building the named client constructs the handler pipeline, which invokes the
    // registration delegates and creates the metrics in the target registry.
    private static void BuildClient(IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();
        _ = provider.GetRequiredService<IHttpClientFactory>().CreateClient(_client);
    }

    [Fact]
    public void AddHttpClientMetrics_WithOptions_RegistersHandlersInProvidedRegistry()
    {
        var registry = new CollectorRegistry();
        var services = new ServiceCollection();

        services.AddHttpClient(_client).AddHttpClientMetrics(new HttpClientMetricsOptions { CollectorRegistry = registry });

        BuildClient(services);

        Assert.True(registry.TryGet(HttpClientInProgressHandler.MetricName, out _));
        Assert.True(registry.TryGet(HttpClientRequestDurationHandler.MetricName, out _));
    }

    [Fact]
    public void AddHttpClientMetrics_WithConfigure_InvokesConfigureAndUsesItsRegistry()
    {
        var registry = new CollectorRegistry();
        var services = new ServiceCollection();

        services.AddHttpClient(_client).AddHttpClientMetrics(options => options.CollectorRegistry = registry);

        BuildClient(services);

        Assert.True(registry.TryGet(HttpClientInProgressHandler.MetricName, out _));
        Assert.True(registry.TryGet(HttpClientRequestDurationHandler.MetricName, out _));
    }

    [Fact]
    public void AddHttpClientMetrics_WithoutRegistry_ResolvesRegistryFromServiceProvider()
    {
        var registry = new CollectorRegistry();
        var services = new ServiceCollection();
        services.AddSingleton<ICollectorRegistry>(registry);

        services.AddHttpClient(_client).AddHttpClientMetrics();

        BuildClient(services);

        Assert.True(registry.TryGet(HttpClientInProgressHandler.MetricName, out _));
        Assert.True(registry.TryGet(HttpClientRequestDurationHandler.MetricName, out _));
    }

    [Fact]
    public void AddHttpClientMetrics_WithNullConfigure_FallsBackToDefaultRegistry()
    {
        var services = new ServiceCollection();

        services.AddHttpClient(_client).AddHttpClientMetrics((Action<HttpClientMetricsOptions>)null);

        BuildClient(services);

        Assert.True(Metrics.DefaultCollectorRegistry.TryGet(HttpClientInProgressHandler.MetricName, out _));
        Assert.True(Metrics.DefaultCollectorRegistry.TryGet(HttpClientRequestDurationHandler.MetricName, out _));
    }
}
