using Android.Bluetooth;
using Android.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZebraPrinter.Platforms.Android
{
    public class BluetoothDiscovery
    {
        private readonly BluetoothAdapter _bluetoothAdapter;
        private readonly Context _context;

        // Construtor modificado para aceitar o contexto como parâmetro
        public BluetoothDiscovery(Context context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
        }

        public async Task<List<BluetoothDevice>> DiscoverDevicesAsync()
        {
            var tcs = new TaskCompletionSource<List<BluetoothDevice>>();
            var devices = new List<BluetoothDevice>();

            if (_bluetoothAdapter == null)
            {
                tcs.SetException(new Exception("Bluetooth not supported on this device."));
                return await tcs.Task;
            }

            if (!_bluetoothAdapter.IsEnabled)
            {
                tcs.SetException(new Exception("Bluetooth is not enabled."));
                return await tcs.Task;
            }

            var receiver = new BluetoothReceiver();
            receiver.DeviceFound += (sender, args) => devices.Add(args.Device);
            receiver.DiscoveryFinished += (sender, args) => tcs.SetResult(devices);

            var context = _context; // Contexto correto
            var filter = new IntentFilter(BluetoothDevice.ActionFound);
            context.RegisterReceiver(receiver, filter);

            filter = new IntentFilter(BluetoothAdapter.ActionDiscoveryFinished);
            context.RegisterReceiver(receiver, filter);

            _bluetoothAdapter.StartDiscovery();

            // Timeout for discovery
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(10));
            var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

            if (completedTask == timeoutTask)
            {
                _bluetoothAdapter.CancelDiscovery();
                tcs.SetException(new Exception("Discovery timed out."));
            }

            return await tcs.Task;
        }

        private class BluetoothReceiver : BroadcastReceiver
        {
            public event EventHandler<BluetoothDeviceEventArgs> DeviceFound;
            public event EventHandler DiscoveryFinished;

            public override void OnReceive(Context context, Intent intent)
            {
                var action = intent.Action;
                Console.WriteLine($"Action Received: {action}"); // Log for debugging

                if (BluetoothDevice.ActionFound.Equals(action))
                {
                    var device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
                    Console.WriteLine($"Device Found: {device.Name}"); // Log for debugging
                    DeviceFound?.Invoke(this, new BluetoothDeviceEventArgs(device));
                }
                else if (BluetoothAdapter.ActionDiscoveryFinished.Equals(action))
                {
                    Console.WriteLine("Discovery Finished"); // Log for debugging
                    DiscoveryFinished?.Invoke(this, EventArgs.Empty);
                }
            }

        }

        public class BluetoothDeviceEventArgs : EventArgs
        {
            public BluetoothDevice Device { get; }

            public BluetoothDeviceEventArgs(BluetoothDevice device)
            {
                Device = device;
            }
        }
    }
}
