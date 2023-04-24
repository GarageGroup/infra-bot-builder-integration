namespace GGroupp.Infra;

internal static partial class BotFunctionBuilder
{
    private const string EmptyStringConstantSourceCode = "\"\"";

    private static string AsStringSourceCode(this string? source, string defaultSourceCode = "null")
        =>
        string.IsNullOrEmpty(source) ? defaultSourceCode : $"\"{source.EncodeString()}\"";

    private static string? EncodeString(this string? source)
        =>
        source?.Replace("\"", "\\\"");
}