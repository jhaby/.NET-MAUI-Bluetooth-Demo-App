using CommunityToolkit.Mvvm.Messaging;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using Shiny.Jobs;
using SmartAsthmaAssistane.Models;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace SmartAsthmaAssistane.Services;

public class DataBackgroundJob : IJob
{
    private readonly IBluetoothService _bluetoothService;
    private readonly DataService _database;

    public DataBackgroundJob(
        IBluetoothService bluetoothService,
        DataService database)
    {
        _bluetoothService = bluetoothService;
        _database = database;
    }

    public async Task Run(JobInfo jobInfo, CancellationToken cancelToken)
    {
        try
        {
            if (await Permissions.CheckStatusAsync<Permissions.Bluetooth>() != PermissionStatus.Granted)
            {
                await Permissions.RequestAsync<Permissions.Bluetooth>();
            }
#if ANDROID
            var enable = new Android.Content.Intent(
            Android.Bluetooth.BluetoothAdapter.ActionRequestEnable);

            enable.SetFlags(Android.Content.ActivityFlags.NewTask);

            var disable = new Android.Content.Intent(
            Android.Bluetooth.BluetoothAdapter.ActionRequestDiscoverable);

            disable.SetFlags(Android.Content.ActivityFlags.NewTask);

            var bluetoothManager = (Android.Bluetooth.BluetoothManager)Android.App.Application.Context.GetSystemService(Android.Content.Context.BluetoothService);
            var bluetoothAdapter = bluetoothManager.Adapter;

            if (!bluetoothAdapter.IsEnabled)
            {
                await Shell.Current.DisplayAlert("Bluetooth", "Please enable blueTooth and connect again", "Ok");
                Android.App.Application.Context.StartActivity(enable);
                return;
            }

#endif
            var devices = _bluetoothService.GetConnectedDevices();
            if (devices != null)
            {
                IDevice? device = devices.FirstOrDefault(x => x.Name == Constants.DeviceName);
                if (device != null)
                {
                    if (device.State == Plugin.BLE.Abstractions.DeviceState.Disconnected)
                    {
                        await _bluetoothService.Connect(device.Name);
                    }

                    if (device.State == Plugin.BLE.Abstractions.DeviceState.Connected)
                    {
                        await Shell.Current.DisplayAlert("Success", $"Connected to {Constants.DeviceName}", "Ok");
                        WeakReferenceMessenger.Default.Send(new DeviceConnectedMessage(Constants.DeviceName));
                    }
                }
            }

            while (true)
            {
                await _bluetoothService.Send(Constants.DeviceName, Encoding.UTF8.GetBytes("ping"));
                var response = await _bluetoothService.Receive(Constants.DeviceName);
                string readingRaw = Encoding.UTF8.GetString(response);

                //await Shell.Current.DisplayAlert("Received Temperature", $"{readingRaw} degrees Celsius", "Ok");

                if (!string.IsNullOrEmpty(readingRaw) &&
                    readingRaw.Contains("Temp", StringComparison.CurrentCultureIgnoreCase))
                {
                    Debug.Write($"RECEIVED MSG: {readingRaw}");
                    string pattern = @"Temp = (\d+\.\d+)";
                    Match match = Regex.Match(readingRaw, pattern);
                    if (match.Success)
                    {
                        // Accessing the matched value
                        string tempValue = match.Groups[1].Value;
                        //await Shell.Current.DisplayAlert("Received Temperature", $"{tempValue} degrees Celsius", "Ok");
                        if (float.TryParse(tempValue, out float temp))
                        {
                            Preferences.Set(Constants.TempValue, temp);

                            WeakReferenceMessenger.Default.Send(new DataReceivedMessage(new Item
                            {
                                RecordedOn = DateTime.Now,
                                Parameter = "Temp",
                                Value = temp
                            }));
                        }
                    }
                }
                if (!string.IsNullOrEmpty(readingRaw) &&
                    readingRaw.Contains("Humidity", StringComparison.CurrentCultureIgnoreCase))
                {
                    string humidityPattern = @"Humidity = (\d+\.\d+)";
                    Match humidityMatch = Regex.Match(readingRaw, humidityPattern);
                    if (humidityMatch.Success)
                    {
                        // Accessing the matched value
                        string humidityValue = humidityMatch.Groups[1].Value;
                        //await Shell.Current.DisplayAlert("Received Temperature", $"{humidityValue} degrees Celsius", "Ok");
                        if (float.TryParse(humidityValue, out float humidity))
                        {
                            Preferences.Set(Constants.TempValue, humidity);

                            WeakReferenceMessenger.Default.Send(new DataReceivedMessage(new Item
                            {
                                RecordedOn = DateTime.Now,
                                Parameter = "Humidity",
                                Value = humidity
                            }));
                        }

                    }

                    //await Shell.Current.DisplayAlert("Received Temperature", $"{readingParsed} degrees Celsius", "Ok");

                }

                await Task.Delay(TimeSpan.FromSeconds(5), cancelToken);
            }

        }
        catch (DeviceConnectionException ex)
        {
            Debug.Write(ex.Message);
            //await Shell.Current.DisplayAlert("Connection error", "No device connected", "Ok");
            WeakReferenceMessenger.Default.Send(new DeviceConnectedMessage(string.Empty));
        }
        catch (ArgumentNullException)
        {
            Debug.WriteLine("No device connected");
            await Shell.Current.DisplayAlert("Connection error", "No device connected", "Ok");
            WeakReferenceMessenger.Default.Send(new DeviceConnectedMessage(string.Empty));
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            //await Shell.Current.DisplayAlert("No data",
            //    ex.Message,
            //    "Ok");
            WeakReferenceMessenger.Default.Send(new DeviceConnectedMessage(string.Empty));

        }


    }

}
