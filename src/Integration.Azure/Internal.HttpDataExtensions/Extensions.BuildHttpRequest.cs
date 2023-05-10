using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace GarageGroup.Infra;

partial class HttpDataExtensions
{
    internal static HttpRequest BuildHttpRequest(this JsonElement requestData)
        =>
        requestData.Deserialize<RequestDataJson>(SerializerOptions).BuildHttpRequest();
}