using System;
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

    private readonly Func<ITurnContext, CancellationToken, ValueTask<Unit>> middleware;

    internal BotImpl(
        ConversationState conversationState,
        UserState userState,
        ILoggerFactory loggerFactory,
        Func<ITurnContext, CancellationToken, ValueTask<Unit>> middleware)
    {
        this.conversationState = conversationState;
        this.userState = userState;
        this.middleware = middleware;
        logger = loggerFactory.CreateLogger<BotImpl>();
    }
}