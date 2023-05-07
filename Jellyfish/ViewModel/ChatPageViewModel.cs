using Microsoft.Maui.ApplicationModel;
//using Plugin.LocalNotification;
//using Plugin.LocalNotification.AndroidOption;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.Controls.Shapes;
using JellyFish.Model;
using JellyFish.ControlExtension;
using CommunityToolkit.Mvvm.Messaging;
using JellyFish.Service;
using JellyFish.Handler.Device.Media.Camera;
using JellyFish.View;
using JellyFish.Handler.Device.Extension;

namespace JellyFish.ViewModel
{
    public class ChatPageViewModel : BaseViewModel
    {
        //private readonly INotificationService _notificationService;
        private Chat _chat;
        public Chat Chat
        {
            get { return _chat; }
        }
        private ObservableCollection<MessageGroup> _messages;
        public ObservableCollection<MessageGroup> Messages
        {
            get
            {
                return _messages;
            }
            set
            {
                _messages = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<CameraMediaModel> TakenPhotos { get; set; }
        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }
        private bool _sendReceiveMessageState;
        public bool SendReceiveMessageState
        {
            get
            {
                return _sendReceiveMessageState
                    ;
            }
            set
            {
                _sendReceiveMessageState = value;
                OnPropertyChanged();
            }
        }
        private bool _isChatRefreshing;
        public bool IsChatRefreshing
        {
            get
            {
                return _isChatRefreshing
                    ;
            }
            set
            {
                _isChatRefreshing = value;
                OnPropertyChanged();
            }
        }
        private Message _selectedMessage;
        public Message SelectedMessage
        {
            get
            {
                return _selectedMessage;
            }
            set
            {
                _selectedMessage = value;
                OnMessageSelected(value);
            }
        }

        private bool _focusLastMessage;
        public bool FocusLastMessage
        {
            get { return _focusLastMessage; }
            set
            {
                _focusLastMessage = value;
                OnPropertyChanged();
            }
        }
        private bool _expandAttachmentsIsExpanded;
        public bool ExpandAttachmentsIsExpanded
        {
            get { return _expandAttachmentsIsExpanded; }
            set
            {
                _expandAttachmentsIsExpanded = value;
                OnPropertyChanged();
            }
        }
        private bool _expandChatMenusIsExpanded;
        public bool ExpandChatMenuIsExpanded
        {
            get { return _expandChatMenusIsExpanded; }
            set
            {
                _expandChatMenusIsExpanded = value;
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
        private JellyFish.Model.Contact _selectedContact;
        public JellyFish.Model.Contact SelectedContact
        {
            get
            {
                return _selectedContact;
            }
            set
            {
                _selectedContact = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsMessagePresetEntitySelected));
                OnPropertyChanged(nameof(IsContactSelected));
            }
        }
        private Location _selectedGpsLocation;
        public Location SelectedGpsLocation
        {
            get
            {
                return _selectedGpsLocation;
            }
            set
            {
                _selectedGpsLocation = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsMessagePresetEntitySelected));
                OnPropertyChanged(nameof(IsGpsLocationSelected));
                OnPropertyChanged(nameof(SelectedGpsLocationString));
            }
        }
        public string SelectedGpsLocationString
        {
            get
            {
                return _selectedGpsLocation.ToGpsString();
            }
        }

        public bool IsMessagePresetEntitySelected
        {
            get
            {
                return _selectedGpsLocation != null ||_selectedContact != null ;
            }
        }

        public bool IsContactSelected { get => _selectedContact != null; }

        public bool IsGpsLocationSelected { get => _selectedGpsLocation != null; }

