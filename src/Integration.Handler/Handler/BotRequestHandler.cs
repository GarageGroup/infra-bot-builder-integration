using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;

namespace GarageGroup.Infra.Bot.Builder;

internal sealed partial class BotRequestHandler : IBotRequestHandler
{
    private readonly IBotFrameworkHttpAdapter botFrameworkHttpAdapter;

    private readonly IBot bot;

    internal BotRequestHandler(IBotFrameworkHttpAdapter botFrameworkHttpAdapter, IBot bot)
    {
        this.botFrameworkHttpAdapter = botFrameworkHttpAdapter;
        this.bot = bot;
    }
}