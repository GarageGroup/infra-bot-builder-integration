using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace GGroupp.Infra;

internal static partial class SourceGeneratorExtensions
{
    private const string DefaultNamespace = "GGroupp.Infra";

    private const string ResolverStandardStart = "Use";

    private static string BuildBotFunctionName(this IMethodSymbol methodSymbol)
    {
        var name = methodSymbol.Name.RemoveStandardStart();
        return $"Handle{name}MessageAsync";
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