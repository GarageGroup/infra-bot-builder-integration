using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GarageGroup.Infra.Bot.Builder;

using BotMiddlewareFunc = Func<IBotContext, CancellationToken, ValueTask<Unit>>;

public sealed partial class BotBuilder : IBotBuilder
{
    private readonly IServiceProvider serviceProvider;

    private readonly ConversationState conversationState;

    private readonly UserState userState;

    private readonly IBotTelemetryClient botTelemetryClient;

    private readonly ILoggerFactory loggerFactory;

    private readonly IReadOnlyCollection<BotMiddlewareFunc> middlewares;

    private BotBuilder(
        IServiceProvider serviceProvider,
        ConversationState conversationState,
        UserState userState,
        IBotTelemetryClient botTelemetryClient,
        ILoggerFactory loggerFactory,
        IReadOnlyCollection<BotMiddlewareFunc>? middlewares = null)
    {
        this.serviceProvider = serviceProvider;
        this.conversationState = conversationState;
        this.userState = userState;
        this.botTelemetryClient = botTelemetryClient;
        this.loggerFactory = loggerFactory;
        this.middlewares = middlewares ?? Array.Empty<BotMiddlewareFunc>();
    }

    private IStorageLockSupplier? GetStorageLockSupplier()
        =>
        serviceProvider.GetService<IStorage>() as IStorageLockSupplier;
}