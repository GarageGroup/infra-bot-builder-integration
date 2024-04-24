using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Extensions.Logging;

namespace GarageGroup.Infra.Bot.Builder;

internal sealed partial class BotImpl : IBot
{
    private const string LockingMessageDefault = "The previous message is still being processed...";

    private const string ErrorMessageDefault = "An unexpected error occurred while running the bot";

    private readonly ConversationState conversationState;

    private readonly UserState userState;

    private readonly ILogger logger;

    private readonly Func<ITurnContext, CancellationToken, ValueTask<Unit>> middleware;

    private readonly IStorageLockSupplier? lockSupplier;

    private readonly string? lockingMessage;

    internal BotImpl(
        ConversationState conversationState,
        UserState userState,
        ILoggerFactory loggerFactory,
        Func<ITurnContext, CancellationToken, ValueTask<Unit>> middleware,
        IStorageLockSupplier? lockSupplier,
        string? lockingMessage)
    {
        this.conversationState = conversationState;
        this.userState = userState;
        this.middleware = middleware;
        logger = loggerFactory.CreateLogger<BotImpl>();
        this.lockSupplier = lockSupplier;
        this.lockingMessage = lockingMessage;
    }
}