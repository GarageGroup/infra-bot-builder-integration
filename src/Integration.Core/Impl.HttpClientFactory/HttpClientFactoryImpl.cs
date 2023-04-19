using System.Net.Http;

namespace GGroupp.Infra.Bot.Builder;

internal sealed partial class HttpClientFactoryImpl : IHttpClientFactory
{
    private readonly ISocketsHttpHandlerProvider socketsHttpHandlerProvider;

    internal HttpClientFactoryImpl(ISocketsHttpHandlerProvider socketsHttpHandlerProvider)
        =>
        this.socketsHttpHandlerProvider = socketsHttpHandlerProvider;
}