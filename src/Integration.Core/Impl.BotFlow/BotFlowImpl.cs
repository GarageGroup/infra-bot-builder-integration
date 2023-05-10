using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace GarageGroup.Infra.Bot.Builder;

internal sealed partial class BotFlowImpl : IBotFlow
{
    private readonly ITurnContext turnContext;

    private readonly Func<ITurnContext, CancellationToken, ValueTask<Unit>> firstMiddleware;

    private readonly Func<ITurnContext, CancellationToken, ValueTask<Unit>> nextMiddleware;

    internal BotFlowImpl(
        ITurnContext turnContext,
        Func<ITurnContext, CancellationToken, ValueTask<Unit>> firstMiddleware,
        Func<ITurnContext, CancellationToken, ValueTask<Unit>> nextMiddleware)
    {
        this.turnContext = turnContext;
        this.firstMiddleware = firstMiddleware;
        this.nextMiddleware = nextMiddleware;
    }

    private ITurnContext GetTurnContext(Activity activity)
        =>
        activity == turnContext.Activity ? turnContext : new TurnContext(turnContext, activity);
}