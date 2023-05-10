namespace GarageGroup.Infra;

internal sealed record class HttpBotResolverMetadata : BotResolverMetadata
{
    public HttpBotResolverMetadata(
        string resolverMethodName,
        string functionMethodName,
        string functionName,
        string? functionRoute,
        int authorizationLevel)
        : base(resolverMethodName, functionMethodName, functionName)
    {
        FunctionRoute = functionRoute;
        AuthorizationLevel = authorizationLevel;
    }

    public string? FunctionRoute { get; }

    public int AuthorizationLevel { get; }
}