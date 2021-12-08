using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Extensions.Logging;

namespace GGroupp.Infra.Bot.Builder;

partial class BotImpl
{
    public Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
    {
        _ = turnContext ?? throw new ArgumentNullException(nameof(turnContext));

        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled(cancellationToken);
        }

        return InnerOnTurnAsync(turnContext, cancellationToken);
    }

    private async Task InnerOnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken)
    {
        if (middlewares.Any() is false)
        {
            return;
        }

        foreach (var middleware in middlewares)
        {   
            var turnState = await TryInvokeAsync(middleware, turnContext, cancellationToken).ConfigureAwait(false);
            if (turnState is TurnState.Awaiting || turnState is TurnState.Interrupted)
            {
                break;
            }
        }

        await conversationState.SaveChangesAsync(turnContext, true, cancellationToken).ConfigureAwait(false);
        await userState.SaveChangesAsync(turnContext, true, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<TurnState> TryInvokeAsync(
        Func<ITurnContext, CancellationToken, ValueTask<TurnState>> middleware, ITurnContext context, CancellationToken token)
    {
        try
        {
            return await middleware.Invoke(context, token).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Bot middleware threw an unexpected exception");

            var activity = MessageFactory.Text("При выполнении бота произошла непредвиденная ошибка");
            await context.SendActivityAsync(activity, token).ConfigureAwait(false);

            return TurnState.Interrupted;
        }
    }
}