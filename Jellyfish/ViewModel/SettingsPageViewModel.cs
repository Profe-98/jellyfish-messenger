using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
        public ObservableCollection<User> UserFriendInvitesList { get; private set; }

        private User _selectedUserInvite;
        /// <summary>
        /// Currents user friend invite
        /// </summary>
        public User SelectedUserInvite
        {
            get
            {
                return _selectedUserInvite;
            }
            set
            {
                _selectedUserInvite = value;
                OnPropertyChanged(nameof(SelectedUserInvite));
            }
        }
        public bool HasUserFriendInvites { get => UserFriendInvitesList != null && UserFriendInvitesList.Count != 0; }

        private readonly NavigationService _navigationService;
        public ICommand OpenAccountPage { get; private set; }
        public ICommand OpenChatsPage { get; private set; }
        public ICommand OpenNotificationsPage { get; private set; }
        public ICommand OpenNetworkConfigPage { get; private set; }
        public ICommand AcceptFriendshipInviteCommand { get; private set; }
        private Dictionary<ICommand, AbstractSettingsPageGenericViewModel> _commandsToViewsRelation = new Dictionary<ICommand, AbstractSettingsPageGenericViewModel>();
        public ObservableCollection<SettingsPageSettingItem> SettingsPageSettingItems { get; set; }
        public SettingsPageViewModel(NavigationService navigationService,ApplicationConfigHandler applicationConfigHandler)
        {
            _navigationService = navigationService;
            OpenAccountPage = new RelayCommand<ICommand>(OpenSubSettingPage);
            OpenChatsPage = new RelayCommand<ICommand>(OpenSubSettingPage);
            OpenNotificationsPage = new RelayCommand<ICommand>(OpenSubSettingPage);
            OpenNetworkConfigPage = new RelayCommand<ICommand>(OpenSubSettingPage);
            AcceptFriendshipInviteCommand = new RelayCommand<User>(AcceptFriendshipInviteAction);
            _commandsToViewsRelation.Add(OpenAccountPage, new SettingsPageGenericViewModel<AccountConfig,AccountConfigViewModel>("Account Settings",navigationService, applicationConfigHandler, new AccountConfigViewModel(applicationConfigHandler.ApplicationConfig.AccountConfig)));
            _commandsToViewsRelation.Add(OpenChatsPage, new SettingsPageGenericViewModel<ChatConfig,ChatConfigViewModel>("Chat Settings", navigationService, applicationConfigHandler, new ChatConfigViewModel(applicationConfigHandler.ApplicationConfig.ChatConfig)));
            _commandsToViewsRelation.Add(OpenNotificationsPage, new SettingsPageGenericViewModel<NotificationConfig,NotificationConfigViewModel>("Notification Settings", navigationService, applicationConfigHandler, new NotificationConfigViewModel(applicationConfigHandler.ApplicationConfig.NotificationConfig)));
            _commandsToViewsRelation.Add(OpenNetworkConfigPage, new SettingsPageGenericViewModel<NetworkConfig,NetworkConfigViewModel>("Network Settings", navigationService, applicationConfigHandler, new NetworkConfigViewModel(applicationConfigHandler.ApplicationConfig.NetworkConfig)));

            PathGeometry failoverValue = (PathGeometry)App.ResourceDictionary["Svg"]["icons8picturesvg"];
            SettingsPageSettingItems = new ObservableCollection<SettingsPageSettingItem>()
            {
                new SettingsPageSettingItem{ Title = "Konto", SubTitle = "Sicherheitsbenachrichtigung, Nummer ändern", ExecCommand = OpenAccountPage, SvgPath = failoverValue},
                new SettingsPageSettingItem{ Title = "Chats", SubTitle = "Design, Hintergrund, Chatverlauf", ExecCommand = OpenChatsPage, SvgPath = failoverValue },
                new SettingsPageSettingItem{ Title = "Benachrichtigungen", SubTitle = "Nachrichten-, Gruppen- und Anruftöne", ExecCommand = OpenNotificationsPage, SvgPath = failoverValue },
                new SettingsPageSettingItem{ Title = "Network", SubTitle = "Network Settings", ExecCommand = OpenNetworkConfigPage, SvgPath = failoverValue },
            };
            LoadSampleData();
        }
        private void AcceptFriendshipInviteAction(User user)
        {
            UserFriendInvitesList.Remove(user);
            OnPropertyChanged(nameof(UserFriendInvitesList));
            OnPropertyChanged(nameof(HasUserFriendInvites));
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
        private void LoadSampleData()
        {

            UserFriendInvitesList = new ObservableCollection<User>();
            Random random = new Random();
            for (int i = 0; i < 3; i++)
            {
                User user = new User();
                user.NickName = "Userwanttobeyourfriend+" + random.Next(0, 50);
                UserFriendInvitesList.Add(user);
            }
        }
    }
}
