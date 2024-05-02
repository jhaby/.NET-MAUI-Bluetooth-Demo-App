using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore;
using SmartAsthmaAssistane.Models;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LiveChartsCore.Measure;
using Shiny.Jobs;
using SmartAsthmaAssistane.Services;
using Plugin.BLE.Abstractions.Contracts;

namespace SmartAsthmaAssistane.ViewModels;

public partial class MainViewModel : BaseViewModel, IRecipient<DataReceivedMessage>
{
    public MainViewModel(DataService dataService, IAdapter adapter,
        IBluetoothService bluetoothService, IJobManager jobManager)
    {
        this.dataService = dataService;
        this.adapter = adapter;
        this.bluetoothService = bluetoothService;
        _jobManager = jobManager;

        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    [ObservableProperty]
    IEnumerable<ISeries> tempSeries =
       GaugeGenerator.BuildSolidGauge(
           new GaugeItem(15, series =>
           {
               series.Fill = new SolidColorPaint(SKColors.YellowGreen);
               series.DataLabelsSize = 50;
               series.DataLabelsPaint = new SolidColorPaint(SKColors.Red);
               series.DataLabelsPosition = PolarLabelsPosition.ChartCenter;
               series.InnerRadius = 75;
           }),
           new GaugeItem(GaugeItem.Background, series =>
           {
               series.InnerRadius = 75;
               series.Fill = new SolidColorPaint(new SKColor(100, 181, 246, 90));
           }));

    [ObservableProperty]
    IEnumerable<ISeries> humiditySeries =
        GaugeGenerator.BuildSolidGauge(
            new GaugeItem(30, series =>
            {
                series.Fill = new SolidColorPaint(SKColors.YellowGreen);
                series.DataLabelsSize = 50;
                series.DataLabelsPaint = new SolidColorPaint(SKColors.Red);
                series.DataLabelsPosition = PolarLabelsPosition.ChartCenter;
                series.InnerRadius = 75;
            }),
            new GaugeItem(GaugeItem.Background, series =>
            {
                series.InnerRadius = 75;
                series.Fill = new SolidColorPaint(new SKColor(100, 181, 246, 90));
            }));
    private readonly DataService dataService;
    private readonly IAdapter adapter;
    private readonly IBluetoothService bluetoothService;
    private readonly IJobManager _jobManager;

    [RelayCommand]
    public async Task LoadItems()
    {
#if __MOBILE__

        if (!_jobManager.IsRunning)
        {
            await _jobManager.RunAll();
        }

#endif

    }

    [RelayCommand]
    public async Task Connect()
    {
#if __MOBILE__

        if (!_jobManager.IsRunning)
        {
            await _jobManager.RunAll();
        }
#endif

    }
    public async Task OnAppearing() => await LoadItems();

    public void Receive(DataReceivedMessage message)
    {
        if (message.Value.Parameter == "Temp")
        {
            TempSeries = GaugeGenerator.BuildSolidGauge(
            new GaugeItem(message.Value.Value, series =>
            {
                series.Fill = new SolidColorPaint(SKColors.YellowGreen);
                series.DataLabelsSize = 50;
                series.DataLabelsPaint = new SolidColorPaint(SKColors.Red);
                series.DataLabelsPosition = PolarLabelsPosition.ChartCenter;
                series.InnerRadius = 75;
            }),
            new GaugeItem(GaugeItem.Background, series =>
            {
                series.InnerRadius = 75;
                series.Fill = new SolidColorPaint(new SKColor(100, 181, 246, 90));
            }));
        }
        if (message.Value.Parameter == "Humidity")
        {
            HumiditySeries = GaugeGenerator.BuildSolidGauge(
            new GaugeItem(message.Value.Value, series =>
            {
                series.Fill = new SolidColorPaint(SKColors.YellowGreen);
                series.DataLabelsSize = 50;
                series.DataLabelsPaint = new SolidColorPaint(SKColors.Red);
                series.DataLabelsPosition = PolarLabelsPosition.ChartCenter;
                series.InnerRadius = 75;
            }),
            new GaugeItem(GaugeItem.Background, series =>
            {
                series.InnerRadius = 75;
                series.Fill = new SolidColorPaint(new SKColor(100, 181, 246, 90));
            }));
        }

    }
}
