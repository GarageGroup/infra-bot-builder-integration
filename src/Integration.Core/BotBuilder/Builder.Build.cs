using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;

namespace GarageGroup.Infra.Bot.Builder;

partial class BotBuilder
{
    public IBot Build()
        =>
        InnerBuild(false, null);

    public IBot Build(bool useLocking, string? lockingMessage = null)
        =>
        InnerBuild(useLocking, lockingMessage);

    private IBot InnerBuild(bool useLocking, string? lockingMessage)
    {
        if (middlewares.Any() is false)
        {
            return EmptyBotImpl.Instance;
        }

        var lockSupplier = useLocking ? GetStorageLockSupplier() : null;
        return new BotImpl(conversationState, userState, loggerFactory, InvokeBotAsync, lockSupplier, lockingMessage);
    }

    private ValueTask<Unit> InvokeBotAsync(ITurnContext turnContext, CancellationToken cancellationToken)
    {
        var middlewaresEnumerator = middlewares.GetEnumerator();
        return NextAsync(turnContext, cancellationToken);

        ValueTask<Unit> NextAsync(ITurnContext nextContext, CancellationToken nextToken)
        {
            var middleware = middlewaresEnumerator.MoveNext() ? middlewaresEnumerator.Current : default;
            if (middleware is null)
            {
                return default;
            }

            var botContext = new BotContextImpl(
                turnContext: nextContext,
                botFlow: new BotFlowImpl(
                    turnContext: nextContext,
                    firstMiddleware: InvokeBotAsync,
                    nextMiddleware: NextAsync),
                userState: userState,
                conversationState: conversationState,
                botTelemetryClient: botTelemetryClient,
                botUserProvider: new BotUserProviderImpl(
                    userState: userState,
                    turnContext: nextContext),
                loggerFactory: loggerFactory,
                serviceProvider: serviceProvider);

            return middleware.Invoke(botContext, nextToken);
        }
    }
}