using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore;
using Plugin.BLE.Abstractions.Contracts;
using Shiny.Jobs;
using SmartAsthmaAssistane.Models;
using SmartAsthmaAssistane.Services;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LiveChartsCore.Measure;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace SmartAsthmaAssistane.ViewModels;
public partial class DashboardViewModel : 
    BaseViewModel, IRecipient<DataReceivedMessage>, IRecipient<DeviceConnectedMessage>
{
    public DashboardViewModel(DataService dataService, IAdapter adapter,
        IBluetoothService bluetoothService
#if __MOBILE__
        ,IJobManager jobManager
#endif
        )
    {
        this.dataService = dataService;
        this.adapter = adapter;
        this.bluetoothService = bluetoothService;
#if __MOBILE__
        _jobManager = jobManager;
#endif

        WeakReferenceMessenger.Default.RegisterAll(this);
        
    }

    private readonly DataService dataService;
    private readonly IAdapter adapter;
    private readonly IBluetoothService bluetoothService;
#if __MOBILE__
    private readonly IJobManager _jobManager;
#endif

    [ObservableProperty]
    UserInfo? user;

    [ObservableProperty]
    string? connectedDevice;

    [ObservableProperty]
    bool isBtnActive = true;

    [ObservableProperty]
    ObservableCollection<Reminder> reminders = [
        new Reminder{
            Name = "Trigger temperatures detected",
            Description = "Take 4 puffs of inhaler in the next 30 mins to prevent attack"
        },
        new Reminder 
        {
            Name = "Trigger temperatures detected",
            Description = "Take 4 puffs of inhaler in the next 30 mins to prevent attack"
        },
        new Reminder
        {
            Name = "Trigger temperatures detected",
            Description = "Take 4 puffs of inhaler in the next 30 mins to prevent attack"
        }
        ];

    [ObservableProperty]
    IEnumerable<ISeries> tempSeries =
        GaugeGenerator.BuildSolidGauge(
            new GaugeItem(15, series =>
            {
                series.Fill = new SolidColorPaint(SKColors.DarkSlateBlue);
                series.DataLabelsSize = 32;
                series.DataLabelsPaint = new SolidColorPaint(SKColors.SlateBlue);
                series.DataLabelsPosition = PolarLabelsPosition.ChartCenter;
                series.InnerRadius = 55;
            }),
            new GaugeItem(GaugeItem.Background, series =>
            {
                series.InnerRadius = 55;
                series.Fill = new SolidColorPaint(new SKColor(100, 181, 246, 90));
            }));

    [ObservableProperty]
    IEnumerable<ISeries> humiditySeries =
        GaugeGenerator.BuildSolidGauge(
            new GaugeItem(30, series =>
            {
                series.Fill = new SolidColorPaint(SKColors.DeepPink);
                series.DataLabelsSize = 32;
                series.DataLabelsPaint = new SolidColorPaint(SKColors.LightPink);
                series.DataLabelsPosition = PolarLabelsPosition.ChartCenter;
                series.InnerRadius = 55;
            }),
            new GaugeItem(GaugeItem.Background, series =>
            {
                series.InnerRadius = 55;
                series.Fill = new SolidColorPaint(new SKColor(160, 191, 246, 90));
            }));

    [RelayCommand]
    public async Task LoadItems()
    {
        IsBusy = true;
        string? userRaw = Preferences.Get(Constants.firebaseUser, null);
        if (!string.IsNullOrEmpty(userRaw))
        {
            User = JsonConvert.DeserializeObject<UserInfo>(userRaw);
        }
        await Task.Delay(500);
#if __MOBILE__

        if (!_jobManager.IsRunning)
        {
            await _jobManager.RunAll();
        }

#endif
        IsBusy = false;
        await Task.CompletedTask;
    }
    public async Task OnAppearing() => await LoadItems();
    public void Receive(DataReceivedMessage message)
    {
        if (message.Value.Parameter == "Temp")
        {
            TempSeries = GaugeGenerator.BuildSolidGauge(
            new GaugeItem(message.Value.Value, series =>
            {
                series.Fill = new SolidColorPaint(SKColors.DarkSlateBlue);
                series.DataLabelsSize = 32;
                series.DataLabelsPaint = new SolidColorPaint(SKColors.SlateBlue);
                series.DataLabelsPosition = PolarLabelsPosition.ChartCenter;
                series.InnerRadius = 55;
            }),
            new GaugeItem(GaugeItem.Background, series =>
            {
                series.InnerRadius = 55;
                series.Fill = new SolidColorPaint(new SKColor(100, 181, 246, 90));
            }));

            Reminders.Insert(0,new Reminder
            {
                Name = "Trigger temperatures detected",
                Description = "Take 4 puffs of inhaler in the next 30 mins to prevent attack"
            });

            Reminders.RemoveAt(2);

        }
        if (message.Value.Parameter == "Humidity")
        {
            HumiditySeries = GaugeGenerator.BuildSolidGauge(
            new GaugeItem(message.Value.Value, series =>
            {
                series.Fill = new SolidColorPaint(SKColors.DeepPink);
                series.DataLabelsSize = 32;
                series.DataLabelsPaint = new SolidColorPaint(SKColors.LightPink);
                series.DataLabelsPosition = PolarLabelsPosition.ChartCenter;
                series.InnerRadius = 55;
            }),
            new GaugeItem(GaugeItem.Background, series =>
            {
                series.InnerRadius = 55;
                series.Fill = new SolidColorPaint(new SKColor(100, 181, 246, 90));
            }));

            Reminders.Add(new Reminder
            {
                Name = "High humidity",
                Description = "Take 4 puffs of inhaler 2 times 30 mins apart"
            });

            Reminders.RemoveAt(3);
        }

    }

    public void Receive(DeviceConnectedMessage message)
    {
        if (!string.IsNullOrEmpty(message.Value))
        {
            ConnectedDevice = message.Value;
            IsBtnActive = false;
        }
        else
        {
            ConnectedDevice = null;
            IsBtnActive = true;
        }
    }
}
