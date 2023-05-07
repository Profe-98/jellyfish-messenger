using JellyFish.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JellyFish.View;
using JellyFish.ViewModel;

namespace JellyFish.Handler.Device.Media.Camera
{
    public class CustomCameraHandler : CameraHandler
    {
        public CustomCameraHandler() 
        {
        
        }

    }
    public static class CustomCameraHandlerExtension
    {

        public static async Task OpenCustomCameraHandler(this NavigationService navigationService,ChatPageViewModel chatPageViewModel)
        {
            var vm = new CameraHandlerPageViewModel(navigationService, chatPageViewModel);
            try
            {
                var page = new CameraHandlerPage();
                page.BindingContext = vm;
                await navigationService.PushAsync(page);
            }
            catch( Exception ex)
            {

            }


        }
    }
}
