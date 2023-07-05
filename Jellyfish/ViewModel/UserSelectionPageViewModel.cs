//#define SAMPLE_DATA

using CommunityToolkit.Mvvm.Messaging;
using JellyFish.Model;
using JellyFish.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace JellyFish.ViewModel
{
    public class UserSelectionPageViewModel : BaseViewModel
    {
        public List<User> MultiSelectedUsers { get; set; }
        public ObservableCollection<User> UserFriendsSelectionCollection { get; private set; }
        public ObservableCollection<User> UserFriendsRemoveCollection { get; private set; } = new ObservableCollection<User>();
        public ObservableCollection<User> UserSearchHitsCollection { get; private set; }

        private User _selectedUserFriend;
        /// <summary>
        /// Currents user friends
        /// </summary>
        public User SelectedUserFriend
        {
            get
            {
                return _selectedUserFriend;
            }
            set
            {
                if(_selectedUserFriend!= null)
                {
                    _selectedUserFriend.IsSelected = false;
                }
                _selectedUserFriend = value;
                if(_selectedUserFriend !=null)
                {
                    _selectedUserFriend.IsSelected = true;
                }
                OnPropertyChanged(nameof(SelectedUserFriend));
                OnPropertyChanged(nameof(IsAnyUserSelected));
            }
        }
        private User _selectedUserSearchHit;
        /// <summary>
        /// Online user search to add friends, this collection represent the search results for potential new friends
        /// </summary>
        public User SelectedUserSearchHit
        {
            get
            {
                return _selectedUserSearchHit;
            }
            set
            {
                _selectedUserSearchHit = value;
                OnPropertyChanged(nameof(SelectedUserSearchHit));
                OnPropertyChanged(nameof(IsAnyUserSelected));
            }
        }
        private string _searchText;
        /// <summary>
        /// Searchtext binding
        /// </summary>
        public string SearchText
        {
            get
            {
                return _searchText;
            }
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                OnPropertyChanged(nameof(IsSearchTextEntered));
                OnPropertyChanged(nameof(IsSearchTextEnteredAndNoMultiSelect));
                SearchingAction();
            }
        }
        /// <summary>
        /// When a search text is entered
        /// </summary>
        public bool IsSearchTextEntered
        {
            get
            {
                var res = !String.IsNullOrWhiteSpace(_searchText);
                return res;
            }
        }
        public bool IsSearchTextEnteredAndNoMultiSelect
        {
            get
            {
                return IsSearchTextEntered && !IsFriendMultiSelectionEnabled;
            }
        }
        private bool _isSearchingLocal;
        /// <summary>
        /// State that indicates the search state local search
        /// </summary>
        public bool IsSearchingLocal
        {
            get
            {
                return _isSearchingLocal;
            }
            set
            {
                _isSearchingLocal = value;
                OnPropertyChanged(nameof(IsSearchingLocal));
            }
        }
        private bool _isSearchingOnline;
        /// <summary>
        /// State that indicates the search state for current search (true when data is transmitted to backend and false when the result from backend is there)
        /// </summary>
        public bool IsSearchingOnline
        {
            get
            {
                return _isSearchingOnline;
            }
            set
            {
                _isSearchingOnline = value;
                OnPropertyChanged(nameof(IsSearchingOnline));
            }
        }
        private bool _isFriendMultiSelectionEnabled = false;
        /// <summary>
        /// Multi selection for friend to create groups
        /// </summary>
        public bool IsFriendMultiSelectionEnabled
        {
            get
            {
                return _isFriendMultiSelectionEnabled;
            }
            set
            {
                _isFriendMultiSelectionEnabled = value;
                if (value)
                {
                    if (MultiSelectedUsers == null)
                        MultiSelectedUsers = new List<User>();

                }
                OnPropertyChanged(nameof(IsFriendMultiSelectionEnabled));
            }
        }
        public bool HasUserFriendsSelectionCollectionItems { get => UserFriendsSelectionCollection != null && UserFriendsSelectionCollection.Count != 0; }
        public bool HasUserSearchHitsCollectionItems { get=> UserSearchHitsCollection!=null&& UserSearchHitsCollection.Count!=0; }
        public bool IsAnyUserSelected
        {
            get
            {
                return this.SelectedUserFriend != null || this.SelectedUserSearchHit != null|| (this.MultiSelectedUsers!=null&&this.MultiSelectedUsers.Count!=0);
            }
        }
        public string SearchOnlineResultText { get; private set; }
        private readonly NavigationService _navigationService;
        public ICommand SearchOnlineCommand { get; private set; }
        public ICommand AddOnlineUserCommand { get; private set; }
        public ICommand RemoveFriendCommand { get; private set; }
        public ICommand SelectCommand { get; private set; }
        public ICommand ResetSearchCommand { get; private set; }
        public ICommand ContinueCommand { get; private set; }
        public string MessageBusQueue { get;private set; }
        private bool _isRemoveFriendButtonEnabled = false;
        public bool IsRemoveFriendButtonEnabled
        {
            get
            {
                return _isRemoveFriendButtonEnabled;
            }
            private set
            {
                _isRemoveFriendButtonEnabled = value;   
            }
        }

        public UserSelectionPageViewModel(NavigationService navigationService,string messageBusQueue)
        {
            MessageBusQueue = messageBusQueue;    
            _navigationService = navigationService;
            SearchOnlineCommand = new RelayCommand(SearchingAction);
            AddOnlineUserCommand = new RelayCommand<User>(AddOnlineUserAction);
            RemoveFriendCommand = new RelayCommand<User>(RemoveFriendAction);
            ResetSearchCommand = new RelayCommand(ResetSearchAction);
            SelectCommand = new RelayCommand<User>(SelectAction);
            ContinueCommand = new RelayCommand(ContinueAction);
#if SAMPLE_DATA
            LoadSampleData();
#endif
        }
        public void ContinueAction()
        {
            string queueName = MessageBusQueue;
            WeakReferenceMessenger.Default.Send(new MessageBus.MessageModel(queueName, new object[] { this }));

            _navigationService.CloseCurrentPage();
        }
        public void SelectAction(User user)
        {
            if(IsFriendMultiSelectionEnabled)
            {
                user.IsSelected = !user.IsSelected;

            }
            else
            {
                this.SelectedUserFriend = user;
            }
        }
        /// <summary>
        /// Reset the search entry control binding 
        /// </summary>
        public void ResetSearchAction()
        {
            if (UserSearchHitsCollection != null)
                UserSearchHitsCollection.Clear();

            SearchText = null;

        }
        private void FilterUserItemTypeCollections(ObservableCollection<User> collection, ObservableCollection<User> removeCollection)
        {
            if (SearchText == null)
            {
                if (collection != null)
                {
                    foreach (var item in removeCollection)
                    {
                        collection.Add(item);   
                    }

                }
            }
            if (SearchText != null)
            {
                if (collection != null)
                {
                    foreach (var item in collection)
                    {
                        var searchResult = item.NickName.ToLower().Contains(SearchText.ToLower());
                        if (!searchResult)
                        {
                            removeCollection.Add(item);
                        }
                    }
                    removeCollection.ToList().ForEach(x=>collection.Remove(x));
                    List<User> usersAdd = new List<User>();    
                    foreach (var item in removeCollection)
                    {
                        var searchResult = item.NickName.ToLower().Contains(SearchText.ToLower());
                        if (searchResult)
                        {
                            usersAdd.Add(item);
                            collection.Add(item);
                        }
                    }
                    usersAdd.ForEach(x => removeCollection.Remove(x));

                }
            }
        }

        /// <summary>
        /// Action that perfom the search communication to backend and the result processing for ui or search local in friend list
        /// </summary>
        public async void SearchingAction()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            IsSearchingLocal = true;
            //async local search code
            IsSearchingLocal = false;
            FilterUserItemTypeCollections(UserFriendsSelectionCollection,UserFriendsRemoveCollection);
            OnPropertyChanged(nameof(HasUserFriendsSelectionCollectionItems));
            IsSearchingOnline = true;
            //async code to backend with searchtext as filter to get the best result

            IsSearchingOnline = false;
            stopwatch.Stop();

            OnPropertyChanged(nameof(HasUserSearchHitsCollectionItems));
            int c1 = UserSearchHitsCollection.Count;
            int c2 = UserFriendsSelectionCollection.Count;
            SearchOnlineResultText = string.Format("{0} results in {1}ms", c1 + c2, stopwatch.ElapsedMilliseconds);
            OnPropertyChanged(nameof(SearchOnlineResultText));
        }
        /// <summary>
        /// Send an friend invite to target user
        /// </summary>
        public async void AddOnlineUserAction(User user)
        {

        }
        /// <summary>
        /// Action that remove a current friend 
        /// </summary>
        public async void RemoveFriendAction(User user)
        {

        }
#if SAMPLE_DATA
        /// <summary>
        /// Test function for sample data to test the ui rendering and so on
        /// </summary>
        public void LoadSampleData()
        {
            UserFriendsSelectionCollection = new ObservableCollection<User>();
            UserSearchHitsCollection = new ObservableCollection<User>();
            Random random = new Random();
            for (int i = 0; i < 50; i++)
            {
                User user = new User();
                user.NickName = "User+"+random.Next(0,50);
                UserFriendsSelectionCollection.Add(user);
            }
            for (int i = 0; i < 30; i++)
            {
                User user = new User();
                user.NickName = "SearchedUser+" + random.Next(0, 50);
                UserSearchHitsCollection.Add(user);
            }
        }
#endif

    }
}
