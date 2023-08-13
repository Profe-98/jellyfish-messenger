using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Reflection;
using Application.Shared.Kernel.Data.Web.Api.Abstractions.JsonApiV1;
using Application.Shared.Kernel.Web.Authentification;
using Application.Shared.Kernel.Web.Http.Api.Abstractions.JsonApiV1;
using Application.Shared.Kernel.Threading.Service;
using Application.Shared.Kernel.Data.Format.Json;
using Application.Shared.Kernel.MicroService;
using Application.Shared.Kernel.Security.Encryption;
using Microsoft.AspNetCore.Authorization;
using Application.Web.Api.JellyFishBackend.SignalR.Hub;
using Microsoft.AspNetCore.SignalR;
using Application.Shared.Kernel.Web.AspNet.ModelBinder.JsonApiV1;
using Application.Shared.Kernel.Web.AspNet.Controller;
using Application.Shared.Kernel.Web.AspNet.Swagger.Attribut;
using Application.Shared.Kernel.Configuration.Service;
using Application.Shared.Kernel.Application.Model.DataTransferObject.ConcreteImplementation.Jellyfish;
using Application.Shared.Kernel.Infrastructure.Mail;
using Application.Shared.Kernel.Infrastructure.Cache.Distributed.RedisCache;
using Application.Shared.Kernel.Infrastructure.Ampq.Rabbitmq;
using Application.Shared.Kernel.Infrastructure.Antivirus.nClam;
using Application.Shared.Kernel.Infrastructure.Database;
using Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.Jellyfish.Table;
using Application.Shared.Kernel.Application.Model.Internal;
using Application.Shared.Kernel.Application.Controller.Modules.Jellyfish;
using Application.Shared.Kernel.Application.Controller.Modules;
using Application.Shared.Kernel.Application.WebSocket.SignalR;

namespace Application.Web.Api.JellyFishBackend.Controller
{
#if DEBUG

    [ApiExplorerSettings(IgnoreApi = false)]
#else

    [ApiExplorerSettings(IgnoreApi = true)]
#endif
    [Authorize()]
    public class MessageController :AbstractController<MessageModel>
    {
        private readonly IAuthHandler _auth;
        private readonly IConfiguration _configuration;
        private readonly IEncryptionHandler _encryptionHandler;
        private readonly Application.Shared.Kernel.Application.Controller.Modules.Jellyfish.UserModule _userModule;
        private readonly ChatModule _chatModule;
        private readonly IHubContext<MessengerHub, IMessengerClient> _messengerHub;

        public MessageController(ILogger<CustomApiControllerBase<MessageModel>> logger, IScopedVulnerablityHandler vulnerablityHandler, IMailHandler mailHandler, IAuthHandler authHandler, IScopedDatabaseHandler databaseHandler, IJsonApiDataHandler jsonApiHandler, ITaskSchedulerBackgroundServiceQueuer queue, IScopedJsonHandler jsonHandler, ICachingHandler cache, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, IWebHostEnvironment env, IConfiguration configuration, IRabbitMqHandler rabbitMqHandler, IAppconfig appConfig, INodeManagerHandler nodeManagerHandler, IScopedEncryptionHandler scopedEncryptionHandler, IAbstractBackendModule<MessageModel> abstractBackendModule, IServiceProvider serviceProvider, IHubContext<MessengerHub, IMessengerClient> messengerHub, IAbstractBackendModule<UserModel> userModule, IAbstractBackendModule<ChatModel> chatModule) :
            base(logger, vulnerablityHandler, mailHandler, authHandler, databaseHandler, jsonApiHandler, queue, jsonHandler, cache, actionDescriptorCollectionProvider, env, configuration, rabbitMqHandler, appConfig, nodeManagerHandler, scopedEncryptionHandler, abstractBackendModule, serviceProvider)
        {
            _messengerHub = messengerHub;
            _auth = authHandler;
            _configuration = configuration;
            _encryptionHandler = scopedEncryptionHandler;
            _userModule = (Application.Shared.Kernel.Application.Controller.Modules.Jellyfish.UserModule)userModule;
            _chatModule = (ChatModule)chatModule;
        }


