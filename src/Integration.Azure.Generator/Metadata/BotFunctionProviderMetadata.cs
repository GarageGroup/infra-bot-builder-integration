using System;
using System.Collections.Generic;
using GGroupp;

namespace GarageGroup.Infra;

internal sealed record class BotFunctionProviderMetadata
{
    internal BotFunctionProviderMetadata(
        string @namespace,
        string typeName,
        DisplayedTypeData providerType,
        IReadOnlyList<BotResolverMetadata> resolverTypes)
    {
        Namespace = @namespace ?? string.Empty;
        TypeName = typeName ?? string.Empty;
        ProviderType = providerType;
        ResolverTypes = resolverTypes ?? Array.Empty<BotResolverMetadata>();
    }

    public string Namespace { get; }

    public string TypeName { get; }

    public DisplayedTypeData ProviderType { get; }

    public IReadOnlyList<BotResolverMetadata> ResolverTypes { get; }
}