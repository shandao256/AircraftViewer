using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AircraftViewer.Infrastructure.OpenSky;

/// <summary>
/// Maps directly onto opensky-credentials.json (AppContext.BaseDirectory, .gitignored).
/// </summary>
public sealed class OpenSkyCredentials
{
    [JsonPropertyName("clientId")]
    public required string ClientId { get; init; }

    [JsonPropertyName("clientSecret")]
    public required string ClientSecret { get; init; }

    public static OpenSkyCredentials Load(string path)
    {
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<OpenSkyCredentials>(json)
               ?? throw new InvalidOperationException($"Failed to parse credentials at '{path}'.");
    }
}