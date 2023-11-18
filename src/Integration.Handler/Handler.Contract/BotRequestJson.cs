using System.Collections.Generic;

namespace GarageGroup.Infra.Bot.Builder;

public sealed record class BotRequestJson
{
    public string? Method { get; init; }

    public IReadOnlyDictionary<string, string>? Headers { get; init; }

    public string? Body { get; init; }
}