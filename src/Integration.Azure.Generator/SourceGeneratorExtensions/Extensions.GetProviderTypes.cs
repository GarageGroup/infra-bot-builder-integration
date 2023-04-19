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
        var resolverTypes = typeSymbol.GetMembers().OfType<IMethodSymbol>().Select(GetResolverMetadata).NotNull().ToArray();
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
    }

    private static BotResolverMetadata? GetResolverMetadata(IMethodSymbol methodSymbol)
    {
        var functionAttribute = methodSymbol.GetAttributes().FirstOrDefault(IsFunctionAttribute);
        if (functionAttribute is null)
        {
            return null;
        }

        if (methodSymbol.IsStatic is false)
        {
            throw methodSymbol.CreateInvalidMethodException("must be static");
        }

        if (methodSymbol.DeclaredAccessibility is not (Accessibility.Public or Accessibility.Internal))
        {
            throw methodSymbol.CreateInvalidMethodException("must be public or internal");
        }

        if (methodSymbol.Parameters.Any())
        {
            throw methodSymbol.CreateInvalidMethodException("must not have parameters");
        }

        if (methodSymbol.TypeParameters.Any())
        {
            throw methodSymbol.CreateInvalidMethodException("must not have generic arguments");
        }

        if (methodSymbol.IsBotDependencyType() is false)
        {
            throw methodSymbol.CreateInvalidMethodException("return type must be PrimeFuncPack.Dependency<IBot>");
        }

        var name = methodSymbol.Name.RemoveStandardStart();

        return new(
            resolverMethodName: methodSymbol.Name,
            functionMethodName: methodSymbol.BuildBotFunctionName(),
            functionName: functionAttribute.GetAttributeValue(0)?.ToString() ?? string.Empty,
            functionRoute: functionAttribute.GetAttributeValue(1)?.ToString(),
            authorizationLevel: functionAttribute.GetAuthorizationLevel());

        static bool IsFunctionAttribute(AttributeData attributeData)
            =>
            attributeData.AttributeClass?.IsType(DefaultNamespace, "BotFunctionAttribute") is true;
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