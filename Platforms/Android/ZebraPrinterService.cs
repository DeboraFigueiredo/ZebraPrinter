using Android.Bluetooth;
using Java.Util;
using System;
using System.Text;
using System.Threading.Tasks;

namespace ZebraPrinter.Platforms.Android
{
    public class ZebraPrinterService
    {
        private BluetoothSocket _bluetoothSocket;
        private BluetoothDevice _bluetoothDevice;

        public ZebraPrinterService(BluetoothDevice device)
        {
            _bluetoothDevice = device;
        }

        public async Task ConnectAsync()
        {
            if (_bluetoothDevice == null)
                throw new Exception("No Bluetooth device selected.");

            try
            {
                var uuid = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");
                var bluetoothSocket = _bluetoothDevice.CreateRfcommSocketToServiceRecord(uuid);
                await bluetoothSocket.ConnectAsync();
                _bluetoothSocket = bluetoothSocket;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to connect to the printer: " + ex.Message);
            }
        }

        public void Print(string textToPrint)
        {
            if (_bluetoothSocket == null || !_bluetoothSocket.IsConnected)
            {
                throw new Exception("Printer not connected!");
            }

            try
            {
                var zpl = "^XA\n" +
                          "^FO50,50\n" +
                          "^A0N,50,50\n" +
                          "^FD" + textToPrint + "\n" +
                          "^XZ";

                var bytes = Encoding.ASCII.GetBytes(zpl);
                var outputStream = _bluetoothSocket.OutputStream;
                outputStream.Write(bytes, 0, bytes.Length);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to print text: " + ex.Message);
            }
        }

        public void Disconnect()
        {
            try
            {
                if (_bluetoothSocket != null && _bluetoothSocket.IsConnected)
                {
                    _bluetoothSocket.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to disconnect from the printer: " + ex.Message);
            }
        }
    }
}
