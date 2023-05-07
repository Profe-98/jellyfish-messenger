using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JellyFish.Handler.Abstraction;
using AppModel = Microsoft.Maui.ApplicationModel;
#if IOS
using AVFoundation;
using JellyFish;
using JellyFish.Handler;
using JellyFish.Handler.Device.Media.Audio;
#endif
#if ANDROID


#endif
//using Plugin.AudioRecorder;//für anroid, in XAMARIN war es MediaRecorder native aus der Android SDK

namespace JellyFish.Handler.Device.Media.Audio
{
    //AVFoundation: https://learn.microsoft.com/de-de/dotnet/api/avfoundation.avaudiorecorder?view=xamarin-ios-sdk-12
    //MediaRecord: https://learn.microsoft.com/en-us/dotnet/api/android.media.mediarecorder?view=xamarin-android-sdk-13
    public abstract class AbstractAudioRecorderHandler : AbstractDeviceActionHandler<Permissions.Microphone, Permissions.StorageRead, Permissions.StorageWrite>, IAudioRecorder
    {
        public virtual void PauseRecord()
        {
            throw new NotImplementedException();
        }

        public virtual void ResetRecord()
        {
            throw new NotImplementedException();
        }

        public virtual void StartRecord()
        {
            throw new NotImplementedException();
        }

        public virtual string StopRecord()
        {
            throw new NotImplementedException();
        }
    }
}
