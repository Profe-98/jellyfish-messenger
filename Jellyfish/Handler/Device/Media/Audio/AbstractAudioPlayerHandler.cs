using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JellyFish.Handler.Abstraction;
using AppModel = Microsoft.Maui.ApplicationModel;

namespace JellyFish.Handler.Device.Media.Audio
{

    public abstract class AbstractAudioPlayerHandler : AbstractDeviceActionHandler<Permissions.Microphone, Permissions.StorageRead, Permissions.StorageWrite>, IAudioPlayer
    {
        public virtual bool CheckFinishedPlayingAudio()
        {
            throw new NotImplementedException();
        }

        public virtual string GetCurrentPlayTime()
        {
            throw new NotImplementedException();
        }

        public virtual void Pause()
        {
            throw new NotImplementedException();
        }

        public virtual void PlayAudio(string filePath)
        {
            throw new NotImplementedException();
        }

        public virtual void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
