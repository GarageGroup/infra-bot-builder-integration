using System;
using Microsoft.Bot.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GarageGroup.Infra.Bot.Builder;
partial class BotBuilder
{
    public static BotBuilder Resolve(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        return new(
            serviceProvider: serviceProvider,
            conversationState: serviceProvider.GetRequiredService<ConversationState>(),
            userState: serviceProvider.GetRequiredService<UserState>(),
            botTelemetryClient: serviceProvider.GetRequiredService<IBotTelemetryClient>(),
            loggerFactory: serviceProvider.GetRequiredService<ILoggerFactory>());
    }
}