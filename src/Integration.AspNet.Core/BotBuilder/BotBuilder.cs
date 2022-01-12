using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Extensions.Logging;

namespace GGroupp.Infra.Bot.Builder;

using BotMiddlewareFunc = Func<IBotContext, CancellationToken, ValueTask<Unit>>;

public sealed partial class BotBuilder : IBotBuilder
{
    internal static BotBuilder InternalCreate(
        IServiceProvider serviceProvider,
        ConversationState conversationState,
        UserState userState,
        ILoggerFactory loggerFactory)
        =>
        new(
            serviceProvider: serviceProvider,
            conversationState: conversationState,
            userState: userState,
            loggerFactory: loggerFactory,
            middlewares: Array.Empty<BotMiddlewareFunc>());

    private readonly IServiceProvider serviceProvider;

    private readonly ConversationState conversationState;

    private readonly UserState userState;

    private readonly ILoggerFactory loggerFactory;

    private readonly IReadOnlyCollection<BotMiddlewareFunc> middlewares;

    private BotBuilder(
        IServiceProvider serviceProvider,
        ConversationState conversationState,
        UserState userState,
        ILoggerFactory loggerFactory,
        IReadOnlyCollection<BotMiddlewareFunc> middlewares)
    {
        this.serviceProvider = serviceProvider;
        this.conversationState = conversationState;
        this.userState = userState;
        this.loggerFactory = loggerFactory;
        this.middlewares = middlewares;
    }
}