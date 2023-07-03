using System.Net.Http;

namespace GarageGroup.Infra.Bot.Builder;

internal sealed partial class HttpClientFactoryImpl : IHttpClientFactory
{
    private readonly ISocketsHttpHandlerProvider socketsHttpHandlerProvider;

    internal HttpClientFactoryImpl(ISocketsHttpHandlerProvider socketsHttpHandlerProvider)
        =>
        this.socketsHttpHandlerProvider = socketsHttpHandlerProvider;
}