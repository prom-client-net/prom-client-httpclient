using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;
using NSubstitute;
using System.Threading;
using System.Threading.Tasks;
using Prometheus.Client.Collectors;

namespace Prometheus.Client.HttpClient.Tests.MessageHandlers
{
    public abstract class HandlerTestsBase
    {
        protected const string CLIENT_NAME = "TestClient";
        protected const string HOST = "localhost";
        protected DelegatingHandler handler;
        protected CollectorRegistry collectorRegistry;

        public HandlerTestsBase()
        {
            collectorRegistry = new Collectors.CollectorRegistry();
        }

        protected static DelegatingHandler CreateMockHandler(Action action)
        {
            var handlerMock = Substitute.For<DelegatingHandler>();
            handlerMock
                .GetType()
                .GetMethod("SendAsync", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(handlerMock, new object[] { Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>() })
                .Returns(x =>
                {
                    action();
                    return Task.FromResult(new HttpResponseMessage());
                });
            return handlerMock;
        }

        public static IEnumerable<object[]> TestData()
        {
            yield return new object[] { () => { }, "200" };
            yield return new object[] { () => { throw new Exception(); }, "" };
        }
    }
}
