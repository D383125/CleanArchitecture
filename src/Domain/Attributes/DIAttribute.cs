using Microsoft.Extensions.DependencyInjection;

namespace Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
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
