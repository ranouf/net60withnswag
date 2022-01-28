using SK.Settings.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SK.Settings.HealthChecks
{
    public class SettingsHealthCheck<TSettings> : IHealthCheck where TSettings : class, new()
    {
        protected TSettings Settings { get; private set; }
        public SettingsHealthCheck(IOptions<TSettings> options)
        {
            Settings = options.Value;
        }

        public async virtual Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            // Settings Validation
            if (!Settings.IsValid(out var errors))
            {
                var errorMessage = string.Join(", ", errors.Select(vr => vr.ErrorMessage));
                return await Task.FromResult(HealthCheckResult.Unhealthy(
                    description: $"Found {errors.Count()} configuration error(s) in {typeof(TSettings).Name}, error(s): {errorMessage}"
                ));
            }
            return await Task.FromResult(HealthCheckResult.Healthy());
        }
    }
}
