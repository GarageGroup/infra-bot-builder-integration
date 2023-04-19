namespace GGroupp.Infra;

internal sealed record class BotResolverMetadata
{
    public BotResolverMetadata(
        string resolverMethodName,
        string functionMethodName,
        string functionName,
        string? functionRoute,
        int authorizationLevel)
    {
        ResolverMethodName = resolverMethodName ?? string.Empty;
        FunctionMethodName = functionMethodName ?? string.Empty;
        FunctionName = functionName ?? string.Empty;
        FunctionRoute = functionRoute;
        AuthorizationLevel = authorizationLevel;
    }

    public string ResolverMethodName { get; }

    public string FunctionMethodName { get; }

    public string FunctionName { get; }

    public string? FunctionRoute { get; }

    public int AuthorizationLevel { get; }
}