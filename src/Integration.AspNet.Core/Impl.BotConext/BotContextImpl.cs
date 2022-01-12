using System;
using Microsoft.Bot.Builder;
using Microsoft.Extensions.Logging;

namespace GGroupp.Infra.Bot.Builder;

internal sealed class BotContextImpl : IBotContext
{
    internal BotContextImpl(
        ITurnContext turnContext,
        UserState userState,
        ConversationState conversationState,
        ILoggerFactory loggerFactory,
        IBotUserProvider botUserProvider,
        IBotFlow botFlow,
        IServiceProvider serviceProvider)
    {
        TurnContext = turnContext;
        UserState = userState;
        ConversationState = conversationState;
        LoggerFactory = loggerFactory;
        BotUserProvider = botUserProvider;
        BotFlow = botFlow;
        ServiceProvider = serviceProvider;
    }

    public ITurnContext TurnContext { get; }

    public UserState UserState { get; }

    public ConversationState ConversationState { get; }

    public ILoggerFactory LoggerFactory { get; }

    public IBotUserProvider BotUserProvider { get; }

    public IBotFlow BotFlow { get; }

    public IServiceProvider ServiceProvider { get; }
}