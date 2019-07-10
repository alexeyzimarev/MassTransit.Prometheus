using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenPipes;

namespace MassTransit.Prometheus
{
    internal class PrometheusFilter<T> : IFilter<T>
        where T : class, ConsumeContext
    {
        readonly ParseMessageType _parseMessageType;

        internal PrometheusFilter(ParseMessageType parseMessageType)
        {
            _parseMessageType = parseMessageType;
        }

        public async Task Send(T context, IPipe<T> next)
        {
            var consumerMetrics = new ConsumerMetrics(context, _parseMessageType);

            consumerMetrics.CountMessage();
            try
            {
                await consumerMetrics.Measure(() => next.Send(context)).ConfigureAwait(false);

                consumerMetrics.MeasureCriticalTime();
            }
            catch (Exception)
            {
                consumerMetrics.CountError();
                throw;
            }
        }

        public void Probe(ProbeContext context) => context.CreateFilterScope("PrometheusFilter");
    }

    public class PrometheusSpecification<T> : IPipeSpecification<T>
        where T : class, ConsumeContext
    {
        readonly ParseMessageType _parseMessageType;

        public PrometheusSpecification(ParseMessageType parseMessageType)
        {
            _parseMessageType = parseMessageType;
        }

        public void Apply(IPipeBuilder<T> builder)
            => builder.AddFilter(new PrometheusFilter<T>(_parseMessageType));

        public IEnumerable<ValidationResult> Validate() => Enumerable.Empty<ValidationResult>();
    }
}