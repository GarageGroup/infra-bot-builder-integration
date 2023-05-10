using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace GarageGroup.Infra;

internal static partial class HttpDataExtensions
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private static HttpRequest BuildHttpRequest(this RequestDataJson? requestJson)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Method = requestJson?.Method.OrNullIfEmpty() ?? "POST";

        if (string.IsNullOrEmpty(requestJson?.Body) is false)
        {
            var byteArray = Encoding.UTF8.GetBytes(requestJson.Body);
            httpContext.Request.Body = new MemoryStream(byteArray);
        }

        if (requestJson?.Headers?.Count is not > 0)
        {
            return httpContext.Request;
        }

        foreach (var header in requestJson.Headers)
        {
            httpContext.Request.Headers[header.Key] = header.Value;
        }

        return httpContext.Request;
    }

    private sealed record RequestDataJson
    {
        public string? Method { get; init; }

        public Dictionary<string, string>? Headers { get; init; }

        public string? Body { get; init; }
    }
}