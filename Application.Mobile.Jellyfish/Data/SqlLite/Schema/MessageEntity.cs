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
    [SQLite.Table("message")]
    public class MessageEntity : AbstractEntity
    {
        private int _msgId;
        private int _userId;
        private UserEntity _user;
        private int _chatId;
        private DateTime _messageDateTime;
        private bool _readed;
        private string _text;
        private MessageLocationEntity _location;
        private ChatEntity _chat;

        [PrimaryKey, AutoIncrement]
        public int MessageId
        {
            get { return _msgId; }
            set { _msgId = value; }
        }
        private Guid _messageUuid;
        public Guid MessageUuid
        {
            get { return _messageUuid; }
            set { _messageUuid = value; }
        }

        [ForeignKey(typeof(ChatEntity))]
        public int ChatId
        {
            get { return _chatId; }
            set { _chatId = value; }
        }
        //, welcher die Nachricht abgesetzt hat
        [ForeignKey(typeof(UserEntity))]
        public int UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        /*[OneToOne()]
        public UserEntity User
        {
            get { return _user; }
            set { _user = value; }
        }
        [OneToOne()]
        public MessageLocationEntity Location
        {
            get { return _location; }
            set
            {
                _location = value;
            }
        }*/

        public DateTime MessageDateTime
        {
            get { return _messageDateTime; }
            set
            {
                _messageDateTime = value;
            }
        }
        public bool Readed
        {
            get
            { return _readed; }
            set
            {
                _readed = value;
            }
        }

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
            }
        }

        /*[ManyToOne(CascadeOperations = CascadeOperation.CascadeRead)]
        public ChatEntity Chat
        {
            get
            {
                return _chat;
            }
            set
            {
                _chat= value;
            }
        }*/
    }
}
