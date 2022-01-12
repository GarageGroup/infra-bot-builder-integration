using System;
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
        try
        {
            await middleware.Invoke(turnContext, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Bot middleware threw an unexpected exception");

            var activity = MessageFactory.Text("При выполнении бота произошла непредвиденная ошибка");
            await turnContext.SendActivityAsync(activity, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            await conversationState.SaveChangesAsync(turnContext, true, cancellationToken).ConfigureAwait(false);
            await userState.SaveChangesAsync(turnContext, true, cancellationToken).ConfigureAwait(false);
        }
    }
}