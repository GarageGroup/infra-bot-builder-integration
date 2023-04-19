using System;
using Microsoft.Azure.Functions.Worker;

namespace GGroupp.Infra;

[AttributeUsage(AttributeTargets.Method)]
public sealed class BotFunctionAttribute : Attribute
{
    public BotFunctionAttribute(string name, string route = "messages")
    {
        Name = name ?? string.Empty;
        Route = route ?? string.Empty;
    }

    public string Name { get; }

    public string Route { get; }

    public AuthorizationLevel AuthLevel { get; set; }
}