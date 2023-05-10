using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GarageGroup.Infra;

partial class HttpDataExtensions
{
    internal static async ValueTask ThrowIfFailureAsync(this HttpResponse response, CancellationToken cancellationToken)
    {
        if (response.StatusCode is >= 200 and < 300)
        {
            return;
        }

        var responseBody = await response.ReadBodyAsync(cancellationToken).ConfigureAwait(false);
        throw new InvalidOperationException($"Bot request failed with status code {response.StatusCode}. Response body: {responseBody}");
    }

    private static async Task<string> ReadBodyAsync(this HttpResponse response, CancellationToken cancellationToken)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(response.Body);

#if NET7_0_OR_GREATER
        return await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
#else
        cancellationToken.ThrowIfCancellationRequested();
        return await reader.ReadToEndAsync().ConfigureAwait(false);
#endif
    }
}