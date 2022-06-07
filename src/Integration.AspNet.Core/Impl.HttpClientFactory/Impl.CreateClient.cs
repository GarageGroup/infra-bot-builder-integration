using System.Net.Http;

namespace GGroupp.Infra.Bot.Builder;

partial class HttpClientFactoryImpl
{
    public HttpClient CreateClient(string name)
        =>
        new(
            handler: socketsHttpHandlerProvider.GetOrCreate(name),
            disposeHandler: false);
}