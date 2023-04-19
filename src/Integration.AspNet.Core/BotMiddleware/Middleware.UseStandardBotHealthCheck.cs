using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Builder;

partial class BotMiddleware
{
    public static IApplicationBuilder UseStandardBotHealthCheck(this IApplicationBuilder appBuilder)
    {
        ArgumentNullException.ThrowIfNull(appBuilder);
        return appBuilder.InternalUseStandardBotHealthCheck();
    }

    internal static IApplicationBuilder InternalUseStandardBotHealthCheck(this IApplicationBuilder appBuilder)
    {
        return appBuilder.Map(
            new PathString("/health"),
            app => app.Use(_ => InvokeHealthCheckAsync));

        static Task InvokeHealthCheckAsync(HttpContext context)
        {
            context.Response.StatusCode = 200;
            return context.Response.WriteAsync("Healthy", context.RequestAborted);
        }
    }
}