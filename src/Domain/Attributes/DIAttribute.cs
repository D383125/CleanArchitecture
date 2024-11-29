using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Attributes
{
    public class DIAttribute : Attribute
    {
        public ServiceLifetime Lifetime { get; }

        public DIAttribute(ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
                Lifetime = lifetime;
        }
    }
}
