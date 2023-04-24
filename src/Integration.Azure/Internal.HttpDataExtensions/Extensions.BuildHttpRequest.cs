using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace GGroupp.Infra;

partial class HttpDataExtensions
{
    internal static HttpRequest BuildHttpRequest(this JsonElement requestData)
    {
        var requestDataJson = requestData.Deserialize<RequestDataJson>(SerializerOptions);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Method = requestDataJson?.Method.OrNullIfEmpty() ?? "POST";

        if (string.IsNullOrEmpty(requestDataJson?.Body) is false)
        {
            var byteArray = Encoding.UTF8.GetBytes(requestDataJson.Body);
            httpContext.Request.Body = new MemoryStream(byteArray);
        }

        if (requestDataJson?.Headers?.Count is not > 0)
        {
            return httpContext.Request;
        }

        foreach (var header in requestDataJson.Headers)
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