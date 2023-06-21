using JellyFish.ViewModel;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Communication = Microsoft.Maui.ApplicationModel.Communication;

namespace JellyFish.Model
{
    public class User :BaseViewModel
    {
        private string _nickName;
        private byte[] _profilePicture = null;
        private int _userId;
        public int UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        public string NickName
        {
            get
            {
                return _nickName;
            }
            set
            {
                _nickName = value;
            }
        }
        private string _status = "Hello iam using Jellyfish!";
        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
                OnPropertyChanged(nameof(StatusDisplay));
            }
        }
        public string StatusDisplay
        {
            get
            {
                return string.Format("\"{0}\"", _status);
            }
        }
        public byte[] ProfilePicture
        {
            get
            {
                return _profilePicture;
            }
            set
            {
                _profilePicture = value;
            }
        }
        private bool _isVisible = true;

        public bool IsVisible
        {
            get
            {
                return _isVisible;

            }
            set
            {
                _isVisible = value;
                OnPropertyChanged(nameof(IsVisible));
            }
        }
        private bool _isSelected = false;

        public bool IsSelected
        {
            get
            {
                return _isSelected;

            }
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }
        public User()
        {

        }
    }
}
