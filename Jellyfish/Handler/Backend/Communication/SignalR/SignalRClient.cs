using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiFunction.Web.Websocket.SignalR.HubClient;
using WebApiFunction.Application.WebSocket.SignalR.JellyFish;
using WebApiFunction.Application.Model.Database.MySQL.Jellyfish;

namespace JellyFish.Handler.Backend.Communication.SignalR
{
    public class SignalRClient : AbstractSignalRClient, IMessengerClient
    {
        public SignalRClient() : base()
        {
        }

        public Task AcceptFriendshipRequest(UserDTO userDTO)
        {
            throw new NotImplementedException();
        }

        public Task ReceiveFriendshipRequest(UserFriendshipRequestDTO request)
        {
            throw new NotImplementedException();
        }

        public Task ReceiveMessage(List<MessageDTO> messages)
        {
            throw new NotImplementedException();
        }
    }
}
