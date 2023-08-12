using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Mobile.Jellyfish.Data.SqlLite.Schema
{
    public class UserEntity : AbstractEntity
    {
        private string _nickName;
        private byte[] _profilePicture = null;
        private int _userId;
        private Guid _useruUuid;
        private List<ChatEntity> _chats;

        [PrimaryKey, AutoIncrement]
        public int UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }
        public Guid UserUuid
        {
            get { return _useruUuid; }
            set { _useruUuid = value; }
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
        /*[ManyToMany(typeof(UserLinkChatEntity), CascadeOperations = CascadeOperation.CascadeRead)]
        public List<ChatEntity> Chats
        {
            get { return _chats; }
            set { _chats = value; }
        }*/
    }
}
