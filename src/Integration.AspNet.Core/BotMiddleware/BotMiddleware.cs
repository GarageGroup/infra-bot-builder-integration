using System;
using System.Threading.Tasks;
using GGroupp.Infra.Bot.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Builder;

public static partial class BotMiddleware
{
    private static readonly object lockObject = new();

    private static volatile BotFrameworkHttpAdapter? adapter;

    private static Task InvokeBotAsync(HttpContext context, Func<IServiceProvider, IBot> botResolver)
        =>
        context.RequestServices.GetBotFrameworkHttpAdapter()
        .ProcessAsync(
            httpRequest: context.Request,
            httpResponse: context.Response,
            bot: botResolver.Invoke(context.RequestServices),
            cancellationToken: context.RequestAborted);

    private static BotFrameworkHttpAdapter GetBotFrameworkHttpAdapter(this IServiceProvider serviceProvider)
    {
        if (adapter is not null)
        {
            return adapter;
        }

        lock (lockObject)
        {
            if (adapter is not null)
            {
                return adapter;
            }

            adapter = new AdapterWithErrorHandler(
                configuration: serviceProvider.GetRequiredService<IConfiguration>(),
                logger: serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<AdapterWithErrorHandler>());
        }

        return adapter;
    }
}

