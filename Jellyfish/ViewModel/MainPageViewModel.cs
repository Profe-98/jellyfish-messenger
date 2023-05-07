using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using JellyFish.ControlExtension;
using JellyFish.Model;
using JellyFish.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using JellyFish.View;
using JellyFish.Handler.Data;
using JellyFish.Data.SqlLite.Schema;
using JellyFish.ApplicationSpecific;
using JellyFish.Handler.Device.Vibrate;
using JellyFish.Handler.Device.Permission;
using JellyFish.Handler.AppConfig;

namespace JellyFish.ViewModel
{

    public class MainPageViewModel : BaseViewModel
    {
        private readonly NavigationService _navigationService;
        private readonly PermissionHandler _permissionHandler;

        private readonly IServiceProvider _serviceProvider;
        public ChatsPageViewModel ChatsPageViewModel
        {
            get
            {
                var data = (_serviceProvider != null ? _serviceProvider.GetService<ChatsPageViewModel>() : null);
                return data;
            }
        }
        public StatusPageViewModel StatusPageViewModel
        {
            get
            {
                var data = (_serviceProvider != null ? _serviceProvider.GetService<StatusPageViewModel>() : null);
                return data;
            }
        }
        public CallsPageViewModel CallsPageViewModel
        {
            get
            {
                var data = (_serviceProvider != null ? _serviceProvider.GetService<CallsPageViewModel>() : null);
                return data;
            }
        }

        public ICommand BindingContextChangedCommand { get; private set; }
        public ICommand SwipeLeftCommand { get; private set; }
        public ICommand SwipeRightCommand { get; private set; }
        public ICommand BackButtonCommand { get; private set; }
        public ICommand ExpandSettingsMenuIsExpandedCommand { get; private set; }
        public ICommand ExpandSearchIsExpandedCommand { get; private set; }
        public ICommand CreateNewGroupCommand { get; private set; }
        public ICommand CreateNewChatCommand { get; private set; }
        public ICommand CreateNewBroadcastCommand { get; private set; }
        public ICommand OpenSettingsPageCommand { get; private set; }

        public int CalcHeigtByItems { get; private set; }
        private bool _expandSettingsMenuIsExpanded = false;
        public bool ExpandSettingsMenuIsExpanded
        {
            get { return _expandSettingsMenuIsExpanded; }
            set
            {
                _expandSettingsMenuIsExpanded = value;
                OnPropertyChanged();
            }
        }
        private bool _isSearchExpanded = false;
        public bool IsSearchExpanded
        {
            get { return _isSearchExpanded; }
            set
            {
                _isSearchExpanded = value;
                OnPropertyChanged();
            }
        }
        private bool _blockBackSwitch;
        public bool BlockBackSwitch
        {
            get { return _blockBackSwitch; }
            set
            {
                _blockBackSwitch = value;
                OnPropertyChanged();
            }
        }

        private ViewTemplateModel _selectedViewTemplate;
        public ViewTemplateModel SelectedViewTemplate
        {
            get
            {

                return _selectedViewTemplate;
            }
            set
            {
                if (_selectedViewTemplate != null)
                    _selectedViewTemplate.IsSelected = false;
                value.IsSelected = true;    
                _selectedViewTemplate = value;

                OnPropertyChanged();
            }
        }
        public ViewTemplateModel[] ViewTemplates
        {
            get;
            private set;
        }
        public ObservableCollection<MenuItemModel> MenuItems { get;private set; }

