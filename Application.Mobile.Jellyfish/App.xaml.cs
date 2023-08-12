using Application.Mobile.Jellyfish.Handler.AppConfig;
using Application.Mobile.Jellyfish.Handler.Backend.Communication.SignalR;
using Application.Mobile.Jellyfish.Handler.Backend.Communication.WebApi;
using Application.Mobile.Jellyfish.Handler.Data;
using Application.Mobile.Jellyfish.Handler.Device.Media;
using Application.Mobile.Jellyfish.Service;
using Application.Mobile.Jellyfish.View;
using Application.Mobile.Jellyfish.ViewModel;

namespace Application.Mobile.Jellyfish
{
    public partial class App : Microsoft.Maui.Controls.Application
    {
        private CancellationTokenSource _webApiActionCancelationToken = new CancellationTokenSource();
        public App(
            SignalRClient signalRClient,
            JellyfishWebApiRestClient jellyfishWebApiRestClient,
            ApplicationConfigHandler applicationConfigHandler,
            LoginPageViewModel loginPageViewModel,
            MainPageViewModel mainPageViewModel,
            NavigationService navigationService,
            JellyfishSqlliteDatabaseHandler jellyfishSqlliteDatabaseHandler,
            ApplicationResourcesHandler applicationResourcesHandler)
        {
            InitializeComponent();
            Page viewPage = new NavigationPage(new LoginPage(loginPageViewModel));
            MainPage = viewPage;
            Load(signalRClient,jellyfishWebApiRestClient, applicationConfigHandler, loginPageViewModel, mainPageViewModel, navigationService, jellyfishSqlliteDatabaseHandler, applicationResourcesHandler);

        }
        public static Dictionary<string, ResourceDictionary> ResourceDictionary;
        public async Task Load(
            SignalRClient signalRClient,
            JellyfishWebApiRestClient jellyfishWebApiRestClient,
            ApplicationConfigHandler applicationConfigHandler,
            LoginPageViewModel loginPageViewModel,
            MainPageViewModel mainPageViewModel,
            NavigationService navigationService,
            JellyfishSqlliteDatabaseHandler jellyfishSqlliteDatabaseHandler,
            ApplicationResourcesHandler applicationResourcesHandler)
        {
            bool loggedin = applicationConfigHandler.ApplicationConfig.AccountConfig.UserSession?.TokenExpires > DateTime.Now;





            if (loggedin)
            {
                await navigationService.PushAsync(new MainPage(mainPageViewModel));

            }

            ResourceDictionary = new Dictionary<string, ResourceDictionary>();
            foreach (var dictionary in Microsoft.Maui.Controls.Application.Current.Resources.MergedDictionaries)
            {
                if (dictionary.GetType().FullName.ToLower().Contains("skia"))
                    continue;
                string key = dictionary.Source.OriginalString.Split(';').First().Split('/').Last().Split('.').First();
                ResourceDictionary.Add(key, dictionary);
            }
            applicationResourcesHandler.ResourceDictionary = ResourceDictionary;
        }
    }
}