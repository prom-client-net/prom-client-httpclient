using System;
using System.Collections.Generic;
using System.Text;

namespace Prometheus.Client.HttpClient
{
    public static class Constants
    {
        public const string Method = "method";
        public const string Host = "host";
        public const string Client = "client";
        public const string StatusCode = "status_code";

        public static readonly string[] AllLabels =
        {
            Method,
            Host,
            Client,
            StatusCode
        };

        public static readonly string[] PreResponseLabels =
        {
            Method,
            Host,
            Client
        };
    }
}
