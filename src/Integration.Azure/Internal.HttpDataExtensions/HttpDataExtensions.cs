using System.Text.Json;

namespace GGroupp.Infra;

internal static partial class HttpDataExtensions
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
}