using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AircraftViewer.Infrastructure.OpenSky;

public sealed class OpenSkyApiClient
{
    private const string StatesUrl = "https://opensky-network.org/api/states/all";
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(90);

    // Temporary fixed bounding box (Western Europe) to limit poll size and
    // keep aircraft-layer rebuilds fast. TODO: replace with the map's live
    // viewport bounds once dynamic bbox-following is built.
    private const double LaMin = 35.0;
    private const double LaMax = 60.0;
    private const double LoMin = -10.0;
    private const double LoMax = 20.0;

    private readonly HttpClient _http;
    private readonly OpenSkyAuthService _auth;

    private CancellationTokenSource? _cts;
    private Task? _loopTask;

    public event Action<string>? StatesReceived;

    public OpenSkyApiClient(HttpClient http, OpenSkyAuthService auth)
    {
        _http = http;
        _auth = auth;
    }

    public void Start()
    {
        if (_loopTask is not null)
            return;

        _cts = new CancellationTokenSource();
        _loopTask = Task.Run(() => PollLoopAsync(_cts.Token));
    }

    public void Stop()
    {
        _cts?.Cancel();
        _cts = null;
        _loopTask = null;
    }

    private async Task PollLoopAsync(CancellationToken ct)
    {
        var requestUrl = $"{StatesUrl}?lamin={LaMin}&lomin={LoMin}&lamax={LaMax}&lomax={LoMax}";

        while (!ct.IsCancellationRequested)
        {
            try
            {
                Console.WriteLine($"[{DateTime.UtcNow:HH:mm:ss}] [OpenSkyApiClient] Requesting token...");
                var token = await _auth.GetValidTokenAsync(ct);

                using var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                request.Headers.Authorization = new("Bearer", token);

                using var response = await _http.SendAsync(request, ct);
                Console.WriteLine($"[{DateTime.UtcNow:HH:mm:ss}] [OpenSkyApiClient] HTTP {(int)response.StatusCode} {response.StatusCode}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync(ct);
                Console.WriteLine($"[{DateTime.UtcNow:HH:mm:ss}] [OpenSkyApiClient] Received {json.Length} bytes");

                StatesReceived?.Invoke(json);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.UtcNow:HH:mm:ss}] [OpenSkyApiClient] ERROR: {ex.GetType().Name}: {ex.Message}");
            }

            try
            {
                await Task.Delay(PollInterval, ct);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}