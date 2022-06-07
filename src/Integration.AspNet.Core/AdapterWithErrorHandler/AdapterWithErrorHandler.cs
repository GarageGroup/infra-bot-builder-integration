using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

namespace GGroupp.Infra.Bot.Builder;

internal sealed class AdapterWithErrorHandler : CloudAdapter
{
    static AdapterWithErrorHandler()
        =>
        HttpHelper.BotMessageSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

    internal AdapterWithErrorHandler(
        IConfiguration configuration,
        ISocketsHttpHandlerProvider? handlerProvider,
        ILogger<AdapterWithErrorHandler> logger)
        :
        base(configuration, CreateHttpClientFactory(handlerProvider), logger)
    {
        OnTurnError = (turnContext, exception) =>
        {
            logger.LogError(exception, "[OnTurnError] unhandled error : {message}", exception.Message);
            return Task.CompletedTask;
        };
    }

    private static IHttpClientFactory? CreateHttpClientFactory(ISocketsHttpHandlerProvider? handlerProvider)
        =>
        handlerProvider is null ? null : new HttpClientFactoryImpl(handlerProvider);
}