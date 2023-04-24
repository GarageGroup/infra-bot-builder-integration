namespace GGroupp.Infra;

internal abstract record class BotResolverMetadata
{
    public BotResolverMetadata(string resolverMethodName, string functionMethodName, string functionName)
    {
        ResolverMethodName = resolverMethodName ?? string.Empty;
        FunctionMethodName = functionMethodName ?? string.Empty;
        FunctionName = functionName ?? string.Empty;
    }

    public string ResolverMethodName { get; }

    public string FunctionMethodName { get; }

    public string FunctionName { get; }
}