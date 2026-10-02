using System;
using Coordinates = AircraftViewer.Common.Coordinates;

namespace AircraftViewer.Common;

public class Aircraft
{
    public string Icao { get; set; }
    public string Callsign { get; set; }
    public DateTime PosTime { get; set; }
    public Coordinates Coordinates { get; set; }

    public double Latitude => Coordinates.Latitude;
    public double Longitude => Coordinates.Longitude;
    public double Speed {get; set;}
    public double Altitude {get; set;}
    public double Trak { get; set; }

    public Aircraft(string icao, string callsign, DateTime posTime, Coordinates coordinates, double speed,
        double altitude, double trak)
    {
        this.Icao = icao;
        this.Callsign = callsign;
        this.PosTime = posTime;
        this.Coordinates = coordinates;
        this.Speed = speed;
        this.Altitude = altitude;
        this.Trak = trak;
    }

    public override string ToString() => $"BasicAircraft [icao={Icao}, callsign={Callsign}, posTime={PosTime}, coordinate={Coordinates}, speed={Speed}, altitude={Altitude}, trak={Trak}]";   
    
}