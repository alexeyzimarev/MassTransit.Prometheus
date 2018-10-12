using Nexogen.Libraries.Metrics;

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

        internal static void TryConfigure(IMetrics metrics, string serviceName)
        {
            if (_isConfigured) return;

            var bounds = new[] {0, .005, .01, .025, .05, .075, .1, .25, .5, .75, 1, 2.5, 5, 7.5, 10, 30, 60, 120, 180, 240, 300};

            _serviceName = serviceName;

            _consumerTimer = metrics.Histogram()
                .Buckets(bounds)
                .Name("app_message_processing_time_seconds")
                .Help("The time to consume a message, in seconds.")
                .LabelNames("service_name", "message_type")
                .Register();

            _criticalTimer = metrics.Histogram()
                .Buckets(bounds)
                .Name("app_message_critical_time_seconds")
                .Help("The time between when message is sent and when it is consumed, in seconds.")
                .LabelNames("service_name", "message_type")
                .Register();

            _messageCounter = metrics.Counter()
                .Name("app_message_count")
                .Help("The number of messages received.")
                .LabelNames("service_name", "message_type")
                .Register();

            _errorCounter = metrics.Counter()
                .Name("app_message_failures_count")
                .Help("The number of message processing failures.")
                .LabelNames("service_name", "message_type")
                .Register();

            _isConfigured = true;
        }

        private static bool _isConfigured;
        private static string _serviceName;
        private static ILabelledHistogram _consumerTimer;
        private static ILabelledHistogram _criticalTimer;
        private static ILabelledCounter _messageCounter;
        private static ILabelledCounter _errorCounter;
    }
}