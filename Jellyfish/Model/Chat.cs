using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JellyFish.ViewModel;

namespace JellyFish.Model
{
    public class Chat : BaseViewModel
    {
        private Guid _userId;
        public Guid UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }
        private ObservableCollection<MessageGroup> _messages;
        public ObservableCollection<MessageGroup> Messages
        {
            get { return _messages; }
            set { _messages = value; OnPropertyChanged(); OnPropertyChanged("IsNewConversation"); }
        }
        public MessageGroup LastMessageGroup
        {
            get
            {
                return _messages.LastOrDefault();
            }
        }
        public Message LastMessageInMessageGroup
        {
            get
            {
                return LastMessageGroup != null && LastMessageGroup.Count > 0 ? LastMessageGroup.LastOrDefault() : null;
            }
        }




        public bool IsNewConversation
        {
            get
            {
                return _messages == null || _messages.Count() == 0;
            }
        }

    }
}
