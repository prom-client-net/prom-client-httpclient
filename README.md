# Prometheus.Client.HttpClient

[![ci](https://img.shields.io/github/actions/workflow/status/prom-client-net/prom-client-httpclient/ci.yml?branch=main&label=ci&logo=github&style=flat-square)](https://github.com/prom-client-net/prom-client-httpclient/actions/workflows/ci.yml)
[![nuget](https://img.shields.io/nuget/v/Prometheus.Client.HttpClient?logo=nuget&style=flat-square)](https://www.nuget.org/packages/Prometheus.Client.HttpClient)
[![nuget](https://img.shields.io/nuget/dt/Prometheus.Client.HttpClient?logo=nuget&style=flat-square)](https://www.nuget.org/packages/Prometheus.Client.HttpClient)
[![codecov](https://img.shields.io/codecov/c/github/prom-client-net/prom-client-httpclient?logo=codecov&style=flat-square)](https://app.codecov.io/gh/prom-client-net/prom-client-httpclient)
[![license](https://img.shields.io/github/license/prom-client-net/prom-client-httpclient?style=flat-square)](https://github.com/prom-client-net/prom-client-httpclient/blob/main/LICENSE)

Extension for [Prometheus.Client](https://github.com/prom-client-net/prom-client) to collect metrics from `HttpClient`.

## Install

```sh
dotnet add package Prometheus.Client.HttpClient
```

## Use

[Examples](https://github.com/prom-client-net/prom-examples)

```c#
services.AddHttpClient("MyClient")
    .AddHttpClientMetrics();
```

Override the collector registry (defaults to the one registered in DI, otherwise `Metrics.DefaultCollectorRegistry`):

```c#
services.AddHttpClient("MyClient")
    .AddHttpClientMetrics(q =>
    {
        q.CollectorRegistry =App registry;
    });
```

Exposed metrics:

* `httpclient_requests_in_progress` - Number of requests currently being executed by an `HttpClient`. Labels: `method`, `host`, `client`.
* `httpclient_request_duration_seconds` - Duration histogram of HTTP requests performed by an `HttpClient`. Labels: `method`, `host`, `client`, `status_code`.

## Contribute

Contributions to the package are always welcome!

* Report any bugs or issues you find on the [issue tracker](https://github.com/prom-client-net/prom-client-httpclient/issues).
* You can grab the source code at the package's [git repository](https://github.com/prom-client-net/prom-client-httpclient).

## License

All contents of this package are licensed under the [MIT license](https://opensource.org/licenses/MIT).
