using SK.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SK.Settings.Extensions
{
    public static class IServiceCollectionExtensions
    {
        const string settings = "Settings";
        public static IServiceCollection ConfigureAndValidate<T>(
            this IServiceCollection serviceCollection,
            IConfiguration config,
            string section = null
        ) where T : class
        {
            var configType = typeof(T).Name;
            if (string.IsNullOrEmpty(section))
            {
                section = configType;
                if (section.EndsWith(settings))
                {
                    section = section.Remove(configType.Length - settings.Length); // ex: ConfigurationSettings => Configuration
                }
            }

            return serviceCollection
                .Configure<T>(config.GetSection(section))
                .PostConfigure<T>(settings =>
                {
                    if (!settings.IsValid(out var configErrors))
                    {
                        var errorMessages = configErrors.Select(rv => rv.ErrorMessage);
                        var aggrErrors = string.Join(",", errorMessages);
                        var count = errorMessages.Count();
                        throw new ApplicationException($"Found {count} configuration error(s) in {configType}: {aggrErrors}, settings:'{settings.ToJson()}'");
                    }
                });
        }

        public static bool IsValid(this object obj, out ICollection<ValidationResult> results)
        {
            var context = new ValidationContext(obj, serviceProvider: null, items: null);
            results = new List<ValidationResult>();
            return Validator.TryValidateObject(obj, context, results, true); ;
        }
    }
}
