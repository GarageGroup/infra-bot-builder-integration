using System.Net.Http;

namespace GarageGroup.Infra.Bot.Builder;

partial class HttpClientFactoryImpl
{
    public HttpClient CreateClient(string name)
        =>
        new(
            handler: socketsHttpHandlerProvider.GetOrCreate(name),
            disposeHandler: false);
}