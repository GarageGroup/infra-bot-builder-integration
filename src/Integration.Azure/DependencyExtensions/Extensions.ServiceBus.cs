using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra.Bot.Builder;
using Microsoft.ApplicationInsights;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PrimeFuncPack;

namespace GGroupp.Infra;

partial class BotFuncDependencyExtensions
{
    public static Task RunBotFunctionAsync(
        this Dependency<IBot> dependency, JsonElement requestData, FunctionContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dependency);
        ArgumentNullException.ThrowIfNull(context);

        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled(cancellationToken);
        }

        return dependency.InnerRunBotFunctionAsync(requestData, context, cancellationToken);
    }

    private static async Task InnerRunBotFunctionAsync(
        this Dependency<IBot> dependency, JsonElement requestData, FunctionContext context, CancellationToken cancellationToken)
    {
        try
        {
            var adapter = StandardCloudAdapter.Resolve(context.InstanceServices);
            var bot = dependency.Resolve(context.InstanceServices);

            await adapter.InternalProcessAsync(requestData, bot, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            var functionName = context.FunctionDefinition.Name;
            context.GetLogger(functionName).LogError(exception, "An unexpected exception occured while trying to handle a bot Service Bus message");

            context.InstanceServices.GetService<TelemetryClient>()?.TrackEvent(
                "ServiceBusBotFunctionHadnlerError",
                new Dictionary<string, string>
                {
                    ["data"] = requestData.ToString(),
                    ["errorType"] = exception.GetType().FullName.OrEmpty(),
                    ["errorMessage"] = exception.Message,
                    ["errorStackTrace"] = exception.StackTrace.OrEmpty(),
                    ["function"] = functionName
                });
        }
    }

    private static async Task InternalProcessAsync(
        this IBotFrameworkHttpAdapter adapter, JsonElement requestData, IBot bot, CancellationToken cancellationToken)
    {
        var httpRequest = requestData.BuildHttpRequest();
        var httpResponse = httpRequest.HttpContext.Response;

        await adapter.ProcessAsync(httpRequest, httpResponse, bot, cancellationToken).ConfigureAwait(false);
        await httpResponse.ThrowIfFailureAsync(cancellationToken).ConfigureAwait(false);
    }
}