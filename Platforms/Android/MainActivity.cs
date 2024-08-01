using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Activity;
using AndroidX.Core.App;
using AndroidX.Core.Content;

namespace ZebraPrinter
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        private const int RequestPermissionsId = 0;

        private readonly string[] _permissions = {
            Android.Manifest.Permission.Bluetooth,
            Android.Manifest.Permission.BluetoothAdmin,
            Android.Manifest.Permission.BluetoothConnect,
            Android.Manifest.Permission.BluetoothScan,
            Android.Manifest.Permission.AccessFineLocation,
            Android.Manifest.Permission.AccessCoarseLocation
        };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Check and request permissions
            if (ArePermissionsGranted())
                return;

            ActivityCompat.RequestPermissions(this, _permissions, RequestPermissionsId);
        }

        private bool ArePermissionsGranted()
        {
            foreach (var permission in _permissions)
            {
                if (ContextCompat.CheckSelfPermission(this, permission) != Permission.Granted)
                {
                    return false;
                }
            }
            return true;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == RequestPermissionsId)
            {
                // Handle permission result if necessary
            }
        }
    }
}
