using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GreenPipes;
using static System.String;

namespace MassTransit.Prometheus
{
    public static class PrometheusMetricsConfiguration
    {
        public static void UsePrometheusMetrics<T>(this IPipeConfigurator<T> configurator,
            Action<PrometheusMetricsOptions> configureMetrics = null,
            string serviceName = "",
            ParseMessageType parseMessageType = null)
            where T : class, ConsumeContext
        {
            ConfigureMetrics(serviceName, configureMetrics);

            configurator.AddPipeSpecification(new PrometheusSpecification<T>(parseMessageType ?? DefaultMessageTypeParser));
        }

        public static void ConnectMetrics(this IBusControl busControl,
            Action<PrometheusMetricsOptions> configureMetrics = null,
            string serviceName = "")
        {
            ConfigureMetrics(serviceName, configureMetrics);

            busControl.ConnectReceiveObserver(new PrometheusMetricsObservers());
        }

        static void ConfigureMetrics(string serviceName, Action<PrometheusMetricsOptions> configure)
        {
            var options = PrometheusMetricsOptions.Default;
            configure?.Invoke(options);

            PrometheusMetrics.TryConfigure(GetServiceName(serviceName), options);
        }

        static string GetServiceName(string serviceName)
            => IsNullOrWhiteSpace(serviceName)
                ? Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName)
                : serviceName;

        static string DefaultMessageTypeParser(IEnumerable<string> supportedMessageTypes)
        {
            return ExtractMessageType(supportedMessageTypes.FirstOrDefault() ?? "unknown");

            string ExtractMessageType(string urnMessageTypeName)
            {
                if (!urnMessageTypeName.Contains(':')) return urnMessageTypeName;
                try
                {
                    return urnMessageTypeName.Split(':').Last().Split('.').Last().Replace('+', '.');
                }
                catch (Exception)
                {
                    return urnMessageTypeName;
                }
            }
        }
    }

    public delegate string ParseMessageType(IEnumerable<string> supportedMessageTypes);
}