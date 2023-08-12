
using Microsoft.AspNetCore.SignalR.Client;
using Application.Mobile.Jellyfish.Handler.AppConfig;
using Application.Mobile.Jellyfish.ViewModel;
using Application.Shared.Kernel.Application.Model.DataTransferObject.ConcreteImplementation.Jellyfish;
using Application.Shared.Kernel.Web.Websocket.SignalR.HubClient;
using Application.Shared.Kernel.Application.WebSocket.SignalR;
#if ANDROID
using Application.Mobile.Jellyfish.Handler.Data.InternalDataInterceptor.Invoker.Notification.Android;
#elif IOS
using Application.Mobile.Jellyfish.Handler.Data.InternalDataInterceptor.Invoker.Notification.iOS;
#endif

namespace Application.Mobile.Jellyfish.Handler.Backend.Communication.SignalR
{
    public class SignalRClient : AbstractSignalRClient, IMessengerClient
    {
        public List<BaseViewModel> ViewModelsToCommunicate { get; set; }
        private readonly Application.Mobile.Jellyfish.Handler.Data.InternalDataInterceptor.InternalDataInterceptorApplication _messageDataInterceptor;

        public SignalRClient(
            ApplicationConfigHandler applicationConfigHandler,
            Application.Mobile.Jellyfish.Handler.Data.InternalDataInterceptor.InternalDataInterceptorApplication messageDataInterceptor) : base()
        {
            string protocolSignalR = applicationConfigHandler.ApplicationConfig.NetworkConfig.WebApiHttpClientTransportProtocol == Application.Mobile.Jellyfish.Data.AppConfig.ConcreteImplements.NetworkConfig.HTTP_TRANSPORT_PROTOCOLS.HTTP ? "http://" : "https://";
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
            this.HubConnectionReconnectedEvent += new EventHandler<string>(SignalrClientReconnectedToBackendEvent);

        }

        public void SignalrClientReconnectedToBackendEvent(object sender,string args)
        {

        }

        public override void InitClientMethods()
        {

            HubConnection.On<List<MessageDTO>>(nameof(ReceiveMessage), ReceiveMessage);
            HubConnection.On<UserDTO>(nameof(AcceptFriendshipRequest), AcceptFriendshipRequest);
            HubConnection.On<UserFriendshipRequestDTO>(nameof(ReceiveFriendshipRequest), ReceiveFriendshipRequest);
        }

        public Task AcceptFriendshipRequest(UserDTO userDTO)
        {
            _messageDataInterceptor.ReceiveAcceptFriendRequest(userDTO);
            return Task.CompletedTask;
        }

        public Task ReceiveFriendshipRequest(UserFriendshipRequestDTO request)
        {
            _messageDataInterceptor.ReceiveFriendRequest(request);
            return Task.CompletedTask;
        }

        public Task ReceiveMessage(List<MessageDTO> messages)
        {
            _messageDataInterceptor.ReceiveMessage(messages.ToArray());
            return Task.CompletedTask;
        }
    }
}
