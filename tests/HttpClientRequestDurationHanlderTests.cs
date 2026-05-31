using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Prometheus.Client.Collectors;
using Xunit;

namespace Prometheus.Client.HttpClient.Tests;

public class HttpClientRequestDurationHanlderTests
{
    private const string _client = "TestClient";
    private const string _host = "localhost";

    private readonly CollectorRegistry _registry = new();
    private readonly HttpClientRequestDurationHandler _handler;

    public HttpClientRequestDurationHanlderTests()
    {
        _handler = new HttpClientRequestDurationHandler(new MetricFactory(_registry), _client);
    }

    public static IEnumerable<object[]> Requests()
    {
        yield return [() => { }, "200"];
        yield return [() => { throw new Exception(); }, ""];
    }

    [Theory]
    [MemberData(nameof(Requests))]
    public async Task SendAsync_With_Request_Logs_Request_Duration(Action action, string statusCode)
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

        Assert.True(_registry.TryGet(HttpClientRequestDurationHandler.MetricName, out var collector));
        var metric = Assert.IsType<IMetricFamily<IHistogram>>(collector, exactMatch: false);

        Assert.Equal(HttpClientRequestDurationHandler.MetricHelp, Assert.IsType<MetricConfiguration>(collector.Configuration, exactMatch: false).Help);
        Assert.Equal([LabelNames.Method, LabelNames.Host, LabelNames.Client, LabelNames.StatusCode], metric.LabelNames);

        var (labels, histogram) = metric.Labelled.Single();
        Assert.Equal([HttpMethod.Get.ToString(), _host, _client, statusCode], labels);
        Assert.Equal(1, histogram.Value.Count);
    }

    [Fact]
    public async Task SendAsync_WithoutRequestUri_RecordsEmptyHost()
    {
        _handler.InnerHandler = new StubDelegatingHandler(() => { });
        var invoker = new HttpMessageInvoker(_handler);

        await invoker.SendAsync(new HttpRequestMessage { Method = HttpMethod.Get }, CancellationToken.None);

        Assert.True(_registry.TryGet(HttpClientRequestDurationHandler.MetricName, out var collector));
        var metric = Assert.IsType<IMetricFamily<IHistogram>>(collector, exactMatch: false);

        var (labels, _) = metric.Labelled.Single();
        Assert.Equal([HttpMethod.Get.ToString(), string.Empty, _client, "200"], labels);
    }
}
