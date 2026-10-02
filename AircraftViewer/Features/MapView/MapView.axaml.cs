using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;
using AircraftViewer.Common.Messages;
using Mapsui;
using Mapsui.UI.Avalonia;

namespace AircraftViewer.Features.MapView;

public partial class MapView : UserControl
{
    public MapView()
    {
        InitializeComponent();

        var mapControl = this.FindControl<MapControl>("Map");
        if (mapControl is not null)
            mapControl.Info += OnMapInfo;
    }

    private void OnMapInfo(object? sender, MapInfoEventArgs e)
    {
        if (sender is not MapControl mapControl) return;
        
        var mapInfo = e.GetMapInfo(mapControl.Map.Layers);
        var feature = mapInfo?.Feature;
        if (feature is null) return;

        if (feature["icao"] is not string icao) return;

        if (DataContext is not MapViewModel viewModel) return;

        var aircraft = viewModel.FindAircraftByIcao(icao);
        if (aircraft is not null)
            WeakReferenceMessenger.Default.Send(new AircraftSelectedMessage(aircraft));
    }
}