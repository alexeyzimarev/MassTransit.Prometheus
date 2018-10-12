using System.Collections.Generic;
using System.Linq;
using GreenPipes;
using Nexogen.Libraries.Metrics;

namespace MassTransit.Prometheus
{
    public class PrometheusSpecification<T> : IPipeSpecification<T>
        where T : class, ConsumeContext
    {
        public void Apply(IPipeBuilder<T> builder)
            => builder.AddFilter(new PrometheusFilter<T>());

        public IEnumerable<ValidationResult> Validate() => Enumerable.Empty<ValidationResult>();
    }
}