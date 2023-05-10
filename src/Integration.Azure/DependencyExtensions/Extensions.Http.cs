using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra.Bot.Builder;
using Microsoft.ApplicationInsights;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PrimeFuncPack;

namespace GarageGroup.Infra;

partial class BotFuncDependencyExtensions
{
    public static Task<HttpResponseData> RunBotFunctionAsync(
        this Dependency<IBot> dependency, HttpRequestData request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dependency);
        ArgumentNullException.ThrowIfNull(request);

        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled<HttpResponseData>(cancellationToken);
        }

        var adapter = StandardCloudAdapter.Resolve(request.FunctionContext.InstanceServices);
        var bot = dependency.Resolve(request.FunctionContext.InstanceServices);

        return adapter.InternalProcessAsync(request, bot, cancellationToken);
    }

    private static async Task<HttpResponseData> InternalProcessAsync(
        this IBotFrameworkHttpAdapter adapter, HttpRequestData request, IBot bot, CancellationToken cancellationToken)
    {
        var httpRequest = await request.ToHttpRequestAsync(cancellationToken).ConfigureAwait(false);
        var httpResponse = httpRequest.HttpContext.Response;

        await adapter.ProcessAsync(httpRequest, httpResponse, bot, cancellationToken).ConfigureAwait(false);

        if (httpResponse.StatusCode >= 400)
        {
            var sourceData = await ReadStringAsync(request.Body).ConfigureAwait(false);
            var content = await ReadStringAsync(httpResponse.Body).ConfigureAwait(false);

            request.FunctionContext.TrackErrorStatusCode(sourceData, httpResponse.StatusCode, content);
        }

        return await request.CreateResponseAsync(httpResponse, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<string?> ReadStringAsync(this Stream stream)
    {
        using var streamReader = new StreamReader(stream, Encoding.UTF8);
        return await streamReader.ReadToEndAsync().ConfigureAwait(false);
    }

    private static void TrackErrorStatusCode(this FunctionContext context, string? sourceData, int code, string? message)
    {
        var functionName = context.FunctionDefinition.Name;
        context.GetLogger(functionName).LogError("An unexpected HTTP Bot Function status code: {code}. Message: {message}", code, message);

        context.InstanceServices.GetService<TelemetryClient>()?.TrackEvent(
            "HttpBotFunctionHadnlerError",
            new Dictionary<string, string>
            {
                ["data"] = sourceData.OrEmpty(),
                ["errorCode"] = code.ToString(),
                ["errorMessage"] = message.OrEmpty(),
                ["function"] = functionName
            });
    }
}