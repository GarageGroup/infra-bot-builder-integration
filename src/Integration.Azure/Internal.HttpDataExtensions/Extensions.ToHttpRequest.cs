using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker.Http;

namespace GGroupp.Infra;

partial class HttpDataExtensions
{
    internal static HttpRequest ToHttpRequest(this HttpRequestData httpRequestData)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = httpRequestData.Url.Scheme;
        httpContext.Request.Host = new HostString(httpRequestData.Url.Host);
        httpContext.Request.Path = httpRequestData.Url.AbsolutePath;
        httpContext.Request.QueryString = new(httpRequestData.Url.Query);
        httpContext.Request.Method = httpRequestData.Method;

        foreach (var header in httpRequestData.Headers)
        {
            httpContext.Request.Headers[header.Key] = new(header.Value.ToArray());
        }

        httpContext.Request.Body = httpRequestData.Body;
        return httpContext.Request;
    }
}