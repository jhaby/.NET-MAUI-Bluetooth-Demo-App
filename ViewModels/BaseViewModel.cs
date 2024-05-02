using CommunityToolkit.Mvvm.ComponentModel;

namespace SmartAsthmaAssistane.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    bool isBusy = false;   
}
