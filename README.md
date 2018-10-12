# MassTransit metrics for Prometheus

## Description

This library allows you collecting four important metrics for MassTransit consumers and send them to Prometheus. Those metrics are:
* `app_message_processing_time_seconds` - the time it takes to consume one message
* `app_message_critical_time_seconds` - the total time between when the message was sent and when it was consumed
* `app_message_count` - the total number of messages received
* `app_message_failures_count` - the number of messages that failed processing

All metrics are labeled with two labels:
* `service_name` - the name of your application or service
* `message_type` - the type of message for which the metrics are collected

## Prometheus client library

We use `Nexogen.Libraries.Metrics` library as a Prometheus client. The client itself is not included as a reference, we only use the core and the extension libraries.

Check the [GitHub repository](https://github.com/nexogen-international/Nexogen.Libraries.Metrics) for more details about the client library we use.

## Usage

First, you need to add metrics to your application. Start by adding the client library to your project using a NuGet package:

```
dotnet add package Nexogen.Libraries.Metrics.Prometheus
```

Metrics need to be available via an HTTP endpoint, by default it is `/metrics`. Add the ASP.NET Core integration package to get this endpoint out-of-the-box:

```
dotnet add package Nexogen.Libraries.Metrics.Prometheus.AspCore
``` 

Then, create Prometheus metrics object and register it in the services collection. The same instance should be used to configure the bus:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    var metrics = new PrometheusMetrics();
    
    services.AddSingleton(Bus.Factory.CreateUsingRabbitMq(cfg =>
    {
        cfg.UsePrometheusMetrics(metrics);
    
        var host = cfg.Host(new Uri("rabbitmq://localhost"), h => 
        {
            h.UserName("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint(host, "myqueue", e =>
        {
            e.Consumer<MyMessageConsumer>();
        });
    }));
    
    services.AddPrometheus(metrics);
    
    // other services
}
```

Finally, add Prometheus integration to the service configuration:

```csharp
public void Configure(IApplicationBuilder app)
{
    app.UsePrometheus();
}
```