        public MainPageViewModel(IServiceProvider serviceProvider,PermissionHandler permissionHandler,VibrateHandler vibrateHandler,SqlLiteDatabaseHandler<AbstractEntity> sqlLiteDatabaseHandler,NavigationService navigationService,ChatsPageViewModel chatsPageViewModel)
        {
            _serviceProvider = serviceProvider;
            _navigationService = navigationService;
            _permissionHandler = permissionHandler; 
            BindingContextChangedCommand = new RelayCommand<object>(BindingContextChangedAction);
            SwipeLeftCommand = new RelayCommand<object>(SwipeLeftAction);
            SwipeRightCommand = new RelayCommand<object>(SwipeRightAction);

            ExpandSettingsMenuIsExpandedCommand = new RelayCommand(ExpandSettingsMenuIsExpandedCommandAction);
            ExpandSearchIsExpandedCommand = new RelayCommand(ExpandSearchIsExpandedCommandAction);

            BackButtonCommand = new RelayCommand(BackButtonAction);
            CreateNewGroupCommand = new RelayCommand(CreateNewGroupAction);
            CreateNewBroadcastCommand = new RelayCommand(CreateNewBroadcastAction);
            OpenSettingsPageCommand = new RelayCommand(OpenSettingsPageAction);
            CreateNewChatCommand = new RelayCommand(CreateNewChatAction);

            MenuItems = new ObservableCollection<MenuItemModel>()
            {
                new MenuItemModel { Title = "New Chat", ExecCommand = CreateNewChatCommand },
                new MenuItemModel { Title = "New Group", ExecCommand = CreateNewGroupCommand },
                new MenuItemModel { Title = "New Broadcast", ExecCommand = CreateNewBroadcastCommand },
                new MenuItemModel { Title = "Settings",ExecCommand = OpenSettingsPageCommand } };
            CalcHeigtByItems = MenuItems.Count * 45;
        }
        private async void CreateNewChatAction()
        {
                
            var vmFromDi = _serviceProvider.GetService<ContactsPageViewModel>();    
            await _navigationService.PushAsync(new ContactsPage(vmFromDi));
        }
        private void CreateNewGroupAction()
        {

            ExpandSettingsMenuIsExpandedCommand.Execute(null);
        }
        private void CreateNewBroadcastAction()
        {

            ExpandSettingsMenuIsExpandedCommand.Execute(null);
        }
        private async void OpenSettingsPageAction()
        {
            var appConfigService = _serviceProvider.GetService<ApplicationConfigHandler>(); 
            var settingsPageVmFromDi = new SettingsPageViewModel(_navigationService, appConfigService);
            SettingsPage settingsPage = new SettingsPage(settingsPageVmFromDi);
            await _navigationService.PushAsync(settingsPage);
            ExpandSettingsMenuIsExpandedCommand.Execute(null);
        }
        private void ExpandSearchIsExpandedCommandAction()
        {

            IsSearchExpanded = !IsSearchExpanded;
            BlockBackSwitch = IsSearchExpanded;
            ExpandSettingsMenuIsExpanded = false;
        }
        private void ExpandSettingsMenuIsExpandedCommandAction()
        {

            ExpandSettingsMenuIsExpanded = !ExpandSettingsMenuIsExpanded;
            BlockBackSwitch = ExpandSettingsMenuIsExpanded;
            IsSearchExpanded = false;
        }
        public void BackButtonAction()
        {
            if (BlockBackSwitch && !(ExpandSettingsMenuIsExpanded || IsSearchExpanded))
            {
                BlockBackSwitch = false;
            }
            else if (BlockBackSwitch && (ExpandSettingsMenuIsExpanded || IsSearchExpanded))
            {
                ExpandSettingsMenuIsExpanded = false;
                IsSearchExpanded = false;
            }
        }
        private int CalculateIndex(ViewTemplateModel selectedObjectOfList, ViewTemplateModel[] items)
        {
            int index = 0;

            foreach (var item in items)
            {
                if(selectedObjectOfList != null && item.Title == selectedObjectOfList.Title) 
                {
                    return index;
                }
                index++;
            }
            return 0;
        }
        public void BindingContextChangedAction(object collection)
        {
            if (collection == null)
                return;
            ViewTemplateModel[] items = (ViewTemplateModel[])collection;
            SelectedViewTemplate = items.FirstOrDefault();  
            if(ViewTemplates == null)
            {
                ViewTemplates = items;
            }
        }
        public void SwipeLeftAction(object collection)
        {
            if (collection == null)
                return;
            ViewTemplateModel[] items = (ViewTemplateModel[])collection;
            int calcSelIndex = CalculateIndex(SelectedViewTemplate, items);
            if (calcSelIndex < items.Length - 1)
            {
                calcSelIndex++;
            }
            SelectedViewTemplate = items[calcSelIndex];
        }
        public void SwipeRightAction(object collection)
        {
            if (collection == null)
                return;
            ViewTemplateModel[] items = (ViewTemplateModel[])collection;
            int calcSelIndex = CalculateIndex(SelectedViewTemplate, items);
            if (calcSelIndex > 0)
            {
                calcSelIndex--;
            }
            SelectedViewTemplate = items[calcSelIndex];
        }

    }
}
