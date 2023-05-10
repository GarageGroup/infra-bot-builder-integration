using System;
using Microsoft.Azure.Functions.Worker;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Method)]
public sealed class HttpBotFunctionAttribute : Attribute
{
    public HttpBotFunctionAttribute(string name, string route = "messages")
    {
        Name = name ?? string.Empty;
        Route = route ?? string.Empty;
    }

    public string Name { get; }

    public string Route { get; }

    public AuthorizationLevel AuthLevel { get; set; }
}