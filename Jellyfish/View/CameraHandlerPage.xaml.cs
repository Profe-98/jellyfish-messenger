using JellyFish.Controls;
using JellyFish.ViewModel;
using System.Windows.Input;
using Camera.MAUI;
using ImageFormat = Camera.MAUI.ImageFormat;
using CommunityToolkit.Maui.Views;
using Microsoft.Extensions.Logging;

namespace JellyFish.View;

public partial class CameraHandlerPage : CustomContentPage
{
    private double currentScale = 1;
    private double startScale = 1;

    public CameraHandlerPage()
	{
		InitializeComponent();

	}
    ~CameraHandlerPage() { 
    

    }
    private string _currentRecFile = null;
    private async void Button_Clicked(object sender, EventArgs e)
    {
        CameraHandlerPageViewModel viewModel = this.BindingContext as CameraHandlerPageViewModel;
        string localFilePath = System.IO.Path.Combine(FileSystem.CacheDirectory, DateTime.Now.Ticks + (viewModel.IsCameraModeVideoRecMode?(".mp4") :(".jpg")));
        if (viewModel.IsCameraModeVideoRecMode)
        {
            if(viewModel.ActiveVideoRecording)
            {
                _currentRecFile = localFilePath;
                var camRes = await Camera.StartRecordingAsync(_currentRecFile);
                if (camRes == CameraResult.Success)
                {

                }
            }
            else
            {
                var camRes = await Camera.StopRecordingAsync();
                
                if (camRes == CameraResult.Success)
                {
                    var model = new Model.CameraMediaModel { VideoSourcePath = _currentRecFile };
                    viewModel.AddCapturedMedia(model);
                }
            }
        }
        else
        {
            bool writeResponse = await Camera.SaveSnapShot(ImageFormat.JPEG, localFilePath);
            if(writeResponse) { 
                viewModel.AddCapturedMedia(new Model.CameraMediaModel { ImageSourceFilePath = localFilePath, ImageSource = Camera.SnapShot });
            }

        }
    }

    private void Camera_CamerasLoaded(object sender, EventArgs e)
    {
        Camera.Camera = Camera.Cameras.First();

        MainThread.BeginInvokeOnMainThread(async () => {
            System.Diagnostics.Debug.WriteLine("camera starting/switching");
            //await Camera.StopCameraAsync();
            await Camera.StartCameraAsync();
        });
    }
    private void PinchGestureRecognizer_PinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
    {
        currentScale += (e.Scale - 1) * startScale;
        currentScale = Math.Max(1, currentScale);
        if (e.Status == GestureStatus.Started)
        {
            startScale = Content.Scale;
            Content.AnchorX = 0;
            Content.AnchorY = 0;
        }
        if (e.Status == GestureStatus.Running)
        {
            currentScale += (e.Scale - 1) * startScale;
            currentScale = Math.Max(1, currentScale);

            Camera.ZoomFactor = (float)currentScale;
        }
    }

    private void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
    {

    }
}