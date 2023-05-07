using JellyFish.Service;
using JellyFish.View;
using JellyFish.ViewModel;

namespace JellyFish
{
    public partial class App : Application
    {
        public App(LoginPageViewModel loginPageViewModel,MainPageViewModel mainPageViewModel,NavigationService navigationService)
        {
            InitializeComponent();
            bool loggedin = false;
            Page viewPage= null;
            if (loggedin)
            {
                viewPage = new NavigationPage(new MainPage(mainPageViewModel));
            }
            else
            {
                viewPage = new NavigationPage(new LoginPage(loginPageViewModel));
            }
            
            MainPage = viewPage; 
            ResourceDictionary = new Dictionary<string, ResourceDictionary>();
            foreach (var dictionary in Application.Current.Resources.MergedDictionaries)
            {
                if (dictionary.GetType().FullName.ToLower().Contains("skia"))
                    continue;
                string key = dictionary.Source.OriginalString.Split(';').First().Split('/').Last().Split('.').First();
                ResourceDictionary.Add(key, dictionary);
            }

        }
        public static Dictionary<string, ResourceDictionary> ResourceDictionary;

    }
}