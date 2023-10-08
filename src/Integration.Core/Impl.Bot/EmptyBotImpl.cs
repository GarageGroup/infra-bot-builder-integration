using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;

namespace GarageGroup.Infra.Bot.Builder;

internal sealed class EmptyBotImpl : IBot
{
    internal static readonly EmptyBotImpl Instance;

    static EmptyBotImpl()
        =>
        Instance = new();

    private EmptyBotImpl()
    {
    }

    public Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        =>
        Task.CompletedTask;
}