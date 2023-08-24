using System;
using System.Collections.Generic;
using System.Text;

namespace Prometheus.Client.HttpClient
{
    public sealed class HttpClientIdentity
    {
        public string Name { get; }
        public HttpClientIdentity(string name)
        {
            Name = name;
        }
    }
}
