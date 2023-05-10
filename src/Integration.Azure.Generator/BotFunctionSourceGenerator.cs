using System;
using Microsoft.CodeAnalysis;

namespace GarageGroup.Infra;

[Generator]
internal sealed class BotFunctionSourceGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        foreach (var providerType in context.GetBotFunctionProviderTypes())
        {
            var constructorSourceCode = providerType.BuildConstructorSourceCode();
            context.AddSource($"{providerType.TypeName}.g.cs", constructorSourceCode);

            foreach (var resolverType in providerType.ResolverTypes)
            {
                if (resolverType is HttpBotResolverMetadata httpBotResolver)
                {
                    var httpFunctionSourceCode = providerType.BuildHttpFunctionSourceCode(httpBotResolver);
                    context.AddSource($"{providerType.TypeName}.{resolverType.FunctionMethodName}.g.cs", httpFunctionSourceCode);
                }
                else if (resolverType is ServiceBusBotResolverMetadata serviceBusBotResolver)
                {
                    var httpFunctionSourceCode = providerType.BuildServiceBusFunctionSourceCode(serviceBusBotResolver);
                    context.AddSource($"{providerType.TypeName}.{resolverType.FunctionMethodName}.g.cs", httpFunctionSourceCode);
                }
                else
                {
                    throw new InvalidOperationException($"An unexpected bot metadata type: {resolverType.GetType().FullName}");
                }
            }
        }
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        // No initialization required for this one
    }
}