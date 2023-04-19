namespace GGroupp.Infra;

partial class BotFunctionBuilder
{
    internal static string BuildConstructorSourceCode(this BotFunctionProviderMetadata provider)
        =>
        new SourceBuilder(
            provider.Namespace)
        .AppendCodeLine(
            $"public static partial class {provider.TypeName}")
        .BeginCodeBlock()
        .EndCodeBlock()
        .Build();
}