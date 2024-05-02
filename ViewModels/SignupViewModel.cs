using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartAsthmaAssistane.Services;
using SmartAsthmaAssistane.Views;

namespace SmartAsthmaAssistane.ViewModels;

public partial class SignupViewModel(FirebaseService firebaseService) : BaseViewModel
{
    [ObservableProperty]
    string email = string.Empty;
    [ObservableProperty]
    string password = string.Empty;
    [ObservableProperty]
    string confirmPassword = string.Empty;

    [RelayCommand]
    public async Task SignupAsync()
    {
        if(Connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            await Shell.Current.DisplayAlert("No internet!", "Please check your internet connection and try again", "ok");
            return;
        }

        IsBusy = true;
        if(Password == ConfirmPassword)
        {
            if (await firebaseService.CreateFirebaseAccount(Email, Password))
            {
                await Shell.Current.GoToAsync($"//{nameof(DashboardView)}");
                return;
            }
        }

        await Shell.Current.DisplayAlert("Operation failed!", "Please ensure that the passwords match and the email is valid", "Try again");

        IsBusy = false;
    }

    [RelayCommand]
    public async Task GoToLogin() => await Shell.Current.GoToAsync("..");
}
