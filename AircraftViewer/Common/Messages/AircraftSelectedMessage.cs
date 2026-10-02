using AircraftViewer.Common;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AircraftViewer.Common.Messages;

/// <summary>
/// Sent when the user clicks an aircraft on the map. Carries the selected
/// Aircraft so any feature (currently just FlightInfo) can react without
/// MapView/MapViewModel needing a direct reference to it.
/// </summary>
public sealed class AircraftSelectedMessage(Aircraft aircraft) : ValueChangedMessage<Aircraft>(aircraft);