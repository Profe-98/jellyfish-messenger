using JellyFish.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace JellyFish.ViewModel
{
    public class VideoPlayerPageViewModel : BaseViewModel
    {
        public CameraMediaModel VideoModel { get; private set; }
        public ICommand BackButtonCommand { get; private set; }
        public VideoPlayerPageViewModel()
        {

            BackButtonCommand = new RelayCommand(BackButtonAction);
        }

        public void SetVideoModel(CameraMediaModel videoModel)
        {
            VideoModel = videoModel;
            OnPropertyChanged(nameof(VideoModel));    
        }
        public void BackButtonAction()
        {

        }
    }
}
