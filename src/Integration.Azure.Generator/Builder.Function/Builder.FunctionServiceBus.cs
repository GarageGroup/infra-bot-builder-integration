using System.Text;
using GGroupp;

namespace GarageGroup.Infra;

partial class BotFunctionBuilder
{
    internal static string BuildServiceBusFunctionSourceCode(this BotFunctionProviderMetadata provider, ServiceBusBotResolverMetadata resolver)
        =>
        new SourceBuilder(
            provider.Namespace)
        .AddUsing(
            "System.Text.Json",
            "System.Threading",
            "System.Threading.Tasks",
            "GarageGroup.Infra",
            "Microsoft.Azure.Functions.Worker")
        .AppendCodeLine(
            $"partial class {provider.TypeName}")
        .BeginCodeBlock()
        .AppendCodeLine(
            $"[Function({resolver.FunctionName.AsStringSourceCode(EmptyStringConstantSourceCode)})]",
            $"public static Task {resolver.FunctionMethodName}(")
        .BeginArguments()
        .AppendCodeLine(
            $"{resolver.BuildServiceBusTriggerAttributeSourceCode()} JsonElement requestData,",
            "FunctionContext context,",
            "CancellationToken cancellationToken)")
        .EndArguments()
        .BeginLambda()
        .AppendCodeLine(
            $"{provider.ProviderType.DisplayedTypeName}.{resolver.ResolverMethodName}().RunBotFunctionAsync(requestData, context, cancellationToken);")
        .EndLambda()
        .EndCodeBlock()
        .Build();

    private static string BuildServiceBusTriggerAttributeSourceCode(this ServiceBusBotResolverMetadata resolver)
    {
        var queueNameSourceCode = resolver.QueueName.AsStringSourceCode(EmptyStringConstantSourceCode);
        var builder = new StringBuilder("[ServiceBusTrigger(").Append(queueNameSourceCode);

        if (string.IsNullOrEmpty(resolver.Connection) is false)
        {
            builder = builder.Append(", Connection = ").Append(resolver.Connection.AsStringSourceCode());
        }

        return builder.Append(")]").ToString();
    }
}