using JellyFish.Handler.AppConfig;
using JellyFish.Handler.Backend.Communication.SignalR;
using JellyFish.Handler.Backend.Communication.WebApi;
using JellyFish.Handler.Data;
using JellyFish.Handler.Device.Media;
using JellyFish.Service;
using JellyFish.View;
using JellyFish.ViewModel;

namespace JellyFish
{
    public partial class App : Application
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
            foreach (var dictionary in Application.Current.Resources.MergedDictionaries)
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