using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GarageGroup.Infra.Bot.Builder;

internal sealed record class BotUserJson
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("mail")]
    public string? Mail { get; init; }

    [JsonPropertyName("diplayName")]
    public string? DisplayName { get; init; }

    [JsonPropertyName("claims")]
    public Dictionary<string, string>? Claims { get; init; }
}