using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Shiny;
using SmartAsthmaAssistane.ViewModels;
using SmartAsthmaAssistane.Services;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE;
using Shiny.Jobs;
using SmartAsthmaAssistane.Views;
using InputKit.Handlers;

namespace SmartAsthmaAssistane
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseSkiaSharp()
                .UseShiny()
                .UseMauiCommunityToolkit()
                .ConfigureMauiHandlers(handlers =>
                {
                    handlers.AddInputKitHandlers();
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<DataService>();
            builder.Services.AddSingleton<IBluetoothLE>(CrossBluetoothLE.Current);
            builder.Services.AddSingleton<IAdapter>(CrossBluetoothLE.Current.Adapter);
            builder.Services.AddSingleton<IBluetoothService, BluetoothService>();

#if __MOBILE__

            JobInfo bluetoothJob = new JobInfo(
                Identifier: "DataBackgroundJob",
                JobType: typeof(DataBackgroundJob),
                false,
                new Dictionary<string, string>
                {
                                { "loops", "100" }
                },
                InternetAccess.Any,
                false,
                false,
                false
            );

            builder.Services.AddJob(bluetoothJob);

#endif
            builder.Services.AddScoped<FirebaseService>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<SignupViewModel>();
            builder.Services.AddScoped<MainViewModel>();
            builder.Services.AddScoped<DashboardViewModel>();

            builder.Services.AddTransient<SignupPage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddScoped<MainPage>();
            builder.Services.AddScoped<DashboardView>();

            return builder.Build();
        }
    }
}
