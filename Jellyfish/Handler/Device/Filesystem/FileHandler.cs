using JellyFish.ApplicationSpecific;
using JellyFish.Handler.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JellyFish.Handler.Device.Filesystem
{
    public class FileHandler : AbstractDeviceActionHandler<Permissions.StorageRead, Permissions.StorageWrite>
    {
        public FileHandler() 
        {
            Init();
        }
        private void Init()
        {
            CreateDirIfNotExists(Global.CacheDirectory);
            CreateDirIfNotExists(Global.MediaDirectory);
            CreateDirIfNotExists(Global.MediaPhotosDirectory);
            CreateDirIfNotExists(Global.MediaVideosDirectory);
            CreateDirIfNotExists(Global.LoggingDirectory);
        }

        public void CreateDirIfNotExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public async Task<FileResult> SelectFile(PickOptions option)
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(option); 

                return result;
            }
            catch (Exception ex)
            {
            }
            return null;
        }
    }
}
