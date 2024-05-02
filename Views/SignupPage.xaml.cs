using SmartAsthmaAssistane.ViewModels;

namespace SmartAsthmaAssistane.Views;

public partial class SignupPage : ContentPage
{
    private readonly SignupViewModel _viewModel;

    public SignupPage(SignupViewModel viewModel)
	{
		InitializeComponent();
        BindingContext =  _viewModel = viewModel;
    }
}