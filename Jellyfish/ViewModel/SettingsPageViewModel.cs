using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JellyFish.Data.AppConfig.Abstraction;
using JellyFish.Data.AppConfig.ConcreteImplements;
using JellyFish.Handler.AppConfig;
using JellyFish.Model;
using JellyFish.Service;
using JellyFish.View.SettingsSubPages;
using JellyFish.ViewModel.SettingsSubPage;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;

namespace JellyFish.ViewModel
{
    public class SettingsPageViewModel : BaseViewModel
    {
        private readonly NavigationService _navigationService;
        public ICommand OpenAccountPage { get; private set; }
        public ICommand OpenChatsPage { get; private set; }
        public ICommand OpenNotificationsPage { get; private set; }
        public ICommand OpenNetworkConfigPage { get; private set; }
        private Dictionary<ICommand, AbstractSettingsPageGenericViewModel> _commandsToViewsRelation = new Dictionary<ICommand, AbstractSettingsPageGenericViewModel>();
        public ObservableCollection<SettingsPageSettingItem> SettingsPageSettingItems { get; set; }
        public SettingsPageViewModel(NavigationService navigationService,ApplicationConfigHandler applicationConfigHandler)
        {
            _navigationService = navigationService;
            OpenAccountPage = new RelayCommand<ICommand>(OpenSubSettingPage);
            OpenChatsPage = new RelayCommand<ICommand>(OpenSubSettingPage);
            OpenNotificationsPage = new RelayCommand<ICommand>(OpenSubSettingPage);
            OpenNetworkConfigPage = new RelayCommand<ICommand>(OpenSubSettingPage);
            _commandsToViewsRelation.Add(OpenAccountPage, new SettingsPageGenericViewModel<AccountConfig>("Account Settings",applicationConfigHandler.ApplicationConfig.AccountConfig));
            _commandsToViewsRelation.Add(OpenChatsPage, new SettingsPageGenericViewModel<ChatConfig>("Chat Settings", applicationConfigHandler.ApplicationConfig.ChatConfig));
            _commandsToViewsRelation.Add(OpenNotificationsPage, new SettingsPageGenericViewModel<NotificationConfig>("Notification Settings", applicationConfigHandler.ApplicationConfig.NotificationConfig));
            _commandsToViewsRelation.Add(OpenNetworkConfigPage, new SettingsPageGenericViewModel<NetworkConfig>("Network Settings", applicationConfigHandler.ApplicationConfig.NetworkConfig));

            PathGeometry failoverValue = (PathGeometry)App.ResourceDictionary["Svg"]["icons8picturesvg"];
            SettingsPageSettingItems = new ObservableCollection<SettingsPageSettingItem>()
            {
                new SettingsPageSettingItem{ Title = "Konto", SubTitle = "Sicherheitsbenachrichtigung, Nummer ändern", ExecCommand = OpenAccountPage, SvgPath = failoverValue},
                new SettingsPageSettingItem{ Title = "Chats", SubTitle = "Design, Hintergrund, Chatverlauf", ExecCommand = OpenChatsPage, SvgPath = failoverValue },
                new SettingsPageSettingItem{ Title = "Benachrichtigungen", SubTitle = "Nachrichten-, Gruppen- und Anruftöne", ExecCommand = OpenNotificationsPage, SvgPath = failoverValue },
                new SettingsPageSettingItem{ Title = "Network", SubTitle = "Network Settings", ExecCommand = OpenNetworkConfigPage, SvgPath = failoverValue },
            };
        }
        
        private void OpenSubSettingPage(ICommand command)
        {
            if(_commandsToViewsRelation.ContainsKey(command))
            {
                var pageVm = (AbstractSettingsPageGenericViewModel)_commandsToViewsRelation[command];
                var page = new SettingsPageGeneric();
                page.BindingContext = pageVm;
                
                _navigationService.PushAsync(page);
                pageVm.RefreshUi(); 
            }
        }
    }
}
