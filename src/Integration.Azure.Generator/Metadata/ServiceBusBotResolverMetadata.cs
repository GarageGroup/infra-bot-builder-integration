namespace GarageGroup.Infra;

internal sealed record class ServiceBusBotResolverMetadata : BotResolverMetadata
{
    public ServiceBusBotResolverMetadata(
        string resolverMethodName,
        string functionMethodName,
        string functionName,
        string queueName,
        string? connection)
        : base(resolverMethodName, functionMethodName, functionName)
    {
        QueueName = queueName ?? string.Empty;
        Connection = connection;
    }

    public string QueueName { get; }

    public string? Connection { get; }
}