        private readonly NavigationService _navigationService;
        public ICommand BackButtonCommand { get; private set; }
        public ICommand MessageSwipeLeftCommand { get; private set; }
        public ICommand MessageSwipeRightCommand { get; private set; }
        public ICommand SendMessageCommand { get; private set; }
        public ICommand RefreshChatViewCommand { get; private set; }
        public ICommand TapLinkCommand { get; private set; }
        public ICommand TapGpsLocationCommand { get; private set; }
        public ICommand ExpandAttachmentsCommand { get; private set; }
        public ICommand ExpandChatsMenuCommand { get; private set; }
        public ICommand TakePhotoCommand { get; private set; }
        public ICommand PlayVideoCommand { get; private set; }
        public ICommand VoiceRecordMessageCommand { get; private set; }
        public ICommand SendContactCommand { get; private set; }
        public ICommand SendMediaPhotoCommand { get; private set; }
        public ICommand SendGpsLocationCommand { get; private set; }
        public ChatPageViewModel(NavigationService navigationService)//INotificationService notificationService)
        {
            _navigationService = navigationService;
            //_notificationService = notificationService;
            MessageSwipeLeftCommand = new RelayCommand(MessageSwipeLeftAction);
            MessageSwipeRightCommand = new RelayCommand<Message>(MessageSwipeRightAction);
            SendMessageCommand = new RelayCommand(SendMessageAction);
            RefreshChatViewCommand = new RelayCommand(RefreshChatViewAction);
            TapLinkCommand = new RelayCommand<Message>(OpenLinkFromMessageAction);
            TapGpsLocationCommand = new RelayCommand<Message>(OpenGpsLocationFromMessageAction);
            ExpandAttachmentsCommand = new RelayCommand(ExpandAttachmentsAction);
            ExpandChatsMenuCommand = new RelayCommand(ExpandChatsMenuAction);
            TakePhotoCommand = new RelayCommand(TakePhoto);
            BackButtonCommand = new RelayCommand(BackButtonAction);
            PlayVideoCommand = new RelayCommand<CameraMediaModel>(PlayVideoAction);
            VoiceRecordMessageCommand = new RelayCommand(VoiceRecordMessageAction);
            SendContactCommand = new RelayCommand(SendContactAction); 
            SendMediaPhotoCommand = new RelayCommand(SendMediaPhotoAction); 
            SendGpsLocationCommand = new RelayCommand(SendGpsLocationAction);
            WeakReferenceMessenger.Default.Register<MessageBus.MessageModel>(this, (r, m) =>
            {
                if(m.Message!= null)
                {
                    if(m.Message == "OnBackButtonPressed")
                    {

                    }
                }
            });
        }
        private async void SendContactAction()
        {

            try
            {

                /*var contacts = await Microsoft.Maui.ApplicationModel.Communication.Contacts.GetAllAsync();
                var contactList = contacts.ToList();
                var contact = contactList.First();
                if (contact == null)
                    return;

                string id = contact.Id;
                string namePrefix = contact.NamePrefix;
                string givenName = contact.GivenName;
                string middleName = contact.MiddleName;
                string familyName = contact.FamilyName;
                string nameSuffix = contact.NameSuffix;
                string displayName = contact.DisplayName;
                List<ContactPhone> phones = contact.Phones; 
                List<ContactEmail> emails = contact.Emails; */
                var selectedContract = await Microsoft.Maui.ApplicationModel.Communication.Contacts.Default.PickContactAsync();
                if(selectedContract != null)
                {
                    var contactModel = JellyFish.Model.Contact.Create(selectedContract);
                    this.SelectedContact = contactModel;    
                    ExpandAttachmentsIsExpanded = false;
                }

            }
            catch (Exception ex)
            {

            }
        }
        private async void SendMediaPhotoAction()
        {

        }
        private async void SendGpsLocationAction()
        {
            ExpandAttachmentsIsExpanded = false;
            try
            {
                var location = await GetCurrentLocation();
                if(location != null)
                {
                    this.SelectedGpsLocation = location;
                }

            }
            catch (Exception ex)
            {

            }
        }
        private async void VoiceRecordMessageAction()
        {
            
        }
        private async void PlayVideoAction(CameraMediaModel data)
        {
            if (data == null)
                return;

            var page = new VideoPlayerPage();
            var vm = new VideoPlayerPageViewModel();
            page.BindingContext = vm;   
            vm.SetVideoModel(data); 
            await _navigationService.PushAsync(page);
        }
        public async void TakePhoto()
        {
            await _navigationService.OpenCustomCameraHandler(this);
            ExpandAttachmentsAction();
        }
        public void BackButtonAction()
        {
            if(BlockBackSwitch && !(ExpandChatMenuIsExpanded || ExpandAttachmentsIsExpanded))
            {
                BlockBackSwitch = false;
            }
            else if(BlockBackSwitch && (ExpandChatMenuIsExpanded || ExpandAttachmentsIsExpanded))
            {
                ExpandChatMenuIsExpanded = false;
                ExpandAttachmentsIsExpanded = false;
            }
        }
        public void ExpandChatsMenuAction()
        {
            ExpandChatMenuIsExpanded = !ExpandChatMenuIsExpanded;
            BlockBackSwitch = ExpandChatMenuIsExpanded;
            ExpandAttachmentsIsExpanded = false;
        }
        public void ExpandAttachmentsAction()
        {
            ExpandAttachmentsIsExpanded = !ExpandAttachmentsIsExpanded;
            BlockBackSwitch = ExpandAttachmentsIsExpanded;
            ExpandChatMenuIsExpanded = false;
        }
        public void OnMessageSelected(Message message)
        {
            if (message.IsLink)
            {
                TapLinkCommand.Execute(message);

            }
            else if (message.IsGpsMessage)
            {
                TapGpsLocationCommand.Execute(message);

            }
            else if(message.IsImg)
            {

            }
        }
        public async void OpenLinkFromMessageAction(Message message)
        {
            try
            {

                await Browser.Default.OpenAsync(message.ExtractedUrlFromText, BrowserLaunchMode.SystemPreferred);
            }
            catch
            {

            }
        }
        public async void OpenGpsLocationFromMessageAction(Message message)
        {
            try
            {

                await message.Location.OpenMapsAsync();
            }
            catch
            {

            }
        }
        public void MessageSwipeLeftAction()
        {

        }
        public void MessageSwipeRightAction(Message message)
        {

        }

