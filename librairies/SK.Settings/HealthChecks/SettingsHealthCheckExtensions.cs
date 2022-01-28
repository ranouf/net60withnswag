using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;

namespace SK.Settings.HealthChecks
{
    public static class SettingsHealthCheckExtensions
    {
        public static IHealthChecksBuilder AddCheckSettings<TSettings>(this IHealthChecksBuilder builder, HealthStatus? failureStatus = null, IEnumerable<string> tags = null, TimeSpan? timeout = null) where TSettings : class, new()
        {
            return builder.AddCheck<SettingsHealthCheck<TSettings>>(typeof(TSettings).Name, failureStatus, tags);
        }
    }
}
