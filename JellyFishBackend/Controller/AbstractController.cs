using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Application.Controller.Modules;
using WebApiFunction.Application.Model.Database.MySQL;
using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Configuration;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Database;
using WebApiFunction.Mail;
using WebApiFunction.MicroService;
using WebApiFunction.Security.Encryption;
using WebApiFunction.Threading.Service;
using WebApiFunction.Web.AspNet.Controller;
using WebApiFunction.Web.Authentification;
using WebApiFunction.Web.Http.Api.Abstractions.JsonApiV1;

namespace JellyFishBackend.Controller
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    [ApiController]
    [Area("jelly-api-1")]
    [Route("[area]/[controller]")]
    public abstract class AbstractController<T> : CustomApiV1ControllerBase<T> where T : AbstractModel
    {
        public AbstractController() : base()
        {
            
        }
        protected AbstractController(ILogger<CustomApiControllerBase<T>> logger, IScopedVulnerablityHandler vulnerablityHandler, IMailHandler mailHandler, IAuthHandler authHandler, IScopedDatabaseHandler databaseHandler, IJsonApiDataHandler jsonApiHandler, ITaskSchedulerBackgroundServiceQueuer queue, IScopedJsonHandler jsonHandler, ICachingHandler cache, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, IWebHostEnvironment env, IConfiguration configuration, IRabbitMqHandler rabbitMqHandler, IAppconfig appConfig, INodeManagerHandler nodeManagerHandler, IScopedEncryptionHandler scopedEncryptionHandler, IAbstractBackendModule<T> abstractBackendModule, IServiceProvider serviceProvider) : 
            base(logger, vulnerablityHandler, mailHandler, authHandler, databaseHandler, jsonApiHandler, queue, jsonHandler, cache, actionDescriptorCollectionProvider, env, configuration, rabbitMqHandler, appConfig, nodeManagerHandler, scopedEncryptionHandler, abstractBackendModule, serviceProvider)
        {
        }


    }
}
