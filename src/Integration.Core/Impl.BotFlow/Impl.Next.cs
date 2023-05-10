using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;

namespace GarageGroup.Infra.Bot.Builder;

partial class BotFlowImpl
{
    public ValueTask<Unit> NextAsync(CancellationToken cancellationToken)
        =>
        nextMiddleware.Invoke(turnContext, cancellationToken);

    public ValueTask<Unit> NextAsync(Activity nextActivity, CancellationToken cancellationToken)
    {
        var context = GetTurnContext(nextActivity);
        return nextMiddleware.Invoke(context, cancellationToken);
    }
}