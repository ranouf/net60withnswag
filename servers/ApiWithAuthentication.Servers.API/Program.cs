
using ApiWithAuthentication.Domains.Core.Identity.Configuration;
using ApiWithAuthentication.Domains.Core.Identity.Entities;
using ApiWithAuthentication.Domains.Infrastructure.SqlServer;
using ApiWithAuthentication.Librairies.Common;
using ApiWithAuthentication.Servers.API;
using ApiWithAuthentication.Servers.API.Configuration;
using ApiWithAuthentication.Servers.API.Filters;
using ApiWithAuthentication.Servers.API.Middlewares;
using ApiWithAuthentication.Servers.API.SignalR;
using ApiWithAuthentication.Servers.API.SignalR.Hubs;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using SK.Authentication.Extensions;
using SK.Settings.HealthChecks;
using SK.Smtp.Configuration;
using SK.Smtp.HealthChecks;
using SK.Storage.Configuration;
using StackExchange.Profiling;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);


// Dependancy Injection
builder.Services.AddAutofac();
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
{
    builder.RegisterModule<APIModule>();
});

// SQL Server
builder.Services.AddDbContext<SKSamplesDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Default"),
        opt => opt.EnableRetryOnFailure()
    )
);

// Identity
builder.Services
    .AddIdentity<User, Role>(options =>
    {
        options.SignIn.RequireConfirmedEmail = true;
        options.Lockout.AllowedForNewUsers = false;
    })
    .AddEntityFrameworkStores<SKSamplesDbContext>()
    .AddDefaultTokenProviders()
    .AddUserStore<UserStore<User, Role, SKSamplesDbContext, Guid, IdentityUserClaim<Guid>, UserRole, IdentityUserLogin<Guid>, IdentityUserToken<Guid>, IdentityRoleClaim<Guid>>>()
    .AddRoleStore<RoleStore<Role, SKSamplesDbContext, Guid, UserRole, IdentityRoleClaim<Guid>>>();

//Caching response for middlewares
builder.Services.AddResponseCaching();
builder.Services.AddMemoryCache();

// Authentication
builder.Services.RegisterAuthentication();

// SignalR
builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, UserIdProvider>();

// Insights
builder.Services.AddApplicationInsightsTelemetry(builder.Configuration);

// Add framework builder.Services.
builder.Services.AddControllers(options =>
{
    //options.EnableEndpointRouting = false;
    options.Filters.AddService(typeof(ApiExceptionFilter));
    options.Filters.Add(typeof(ValidateModelStateAttribute));
}).AddNewtonsoftJson(options =>
{
    options.SerializerSettings.Converters.Add(new StringEnumConverter());
    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
});

// Swagger-ui 
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = $"{Constants.Project.Name} API",
        Version = "v1",
        Description = $"Welcome to the marvellous {Constants.Project.Name} API!",
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                },
                new[] { "readAccess", "writeAccess" }
            }
        });
});
builder.Services.AddSwaggerDocument();

// HealthCheck
builder.Services.AddHealthChecksUI(setupSettings: options =>
{
    var settings = HealthCheckSettings.FromConfiguration(builder.Configuration);
    if (settings != null)
    {
        options.AddHealthCheckEndpoint(
            settings.Name,
            settings.Uri
        );
        options.SetEvaluationTimeInSeconds(settings.EvaluationTimeinSeconds);
        options.SetMinimumSecondsBetweenFailureNotifications(settings.MinimumSecondsBetweenFailureNotifications);
    }
}).AddInMemoryStorage();

builder.Services.AddHealthChecks()
    .AddCheck<SmtpHealthCheck>("SMTP")
    .AddAzureBlobStorage(builder.Configuration["AzureStorage:ConnectionString"])
    .AddCheckSettings<IdentitySettings>()
    .AddCheckSettings<SmtpSettings>()
    .AddCheckSettings<AzureStorageSettings>()
    .AddDbContextCheck<SKSamplesDbContext>("Default");

// Profiling
builder.Services.AddMemoryCache();
builder.Services.AddMiniProfiler(options => {
    options.PopupRenderPosition = RenderPosition.Left;
    options.RouteBasePath = "/profiler";
    options.ColorScheme = StackExchange.Profiling.ColorScheme.Auto;
});

// Automapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Controllers
builder.Services.AddControllers();

// Settings
builder.Services
    .AddOptions()
    .Configure<IdentitySettings>(builder.Configuration.GetSection("Identity"))
    .Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"))
    .Configure<AzureStorageSettings>(builder.Configuration.GetSection("AzureStorage"));

var app = builder.Build();
app.UseRouting();

// Https
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    app.UseHsts();
}

//Authentication
app.UseAuthentication();
app.UseAuthorization();

//Caching
app.UseResponseCaching();

// Swagger-ui
app.UseSwagger(c => c.RouteTemplate = "api-endpoints/{documentName}/swagger.json");
app.UseSwaggerUi3(c =>
{
    c.Path = "/api-endpoints";
    c.DocumentPath = "/api-endpoints/{documentName}/swagger.json";
});
//app.UseSwaggerUI(c =>
//{
//    c.RoutePrefix = "api-endpoints";
//    c.SwaggerEndpoint("v1/swagger.json", $"{Constants.Project.Name} V1");
//    c.DisplayRequestDuration();
//    c.DefaultModelsExpandDepth(-1); 
//    c.DocExpansion(DocExpansion.None);
//    c.IndexStream = () => Assembly.GetEntryAssembly().GetManifestResourceStream($"{Constants.Project.Name}.Servers.API.Assets.SwaggerIndex.html");

//});

// Profiling, url to see last profile check: http://localhost:XXXX/profiler/results
app.UseMiniProfiler();
app.UseMiddleware<RequestMiddleware>();

// HealthCheck
app.UseHealthChecks("/healthchecks", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.UseHealthChecksUI(config =>
{
    config.UIPath = "/api-healthchecks";
    config.AddCustomStylesheet("Assets\\HealthChecks.css");
});

app.UseEndpoints(endpoints =>
{
    // SignalR
    endpoints.MapHub<BaseHub>("/hub");

    // Controllers
    endpoints.MapControllers();
});

// Redirect any non-API calls to the Angular application
// so our application can handle the routing
app.Use(async (context, next) =>
{
    await next();
    if (context.Response.StatusCode == 404
        && !Path.HasExtension(context.Request.Path.Value)
        && !context.Request.Path.Value.StartsWith("/api/"))
    {
        context.Request.Path = "/index.html";
        await next();
    }
});

// Last operations before running the API
await InitializeDataBasesAsync(app.Services);

await app.RunAsync();


static async Task InitializeDataBasesAsync(IServiceProvider services)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        logger.LogInformation("Starting the SQLServerDB initialization.");
        await DbInitializer.InitializeAsync(services, logger);
        logger.LogInformation("The SQLServerDB initialization has been done.");
    }
    catch (Exception e)
    {
        logger.LogError("An error occurred while initialization the SQLServerDB.", e);
        throw;
    }
}

public partial class Program { }