        public async void OsTakePhotoTest()
        {
            try
            {

                if (MediaPicker.Default.IsCaptureSupported)
                {
                    FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

                    if (photo != null)
                    {
                        // save the file into local storage
                        string localFilePath = System.IO.Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                        using Stream sourceStream = await photo.OpenReadAsync();
                        using FileStream localFileStream = File.OpenWrite(localFilePath);

                        await sourceStream.CopyToAsync(localFileStream);
                        //SendMessageAction(photo.FullPath);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        public void VibrateTest()
        {

            int secondsToVibrate = Random.Shared.Next(1, 7);
            TimeSpan vibrationLength = TimeSpan.FromSeconds(secondsToVibrate);

            Vibration.Default.Vibrate(vibrationLength);
        }
        public void HapticTest()
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
        }
        public async void OsTakeVideoTest()
        {
            try
            {

                if (MediaPicker.Default.IsCaptureSupported)
                {
                    FileResult vid = await MediaPicker.Default.CaptureVideoAsync();
                    Location location = await GetCurrentLocation();
                    if (vid != null)
                    {
                        // save the file into local storage
                        string localFilePath = System.IO.Path.Combine(FileSystem.CacheDirectory, vid.FileName);

                        using Stream sourceStream = await vid.OpenReadAsync();
                        using FileStream localFileStream = File.OpenWrite(localFilePath);

                        await sourceStream.CopyToAsync(localFileStream);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        private CancellationTokenSource _cancelTokenSource;
        private bool _isCheckingLocation;
        public async Task<Location> GetCurrentLocation()
        {
            Location location = null;
            try
            {
                _isCheckingLocation = true;

                GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

                _cancelTokenSource = new CancellationTokenSource();

                location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);
            }
            // Catch one of the following exceptions:
            //   FeatureNotSupportedException
            //   FeatureNotEnabledException
            //   PermissionException
            catch (Exception ex)
            {
                // Unable to get location
            }
            finally
            {
                _isCheckingLocation = false;
            }
            return location;

        }

        public void CancelRequest()
        {
            if (_isCheckingLocation && _cancelTokenSource != null && _cancelTokenSource.IsCancellationRequested == false)
                _cancelTokenSource.Cancel();
        }
        public async void OsFileAccessTest()
        {
            string cacheDir = FileSystem.Current.CacheDirectory;
            string mainDir = FileSystem.Current.AppDataDirectory;
        }
        public void RefreshChatViewAction()
        {
            IsChatRefreshing = true;
            IsChatRefreshing = false;
        }
        public async void SendMessageAction()
        {
            if (Messages == null)
            {
                Messages = new ObservableCollection<MessageGroup>();
            }
            SendReceiveMessageState = !SendReceiveMessageState;//fuer tests
            var msg = new Message() 
            { 
                Text = Text, 
                Images = this.TakenPhotos, 
                Received = SendReceiveMessageState, 
                MessageDateTime = DateTime.Now, 
                SendToBackend = true,
                Location = this.SelectedGpsLocation,
                Contact = this.SelectedContact
            };
            DateOnly msgGrpKey = DateOnly.FromDateTime(msg.MessageDateTime);
            int index = Messages.IndexOf(x => msgGrpKey == x.Date);
            if (index == -1)
            {


                Messages.Add(new MessageGroup(msgGrpKey));
                index = Messages.Count - 1;
            }
            Messages[index].Add(msg);
            Text = null;
            RefreshChatViewAction();
            OnPropertyChanged("Messages");
            FocusLastMessage = false;
            FocusLastMessage = true;
            this.SelectedContact = null;
            this.SelectedGpsLocation = null;
            /*var request = new NotificationRequest
            {
                NotificationId = 100,
                Title = "title",
                Subtitle = msg.Text,
                Description = $"Tap Count: {102}",
                BadgeNumber = 102,
                ReturningData = "returning data",
                CategoryType = NotificationCategoryType.Status,
                Android =
            {
                IconSmallName =
                {
                    ResourceName = "i2",
                },
                Color =
                {
                    ResourceName = "colorPrimary"
                },
                    IsProgressBarIndeterminate = false,

                ProgressBarMax = 100,
                ProgressBarProgress = 24,
                Priority = AndroidPriority.High
                //AutoCancel = false,
                //Ongoing = true
            },

            };

            request.Sound = DeviceInfo.Platform == DevicePlatform.Android
                ? "good_things_happen"
                : "good_things_happen.aiff";

            try
            {
                if (await _notificationService.AreNotificationsEnabled() == false)
                {
                    await _notificationService.RequestNotificationPermission();
                }

                var ff = await _notificationService.Show(request);

            }
            catch (Exception exception)
            {
            }*/
        }

        public void SetChat(Chat chat)
        {
            _chat = chat;
            OnPropertyChanged("Chat");
            if (Messages == null)
            {
                Messages = new ObservableCollection<MessageGroup>();
            }
            foreach (var item in chat.Messages)
            {
                Messages.Add(item);
            }
            OnPropertyChanged("Messages");
        }

    }
}
