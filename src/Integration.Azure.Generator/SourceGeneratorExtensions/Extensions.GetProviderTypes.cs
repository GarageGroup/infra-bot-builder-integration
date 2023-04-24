using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

internal static partial class SourceGeneratorExtensions
{
    internal static IReadOnlyCollection<BotFunctionProviderMetadata> GetBotFunctionProviderTypes(this GeneratorExecutionContext context)
    {
        var visitor = new ExportedTypesCollector(context.CancellationToken);
        visitor.VisitNamespace(context.Compilation.GlobalNamespace);

        return visitor.GetNonPrivateTypes().Select(GetFunctionMetadata).NotNull().ToArray();
    }

    private static BotFunctionProviderMetadata? GetFunctionMetadata(INamedTypeSymbol typeSymbol)
    {
        var resolverTypes = typeSymbol.GetMembers().OfType<IMethodSymbol>().SelectMany(GetResolvers).NotNull().ToArray();
        if (resolverTypes.Any() is false)
        {
            return null;
        }

        if (typeSymbol.TypeArguments.Any())
        {
            throw new InvalidOperationException($"Bot function provider class '{typeSymbol.Name}' must not have generic arguments");
        }

        return new(
            @namespace: typeSymbol.ContainingNamespace.ToString(),
            typeName: typeSymbol.Name + "BotFunction",
            providerType: typeSymbol.GetDisplayedData(),
            resolverTypes: resolverTypes);

        static IEnumerable<BotResolverMetadata> GetResolvers(IMethodSymbol methodSymbol)
            =>
            new BotResolverMetadata?[]
            {
                GetHttpResolverMetadata(methodSymbol),
                GetServiceBusResolverMetadata(methodSymbol)
            }
            .NotNull();
    }

    private static HttpBotResolverMetadata? GetHttpResolverMetadata(IMethodSymbol methodSymbol)
    {
        var functionAttribute = methodSymbol.GetAttributes().FirstOrDefault(IsHttpBotFunctionAttribute);
        if (functionAttribute is null)
        {
            return null;
        }

        methodSymbol.ValidateOrThrow();

        var name = methodSymbol.Name.RemoveStandardStart();

        return new(
            resolverMethodName: methodSymbol.Name,
            functionMethodName: methodSymbol.BuildHttpBotFunctionName(),
            functionName: functionAttribute.GetAttributeValue(0)?.ToString() ?? string.Empty,
            functionRoute: functionAttribute.GetAttributeValue(1)?.ToString(),
            authorizationLevel: functionAttribute.GetAuthorizationLevel());

        static bool IsHttpBotFunctionAttribute(AttributeData attributeData)
            =>
            attributeData.AttributeClass?.IsType(DefaultNamespace, "HttpBotFunctionAttribute") is true;
    }

    private static ServiceBusBotResolverMetadata? GetServiceBusResolverMetadata(IMethodSymbol methodSymbol)
    {
        var functionAttribute = methodSymbol.GetAttributes().FirstOrDefault(IsServiceBusBotFunctionAttribute);
        if (functionAttribute is null)
        {
            return null;
        }

        methodSymbol.ValidateOrThrow();

        var name = methodSymbol.Name.RemoveStandardStart();

        var queueName = functionAttribute.GetAttributeValue(1)?.ToString();
        if (string.IsNullOrEmpty(queueName))
        {
            throw methodSymbol.CreateInvalidMethodException("queue name must be specified");
        }

        return new(
            resolverMethodName: methodSymbol.Name,
            functionMethodName: methodSymbol.BuildServiceBusBotFunctionName(),
            functionName: functionAttribute.GetAttributeValue(0)?.ToString() ?? string.Empty,
            queueName: queueName ?? string.Empty,
            connection: functionAttribute.GetAttributeValue(2)?.ToString());

        static bool IsServiceBusBotFunctionAttribute(AttributeData attributeData)
            =>
            attributeData.AttributeClass?.IsType(DefaultNamespace, "ServiceBusBotFunctionAttribute") is true;
    }

    private static bool IsBotDependencyType(this IMethodSymbol resolverMethod)
    {
        var returnType = resolverMethod.ReturnType as INamedTypeSymbol;
        if (returnType?.IsType("PrimeFuncPack", "Dependency") is not true || returnType?.TypeArguments.Length is not 1)
        {
            return false;
        }

        var botType = returnType.TypeArguments[0] as INamedTypeSymbol;
        return botType?.IsType("Microsoft.Bot.Builder", "IBot") is true;
    }

    private static int GetAuthorizationLevel(this AttributeData functionAttribute)
    {
        var levelValue = functionAttribute.GetAttributePropertyValue("AuthLevel");
        if (levelValue is null)
        {
            return default;
        }

        if (levelValue is not int level)
        {
            throw new InvalidOperationException($"An unexpected bot function authorization level: {levelValue}");
        }

        return level;
    }
}