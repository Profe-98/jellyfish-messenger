using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mobile.Jellyfish.Handler.Device.Media.Audio
{
    public interface IAudioRecorder
    {
        void StartRecord();
        string StopRecord();
        void PauseRecord();
        void ResetRecord();
    }
}
