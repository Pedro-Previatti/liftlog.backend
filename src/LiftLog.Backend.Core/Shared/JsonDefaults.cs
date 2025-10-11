using System.Text.Json;

namespace LiftLog.Backend.Core.Shared;

public static class JsonDefaults
{
    public static readonly JsonSerializerOptions CamelCaseIndented = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };
}
