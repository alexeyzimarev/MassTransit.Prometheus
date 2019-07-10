using System;
using System.Threading.Tasks;

namespace MassTransit.Prometheus
{
    public class PrometheusMetricsObservers : IReceiveObserver
    {
        public Task PreReceive(ReceiveContext context) => Task.CompletedTask;

        public Task PostReceive(ReceiveContext context) => Task.CompletedTask;

        public Task PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType) where T : class
        {
            var messageType = typeof(T).Name;

            PrometheusMetrics.ConsumeTimer(messageType).Observe(duration.TotalSeconds);
            PrometheusMetrics.MessageCounter(messageType).Inc();

            if (context.SentTime != null)
                PrometheusMetrics.CriticalTimer(messageType)
                    .Observe((DateTime.UtcNow - context.SentTime.Value).TotalSeconds);

            return Task.CompletedTask;
        }

        public Task ConsumeFault<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType,
            Exception exception) where T : class
        {
            var messageType = typeof(T).Name;

            PrometheusMetrics.ErrorCounter(messageType).Inc();

            return Task.CompletedTask;
        }

        public Task ReceiveFault(ReceiveContext context, Exception exception) => Task.CompletedTask;
    }
}