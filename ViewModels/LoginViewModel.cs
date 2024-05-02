using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartAsthmaAssistane.Services;
using SmartAsthmaAssistane.Views;

namespace SmartAsthmaAssistane.ViewModels;

public partial class LoginViewModel(FirebaseService firebaseService) : BaseViewModel
{
    [ObservableProperty]
    string email = string.Empty;

    [ObservableProperty]
    string password = string.Empty;

    [RelayCommand]
    public async Task Login()
    {
        if (Connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            await Shell.Current.DisplayAlert("No internet!", "Please check your internet connection and try again", "ok");
            return;
        }

        IsBusy = true;
        if (await firebaseService.SigninWithEmail(Email, Password))
        {
            await Shell.Current.GoToAsync($"//{nameof(DashboardView)}");
        }
        else
        {
            await Shell.Current.DisplayAlert("Unauthorized", "Wrong email or password", "Try again");
        }

        
        IsBusy = false;
        
    }

    [RelayCommand]
    public async Task CreateAccount() => await Shell.Current.GoToAsync(nameof(SignupPage));
}
