using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;

namespace GGroupp.Infra.Bot.Builder;

partial class BotBuilder
{
    public IBot Build()
        =>
        middlewares.Any()
        ? new BotImpl(conversationState, userState, loggerFactory, InvokeBotAsync)
        : EmptyBotImpl.Instance;

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
                userState: userState,
                conversationState: conversationState,
                loggerFactory: loggerFactory,
                botUserProvider: new BotUserProviderImpl(
                    userState: userState,
                    turnContext: nextContext),
                botFlow: new BotFlowImpl(
                    turnContext: nextContext,
                    firstMiddleware: InvokeBotAsync,
                    nextMiddleware: NextAsync),
                serviceProvider: serviceProvider);

            return middleware.Invoke(botContext, nextToken);
        }
    }
}