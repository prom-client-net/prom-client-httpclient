using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Prometheus.Client.HttpClient.Tests;

internal sealed class StubDelegatingHandler(Action onSend) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        onSend();
        return Task.FromResult(new HttpResponseMessage());
    }
}
