using System.Collections.Generic;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Styles;
using AircraftViewer.Common;
using AircraftViewer.Infrastructure.AircraftData;
using AircraftViewer.Infrastructure.Mapsui;

namespace AircraftViewer.Features.MapView;

/// <summary>
/// Builds the Mapsui "Aircraft" layer from a snapshot of tracked aircraft.
/// Icons are resolved per-aircraft via AircraftTypeLookup + AircraftIconProvider
/// (embedded SVGs) and rotated in place via ImageStyle.SymbolRotation, rather
/// than selecting from pre-rotated PNG frames.
/// </summary>
public static class AircraftMapLayer
{
    public static ILayer CreateEmpty() => Build([], typeLookup: null);

    public static ILayer Build(IEnumerable<Aircraft> aircraftList, AircraftTypeLookup? typeLookup)
    {
        var features = new List<IFeature>();

        foreach (var aircraft in aircraftList)
        {
            var point = SphericalMercator
                .FromLonLat(aircraft.Longitude, aircraft.Latitude)
                .ToMPoint();

            var feature = new PointFeature(point);
            feature["icao"] = aircraft.Icao;

            var typeCode = typeLookup?.GetTypeCode(aircraft.Icao);
            var iconPath = AircraftIconProvider.GetIconPath(typeCode);

            feature.Styles.Add(new ImageStyle
            {
                Image = iconPath,
                SymbolScale = 0.5,
                SymbolRotation = aircraft.Trak,
                RotateWithMap = true,
            });

            features.Add(feature);
        }

        return new MemoryLayer
        {
            Name = MapFactory.AircraftLayerName,
            Features = features,
            Style = null,
        };
    }
}