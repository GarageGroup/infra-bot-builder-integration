using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;

namespace GarageGroup.Infra.Bot.Builder;

partial class BotFlowImpl
{
    public ValueTask<Unit> StartAsync(Activity activity, CancellationToken cancellationToken)
    {
        var context = GetTurnContext(activity);
        return firstMiddleware.Invoke(context, cancellationToken);
    }
}