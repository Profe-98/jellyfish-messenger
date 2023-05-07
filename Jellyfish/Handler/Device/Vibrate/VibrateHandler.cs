using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JellyFish.Handler.Abstraction;
using Vibrate = Microsoft.Maui.ApplicationModel;

namespace JellyFish.Handler.Device.Vibrate
{
    public class VibrateHandler : AbstractDeviceActionHandler<Permissions.Vibrate>
    {
        public const int DefaultVibrationTimeInMilliseconds = 500;
        public void VibrateDevice(int ms = DefaultVibrationTimeInMilliseconds)
        {
            Vibration.Default.Vibrate(ms);
        }
        public void VibrateDevice(TimeSpan timeSpan)
        {
            VibrateDevice(int.TryParse(timeSpan.TotalMilliseconds.ToString(), out int ms) ? ms : DefaultVibrationTimeInMilliseconds);
        }
    }
}
