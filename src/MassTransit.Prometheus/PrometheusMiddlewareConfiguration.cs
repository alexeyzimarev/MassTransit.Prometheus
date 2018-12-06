using System.Diagnostics;
using System.IO;
using GreenPipes;

namespace MassTransit.Prometheus
{
    public static class PrometheusMiddlewareConfiguration
    {
        public static void UsePrometheusMetrics<T>(this IPipeConfigurator<T> configurator,
            string serviceName = null)
            where T : class, ConsumeContext
        {
            PrometheusMetrics.TryConfigure(
                string.IsNullOrWhiteSpace(serviceName)
                    ? Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName)
                    : serviceName);

            configurator.AddPipeSpecification(new PrometheusSpecification<T>());
        }

        public static void ConnectMetrics(this IBusControl busControl, string serviceName = "")
        {
            busControl.ConnectReceiveObserver(new PrometheusMetricsObservers(serviceName));
        }
    }
}