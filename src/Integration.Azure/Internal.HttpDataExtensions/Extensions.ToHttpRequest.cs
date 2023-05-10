using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker.Http;

namespace GarageGroup.Infra;

partial class HttpDataExtensions
{
    internal static async Task<HttpRequest> ToHttpRequestAsync(this HttpRequestData httpRequestData, CancellationToken cancellationToken)
    {
        var body = httpRequestData.Body;
        var requestJson = await JsonSerializer.DeserializeAsync<RequestDataJson>(body, SerializerOptions, cancellationToken).ConfigureAwait(false);
        return requestJson.BuildHttpRequest();
    }
}