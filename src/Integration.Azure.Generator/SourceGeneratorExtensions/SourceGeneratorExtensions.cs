using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

internal static partial class SourceGeneratorExtensions
{
    private const string DefaultNamespace = "GarageGroup.Infra";

    private const string ResolverStandardStart = "Use";

    private static void ValidateOrThrow(this IMethodSymbol methodSymbol)
    {
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
    }

    private static string BuildHttpBotFunctionName(this IMethodSymbol methodSymbol)
    {
        var name = methodSymbol.Name.RemoveStandardStart();
        return $"HandleHttp{name}MessageAsync";
    }

    private static string BuildServiceBusBotFunctionName(this IMethodSymbol methodSymbol)
    {
        var name = methodSymbol.Name.RemoveStandardStart();
        return $"HandleServiceBus{name}MessageAsync";
    }

    private static string RemoveStandardStart(this string name)
    {
        var startLength = ResolverStandardStart.Length;
        if (name.Length <= startLength)
        {
            return name;
        }

        if (name.StartsWith(ResolverStandardStart, StringComparison.InvariantCultureIgnoreCase) is false)
        {
            return name;
        }

        return name.Substring(startLength);
    }

    private static IEnumerable<T> NotNull<T>(this IEnumerable<T?> source)
    {
        foreach (var item in source)
        {
            if (item is null)
            {
                continue;
            }

            yield return item;
        }
    }

    private static InvalidOperationException CreateInvalidMethodException(this IMethodSymbol resolverMethod, string message)
        =>
        new($"Bot function resolver method {resolverMethod.ContainingType?.Name}.{resolverMethod.Name} {message}");
}