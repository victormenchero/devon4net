﻿using Microsoft.AspNetCore.Http;
using System.Net;
using Devon4Net.Infrastructure.Logger.Logging;
using System.Text.Json;

namespace Devon4Net.Infrastructure.Logger.Middleware
{
    public class LogExceptionHandlingMiddleware
    {
        private readonly RequestDelegate next;

        public LogExceptionHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await next(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex).ConfigureAwait(false);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            Devon4NetLogger.Error(exception);
            var result = JsonSerializer.Serialize(new { error = exception.Message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(result);
        }
    }
}
