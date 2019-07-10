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

        internal static void TryConfigure(string serviceName, PrometheusMetricsOptions options)
        {
            if (_isConfigured) return;

            _serviceName = serviceName;
            var labels = new[] {options.ServiceNameLabel, options.MessageTypeLabel};

            _consumerTimer = Metrics.CreateHistogram(
                options.ConsumerTimerMetricName,
                "The time to consume a message, in seconds.",
                new HistogramConfiguration
                {
                    LabelNames = labels,
                    Buckets = options.HistogramBuckets
                });

            _criticalTimer = Metrics.CreateHistogram(
                options.CriticalTimerMetricName,
                "The time between when message is sent and when it is consumed, in seconds.",
                new HistogramConfiguration
                {
                    LabelNames = labels,
                    Buckets = options.HistogramBuckets
                });

            _messageCounter = Metrics.CreateCounter(
                options.MessageCounterMetricName,
                "The number of messages received.",
                new CounterConfiguration {LabelNames = labels});

            _errorCounter = Metrics.CreateCounter(
                options.ErrorCounterMetricName,
                "The number of message processing failures.",
                new CounterConfiguration {LabelNames = labels});

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

        static bool _isConfigured;
        static string _serviceName;
        static Histogram _consumerTimer;
        static Histogram _criticalTimer;
        static Counter _messageCounter;
        static Counter _errorCounter;
    }
}