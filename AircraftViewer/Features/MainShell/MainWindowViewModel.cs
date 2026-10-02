using System;
using System.IO;
using System.Net.Http;
using AircraftViewer.Features.AircraftTracking.Services;
using AircraftViewer.Features.FlightInfo;
using AircraftViewer.Features.MapView;
using AircraftViewer.Infrastructure.AircraftData;
using AircraftViewer.Infrastructure.OpenSky;
using AircraftViewer.ViewModels;

namespace AircraftViewer.Features.MainShell;

public partial class MainWindowViewModel : ViewModelBase
{
    public MapViewModel Map { get; }
    public FlightInfoViewModel FlightInfo { get; }

    private readonly AircraftPositionService _positionService;

    public MainWindowViewModel()
    {
        var credentialsPath = Path.Combine(AppContext.BaseDirectory, "credentials.json");
        var credentials = OpenSkyCredentials.Load(credentialsPath);

        var httpClient = new HttpClient();
        var authService = new OpenSkyAuthService(httpClient, credentials);
        var apiClient = new OpenSkyApiClient(httpClient, authService);

        _positionService = new AircraftPositionService(apiClient);

        var csvPath = Path.Combine(AppContext.BaseDirectory, "Assets", "AircraftDb", "aircraftDatabase.csv");
        var typeLookup = AircraftTypeLookup.LoadFromCsv(csvPath);

        Map = new MapViewModel(_positionService, typeLookup);
        FlightInfo = new FlightInfoViewModel();

        _positionService.Start();
    }
}