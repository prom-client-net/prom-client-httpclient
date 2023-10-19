using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Prometheus.Client.HttpClient.MessageHandlers;
using Prometheus.Client.HttpClient.Tests.Metric;

namespace Prometheus.Client.HttpClient.Tests.MessageHandlers
{
    public class HttpClientInProgressHandlerTests : HandlerTestsBase
    {
        public HttpClientInProgressHandlerTests()
        {
            collectorRegistry = new Collectors.CollectorRegistry();
            handler = new HttpClientInProgressHandler(new MetricFactory(collectorRegistry), CLIENT_NAME);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public async Task HttpClientRequestDurationHanlder_With_Request_Logs_InProgress_Request(Action action, string statusCode)
        {
            handler.InnerHandler = CreateMockHandler(action);
            HttpMessageInvoker invoker = new HttpMessageInvoker(handler);

            try
            {
                await invoker.SendAsync(new HttpRequestMessage(HttpMethod.Get, $"http://{HOST}"), new System.Threading.CancellationToken(false));
            }
            catch (Exception)
            { }

            var data = await MetricDataProcessor.GetMetricsDataAsync(collectorRegistry);
            var metricData = MetricDataProcessor.Parse(data);

            metricData.MetricName.Should().Be(Constants.InProgresMetricName);
            metricData.Lables.Should().HaveCountGreaterThanOrEqualTo(4);
            metricData.Lables[Constants.Host].Should().Be(HOST);
            metricData.Lables[Constants.Client].Should().Be(CLIENT_NAME);
            metricData.Lables[Constants.Method].Should().Be(HttpMethod.Get.ToString());
        }
    }
}
