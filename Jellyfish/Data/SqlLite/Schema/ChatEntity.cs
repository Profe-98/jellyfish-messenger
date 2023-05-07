using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace JellyFish.Data.SqlLite.Schema
{
    [SQLite.Table("chat")]
    public class ChatEntity : AbstractEntity
    {
        private int _chatId;
        private List<UserEntity> _chatUsers;
        private List<MessageEntity> _messages;

        [PrimaryKey, AutoIncrement]
        public int ChatId
        {
            get { return _chatId; }
            set { _chatId = value; }
        }

        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead)]
        public List<MessageEntity> Messages
        {
            get { return _messages; }
            set { _messages = value; }
        }
        //Ein Chat kann ja auch ein Gruppenchat sein d.h. eine Collection an UserEntites
        [ManyToMany(typeof(UserLinkChatEntity), CascadeOperations = CascadeOperation.CascadeRead)]
        public List<UserEntity> Users
        {
            get { return _chatUsers; }
            set { _chatUsers = value; }
        }
    }
}
