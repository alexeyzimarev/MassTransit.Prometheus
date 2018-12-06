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

We use `prometheus-net` library as a Prometheus client since it is the one that is mentioned on the Prometheus client list.

## Usage

Below you can find out how to add metrics to your ASP.NET Core application and register MassTransit metrics.

### Install packages

First, you need to add metrics to your application. Start by adding the client library to your project using a NuGet package:

```
dotnet add package prometheus-net
dotnet add package MassTransit.Prometheus
```

Metrics need to be available via an HTTP endpoint, by default it is `/metrics`. Add the ASP.NET Core integration package to get this endpoint out-of-the-box:

```
dotnet add package prometheus-net.AspCore
``` 

Then, call our static method `PrometheusMetrics.TryConfigure` to have metrics initialised, then use one of two ways to register metrics for MassTransit.

> IMPORTANT: Don't use both ways, otherwise you get all counters doubled.

### Using middleware

Middleware connects to the consume pipeline.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    PrometheusMetrics.TryConfigure("my_service");
    
    // this registration is simplified
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
    
    // other services
}
```

### Using observer

Observer needs to be connected to the `IBusControl` instance before starting the bus.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    PrometheusMetrics.TryConfigure("my_service");
    
    var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
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
        });
    busControl.Connect

    // this registration is simplified
    services.AddSingleton(bus);
    
    // other services
}
```

### Final step

Finally, add Prometheus integration to the service configuration:

```csharp
public void Configure(IApplicationBuilder app)
{
    app.UseMetricServer();
}
```


