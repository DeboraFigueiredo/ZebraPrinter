using Android.Bluetooth;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZebraPrinter.Platforms.Android;

namespace ZebraPrinter
{
    public partial class TelaInicial : ContentPage
    {
        private ZebraPrinterService _zebraPrinterService;
        private BluetoothDevice _selectedDevice;

        public TelaInicial()
        {
            InitializeComponent();
        }

        private async void OnSearchDevicesButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var context = Android.App.Application.Context;
                var discovery = new BluetoothDiscovery(context);
                var devices = await discovery.DiscoverDevicesAsync();

                if (devices.Count == 0)
                {
                    await DisplayAlert("Info", "No devices found.", "OK");
                    return;
                }

                DevicesListView.ItemsSource = devices;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private void OnDeviceSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is BluetoothDevice device)
            {
                _selectedDevice = device;
                _zebraPrinterService = new ZebraPrinterService(device);
                DisplayAlert("Info", $"Selected device: {device.Name}", "OK");
            }
        }

        private async void OnConnectButtonClicked(object sender, EventArgs e)
        {
            try
            {
                if (_selectedDevice != null)
                {
                    _zebraPrinterService = new ZebraPrinterService(_selectedDevice);
                    await _zebraPrinterService.ConnectAsync();
                    await DisplayAlert("Success", "Connected to printer.", "OK");
                }
                else
                {
                    await DisplayAlert("Error", "No printer selected. Please search and select a device first.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OnPrintButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var textToPrint = TextToPrintEntry.Text;
                if (_zebraPrinterService != null)
                {
                    _zebraPrinterService.Print(textToPrint);
                    await DisplayAlert("Success", "Print job sent.", "OK");
                }
                else
                {
                    await DisplayAlert("Error", "Printer not connected.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OnDisconnectButtonClicked(object sender, EventArgs e)
        {
            try
            {
                _zebraPrinterService?.Disconnect();
                await DisplayAlert("Success", "Disconnected from printer.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
