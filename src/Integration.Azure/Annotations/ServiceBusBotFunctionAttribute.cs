using System;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Method)]
public sealed class ServiceBusBotFunctionAttribute : Attribute
{
    public ServiceBusBotFunctionAttribute(string name, string queueName, string connection)
    {
        Name = name ?? string.Empty;
        QueueName = queueName ?? string.Empty;
        Connection = connection ?? string.Empty;
    }

    public string Name { get; }

    public string QueueName { get; }

    public string Connection { get; }
}