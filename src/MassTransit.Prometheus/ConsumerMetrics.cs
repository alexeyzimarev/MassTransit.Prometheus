using System;
using System.Linq;
using Nexogen.Libraries.Metrics.Extensions;

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

        internal void CountMessage() => PrometheusMetrics.MessageCounter(_messageType).Increment();

        internal void CountError() => PrometheusMetrics.ErrorCounter(_messageType).Increment();

        internal IDisposable ConsumeTimer() => PrometheusMetrics.ConsumeTimer(_messageType).Timer();
    }
}