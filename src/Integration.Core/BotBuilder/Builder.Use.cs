using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Infra.Bot.Builder;

partial class BotBuilder
{
    public IBotBuilder Use(Func<IBotContext, CancellationToken, ValueTask<Unit>> middleware)
    {
        ArgumentNullException.ThrowIfNull(middleware);

        return new BotBuilder(
            serviceProvider: serviceProvider,
            conversationState: conversationState,
            userState: userState,
            botTelemetryClient: botTelemetryClient,
            loggerFactory: loggerFactory,
            middlewares: new List<Func<IBotContext, CancellationToken, ValueTask<Unit>>>(middlewares)
            {
                middleware
            });
    }
}