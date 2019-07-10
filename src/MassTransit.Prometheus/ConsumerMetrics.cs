using System;
using System.Linq;
using System.Threading.Tasks;

namespace MassTransit.Prometheus
{
    internal class ConsumerMetrics
    {
        readonly string _messageType;
        readonly ConsumeContext _context;

        internal ConsumerMetrics(ConsumeContext context, ParseMessageType parseMessageType)
        {
            _messageType = parseMessageType(context.SupportedMessageTypes);
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