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
        public Type? ImplimentingType { get; }

        public DIAttribute(Type implimentingType = null!, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            Lifetime = lifetime;
            ImplimentingType = implimentingType;
        }
    }
}
