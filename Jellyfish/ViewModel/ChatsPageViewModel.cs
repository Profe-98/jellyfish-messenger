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

namespace JellyFish.ViewModel
{

    public class ChatsPageViewModel : BaseViewModel
    {
        private readonly NavigationService _navigationService;

        private readonly IServiceProvider _serviceProvider;
        private ObservableCollection<Chat> _chats;
        public ObservableCollection<Chat> Chats
        { get { return _chats; } }
        public string Test { get; set; } = "Mein Test Str ChatsPageViewModel";
        private Chat _SelectedChat;
        public Chat SelectedChat
        {
            get
            {
                return _SelectedChat;
            }
            set
            {
                _SelectedChat = value;
            }
        }


        public bool AreChatsAvailable
        {
            get
            {
                return (Chats != null && Chats.Count != 0) ? true : false ;
            }
        }
        private bool _isChatsListRefreshing;
        public bool IsChatsListRefreshing
        {
            get
            {
                return _isChatsListRefreshing
                    ;
            }
            set
            {
                _isChatsListRefreshing = value;
                OnPropertyChanged();
            }
        }
        public ICommand SelectedChatChangedCommand { get; private set; }
        public ICommand TabChatCommand { get; private set; }
        public ICommand DeleteChatCommand { get; private set; }
        public ICommand RefreshChatsViewCommand { get; private set; }
        public ICommand SwipeLeftCommand { get; private set; }
        public ICommand SwipeRightCommand { get; private set; }

        public ChatsPageViewModel(IServiceProvider serviceProvider, NavigationService navigationService)
        {
            _serviceProvider = serviceProvider;
            _navigationService = navigationService;
            DeleteChatCommand = new RelayCommand<Chat>(DeleteChatAction);
            RefreshChatsViewCommand = new RelayCommand(RefreshChatsViewAction);
            SelectedChatChangedCommand = new RelayCommand(SelectedChatChangedAction);
            TabChatCommand = new RelayCommand<Chat>(TabChatAction);


            LoadChats();
        }

        public void SelectedChatChangedAction()
        {

        }
        public void TabChatAction(Chat chat)
        {
            OnChatSelected(chat);
        }
        public void DeleteChatAction(Chat chat)
        {

            _chats.Remove(chat);
            OnPropertyChanged(nameof(AreChatsAvailable));
            OnPropertyChanged(nameof(Chats));
        }

        public void RefreshChatsViewAction()
        {
            IsChatsListRefreshing = true;
            IsChatsListRefreshing = false;
        }

        private async void OnChatSelected(Chat chat)
        {
            var vmFromDi = _serviceProvider.GetService<ChatPageViewModel>();
            ChatPage chatPage = new ChatPage(vmFromDi);
            var vm = chatPage.BindingContext as ChatPageViewModel;
            vm.SetChat(chat);
            await _navigationService.PushAsync(chatPage);
            vm.FocusLastMessage = false;
            vm.FocusLastMessage = true;
            this._SelectedChat = null;//Reset that Chat is clickable/tapable after return from chat to chats view
            OnPropertyChanged(nameof(SelectedChat));

        }

        public async void LoadChats()
        {
            Random random = new Random();
            _chats = new ObservableCollection<Chat>();

            OnPropertyChanged("AreChatsAvailable");
            for (int i = 0; i < 20; i++)
            {
                int randomChatCount = random.Next(1, 44);
                var chat = new Chat() { Name = "User" + i + "", Messages = new ObservableCollection<MessageGroup>() };
                bool sendMessage = false;
                List<Message> messages = new List<Message>();
                for (int j = 0; j < randomChatCount; j++)
                {
                    sendMessage = !sendMessage;
                    var msg = new Message() { Text = "random+" + i + ",demo-msg-id:" + j, Received = sendMessage, MessageDateTime = DateTime.Now, SendToBackend = true };
                    messages.Add(msg);
                }
                var msg2 = new Message() { Text = "1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111", Received = sendMessage, MessageDateTime = DateTime.Now.AddDays(-random.Next(0, 50)), SendToBackend = true };
                var msg3 = new Message() { Text = "https://www.google.com", Received = sendMessage, MessageDateTime = DateTime.Now, SendToBackend = true };
                var msg4 = new Message() { Location = new Location(36.9628066, -122.0194722), Text = "komm hier hin", Received = true, MessageDateTime = DateTime.Now, SendToBackend = true };
                messages.Add(msg2);
                messages.Add(msg3);
                messages.Add(msg4);
                List<MessageGroup> messageGrp = new List<MessageGroup>();
                foreach (Message m in messages)
                {
                    DateOnly key = DateOnly.FromDateTime(m.MessageDateTime);
                    int index = messageGrp.IndexOf(x => x.Date == key);
                    if (index == -1)
                    {
                        messageGrp.Add(new MessageGroup(key));
                        index = messageGrp.Count - 1;
                    }
                    messageGrp[index].Add(m);

                }
                chat.Messages = messageGrp.OrderBy(x => x.SortKey).ToList().ToObservableCollection();
                _chats.Add(chat);

            }
            OnPropertyChanged(nameof(AreChatsAvailable));
            OnPropertyChanged(nameof(Chats));
        }

    }
}
