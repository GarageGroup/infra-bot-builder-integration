using System;
using System.Collections.Generic;
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
        string? unlockKey = null;
        try
        {
            unlockKey = await GetUnlockKeyAsync(turnContext, cancellationToken).ConfigureAwait(false);
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
            await SaveChangesAsync(turnContext, unlockKey, cancellationToken).ConfigureAwait(false);
        }
    }

    private async ValueTask<string?> GetUnlockKeyAsync(ITurnContext turnContext, CancellationToken cancellationToken)
    {
        if (lockSupplier is null)
        {
            return null;
        }

        var key = $"{turnContext.Activity?.ChannelId}/conversations/{turnContext.Activity?.Conversation?.Id}";
        var lockStatus = await lockSupplier.LockAsync(key, cancellationToken).ConfigureAwait(false);

        if (lockStatus is StorageLockStatus.Success)
        {
            return key;
        }

        var lockingActivity = string.IsNullOrWhiteSpace(lockingMessage) switch
        {
            false => MessageFactory.Text(lockingMessage),
            _ => MessageFactory.Text("Предыдущее сообщение все еще обрабатывается...")
        };

        await turnContext.SendActivityAsync(lockingActivity, cancellationToken).ConfigureAwait(false);
        return null;
    }

    private Task SaveChangesAsync(ITurnContext turnContext, string? unlockKey, CancellationToken cancellationToken)
    {
        var tasks = new List<Task>
        {
            conversationState.SaveChangesAsync(turnContext, true, cancellationToken),
            userState.SaveChangesAsync(turnContext, true, cancellationToken)
        };

        if (string.IsNullOrEmpty(unlockKey) is false && lockSupplier is not null)
        {
            tasks.Add(lockSupplier.UnlockAsync(unlockKey, cancellationToken));
        }

        return Task.WhenAll(tasks);
    }
}