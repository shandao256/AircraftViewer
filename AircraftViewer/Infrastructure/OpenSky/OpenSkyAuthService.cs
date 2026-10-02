using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace AircraftViewer.Infrastructure.OpenSky;

public sealed class OpenSkyAuthService
{
    private const string TokenEndpoint =
        "https://auth.opensky-network.org/auth/realms/opensky-network/protocol/openid-connect/token";

    private readonly HttpClient _http;
    private readonly OpenSkyCredentials _credentials;

    private string? _cachedToken;
    private DateTimeOffset _expiresAt = DateTimeOffset.MinValue;
    private readonly SemaphoreSlim _refreshLock = new(1, 1);

    public OpenSkyAuthService(HttpClient http, OpenSkyCredentials credentials)
    {
        _http = http;
        _credentials = credentials;
    }

    /// <summary>
    /// Returns a cached token if still valid, otherwise requests a new one.
    /// Safe to call concurrently; only one refresh happens at a time.
    /// </summary>
    public async Task<string> GetValidTokenAsync(CancellationToken ct = default)
    {
        // 30s safety margin before actual expiry
        if (_cachedToken is not null && DateTimeOffset.UtcNow < _expiresAt - TimeSpan.FromSeconds(30))
            return _cachedToken;

        await _refreshLock.WaitAsync(ct);
        try
        {
            // Re-check: another caller may have refreshed while we waited.
            if (_cachedToken is not null && DateTimeOffset.UtcNow < _expiresAt - TimeSpan.FromSeconds(30))
                return _cachedToken;

            var form = new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = _credentials.ClientId,
                ["client_secret"] = _credentials.ClientSecret,
            };

            using var response = await _http.PostAsync(TokenEndpoint, new FormUrlEncodedContent(form), ct);
            response.EnsureSuccessStatusCode();

            var payload = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: ct)
                          ?? throw new InvalidOperationException("Empty token response from OpenSky auth server.");

            _cachedToken = payload.AccessToken;
            _expiresAt = DateTimeOffset.UtcNow.AddSeconds(payload.ExpiresIn);

            return _cachedToken;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    private sealed class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public required string AccessToken { get; init; }

        [JsonPropertyName("expires_in")]
        public required int ExpiresIn { get; init; }
    }
}