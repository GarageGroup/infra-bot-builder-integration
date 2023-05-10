using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Builder;

partial class BotMiddleware
{
    public static TApplicationBuilder UseStandardBotHealthCheck<TApplicationBuilder>(this TApplicationBuilder appBuilder)
        where TApplicationBuilder : IApplicationBuilder
    {
        ArgumentNullException.ThrowIfNull(appBuilder);
        return appBuilder.InternalUseStandardBotHealthCheck();
    }

    internal static TApplicationBuilder InternalUseStandardBotHealthCheck<TApplicationBuilder>(this TApplicationBuilder appBuilder)
        where TApplicationBuilder : IApplicationBuilder
    {
        _ = appBuilder.Map(
            new PathString("/health"),
            app => app.Use(_ => InvokeHealthCheckAsync));

        return appBuilder;

        static Task InvokeHealthCheckAsync(HttpContext context)
        {
            context.Response.StatusCode = 200;
            return context.Response.WriteAsync("Healthy", context.RequestAborted);
        }
    }
}