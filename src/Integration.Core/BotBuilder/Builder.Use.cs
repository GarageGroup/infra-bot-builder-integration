using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Infra.Bot.Builder;

using BotMiddlewareFunc = Func<IBotContext, CancellationToken, ValueTask<Unit>>;

partial class BotBuilder
{
    public IBotBuilder Use(BotMiddlewareFunc middleware)
        =>
        InnerUse(
            middleware ?? throw new ArgumentNullException(nameof(middleware)));

    private BotBuilder InnerUse(BotMiddlewareFunc middleware)
        =>
        new(
            serviceProvider: serviceProvider,
            conversationState: conversationState,
            userState: userState,
            botTelemetryClient: botTelemetryClient,
            loggerFactory: loggerFactory,
            middlewares: new List<BotMiddlewareFunc>(middlewares)
            {
                middleware
            });
}