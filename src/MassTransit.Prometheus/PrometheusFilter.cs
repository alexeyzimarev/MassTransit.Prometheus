using System;
using System.Threading.Tasks;
using GreenPipes;

namespace MassTransit.Prometheus
{
    internal class PrometheusFilter<T> : IFilter<T>
        where T : class, ConsumeContext
    {
        public async Task Send(T context, IPipe<T> next)
        {
            var consumerMetrics = new ConsumerMetrics(context);

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
}