using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Builder;

partial class HealthCheckMiddleware
{
    public static IApplicationBuilder UseStandardBotHealthCheck(this IApplicationBuilder appBuilder)
        =>
        InternalUseStandardBotHealthCheck(
            appBuilder ?? throw new ArgumentNullException(nameof(appBuilder)));

    internal static IApplicationBuilder InternalUseStandardBotHealthCheck(this IApplicationBuilder appBuilder)
        =>
        appBuilder.Map(
            new PathString("/health"),
            app => app.Use(_ => InvokeHealthCheckAsync));

    private static Task InvokeHealthCheckAsync(HttpContext context)
    {
        context.Response.StatusCode = 200;
        return context.Response.WriteAsync("Healthy", context.RequestAborted);
    }
}