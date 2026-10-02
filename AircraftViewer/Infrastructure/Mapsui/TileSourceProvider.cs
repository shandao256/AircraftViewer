using Mapsui.Layers;
using Mapsui.Tiling;

namespace AircraftViewer.Infrastructure.Mapsui;


public class TileSourceProvider
{
    public static ILayer CreateBaseLayer() => OpenStreetMap.CreateTileLayer();

}