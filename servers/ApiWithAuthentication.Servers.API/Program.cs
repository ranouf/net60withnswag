using ApiWithAuthentication.Librairies.Common;
using ApiWithAuthentication.Servers.API;
using ApiWithAuthentication.Servers.API.Configuration;
using ApiWithAuthentication.Servers.API.Filters;
using ApiWithAuthentication.Servers.API.Middlewares;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using StackExchange.Profiling;
using System.IO;

var builder = WebApplication.CreateBuilder(args);


// Dependancy Injection
builder.Services.AddAutofac();
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
{
    builder.RegisterModule<APIModule>();
});

//Caching response for middlewares
builder.Services.AddResponseCaching();
builder.Services.AddMemoryCache();

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
    .AddAzureBlobStorage(builder.Configuration["AzureStorage:ConnectionString"]);

// Profiling
builder.Services.AddMemoryCache();
builder.Services.AddMiniProfiler(options =>
{
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
    .AddOptions();

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

await app.RunAsync();

public partial class Program { }
