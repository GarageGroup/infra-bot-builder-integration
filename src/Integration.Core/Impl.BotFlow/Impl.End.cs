using System;
using System.Threading;
using System.Threading.Tasks;

namespace GGroupp.Infra.Bot.Builder;

partial class BotFlowImpl
{
    public ValueTask<Unit> EndAsync(CancellationToken cancellationToken)
        =>
        default;
}