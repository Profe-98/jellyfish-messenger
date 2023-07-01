using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Application.Controller.Modules;
using WebApiFunction.Application.Controller.Modules.Jellyfish;
using WebApiFunction.Application.Model.Database.MySQL.Jellyfish;
using WebApiFunction.Application.WebSocket.SignalR.JellyFish;
using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Database;
using WebApiFunction.Web.Authentification;
using WebApiFunction.Web.Http.Api.Abstractions.JsonApiV1;
using WebApiFunction.Web.Websocket.SignalR.HubService;
using WebApiFunction.Web.Websocket.SignalR.HubService.Attribute;

namespace JellyFishBackend.SignalR.Hub
{
    [HubServiceRoute("/messenger")]
    public class MessengerHub : HubService<IMessengerClient>, IStronglyTypedSignalRHub
    {
        private readonly ILogger<MessengerHub> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IRabbitMqHandler _rabbitMqHandler;
        private readonly ICachingHandler _cachingHandler;
        private readonly IJsonApiDataHandler _jsonApiDataHandler;
        private readonly ISingletonDatabaseHandler _singletonDatabaseHandler;
        private readonly WebApiFunction.Application.Controller.Modules.Jellyfish.UserModule _userModule;
        public override HttpConnectionDispatcherOptions HttpConnectionDispatcherOptions => new HttpConnectionDispatcherOptions
        {
            Transports = HttpTransportType.WebSockets,
            MinimumProtocolVersion = 1,
        };
        public MessengerHub(
            ILogger<MessengerHub> logger,
            IServiceProvider serviceProvider,
            IRabbitMqHandler rabbitMqHandler,
            ICachingHandler cachingHandler,
            ISingletonDatabaseHandler singletonDatabaseHandler,
            IJsonApiDataHandler jsonApiDataHandler,
            IAbstractBackendModule<UserModel> userModule)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _jsonApiDataHandler = jsonApiDataHandler;
            _rabbitMqHandler = rabbitMqHandler;
            _cachingHandler = cachingHandler;
            _singletonDatabaseHandler = singletonDatabaseHandler;
            _userModule = (WebApiFunction.Application.Controller.Modules.Jellyfish.UserModule)userModule;

            //Entities: WebApiFunction.Application.Model.Database.MySql.Jellyfish
        }

        /*public async Task SendMessage(string user, string message)
        {

            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendMessageToCaller(string user, string message)
        {
            await Clients.Caller.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendMessageToGroup(string user, string message)
        {
            await Clients.Group("SignalR Users").SendAsync("ReceiveMessage", user, message);
        }*/
        private async void InitConnect()
        {
            var userUuid = this.Context.User.GetUuidFromClaims();
            bool success = await _userModule.SetSignalR(userUuid,this.Context.ConnectionId);
            _logger.LogInformation("SignalR->" + nameof(MessengerHub) + ": OnConnectedAsync->{0}", this.Context.ConnectionId);
        }

        public override Task OnConnectedAsync()
        {
            InitConnect();
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            InitDisconnect();
            _logger.LogInformation("SignalR->" + nameof(MessengerHub) + ": OnDisconnectedAsync->{0}", this.Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
        private async void InitDisconnect()
        {
            var userUuid = this.Context.User.GetUuidFromClaims();
            bool success = await _userModule.SetSignalR(userUuid, this.Context.ConnectionId);
            _logger.LogInformation("SignalR->" + nameof(MessengerHub) + ": OnConnectedAsync->{0}", this.Context.ConnectionId);
        }
    }
}
