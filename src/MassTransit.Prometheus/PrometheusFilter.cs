using System;
using System.Linq;
using System.Threading.Tasks;
using GreenPipes;
using Nexogen.Libraries.Metrics;
using Nexogen.Libraries.Metrics.Extensions;

namespace MassTransit.Prometheus
{
    public class PrometheusFilter<T> : IFilter<T>
        where T : class, ConsumeContext
    {
        public async Task Send(T context, IPipe<T> next)
        {
            var consumerMetrics = new ConsumerMetrics(context);

            consumerMetrics.CountMessage();
            try
            {
                using (consumerMetrics.ConsumeTimer())
                {
                    await next.Send(context).ConfigureAwait(false);
                }

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