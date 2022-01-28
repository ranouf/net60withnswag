using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace SK.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static bool Remove<T>(this IServiceCollection services)
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(T));

            var result = false;
            if (descriptor != null)
            {
                result = services.Remove(descriptor);
            }
            return result;
        }
    }
}
