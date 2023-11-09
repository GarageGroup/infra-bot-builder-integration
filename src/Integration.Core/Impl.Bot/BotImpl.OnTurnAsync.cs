using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Extensions.Logging;

namespace GarageGroup.Infra.Bot.Builder;

partial class BotImpl
{
    public Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(turnContext);

        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled(cancellationToken);
        }

        return InnerOnTurnAsync(turnContext, cancellationToken);
    }

    private async Task InnerOnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken)
    {
        string? unlockKey = await TryGetUnlockKeyAsync(turnContext, cancellationToken).ConfigureAwait(false);
        if (string.IsNullOrEmpty(unlockKey) && lockSupplier is not null)
        {
            return;
        }

        try
        {
            await middleware.Invoke(turnContext, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Bot middleware threw an unexpected exception");

            var activity = MessageFactory.Text(ErrorMessageDefault);
            await turnContext.SendActivityAsync(activity, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            await SaveChangesAsync(turnContext, unlockKey, cancellationToken).ConfigureAwait(false);
        }
    }

    private async ValueTask<string?> TryGetUnlockKeyAsync(ITurnContext turnContext, CancellationToken cancellationToken)
    {
        if (lockSupplier is null)
        {
            return null;
        }

        try
        {
            var key = $"{turnContext.Activity?.ChannelId}/conversations/{turnContext.Activity?.Conversation?.Id}";
            var lockStatus = await lockSupplier.LockAsync(key, cancellationToken).ConfigureAwait(false);

            if (lockStatus is StorageLockStatus.Success)
            {
                return key;
            }

            var lockingActivity = string.IsNullOrWhiteSpace(lockingMessage) switch
            {
                false => MessageFactory.Text(lockingMessage),
                _ => MessageFactory.Text(LockingMessageDefault)
            };

            await turnContext.SendActivityAsync(lockingActivity, cancellationToken).ConfigureAwait(false);
            return null;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Bot locking operation threw an unexpected exception");

            var activity = MessageFactory.Text(ErrorMessageDefault);
            await turnContext.SendActivityAsync(activity, cancellationToken).ConfigureAwait(false);

            return null;
        }
    }

    private async Task SaveChangesAsync(ITurnContext turnContext, string? unlockKey, CancellationToken cancellationToken)
    {
        var states = new BotState[]
        {
            conversationState,
            userState
        };

        try
        {
            await Parallel.ForEachAsync(states, InnerSaveAsync).ConfigureAwait(false);
        }
        finally
        {
            if (string.IsNullOrEmpty(unlockKey) is false && lockSupplier is not null)
            {
                await lockSupplier.UnlockAsync(unlockKey, cancellationToken).ConfigureAwait(false);
            }
        }

        ValueTask InnerSaveAsync(BotState state, CancellationToken cancellationToken)
            =>
            new(
                task: state.SaveChangesAsync(turnContext, true, cancellationToken));
    }
}