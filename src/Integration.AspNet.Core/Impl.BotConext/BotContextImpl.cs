using System;
using Microsoft.Bot.Builder;
using Microsoft.Extensions.Logging;

namespace GGroupp.Infra.Bot.Builder;

internal sealed class BotContextImpl : IBotContext
{
    internal BotContextImpl(
        ITurnContext turnContext,
        IBotFlow botFlow,
        UserState userState,
        ConversationState conversationState,
        IBotTelemetryClient botTelemetryClient,
        IBotUserProvider botUserProvider,
        ILoggerFactory loggerFactory,
        IServiceProvider serviceProvider)
    {
        TurnContext = turnContext;
        BotFlow = botFlow;
        UserState = userState;
        ConversationState = conversationState;
        BotTelemetryClient = botTelemetryClient;
        BotUserProvider = botUserProvider;
        LoggerFactory = loggerFactory;
        ServiceProvider = serviceProvider;
    }

    public ITurnContext TurnContext { get; }

    public IBotFlow BotFlow { get; }

    public UserState UserState { get; }

    public ConversationState ConversationState { get; }

    public IBotTelemetryClient BotTelemetryClient { get; }

    public IBotUserProvider BotUserProvider { get; }

    public ILoggerFactory LoggerFactory { get; }

    public IServiceProvider ServiceProvider { get; }
}