        [Authorize]
        [HttpPost]
        public new async Task<ActionResult<ApiRootNodeModel>> Create([FromBody] ApiRootNodeModel body, [OpenApiIgnoreMethodParameter] bool allowDuplicates = true)
        {
            MethodDescriptor methodInfo = _webHostEnvironment.IsDevelopment() ? new MethodDescriptor { c = GetType().Name, m = MethodBase.GetCurrentMethod().Name } : null;
            var currentContextUserUuid = this.HttpContext.User.GetUuidFromClaims();
            if(currentContextUserUuid == Guid.Empty)
            {
                return await JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Id = Guid.Empty, Detail = "resource not found" }
            }, HttpStatusCode.BadRequest, "an error occurred", "currentContextUserUuid == Guid.Empty", methodInfo).ToJsonApiObjectResultTaskResult();
            }
            var data = body.ExtractByType<MessageModel>();
            if(data == null)
            {
                var res = JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Id = Guid.Empty, Detail = "resource not found" }
            }, HttpStatusCode.BadRequest, "an error occurred", "body.ExtractByType<MessageModel> is null", methodInfo).ToJsonApiObjectResultTaskResult();
                return await res;
            }
            var sortMessagesByChats = from msg in data group msg by msg.ChatUuid into groupedMessages select groupedMessages;


            var result = await base.Create(body, allowDuplicates);
            if(result == null)
            {
                throw new Exception("failed to processing the request");
            }

            foreach (var chat in sortMessagesByChats)
            {

                var chatUsers = await _chatModule.GetChatMembers(chat.Key);
                if (chatUsers == null)
                    continue;
                foreach( var chatUser in chatUsers)
                {
                    if(chatUser.SignalRConnectionId != null)
                    {

                        var messages = chat.ToList();
                        List<MessageDTO> messagesDTOList = new List<MessageDTO>();
                        messages.ForEach(x => messagesDTOList.Add(x.GetMappedDataTransferModel<MessageDTO>())) ;
                        await _messengerHub.Clients.Client(chatUser.SignalRConnectionId).ReceiveMessage(messagesDTOList) ;
                    }
                }
            }


            return result;
        }

        [Authorize]
        [HttpGet("nack")]
        public async Task<ActionResult<ApiRootNodeModel>> GetAllNotReceivedMessages()
        {
            MethodDescriptor methodInfo = _webHostEnvironment.IsDevelopment() ? new MethodDescriptor { c = GetType().Name, m = MethodBase.GetCurrentMethod().Name } : null;
            var currentContextUserUuid = this.HttpContext.User.GetUuidFromClaims();
            if (currentContextUserUuid == Guid.Empty)
            {
                return await JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Id = Guid.Empty, Detail = "resource not found" }
            }, HttpStatusCode.BadRequest, "an error occurred", "currentContextUserUuid == Guid.Empty", methodInfo).ToJsonApiObjectResultTaskResult();
            }
            var nAckMsgs = await this.GetConcreteModule().GetAllChatNotReceivedMessages(currentContextUserUuid);
            var area = this.GetArea();
            return await _jsonApiHandler.CreateApiRootNodeFromModel(area.RouteValue, nAckMsgs);
        }

        [Authorize]
        [HttpPost("ack")]

        public async Task<ActionResult<ApiRootNodeModel>> AcknowledgeMessage([ModelBinder(typeof(ApiRootNodeModelModelBinder<MessageAcknowledgeDTO>))] List<MessageAcknowledgeDTO> messages)
        {
            MethodDescriptor methodInfo = _webHostEnvironment.IsDevelopment() ? new MethodDescriptor { c = GetType().Name, m = MethodBase.GetCurrentMethod().Name } : null;
            var currentContextUserUuid = this.HttpContext.User.GetUuidFromClaims();
            if (currentContextUserUuid == Guid.Empty)
            {
                return await JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Id = Guid.Empty, Detail = "resource not found" }
            }, HttpStatusCode.BadRequest, "an error occurred", "currentContextUserUuid == Guid.Empty", methodInfo).ToJsonApiObjectResultTaskResult();
            }
            if (messages.Count == 0)
            {
                return await JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Id = Guid.Empty, Detail = "collection is empty" }
            }, HttpStatusCode.BadRequest, "an error occurred", "messages.Count == 0", methodInfo).ToJsonApiObjectResultTaskResult();
            }
            var chatUuids = (from msg in messages group msg by msg.ChatUuid into newGroup select newGroup.Key).ToList();
            var invalidChatIdsCount = chatUuids.FindAll(x => x == Guid.Empty).Count();
            if (invalidChatIdsCount!= 0)
            {
                return await JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Id = Guid.Empty, Detail = "invalid uuid in chat_uuid" }
            }, HttpStatusCode.BadRequest, "an error occurred", "chatUuids.FindAll(x=> x == Guid.Empty) != null", methodInfo).ToJsonApiObjectResultTaskResult();
            }
            int i = 0;
            bool[] isMemberInChat = new bool[chatUuids.Count];
            chatUuids.ForEach(async(x)=> {

                var chatMembers= await _chatModule.GetChatMembers(x);
                bool notAMember = (chatMembers == null || chatMembers.Count == 0 || chatMembers.Find(y => y.Uuid == currentContextUserUuid) == null);
                if(!notAMember)
                {
                    isMemberInChat[i] = true;  
                }
                i++;
            });
            if(isMemberInChat.ToList().FindAll(x=> x).Count != chatUuids.Count)
            {
                return await JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Id = Guid.Empty, Detail = "not a member in chat" }
            }, HttpStatusCode.BadRequest, "an error occurred", "isMemberInChat.ToList().FindAll(x=> x).Count != chatUuids.Count", methodInfo).ToJsonApiObjectResultTaskResult();
            }


            var msgIdsFromMethodParam = messages.GroupBy((x) => x.MessageUuid).ToList().Select(x => x.Key).ToArray();
            var currentExistendAcks = await this.GetConcreteModule().GetAcknowledgedMessages(currentContextUserUuid, msgIdsFromMethodParam);
            for (i = 0; i < currentExistendAcks.Count; i++)
            {
                var currentIteration = currentExistendAcks[i];
                var findItem = messages.Find(x => x.MessageUuid == currentIteration.MessageUuid);
                if (findItem == null)
                    continue;
                messages.Remove(findItem);
            }

            var acknowledgedMessages = await this.GetConcreteModule().AcknowledgeMessages(currentContextUserUuid,messages);
            var responseModel = await _jsonApiHandler.CreateApiRootNodeFromModel(this.GetArea().RouteValue, acknowledgedMessages,0);
            return await Task.FromResult(responseModel);
        }

        [NonAction]
        public override MessageModule GetConcreteModule()
        {
            return ((MessageModule)_backendModule);
        }
    }
}
