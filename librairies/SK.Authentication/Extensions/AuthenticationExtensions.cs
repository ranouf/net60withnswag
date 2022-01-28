using SK.Authentication.Configuration;
using SK.Authentication.Helper;
using SK.Settings.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SK.Authentication.Extensions
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection RegisterAuthentication(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetService<IConfiguration>();
            services.ConfigureAndValidate<AuthenticationSettings>(configuration);

            serviceProvider = services.BuildServiceProvider();
            var authenticationSettings = serviceProvider.GetService<IOptions<AuthenticationSettings>>().Value;

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = authenticationSettings.Issuer,
                        ValidateAudience = true,
                        ValidAudience = authenticationSettings.Audience,
                        ValidateActor = false,
                        RequireExpirationTime = true,
                        ValidateLifetime = true,
                        LifetimeValidator = (before, expires, token, param) => expires > DateTime.UtcNow,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = JWTHelper.GetSigningCredentials(authenticationSettings.SecretKey).Key
                    };
                    o.RequireHttpsMetadata = false;
                    o.IncludeErrorDetails = true;
                    o.SaveToken = true;
                    o.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            // If the request is for our hub...
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hub"))
                            {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                        {
                            var te = context.Exception;
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .RequireClaim(ClaimTypes.NameIdentifier)
                    .Build();
            });
            return services;
        }
    }
}
