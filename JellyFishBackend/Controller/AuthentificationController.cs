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
using WebApiFunction.Application.Controller.Modules;
using Microsoft.Extensions.DependencyInjection;
using WebApiFunction.Application.Model.Database.MySQL.Jellyfish;
using WebApiFunction.Web.AspNet.Controller;

namespace JellyFishBackend.Controller
{
#if DEBUG

    [ApiExplorerSettings(IgnoreApi = false)]
#else

    [ApiExplorerSettings(IgnoreApi = true)]
#endif
    [Authorize]
    public class AuthentificationController : AbstractController<AuthModel>
    {

        private readonly ILogger<AuthentificationController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IAuthHandler _authHandler;

        public AuthentificationController(ILogger<AuthentificationController> logger, IScopedVulnerablityHandler vulnerablityHandler, IMailHandler mailHandler, IAuthHandler authHandler, IScopedDatabaseHandler databaseHandler, IJsonApiDataHandler jsonApiHandler, ITaskSchedulerBackgroundServiceQueuer queue, IScopedJsonHandler jsonHandler, ICachingHandler cache, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, IWebHostEnvironment env, IConfiguration configuration, IRabbitMqHandler rabbitMqHandler, IAppconfig appConfig, INodeManagerHandler nodeManagerHandler, IScopedEncryptionHandler scopedEncryptionHandler, IAbstractBackendModule<AuthModel> abstractBackendModule, IServiceProvider serviceProvider) : 
            base(logger, vulnerablityHandler, mailHandler, authHandler, databaseHandler, jsonApiHandler, queue, jsonHandler, cache, actionDescriptorCollectionProvider, env, configuration, rabbitMqHandler, appConfig, nodeManagerHandler, scopedEncryptionHandler, abstractBackendModule, serviceProvider)
        {

            _webHostEnvironment = env;
            _logger = logger;
            _authHandler = authHandler;



        }

        [HttpGet("session/{token}")]
        public async Task<ActionResult<AuthModel>> GetSessionInformation(string token)
        {
            MethodDescriptor methodInfo = _webHostEnvironment.IsDevelopment() ? new MethodDescriptor { c = this.GetType().Name, m = MethodBase.GetCurrentMethod().Name } : null;

            AuthModel authModel = null;
            if (token != null)
            {
                var tmp = await _authHandler.GetSession(token);
                authModel = new AuthModel(tmp);
            }
            return authModel == null ? JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_FORBIDDEN, Detail = "forbidden" }
            }, HttpStatusCode.BadRequest, "an error occurred", "authModel == null", methodInfo) : Ok(authModel);
        }
        [AllowAnonymous()]
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] UserDataTransferModel authUserModel)
        {
            MethodDescriptor methodInfo = _webHostEnvironment.IsDevelopment() ? new MethodDescriptor { c = this.GetType().Name, m = MethodBase.GetCurrentMethod().Name } : null;
            AuthModel authModel = null;
            if (authUserModel != null)
            {
                if (this.ModelState.IsValid)
                {
                    var tmp = await _authHandler.Login(HttpContext, authUserModel);
                    if(tmp != null)
                    {
                        authModel = new AuthModel(tmp);
                    }
                }
            }
            AreaAttribute currentArea = this.GetArea();
            if (authModel != null)
            {
                return Ok(authModel).Cookie(HttpContext, "refresh_token", authModel.RefreshToken, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    HttpOnly = true,
                    Path = "/" + currentArea?.RouteValue.ToLower(),
                    Secure = true,
                    Expires = authModel.RefreshTokenExpires,

                });
            }
            else
            {
                return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_FORBIDDEN, Detail = "forbidden" }
            }, HttpStatusCode.Forbidden, "an error occurred", "authModel == null", methodInfo);
            }
        }
        [AllowAnonymous]
        [HttpGet("connection")]
        public async Task<ActionResult> Test()
        {
            return Ok();
        }
        [HttpPost("logout/{token}")]
        public async Task<ActionResult> Logout(string token)
        {
            MethodDescriptor methodInfo = _webHostEnvironment.IsDevelopment() ? new MethodDescriptor { c = this.GetType().Name, m = MethodBase.GetCurrentMethod().Name } : null;
            if (token != null)
            {
                bool response = await _authHandler.Logout(HttpContext, token);
                if (!response)
                {
                    return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_FORBIDDEN, Detail = "forbidden" }
            }, HttpStatusCode.Forbidden, "an error occurred", "authHttpHeaderKey == null", methodInfo);
                }
                else
                {
                    return Ok();
                }
            }
            else
            {
                return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_FORBIDDEN, Detail = "forbidden" }
            }, HttpStatusCode.Forbidden, "an error occurred", "authHttpHeaderKey == null", methodInfo);
            }
        }
        [HttpPost("refresh/{refresh_token}")]
        public async Task<ActionResult<AuthModel>> Refresh(string refresh_token)
        {
            MethodDescriptor methodInfo = _webHostEnvironment.IsDevelopment() ? new MethodDescriptor { c = this.GetType().Name, m = MethodBase.GetCurrentMethod().Name } : null;
            if (refresh_token != null)
            {
                if (CheckGuid(refresh_token))
                {
                    string token = HttpContext.GetRequestJWTFromHeader();
                    if (token != null)
                    {
                        var tmp = await _authHandler.Refresh(HttpContext, refresh_token, token);
                        AuthModel authModel = new AuthModel(tmp);
                        return Ok(authModel);
                    }
                }
                return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Detail = "bad request" }
            }, HttpStatusCode.BadRequest, "an error occurred", "CheckGuid(refresh_token) == false", methodInfo);
            }
            return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_FORBIDDEN, Detail = "forbidden" }
            }, HttpStatusCode.Forbidden, "an error occurred", "authHttpHeaderKey == null", methodInfo);
        }
        [HttpPost("validate")]
        public async Task<ActionResult> Validate([FromBody] AuthentificationTokenModel authentificationTokenModel)
        {
            MethodDescriptor methodInfo = _webHostEnvironment.IsDevelopment() ? new MethodDescriptor { c = this.GetType().Name, m = MethodBase.GetCurrentMethod().Name } : null;
            var response = await _authHandler.CheckLogin(HttpContext, authentificationTokenModel.Token);
            if (response.IsAuthorizatiOk)
            {
                return Ok();
            }
            return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_FORBIDDEN, Detail = "forbidden" }
            }, HttpStatusCode.Forbidden, "an error occurred", "authHttpHeaderKey == null", methodInfo);
        }

        [NonAction]
        public override AbstractBackendModule<AuthModel> GetConcreteModule()
        {
            throw new NotImplementedException();
        }
    }
}
