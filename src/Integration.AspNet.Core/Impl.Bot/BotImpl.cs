using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Extensions.Logging;

namespace GGroupp.Infra.Bot.Builder;

internal sealed partial class BotImpl : IBot
{
    private readonly ConversationState conversationState;

    private readonly UserState userState;

    private readonly ILogger logger;

    private readonly IReadOnlyCollection<Func<ITurnContext, CancellationToken, ValueTask<TurnState>>> middlewares;

    internal BotImpl(
        ConversationState conversationState,
        UserState userState,
        ILogger<BotImpl> logger,
        IReadOnlyCollection<Func<ITurnContext, CancellationToken, ValueTask<TurnState>>> middlewares)
    {
        this.conversationState = conversationState;
        this.userState = userState;
        this.logger = logger;
        this.middlewares = middlewares;
    }
}