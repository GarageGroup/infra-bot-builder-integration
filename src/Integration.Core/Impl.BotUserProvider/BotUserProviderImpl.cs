using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;

namespace GGroupp.Infra.Bot.Builder;

internal sealed class BotUserProviderImpl : IBotUserProvider
{
    private readonly IStatePropertyAccessor<BotUserJson?> userDataAccessor;

    private readonly ITurnContext turnContext;

    internal BotUserProviderImpl(UserState userState, ITurnContext turnContext)
    {
        userDataAccessor = userState.CreateProperty<BotUserJson?>("__user");
        this.turnContext = turnContext;
    }

    public ValueTask<BotUser?> GetCurrentUserAsync(CancellationToken cancellationToken)
        =>
        cancellationToken.IsCancellationRequested
        ? ValueTask.FromCanceled<BotUser?>(cancellationToken)
        : InnerGetCurrentUserAsync(cancellationToken);

    public ValueTask<Unit> SetCurrentUserAsync(BotUser? user, CancellationToken cancellationToken)
        =>
        cancellationToken.IsCancellationRequested
        ? ValueTask.FromCanceled<Unit>(cancellationToken)
        : InnerSetCurrentUserAsync(user, cancellationToken);

    private async ValueTask<BotUser?> InnerGetCurrentUserAsync(CancellationToken cancellationToken)
    {
        var user = await userDataAccessor.GetAsync(turnContext, static () => default, cancellationToken).ConfigureAwait(false);
        if (user is null)
        {
            return default;
        }

        return new(
            id: user.Id,
            mail: user.Mail,
            displayName: user.DisplayName,
            claims: user.Claims.ToFlatArray());
    }

    private async ValueTask<Unit> InnerSetCurrentUserAsync(BotUser? user, CancellationToken cancellationToken)
    {
        if (user is null)
        {
            await userDataAccessor.DeleteAsync(turnContext, cancellationToken).ConfigureAwait(false);
            return default;
        }

        var userJson = new BotUserJson
        {
            Id = user.Id,
            Mail = user.Mail,
            DisplayName = user.DisplayName,
            Claims = new(user.Claims.AsEnumerable())
        };

        await userDataAccessor.SetAsync(turnContext, userJson, cancellationToken).ConfigureAwait(false);
        return default;
    }
}