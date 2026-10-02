using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using AircraftViewer.Common;
using AircraftViewer.Common.Messages;
using AircraftViewer.ViewModels;

namespace AircraftViewer.Features.FlightInfo;

public partial class FlightInfoViewModel : ViewModelBase, IRecipient<AircraftSelectedMessage>
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelection))]
    private Aircraft? _selectedAircraft;

    // True once anything has ever been selected; never reverts to false.
    public bool HasSelection => SelectedAircraft is not null;

    public FlightInfoViewModel()
    {
        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    public void Receive(AircraftSelectedMessage message)
    {
        SelectedAircraft = message.Value;
    }
}