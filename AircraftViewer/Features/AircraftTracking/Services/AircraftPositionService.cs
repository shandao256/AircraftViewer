using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json.Nodes;
using AircraftViewer.Common;
using AircraftViewer.Infrastructure.OpenSky;
using Avalonia.Threading;

namespace AircraftViewer.Features.AircraftTracking.Services;

/// <summary>
/// Owns the live set of tracked aircraft. Subscribes to OpenSkyApiClient's raw
/// JSON stream, parses it, and exposes the current snapshot as an observable
/// collection for the UI thread to bind to.
/// </summary>
public sealed class AircraftPositionService
{
    private readonly OpenSkyApiClient _apiClient;

    public ObservableCollection<Aircraft> Aircraft { get; } = new();

    /// <summary>Raised on the thread pool whenever a new snapshot has been parsed and applied.</summary>
    public event Action? AircraftUpdated;

    public AircraftPositionService(OpenSkyApiClient apiClient)
    {
        _apiClient = apiClient;
        _apiClient.StatesReceived += OnStatesReceived;
    }

    public void Start() => _apiClient.Start();
    public void Stop() => _apiClient.Stop();

    private void OnStatesReceived(string json)
    {
        var root = JsonNode.Parse(json);
        var states = root?["states"]?.AsArray();
        if (states is null)
            return;

        var updated = new List<Aircraft>();
        foreach (var state in states)
        {
            if (state is not JsonArray arr) continue;
            var ac = AircraftSentence.Parse(arr);
            if (ac is not null) updated.Add(ac);
        }

        Dispatcher.UIThread.Post(() =>
        {
            Aircraft.Clear();
            foreach (var ac in updated)
                Aircraft.Add(ac);
            AircraftUpdated?.Invoke();

        });

    }
}