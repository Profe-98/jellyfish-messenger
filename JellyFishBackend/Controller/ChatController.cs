using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Reflection;
using WebApiFunction.Application.Model.DataTransferObject.Helix.Frontend.Transfer;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiFunction.Web.Authentification;
using WebApiFunction.Application.Controller.Modules.Jellyfish;
using WebApiFunction.Application.Model.Internal;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Mail;
using WebApiFunction.Database;
using WebApiFunction.Web.Http.Api.Abstractions.JsonApiV1;
using WebApiFunction.Threading.Service;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Configuration;
using WebApiFunction.MicroService;
using WebApiFunction.Security.Encryption;
using Microsoft.AspNetCore.Authorization;
using WebApiFunction.Web.AspNet.Filter;
using System.Data.Common;
using WebApiFunction.Web.Authentification.JWT;
using WebApiFunction.Application.Controller.Modules;
using Microsoft.Extensions.DependencyInjection;
using JellyFishBackend.SignalR.Hub;
using Microsoft.AspNetCore.SignalR;
using WebApiFunction.Application.Model.Database.MySQL.Jellyfish;
using WebApiFunction.Web.AspNet.Controller;
using WebApiFunction.Application.WebSocket.SignalR.JellyFish;

namespace JellyFishBackend.Controller
{
    [ApiExplorerSettings(IgnoreApi = false)]
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
        public override Task<ActionResult<ApiRootNodeModel>> Create([FromBody] ApiRootNodeModel body, bool allowDuplicates = true)
        {


            var result = base.Create(body, allowDuplicates);

            return result;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(ApiRootNodeModel), 200)]
        [ProducesResponseType(typeof(ApiRootNodeModel), 400)]
        public override Task<ObjectResult> Get()
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
