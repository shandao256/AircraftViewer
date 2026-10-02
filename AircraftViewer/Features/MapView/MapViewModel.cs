using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using AircraftViewer.Common;
using AircraftViewer.Features.AircraftTracking.Services;
using AircraftViewer.Infrastructure.AircraftData;
using AircraftViewer.Infrastructure.Mapsui;
using AircraftViewer.ViewModels;
using Mapsui;

namespace AircraftViewer.Features.MapView;

public partial class MapViewModel : ViewModelBase
{
    private readonly AircraftPositionService _positionService;
    private readonly AircraftTypeLookup _typeLookup;

    [ObservableProperty]
    private global::Mapsui.Map _map;

    public MapViewModel(AircraftPositionService positionService, AircraftTypeLookup typeLookup)
    {
        _positionService = positionService;
        _typeLookup = typeLookup;

        _map = MapFactory.Create();

        _positionService.AircraftUpdated += OnAircraftUpdated;
    }

    /// <summary>Looks up a currently-tracked aircraft by its ICAO24 hex address, or null if not found.</summary>
    public Aircraft? FindAircraftByIcao(string icao) =>
        _positionService.Aircraft.FirstOrDefault(a => a.Icao == icao);

    private void OnAircraftUpdated()
    {
        var newLayer = AircraftMapLayer.Build(_positionService.Aircraft, _typeLookup);

        var existing = Map.Layers.FindLayer(MapFactory.AircraftLayerName).FirstOrDefault();
        if (existing is not null)
            Map.Layers.Remove(existing);

        Map.Layers.Add(newLayer);
        Map.RefreshData();
    }
}