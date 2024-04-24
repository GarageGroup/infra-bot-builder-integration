using System;
using System.Threading.Tasks;
using GarageGroup.Infra.Bot.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;

namespace Microsoft.AspNetCore.Builder;

partial class BotMiddleware
{
    public static TApplicationBuilder UseBot<TApplicationBuilder>(
        this TApplicationBuilder appBuilder, Func<IServiceProvider, IBot> botResolver)
        where TApplicationBuilder : IApplicationBuilder
    {
        ArgumentNullException.ThrowIfNull(appBuilder);
        ArgumentNullException.ThrowIfNull(botResolver);

        return appBuilder.InternalUseBot(botResolver);
    }

    internal static TApplicationBuilder InternalUseBot<TApplicationBuilder>(
        this TApplicationBuilder appBuilder, Func<IServiceProvider, IBot> botResolver)
        where TApplicationBuilder : IApplicationBuilder
    {
        _ = appBuilder.Map(
            new PathString("/api/messages"),
            app => app.Use(_ => InvokeBotAsync));

        return appBuilder;

        Task InvokeBotAsync(HttpContext context)
            =>
            StandardCloudAdapter.Resolve(context.RequestServices).ProcessAsync(
                httpRequest: context.Request,
                httpResponse: context.Response,
                bot: botResolver.Invoke(context.RequestServices),
                cancellationToken: context.RequestAborted);
    }
}