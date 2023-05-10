using System.Net.Http;
using GGroupp.Infra;

namespace GarageGroup.Infra.Bot.Builder;

internal sealed partial class HttpClientFactoryImpl : IHttpClientFactory
{
    private readonly ISocketsHttpHandlerProvider socketsHttpHandlerProvider;

    internal HttpClientFactoryImpl(ISocketsHttpHandlerProvider socketsHttpHandlerProvider)
        =>
        this.socketsHttpHandlerProvider = socketsHttpHandlerProvider;
}