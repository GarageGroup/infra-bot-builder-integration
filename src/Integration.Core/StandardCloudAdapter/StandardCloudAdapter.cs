using System.Net.Http;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

namespace GarageGroup.Infra.Bot.Builder;

public sealed partial class StandardCloudAdapter : CloudAdapter
{
    private static readonly object lockObject = new();

    private static volatile StandardCloudAdapter? instance;

    static StandardCloudAdapter()
        =>
        HttpHelper.BotMessageSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

    private StandardCloudAdapter(
        IConfiguration configuration,
        ISocketsHttpHandlerProvider? handlerProvider,
        ILoggerFactory? loggerFactory)
        :
        base(configuration, CreateHttpClientFactory(handlerProvider), loggerFactory?.CreateLogger<StandardCloudAdapter>())
    {
    }

    private static IHttpClientFactory? CreateHttpClientFactory(ISocketsHttpHandlerProvider? handlerProvider)
        =>
        handlerProvider is null ? null : new HttpClientFactoryImpl(handlerProvider);
}