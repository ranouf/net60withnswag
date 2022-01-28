using SK.Extensions;
using Autofac;
using Autofac.Core;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace SK.Settings
{
    public class SettingsValidator
    {
        /// <summary>
        /// Validate the IOptions when the API launchs.
        /// Note: the validation is made only when the IOptions returns the value
        /// </summary>
        public SettingsValidator(
            IServiceProvider services,
            ILifetimeScope scope
        )
        {
            var types = scope.ComponentRegistry.Registrations
                .SelectMany(e => e.Services)
                .Select(s => s as TypedService)
                .Where(s => s.ServiceType.IsAssignableToGenericType(typeof(IConfigureOptions<>)))
                .Select(s => s.ServiceType.GetGenericArguments()[0])
                .Where(s => s.Name.EndsWith("Settings"))
                .ToList();

            foreach (var t in types)
            {
                var option = services.GetService(typeof(IOptions<>).MakeGenericType(new Type[] { t }));
                option.GetPropertyValue("Value");
            }
        }
    }
}
