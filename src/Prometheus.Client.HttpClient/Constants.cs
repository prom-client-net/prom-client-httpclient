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

        public const string CountMetricName = "httpclient_requests_sent_total";
        public const string CountMetricHelp = "Count of HTTP requests that have been completed by an HttpClient.";
        public const string InProgresMetricName = "httpclient_requests_in_progress";
        public const string InProgresMetricHelp = "Number of requests currently being executed by an HttpClient.";
        public const string RequestDurationMetricName = "httpclient_request_duration_seconds";
        public const string RequestDurationMetricHelp = "Duration histogram of HTTP requests performed by an HttpClient.";


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
