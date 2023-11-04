using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fennel.CSharp;
using FluentAssertions;
using NSubstitute;
using NSubstitute.Routing.Handlers;
using Prometheus.Client.HttpClient.MessageHandlers;
using Prometheus.Client.HttpClient.Tests.Metric;
using static System.Collections.Specialized.BitVector32;

namespace Prometheus.Client.HttpClient.Tests.MessageHandlers
{
    public class HttpClientRequestDurationHanlderTests : HandlerTestsBase
    {
        public HttpClientRequestDurationHanlderTests()
            :base()
        {
            handler = new HttpClientRequestDurationHandler(new MetricFactory(collectorRegistry), CLIENT_NAME);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public async Task HttpClientRequestDurationHanlder_With_Request_Logs_Request_Duration(Action action, string statusCode)
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

            metricData.MetricName.Should().Contain(Constants.RequestDurationMetricName);
            metricData.Lables.Should().HaveCountGreaterThanOrEqualTo(4);
            metricData.Lables[Constants.Host].Should().Be(HOST);
            metricData.Lables[Constants.Client].Should().Be(CLIENT_NAME);
            metricData.Lables[Constants.Method].Should().Be(HttpMethod.Get.ToString());
            metricData.Lables[Constants.StatusCode].Should().Be(statusCode);
        }
    }
}
