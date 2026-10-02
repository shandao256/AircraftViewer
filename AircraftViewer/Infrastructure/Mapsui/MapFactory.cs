using AircraftViewer.Features.MapView;
using global::Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Projections;

namespace AircraftViewer.Infrastructure.Mapsui;

public static class MapFactory
{
    public const string AircraftLayerName = "Aircraft";

    public static Map Create()
    {
        var map = new Map();
        map.Layers.Add(TileSourceProvider.CreateBaseLayer());
        map.Layers.Add(AircraftMapLayer.CreateEmpty());

        // Centered roughly on central Europe, matching OpenSkyApiClient's
        // current fixed bounding box (lat 35-60, lon -10-20).
        var center = SphericalMercator.FromLonLat(10, 50).ToMPoint();
        map.Navigator.CenterOnAndZoomTo(center, map.Navigator.Resolutions[3]);

        return map;
    }
}