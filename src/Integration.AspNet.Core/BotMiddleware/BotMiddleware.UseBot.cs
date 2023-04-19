using System;
using System.Threading.Tasks;
using GGroupp.Infra.Bot.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;

namespace Microsoft.AspNetCore.Builder;

partial class BotMiddleware
{
    public static IApplicationBuilder UseBot(this IApplicationBuilder appBuilder, Func<IServiceProvider, IBot> botResolver)
    {
        ArgumentNullException.ThrowIfNull(appBuilder);
        ArgumentNullException.ThrowIfNull(botResolver);

        return appBuilder.InternalUseBot(botResolver);
    }

    internal static IApplicationBuilder InternalUseBot(this IApplicationBuilder appBuilder, Func<IServiceProvider, IBot> botResolver)
    {
        return appBuilder.Map(
            new PathString("/api/messages"),
            app => app.Use(_ => InvokeBotAsync));

        Task InvokeBotAsync(HttpContext context)
            =>
            StandardCloudAdapter.Resolve(context.RequestServices).ProcessAsync(
                httpRequest: context.Request,
                httpResponse: context.Response,
                bot: botResolver.Invoke(context.RequestServices),
                cancellationToken: context.RequestAborted);
    }
}