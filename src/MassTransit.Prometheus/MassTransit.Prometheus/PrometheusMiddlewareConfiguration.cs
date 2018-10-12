using System.Diagnostics;
using System.IO;
using GreenPipes;
using Nexogen.Libraries.Metrics;

namespace MassTransit.Prometheus
{
    public static class PrometheusMiddlewareConfiguration
    {
        public static void UsePrometheusMetrics<T>(this IPipeConfigurator<T> configurator,
            IMetrics metrics,
            string serviceName = null)
            where T : class, ConsumeContext
        {
            PrometheusMetrics.TryConfigure(metrics,
                string.IsNullOrWhiteSpace(serviceName)
                    ? Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName)
                    : serviceName);

            configurator.AddPipeSpecification(new PrometheusSpecification<T>());
        }
    }
}