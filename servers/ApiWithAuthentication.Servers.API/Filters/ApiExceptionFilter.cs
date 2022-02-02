using ApiWithAuthentication.Servers.API.Filters.Dtos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ApiWithAuthentication.Servers.API.Filters
{

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public sealed class ApiExceptionFilter : ExceptionFilterAttribute
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ApiExceptionFilter> _logger;

        public ApiExceptionFilter(
            IWebHostEnvironment environment,
            ILogger<ApiExceptionFilter> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            ApiErrorDto apiError;
            var properties = new Dictionary<string, string>{
            };

            if (context.Exception is UnauthorizedAccessException)
            {
                _logger.LogWarning(context.Exception.Message, properties);

                apiError = new ApiErrorDto("Unauthorized Access");
                context.HttpContext.Response.StatusCode = 401;
            }
            else
            {
                _logger.LogError(context.Exception.Message, context.Exception, properties);

                // Unhandled errors
                if (_environment.IsDevelopment())
                {
                    apiError = new ApiErrorDto(
                        context.Exception.GetBaseException().Message,
                        context.Exception.StackTrace
                    );
                }
                else
                {
                    apiError = new ApiErrorDto(
                        "An unhandled error occurred.",
                        null
                    );
                }

                context.HttpContext.Response.StatusCode = 500;
            }

            // always return a JSON result
            context.Result = new JsonResult(apiError);

            base.OnException(context);
        }
    }
}
