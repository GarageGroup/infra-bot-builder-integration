using System;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PrimeFuncPack;

namespace GarageGroup.Infra.Bot.Builder;

public static class BotRequestHandlerDependency
{
    public static Dependency<IBotRequestHandler> UseBotRequestHandler<TBot, TBotFrameworkHttpAdapter>(
        this Dependency<TBot, TBotFrameworkHttpAdapter> dependency)
        where TBot : IBot
        where TBotFrameworkHttpAdapter : IBotFrameworkHttpAdapter
    {
        ArgumentNullException.ThrowIfNull(dependency);
        return dependency.InnerUseBotRequestHandler();
    }

    public static Dependency<IBotRequestHandler> UseBotRequestHandler<TBot>(
        this Dependency<TBot> dependency, Func<IServiceProvider, IBotFrameworkHttpAdapter> botFrameworkHttpAdapterResolver)
        where TBot : IBot
    {
        ArgumentNullException.ThrowIfNull(dependency);
        ArgumentNullException.ThrowIfNull(botFrameworkHttpAdapterResolver);

        return dependency.With(botFrameworkHttpAdapterResolver).InnerUseBotRequestHandler();
    }

    public static Dependency<IBotSignalHandler> UseBotSignalHandler<TEntityApi>(
        this Dependency<TEntityApi> dependency, string entityName)
        where TEntityApi : IOrchestrationEntitySignalSupplier
    {
        ArgumentNullException.ThrowIfNull(dependency);
        return dependency.Map<IBotSignalHandler>(CreateHandler);

        BotSignalHandler CreateHandler(IServiceProvider serviceProvider, TEntityApi entityApi)
        {
            ArgumentNullException.ThrowIfNull(serviceProvider);
            ArgumentNullException.ThrowIfNull(entityApi);

            return new BotSignalHandler(
                entityApi: entityApi,
                entityName: entityName.OrEmpty(),
                loggerFactory: serviceProvider.GetService<ILoggerFactory>());
        }

    }

    private static Dependency<IBotRequestHandler> InnerUseBotRequestHandler<TBot, TBotFrameworkHttpAdapter>(
        this Dependency<TBot, TBotFrameworkHttpAdapter> dependency)
        where TBot : IBot
        where TBotFrameworkHttpAdapter : IBotFrameworkHttpAdapter
    {
        return dependency.Fold<IBotRequestHandler>(CreateHandler);

        static BotRequestHandler CreateHandler(TBot bot, TBotFrameworkHttpAdapter botFrameworkHttpAdapter)
        {
            ArgumentNullException.ThrowIfNull(bot);
            ArgumentNullException.ThrowIfNull(botFrameworkHttpAdapter);

            return new(botFrameworkHttpAdapter, bot);
        }
    }
}