using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Prometheus;

namespace MassTransit.Prometheus
{
    internal static class PrometheusMetrics
    {
        internal static IHistogram ConsumeTimer(string messageType)
            => _consumerTimer.Labels(_serviceName, messageType);

        internal static IHistogram CriticalTimer(string messageType)
            => _criticalTimer.Labels(_serviceName, messageType);

        internal static ICounter MessageCounter(string messageType)
            => _messageCounter.Labels(_serviceName, messageType);

        internal static ICounter ErrorCounter(string messageType)
            => _errorCounter.Labels(_serviceName, messageType);

        internal static void TryConfigure(string serviceName)
        {
            if (_isConfigured) return;

            var bounds = new[]
                {0, .005, .01, .025, .05, .075, .1, .25, .5, .75, 1, 2.5, 5, 7.5, 10, 30, 60, 120, 180, 240, 300};

            _serviceName = serviceName;

            _consumerTimer = Metrics.CreateHistogram(
                "app_message_processing_time_seconds",
                "The time to consume a message, in seconds.",
                new HistogramConfiguration
                {
                    LabelNames = new[] {"service_name", "message_type"},
                    Buckets = bounds
                });

            _criticalTimer = Metrics.CreateHistogram(
                "app_message_critical_time_seconds",
                "The time between when message is sent and when it is consumed, in seconds.",
                new HistogramConfiguration
                {
                    LabelNames = new[] {"service_name", "message_type"},
                    Buckets = bounds
                });

            _messageCounter = Metrics.CreateCounter(
                "app_message_count",
                "The number of messages received.",
                new CounterConfiguration { LabelNames = new[] {"service_name", "message_type"} });

            _errorCounter = Metrics.CreateCounter(
                "app_message_failures_count",
                "The number of message processing failures.",
                new CounterConfiguration{LabelNames = new []{"service_name", "message_type"}});

            _isConfigured = true;
        }

        internal static async Task MeasureConsume(Func<Task> action, string messageType)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            await action();

            stopwatch.Stop();
            ConsumeTimer(messageType).Observe(stopwatch.ElapsedTicks / (double) Stopwatch.Frequency);
        }

        private static bool _isConfigured;
        private static string _serviceName;
        private static Histogram _consumerTimer;
        private static Histogram _criticalTimer;
        private static Counter _messageCounter;
        private static Counter _errorCounter;
    }
}