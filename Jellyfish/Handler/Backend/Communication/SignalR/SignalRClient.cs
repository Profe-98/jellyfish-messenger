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
using Microsoft.AspNetCore.SignalR.Client;
using JellyFish.Handler.AppConfig;
using Microsoft.Maui.Handlers;
using JellyFish.Handler.Data.InternalDataInterceptor.Abstraction;
using JellyFish.Handler.Data.InternalDataInterceptor.Invoker;
using JellyFish.Handler.Data.InternalDataInterceptor;
#if ANDROID
using JellyFish.Handler.Data.InternalDataInterceptor.Invoker.Notification.Android;
#elif IOS
using JellyFish.Handler.Data.InternalDataInterceptor.Invoker.Notification.iOS;
#endif

namespace JellyFish.Handler.Backend.Communication.SignalR
{
    public class SignalRClient : AbstractSignalRClient, IMessengerClient
    {
        private readonly MessageDataInterceptor _messageDataInterceptor;
        /*private readonly ViewModelInvoker _viewModelInvoker;
        private readonly SqlLiteDatabaseHandlerInvoker _sqlLiteDatabaseHandlerInvoker;
        private readonly NotificationInvoker _notificationInvoker;*/

        public SignalRClient(
            ApplicationConfigHandler applicationConfigHandler,
            MessageDataInterceptor messageDataInterceptor,
            ViewModelInvoker viewModelInvoker,
            SqlLiteDatabaseHandlerInvoker sqlLiteDatabaseHandlerInvoker,
            NotificationInvoker notificationInvoker) : base()
        {
            string protocolSignalR = applicationConfigHandler.ApplicationConfig.NetworkConfig.WebApiHttpClientTransportProtocol == JellyFish.Data.AppConfig.ConcreteImplements.NetworkConfig.HTTP_TRANSPORT_PROTOCOLS.HTTP ? "http://" : "https://";
            string url =
            protocolSignalR +
                applicationConfigHandler.ApplicationConfig.NetworkConfig.SignalRHubBaseUrl + ":" +
                applicationConfigHandler.ApplicationConfig.NetworkConfig.SignalRHubBaseUrlPort +
                applicationConfigHandler.ApplicationConfig.NetworkConfig.SignalRHubEndpoint;
            this.Initialize(url, async () =>
            {
                return applicationConfigHandler.ApplicationConfig.AccountConfig.UserSession.Token;
            },
            applicationConfigHandler.ApplicationConfig.NetworkConfig.SignalRTransportProtocol,
            applicationConfigHandler.ApplicationConfig.NetworkConfig.SignalRTransferFormat);

            _messageDataInterceptor = messageDataInterceptor;
            _messageDataInterceptor.Add(notificationInvoker);
            _messageDataInterceptor.Add(sqlLiteDatabaseHandlerInvoker);
            _messageDataInterceptor.Add(viewModelInvoker);


            /*_notificationInvoker = notificationInvoker;
            _sqlLiteDatabaseHandlerInvoker = sqlLiteDatabaseHandlerInvoker;
            _viewModelInvoker = viewModelInvoker;*/
        }

        public override void InitClientMethods()
        {

            HubConnection.On<List<MessageDTO>>(nameof(ReceiveMessage), ReceiveMessage);
            HubConnection.On<UserDTO>(nameof(AcceptFriendshipRequest), AcceptFriendshipRequest);
            HubConnection.On<UserFriendshipRequestDTO>(nameof(ReceiveFriendshipRequest), ReceiveFriendshipRequest);
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
            _messageDataInterceptor.Invoke(messages);
            return Task.CompletedTask;
        }
    }
}
