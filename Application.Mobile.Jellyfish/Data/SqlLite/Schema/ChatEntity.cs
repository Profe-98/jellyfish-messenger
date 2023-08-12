using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Application.Mobile.Jellyfish.Data.SqlLite.Schema
{
    [SQLite.Table("chat")]
    public class ChatEntity : AbstractEntity
    {
        private int _chatId;
        private Guid _chatUuid;
        private string _chatName;
        private List<UserEntity> _chatUsers;
        private List<MessageEntity> _messages;

        [PrimaryKey, AutoIncrement]
        public int ChatId
        {
            get { return _chatId; }
            set { _chatId = value; }
        }
        public string ChatName
        {
            get { return _chatName; }
            set { _chatName = value; }
        }
        public Guid ChatUuid
        {
            get
            {
                return _chatUuid;
            }
            set
            {
                _chatUuid = value;
            }
        }

        [OneToMany()]
        public List<MessageEntity> Messages
        {
            get { return _messages; }
            set { _messages = value; }
        }
        //Ein Chat kann ja auch ein Gruppenchat sein d.h. eine Collection an UserEntites
        [ManyToMany(typeof(UserLinkChatEntity))]
        public List<UserEntity> Users
        {
            get { return _chatUsers; }
            set { _chatUsers = value; }
        }
    }
}
