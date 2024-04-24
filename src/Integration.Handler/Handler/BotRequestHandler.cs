using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;

namespace GarageGroup.Infra.Bot.Builder;

internal sealed class BotRequestHandler(IBotFrameworkHttpAdapter botFrameworkHttpAdapter, IBot bot) : IBotRequestHandler
{
    public ValueTask<Result<Unit, Failure<HandlerFailureCode>>> HandleAsync(
        BotRequestJson? input, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return ValueTask.FromCanceled<Result<Unit, Failure<HandlerFailureCode>>>(cancellationToken);
        }

        return InnerHandleAsync(input, cancellationToken);
    }

    private async ValueTask<Result<Unit, Failure<HandlerFailureCode>>> InnerHandleAsync(
        BotRequestJson? requestJson, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(requestJson?.Body))
        {
            return Failure.Create(HandlerFailureCode.Persistent, "Bot request body must be specified");
        }

        var httpRequest = BuildHttpRequest(requestJson);
        var httpResponse = httpRequest.HttpContext.Response;

        await botFrameworkHttpAdapter.ProcessAsync(httpRequest, httpResponse, bot, cancellationToken).ConfigureAwait(false);

        if (httpResponse.StatusCode is >= 200 and < 300)
        {
            return Result.Success<Unit>(default);
        }

        var responseBody = await ReadBodyAsync(httpResponse, cancellationToken).ConfigureAwait(false);

        var failureCode = httpResponse.StatusCode switch
        {
            400 => HandlerFailureCode.Persistent,
            _   => HandlerFailureCode.Transient
        };

        return Failure.Create(failureCode, $"Bot request failed with status code {httpResponse.StatusCode}. Response body: {responseBody}");

        static HttpRequest BuildHttpRequest(BotRequestJson requestJson)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Method = requestJson.Method.OrNullIfEmpty() ?? "POST";

            if (string.IsNullOrEmpty(requestJson.Body) is false)
            {
                var byteArray = Encoding.UTF8.GetBytes(requestJson.Body);
                httpContext.Request.Body = new MemoryStream(byteArray);
            }

            if (requestJson.Headers?.Count is not > 0)
            {
                return httpContext.Request;
            }

            foreach (var header in requestJson.Headers)
            {
                httpContext.Request.Headers[header.Key] = header.Value;
            }

            return httpContext.Request;
        }
    }

    private static async Task<string> ReadBodyAsync(HttpResponse response, CancellationToken cancellationToken)
    {
        response.Body.Seek(0, SeekOrigin.Begin);

        using var reader = new StreamReader(response.Body);
        return await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
    }
}