using System.Text;

namespace GGroupp.Infra;

partial class BotFunctionBuilder
{
    internal static string BuildHttpFunctionSourceCode(this BotFunctionProviderMetadata provider, HttpBotResolverMetadata resolver)
        =>
        new SourceBuilder(
            provider.Namespace)
        .AddUsing(
            "System.Threading",
            "System.Threading.Tasks",
            "GGroupp.Infra",
            "Microsoft.Azure.Functions.Worker",
            "Microsoft.Azure.Functions.Worker.Http")
        .AppendCodeLine(
            $"partial class {provider.TypeName}")
        .BeginCodeBlock()
        .AppendCodeLine(
            $"[Function({resolver.FunctionName.AsStringSourceCode(EmptyStringConstantSourceCode)})]",
            $"public static Task<HttpResponseData> {resolver.FunctionMethodName}(")
        .BeginArguments()
        .AppendCodeLine(
            $"{resolver.BuildHttpTriggerAttributeSourceCode()} HttpRequestData request,",
            "CancellationToken cancellationToken)")
        .EndArguments()
        .BeginLambda()
        .AppendCodeLine(
            $"{provider.ProviderType.DisplayedTypeName}.{resolver.ResolverMethodName}().RunBotFunctionAsync(request, cancellationToken);")
        .EndLambda()
        .EndCodeBlock()
        .Build();

    private static string BuildHttpTriggerAttributeSourceCode(this HttpBotResolverMetadata resolver)
    {
        var authorizationLevelSourceCode = resolver.GetAuthorizationLevelSourceCode();
        var builder = new StringBuilder("[HttpTrigger(").Append(authorizationLevelSourceCode).Append(", \"POST\"");

        if (string.IsNullOrEmpty(resolver.FunctionRoute) is false)
        {
            builder = builder.Append(", Route = ").Append(resolver.FunctionRoute.AsStringSourceCode());
        }

        return builder.Append(")]").ToString();
    }

    private static string GetAuthorizationLevelSourceCode(this HttpBotResolverMetadata resolver)
        =>
        resolver.AuthorizationLevel switch
        {
            0 => "AuthorizationLevel.Anonymous",
            1 => "AuthorizationLevel.User",
            2 => "AuthorizationLevel.Function",
            3 => "AuthorizationLevel.System",
            4 => "AuthorizationLevel.Admin",
            _ => "(AuthorizationLevel)" + resolver.AuthorizationLevel
        };
}