using System;
using System.Linq;
using System.Threading.Tasks;

namespace MassTransit.Prometheus
{
    internal class ConsumerMetrics
    {
        private readonly string _messageType;
        private readonly ConsumeContext _context;

        internal ConsumerMetrics(ConsumeContext context)
        {
            _messageType = context.SupportedMessageTypes.FirstOrDefault() ?? "unknown";
            _context = context;
        }

        internal void MeasureCriticalTime()
        {
            if (_context.SentTime != null)
                PrometheusMetrics.CriticalTimer(_messageType)
                    .Observe((DateTime.UtcNow - _context.SentTime.Value).TotalSeconds);
        }

        internal void CountMessage() => PrometheusMetrics.MessageCounter(_messageType).Inc();

        internal void CountError() => PrometheusMetrics.ErrorCounter(_messageType).Inc();

        internal Task Measure(Func<Task> action) => PrometheusMetrics.MeasureConsume(action, _messageType);
    }
}