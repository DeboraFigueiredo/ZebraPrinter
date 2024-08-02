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

            var receiver = new BluetoothReceiver(devices, tcs);
            var filter = new IntentFilter(BluetoothDevice.ActionFound);
            _context.RegisterReceiver(receiver, filter);

            filter = new IntentFilter(BluetoothAdapter.ActionDiscoveryFinished);
            _context.RegisterReceiver(receiver, filter);

            _bluetoothAdapter.StartDiscovery();

            return await tcs.Task;
        }

        private class BluetoothReceiver : BroadcastReceiver
        {
            private readonly List<BluetoothDevice> _devices;
            private readonly TaskCompletionSource<List<BluetoothDevice>> _tcs;

            public BluetoothReceiver(List<BluetoothDevice> devices, TaskCompletionSource<List<BluetoothDevice>> tcs)
            {
                _devices = devices;
                _tcs = tcs;
            }

            public override void OnReceive(Context context, Intent intent)
            {
                var action = intent.Action;
                if (BluetoothDevice.ActionFound.Equals(action))
                {
                    var device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
                    _devices.Add(device);
                }
                else if (BluetoothAdapter.ActionDiscoveryFinished.Equals(action))
                {
                    context.UnregisterReceiver(this);
                    _tcs.TrySetResult(_devices);
                }
            }
        }
    }
}
