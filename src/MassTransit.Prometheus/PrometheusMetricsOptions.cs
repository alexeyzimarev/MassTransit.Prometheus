namespace MassTransit.Prometheus
{
    public class PrometheusMetricsOptions
    {
        public string ServiceNameLabel { get; set; }
        public string MessageTypeLabel { get; set; }
        public double[] HistogramBuckets { get; set; }
        public string ConsumerTimerMetricName { get; set; }
        public string CriticalTimerMetricName { get; set; }
        public string MessageCounterMetricName { get; set; }
        public string ErrorCounterMetricName { get; set; }

        public static PrometheusMetricsOptions Default
            => new PrometheusMetricsOptions
            {
                ServiceNameLabel = "service_name",
                MessageTypeLabel = "message_type",
                HistogramBuckets = new[] {0, .005, .01, .025, .05, .075, .1, .25, .5, .75, 1, 2.5, 5, 7.5, 10, 30, 60, 120, 180, 240, 300},
                ConsumerTimerMetricName = "app_message_processing_time_seconds",
                CriticalTimerMetricName = "app_message_critical_time_seconds",
                MessageCounterMetricName = "app_message_count",
                ErrorCounterMetricName = "app_message_failures_count"
            };
    }
}