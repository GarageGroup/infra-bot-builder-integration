using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker.Http;

namespace GarageGroup.Infra;

partial class HttpDataExtensions
{
    internal static async Task<HttpResponseData> CreateResponseAsync(
        this HttpRequestData httpRequestData, HttpResponse httpResponse, CancellationToken cancellationToken)
    {
        var response = httpRequestData.CreateResponse((HttpStatusCode)httpResponse.StatusCode);

        foreach (var header in httpResponse.Headers)
        {
            response.Headers.Add(header.Key, header.Value.ToArray());
        }

        if (string.IsNullOrEmpty(httpResponse.ContentType) is false)
        {
            response.Headers.TryAddWithoutValidation("Content-Type", httpResponse.ContentType);
        }

        if (httpResponse.Body is null || httpRequestData.Body.CanRead is false)
        {
            return response;
        }

        var buffer = new Memory<byte>(new byte[response.Body.Length]);
        await response.Body.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);

        await httpResponse.BodyWriter.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
        return response;
    }
}