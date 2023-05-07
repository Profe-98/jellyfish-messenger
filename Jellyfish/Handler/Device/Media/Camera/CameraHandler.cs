using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JellyFish.Handler.Abstraction;
using JellyFish.Handler.Device.Extension;
using AppModel = Microsoft.Maui.ApplicationModel;

namespace JellyFish.Handler.Device.Media.Camera
{
    public class CameraHandler : AbstractDeviceActionHandler<Permissions.Camera, Permissions.StorageRead, Permissions.StorageWrite>
    {
        public virtual async Task<byte[]> CapturePhoto()
        {
            byte[] data = null;
            FileResult media = await MediaPicker.Default.CapturePhotoAsync();
            data = await media.ReadAllBytes();
            return data;
        }
        public virtual async Task<byte[]> CaptureVideo()
        {
            byte[] data = null;
            FileResult media = await MediaPicker.Default.CaptureVideoAsync();
            data = await media.ReadAllBytes();
            return data;
        }
        
        public bool HasCamera
        {
            get
            {
                return MediaPicker.Default.IsCaptureSupported;
            }
        }
    }
}
