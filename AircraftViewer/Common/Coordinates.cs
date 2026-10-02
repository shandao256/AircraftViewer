namespace AircraftViewer.Common;

public class Coordinates
{
    public double Latitude { get; }
    public double Longitude { get; }

    public Coordinates(double latitude, double longitude)
    {
        Longitude  = longitude;
        Latitude = latitude;
    }

    public override string ToString()
    {
        return $"{Latitude},{Longitude}";
    }
}