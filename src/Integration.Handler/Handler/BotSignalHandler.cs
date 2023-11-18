using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;

namespace GarageGroup.Infra.Bot.Builder;

internal sealed class BotSignalHandler : IBotSignalHandler
{
    private const string ChannelIdHeaderName = "channelid";

    private const string ConversationIdHeaderName = "x-ms-conversation-id";

    private const string EntityOperationName = "SendBotRequest";

    private readonly IOrchestrationEntitySignalSupplier entityApi;

    private readonly string entityName;

    private readonly ILogger? logger;

    internal BotSignalHandler(IOrchestrationEntitySignalSupplier entityApi, string entityName, ILoggerFactory? loggerFactory)
    {
        this.entityApi = entityApi;
        this.entityName = entityName;
        logger = loggerFactory?.CreateLogger<BotSignalHandler>();
    }

    public ValueTask<Result<Unit, Failure<HandlerFailureCode>>> HandleAsync(
        BotRequestJson? input, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return ValueTask.FromCanceled<Result<Unit, Failure<HandlerFailureCode>>>(cancellationToken);
        }

        if (input?.Headers is null)
        {
            return new(Failure.Create(HandlerFailureCode.Persistent, "Bot request body must be specified"));
        }

        var channelId = GetHeaderOrDefault(input.Headers, ChannelIdHeaderName);
        if (string.IsNullOrEmpty(channelId))
        {
            return new(Failure.Create(HandlerFailureCode.Persistent, "Bot request channelId must be specified"));
        }

        var conversationId = GetHeaderOrDefault(input.Headers, ConversationIdHeaderName);
        if (string.IsNullOrEmpty(conversationId))
        {
            return new(Failure.Create(HandlerFailureCode.Persistent, "Bot request conversationId must be specified"));
        }

        logger?.LogInformation("Bot channelId: '{channelId}'. Bot conversationId: '{conversationId}'", channelId, conversationId);

        var entitySignalInput = new OrchestrationEntitySignalIn<BotRequestJson>(
            entity: new OrchestrationEntity(
                name: entityName,
                key: $"{HttpUtility.UrlEncode(channelId)}/{HttpUtility.UrlEncode(conversationId)}"),
            operationName: EntityOperationName,
            value: input);

        return entityApi.SignalEntityAsync(entitySignalInput, cancellationToken);
    }

    private static string? GetHeaderOrDefault(IReadOnlyDictionary<string, string> headers, string name)
        =>
        headers.TryGetValue(name, out var value) ? value : default;
}
