using System;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra.Bot.Builder;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using PrimeFuncPack;

namespace GGroupp.Infra;

public static class BotFuncDependencyExtensions
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
        var httpRequest = request.ToHttpRequest();
        var httpResponse = httpRequest.HttpContext.Response;

        await adapter.ProcessAsync(httpRequest, httpResponse, bot, cancellationToken).ConfigureAwait(false);
        return await request.CreateResponseAsync(httpResponse, cancellationToken).ConfigureAwait(false);
    }
}