using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Application.Shared.Kernel.Data.Web.Api.Abstractions.JsonApiV1;
using Application.Shared.Kernel.Web.Authentification;
using Application.Shared.Kernel.Web.Http.Api.Abstractions.JsonApiV1;
using Application.Shared.Kernel.Threading.Service;
using Application.Shared.Kernel.Data.Format.Json;
using Application.Shared.Kernel.MicroService;
using Application.Shared.Kernel.Security.Encryption;
using Microsoft.AspNetCore.Authorization;
using Application.Shared.Kernel.Application.Controller.Modules;
using Application.Web.Api.JellyFishBackend.SignalR.Hub;
using Microsoft.AspNetCore.SignalR;
using Application.Shared.Kernel.Web.AspNet.Controller;
using Application.Shared.Kernel.Web.AspNet.Swagger.Attribut;
using Application.Shared.Kernel.Configuration.Service;
using Application.Shared.Kernel.Infrastructure.Mail;
using Application.Shared.Kernel.Infrastructure.Cache.Distributed.RedisCache;
using Application.Shared.Kernel.Infrastructure.Ampq.Rabbitmq;
using Application.Shared.Kernel.Infrastructure.Antivirus.nClam;
using Application.Shared.Kernel.Infrastructure.Database;
using Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.Jellyfish.Table;
using Application.Shared.Kernel.Application.Controller.Modules.Jellyfish;
using Application.Shared.Kernel.Application.WebSocket.SignalR;

namespace Application.Web.Api.JellyFishBackend.Controller
{
#if DEBUG

    [ApiExplorerSettings(IgnoreApi = false)]
#else

    [ApiExplorerSettings(IgnoreApi = true)]
#endif
    [Authorize()]
    public class ChatController : AbstractController<ChatModel>
    {
        private readonly IAuthHandler _auth;
        private readonly IConfiguration _configuration;
        private readonly IEncryptionHandler _encryptionHandler;
        private readonly IHubContext<MessengerHub, IMessengerClient> _messengerHub;

        public ChatController(ILogger<CustomApiControllerBase<ChatModel>> logger, IScopedVulnerablityHandler vulnerablityHandler, IMailHandler mailHandler, IAuthHandler authHandler, IScopedDatabaseHandler databaseHandler, IJsonApiDataHandler jsonApiHandler, ITaskSchedulerBackgroundServiceQueuer queue, IScopedJsonHandler jsonHandler, ICachingHandler cache, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, IWebHostEnvironment env, IConfiguration configuration, IRabbitMqHandler rabbitMqHandler, IAppconfig appConfig, INodeManagerHandler nodeManagerHandler, IScopedEncryptionHandler scopedEncryptionHandler, IAbstractBackendModule<ChatModel> abstractBackendModule, IServiceProvider serviceProvider, IHubContext<MessengerHub, IMessengerClient> messengerHub, IAbstractBackendModule<UserModel> userModule) : 
            base(logger, vulnerablityHandler, mailHandler, authHandler, databaseHandler, jsonApiHandler, queue, jsonHandler, cache, actionDescriptorCollectionProvider, env, configuration, rabbitMqHandler, appConfig, nodeManagerHandler, scopedEncryptionHandler, abstractBackendModule, serviceProvider)
        {
            _messengerHub = messengerHub;
            _auth = authHandler;
            _configuration = configuration;
            _encryptionHandler = scopedEncryptionHandler;
        }


        [Authorize]
        [HttpPost]
        public new Task<ActionResult<ApiRootNodeModel>> Create([FromBody] ApiRootNodeModel body, [OpenApiIgnoreMethodParameter] bool allowDuplicates = true)
        {


            var result = base.Create(body, allowDuplicates);

            return result;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(ApiRootNodeModel), 200)]
        [ProducesResponseType(typeof(ApiRootNodeModel), 400)]
        public new Task<ObjectResult> Get()
        {
            return base.Get();
        }

        [NonAction]
        public override ChatModule GetConcreteModule()
        {
            return ((ChatModule)_backendModule);
        }

    }
}
