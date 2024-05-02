using SmartAsthmaAssistane.Views;

namespace SmartAsthmaAssistane
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(SignupPage), typeof(SignupPage));
        }
    }
}
