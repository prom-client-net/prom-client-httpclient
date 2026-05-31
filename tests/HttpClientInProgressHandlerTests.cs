using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Prometheus.Client.Collectors;
using Xunit;

namespace Prometheus.Client.HttpClient.Tests;

public class HttpClientInProgressHandlerTests
{
    private const string _client = "TestClient";
    private const string _host = "localhost";

    private readonly CollectorRegistry _registry = new();
    private readonly HttpClientInProgressHandler _handler;

    public HttpClientInProgressHandlerTests()
    {
        _handler = new HttpClientInProgressHandler(new MetricFactory(_registry), _client);
    }

    public static IEnumerable<object[]> Actions()
    {
        yield return [() => { }];
        yield return [() => { throw new Exception(); }];
    }

    [Theory]
    [MemberData(nameof(Actions))]
    public async Task SendAsync_With_Request_Logs_InProgress_Request(Action action)
    {
        _handler.InnerHandler = new StubDelegatingHandler(action);
        var invoker = new HttpMessageInvoker(_handler);

        try
        {
            await invoker.SendAsync(new HttpRequestMessage(HttpMethod.Get, $"http://{_host}"), CancellationToken.None);
        }
        catch (Exception)
        {
            // ignored
        }

        Assert.True(_registry.TryGet(HttpClientInProgressHandler.MetricName, out var collector));
        var metric = Assert.IsType<IMetricFamily<IGauge>>(collector, exactMatch: false);

        Assert.Equal(HttpClientInProgressHandler.MetricHelp, Assert.IsType<MetricConfiguration>(collector.Configuration, exactMatch: false).Help);
        Assert.Equal([LabelNames.Method, LabelNames.Host, LabelNames.Client], metric.LabelNames);
        Assert.DoesNotContain(LabelNames.StatusCode, metric.LabelNames);

        var (labels, gauge) = metric.Labelled.Single();
        Assert.Equal([HttpMethod.Get.ToString(), _host, _client], labels);
        Assert.Equal(0d, gauge.Value); // incremented on send, decremented on completion
    }

    [Fact]
    public async Task SendAsync_WithoutRequestUri_RecordsEmptyHost()
    {
        _handler.InnerHandler = new StubDelegatingHandler(() => { });
        var invoker = new HttpMessageInvoker(_handler);

        await invoker.SendAsync(new HttpRequestMessage { Method = HttpMethod.Get }, CancellationToken.None);

        Assert.True(_registry.TryGet(HttpClientInProgressHandler.MetricName, out var collector));
        var metric = Assert.IsType<IMetricFamily<IGauge>>(collector, exactMatch: false);

        var (labels, _) = metric.Labelled.Single();
        Assert.Equal([HttpMethod.Get.ToString(), string.Empty, _client], labels);
    }
}
