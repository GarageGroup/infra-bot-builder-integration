using System;
using GGroupp.Infra;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.ApplicationInsights.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GarageGroup.Infra.Bot.Builder;

partial class StandardCloudAdapter
{
    public static StandardCloudAdapter Resolve(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        if (instance is not null)
        {
            return instance;
        }

        lock (lockObject)
        {
            if (instance is not null)
            {
                return instance;
            }

            var botAdapter = InnerCreate(serviceProvider);
            botAdapter.Use(ResolveTelemetryInitializerMiddleware(serviceProvider));

            instance = botAdapter;
        }

        return instance;
    }

    private static StandardCloudAdapter InnerCreate(IServiceProvider serviceProvider)
        =>
        new(
            configuration: serviceProvider.GetRequiredService<IConfiguration>(),
            handlerProvider: serviceProvider.GetService<ISocketsHttpHandlerProvider>(),
            loggerFactory: serviceProvider.GetService<ILoggerFactory>());

    private static TelemetryInitializerMiddleware ResolveTelemetryInitializerMiddleware(IServiceProvider serviceProvider)
        =>
        new(
            httpContextAccessor: serviceProvider.GetRequiredService<IHttpContextAccessor>(),
            telemetryLoggerMiddleware: new(
                telemetryClient: serviceProvider.GetRequiredService<IBotTelemetryClient>()));
}