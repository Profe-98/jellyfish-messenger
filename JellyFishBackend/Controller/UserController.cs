﻿using Microsoft.AspNetCore.Mvc.Infrastructure;
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
using Microsoft.Extensions.DependencyInjection;
using WebApiFunction.Application.Model.Database.MySQL.Jellyfish;
using WebApiFunction.Application.Model.Database.MySQL;
using WebApiFunction.Web.AspNet.Controller;

namespace JellyFishBackend.Controller
{
    [Authorize(Policy = "Administrator")]
    public class UserController : AbstractController<UserModel>
    {
        private readonly IAuthHandler _auth;
        private readonly IConfiguration _configuration;
        private readonly IEncryptionHandler _encryptionHandler;

        public UserController(ILogger<CustomApiControllerBase<UserModel>> logger, IScopedVulnerablityHandler vulnerablityHandler, IMailHandler mailHandler, IAuthHandler authHandler, IScopedDatabaseHandler databaseHandler, IJsonApiDataHandler jsonApiHandler, ITaskSchedulerBackgroundServiceQueuer queue, IScopedJsonHandler jsonHandler, ICachingHandler cache, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, IWebHostEnvironment env, IConfiguration configuration, IRabbitMqHandler rabbitMqHandler, IAppconfig appConfig, INodeManagerHandler nodeManagerHandler, IScopedEncryptionHandler scopedEncryptionHandler, WebApiFunction.Application.Controller.Modules.IAbstractBackendModule<UserModel> abstractBackendModule, IServiceProvider serviceProvider) : 
            base(logger, vulnerablityHandler, mailHandler, authHandler, databaseHandler, jsonApiHandler, queue, jsonHandler, cache, actionDescriptorCollectionProvider, env, configuration, rabbitMqHandler, appConfig, nodeManagerHandler, scopedEncryptionHandler, abstractBackendModule, serviceProvider)
        {
            _auth = authHandler;
            _configuration = configuration;
            _encryptionHandler = scopedEncryptionHandler;
        }

        /// <summary>
        /// password/reset/request
        /// </summary>
        /// <param name="passwordResetDataTransferModel"></param>
        /// <returns></returns>
        /// <exception cref="HttpStatusException"></exception>
        [AllowAnonymous]
        [HttpPost("password/reset/request")]
        public async Task<ActionResult> UserPasswordResetRequest([FromBody] PasswordResetRequestDataTransferModel passwordResetDataTransferModel)
        {
            //method body umschreiben
            MethodDescriptor methodInfo = _webHostEnvironment.IsDevelopment() ? new MethodDescriptor { c = this.GetType().Name, m = MethodBase.GetCurrentMethod().Name } : null;

            string passwordResetUrl = _configuration.GetValue<string>("PasswordResetUrl");
            string fromMail = _configuration.GetValue<string>("NoReplyEmail");
            var timeExp = DateTime.Now.AddDays(BackendAPIDefinitionsProperties.PasswordResetExpInDays);
            var userSearchResponse = await _backendModule.Select(new UserModel { User = passwordResetDataTransferModel.EMail }, new UserModel { User = passwordResetDataTransferModel.EMail });
            if (!userSearchResponse.HasData)//user existiert bereits mit email
            {
                return JsonApiErrorResult(new List<ApiErrorModel> {
                            new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_FORBIDDEN, Detail = "forbidden" }
                        }, HttpStatusCode.Forbidden, "an error occurred", "unknown error", methodInfo);
            }

            UserModel userModel = userSearchResponse.FirstRow;
            if (!userModel.Active)//user bereits in db jedoch noch nicht aktiviert
            {
                return JsonApiErrorResult(new List<ApiErrorModel> {
                            new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_FORBIDDEN, Detail = "forbidden" }
                        }, HttpStatusCode.Forbidden, "an error occurred", "unknown error", methodInfo);
            }
            string base64 = _auth.EncodeJWT(new UserModel() { User = userModel.User, FirstName = userModel.FirstName, LastName = userModel.LastName }, timeExp, true);

            userModel.PasswordResetToken = base64;


            userModel.PasswordResetCode = userModel.GenerateCode(BackendAPIDefinitionsProperties.PasswordResetCodeLen);
            userModel.PasswordResetExpiresIn = timeExp;

            userModel.SendPasswordResetRequestMail(_mailHandler, fromMail, passwordResetUrl);
            var queryResponseFromUpdate = await _backendModule.Update(userModel, new UserModel { Uuid = userModel.Uuid });
            if (!queryResponseFromUpdate.HasSuccess)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, ApiErrorModel.ERROR_CODES.INTERNAL, "error while update the user");
            }
            return JsonApiResult(new List<ApiDataModel> { new ApiDataModel { Id = userModel.Uuid, Attributes = passwordResetDataTransferModel } }, HttpStatusCode.OK);
        }
        [AllowAnonymous]
        [HttpPost("password/reset/confirmation")]
        public async Task<ActionResult> UserPasswordResetConfirmation([FromBody] PasswordResetConfirmationCodeDataTransferModel passwordResetDataTransferModel)
        {
            //method body umschreiben
            MethodDescriptor methodInfo = _webHostEnvironment.IsDevelopment() ? new MethodDescriptor { c = this.GetType().Name, m = MethodBase.GetCurrentMethod().Name } : null;
            UserModel userModel = new UserModel();
            if (passwordResetDataTransferModel == null || passwordResetDataTransferModel.PasswordResetCode == null)
            {
                return JsonApiErrorResult(new List<ApiErrorModel> {
                                    new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_FORBIDDEN, Detail = "forbidden" }
                                }, HttpStatusCode.Forbidden, "an error occurred", "passwordResetDataTransferModel == null", methodInfo);
            }
            userModel.Active = true;
            userModel.PasswordResetCode = passwordResetDataTransferModel.PasswordResetCode;
            userModel.PasswordResetCodeConfirmation = false;

            var userSearchResponse = await _backendModule.Select(userModel, userModel);
            if (!userSearchResponse.HasData)
            {

                return JsonApiErrorResult(new List<ApiErrorModel> {
                                    new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_GONE, Detail = "gone" }
                                }, HttpStatusCode.Gone, "an error occurred", "action is still not longer available because, user is activated", methodInfo);
            }

            userModel = userSearchResponse.FirstRow;
            if (DateTime.Now >= userModel.PasswordResetExpiresIn)//token abgelaufen
            {
                return JsonApiErrorResult(new List<ApiErrorModel> {
                                    new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_FORBIDDEN, Detail = "forbidden" }
                                }, HttpStatusCode.Forbidden, "an error occurred", "DateTime.Now >= userModel.ActivationTokenExpires", methodInfo);
            }
            userModel.PasswordResetCodeConfirmation = true;
            var queryResponseFromUpdate = await _backendModule.Update(userModel, new UserModel { Uuid = userModel.Uuid });
            if (!queryResponseFromUpdate.HasSuccess)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, ApiErrorModel.ERROR_CODES.INTERNAL, "error while update the user");
            }


            return JsonApiResult(new List<ApiDataModel> { new ApiDataModel { Id = userModel.Uuid, Attributes = userModel } }, HttpStatusCode.OK);
        }

        [AllowAnonymous]
        [HttpPost("password/reset/{base64?}")]
        public async Task<ActionResult> UserPasswordReset(string? base64, [FromBody] PasswordResetDataTransferModel passwordResetDataTransferModel)
        {
            //method body umschreiben
            MethodDescriptor methodInfo = _webHostEnvironment.IsDevelopment() ? new MethodDescriptor { c = this.GetType().Name, m = MethodBase.GetCurrentMethod().Name } : null;
            UserModel userModel = new UserModel();
            userModel.Active = true;

            if(!String.IsNullOrEmpty(base64))
            {
                userModel.PasswordResetToken = base64;
            }
            else if(passwordResetDataTransferModel != null)
            {
                if(passwordResetDataTransferModel.PasswordResetCode != null)
                {
                    userModel.PasswordResetCode = passwordResetDataTransferModel.PasswordResetCode;
                    userModel.PasswordResetCodeConfirmation = true;
                }

            }
            else
            {
                return JsonApiErrorResult(new List<ApiErrorModel> {
                                    new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_FORBIDDEN, Detail = "forbidden" }
                                }, HttpStatusCode.Forbidden, "an error occurred", "passwordResetDataTransferModel == null", methodInfo);
            }

            var userSearchResponse = await _backendModule.Select(userModel, userModel);
            if (!userSearchResponse.HasData)
            {

                return JsonApiErrorResult(new List<ApiErrorModel> {
                                    new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_GONE, Detail = "gone" }
                                }, HttpStatusCode.Gone, "an error occurred", "action is still not longer available because, user is activated", methodInfo);
            }

            userModel = userSearchResponse.FirstRow;
            if (DateTime.Now >= userModel.PasswordResetExpiresIn)//token abgelaufen
            {
                return JsonApiErrorResult(new List<ApiErrorModel> {
                                    new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_FORBIDDEN, Detail = "forbidden" }
                                }, HttpStatusCode.Forbidden, "an error occurred", "DateTime.Now >= userModel.ActivationTokenExpires", methodInfo);
            }

            userModel.Password = await _encryptionHandler.MD5Async(passwordResetDataTransferModel.Password);
            userModel.PasswordResetCode = null;
            userModel.PasswordResetToken = null;
            var queryResponseFromUpdate = await _backendModule.Update(userModel, new UserModel { Uuid = userModel.Uuid });
            if (queryResponseFromUpdate.HasSuccess)
            {
                string fromMail = _configuration.GetValue<string>("NoReplyEmail");
                string name = _configuration.GetValue<string>("WebPageName");
                userModel.SendPasswordResetComplededMail(_mailHandler, fromMail, name);
                return JsonApiResult(new List<ApiDataModel> { new ApiDataModel { Id = userModel.Uuid, Attributes = userModel } }, HttpStatusCode.OK);
            }
            else
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, ApiErrorModel.ERROR_CODES.INTERNAL, "error while update the user");
            }
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> RegisterFrontEndUser([FromBody] RegisterDataTransferModel registerModel)
        {
            MethodDescriptor methodInfo = _webHostEnvironment.IsDevelopment() ? new MethodDescriptor { c = this.GetType().Name, m = MethodBase.GetCurrentMethod().Name } : null;

            string registrationActivationUrl = _configuration.GetValue<string>("RegistrationActivationUrl");
            string fromMail = _configuration.GetValue<string>("NoReplyEmail");
            var timeExp = DateTime.Now.AddDays(BackendAPIDefinitionsProperties.RegisterActivationExpInDays);
            var userSearchResponse = await _backendModule.Select(new UserModel { User = registerModel.EMail }, new UserModel { User = registerModel.EMail });
            if (userSearchResponse.HasData)//user existiert bereits mit email
            {
                if (!userSearchResponse.FirstRow.Active)//user bereits in db jedoch noch nicht aktiviert
                {
                    if (!registerModel.ResendActivationCode)
                    {

                        return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_GONE, Detail = "gone" }
            }, HttpStatusCode.Gone, "an error occurred", "action is still not longer available because, user must be activate", methodInfo);
                    }
                    else
                    {
                        UserModel userModel = userSearchResponse.FirstRow;
                        userModel.SetRegisterDataTransferModel(registerModel);

                        string actCode = userModel.GenerateCode(BackendAPIDefinitionsProperties.RegisterActivationCodeLen);
                        string base64 = _auth.EncodeJWT(new UserModel() { User = registerModel.EMail, FirstName = registerModel.FirstName, LastName = registerModel.LastName }, timeExp, true);

                        userModel.ActivationCode = actCode;
                        userModel.ActivationToken = base64;
                        userModel.ActivationTokenExpires = timeExp;

                        userModel.SendActivationMail(_mailHandler, fromMail, registrationActivationUrl);
                        var queryResponseFromUpdate = await _backendModule.Update(userModel, new UserModel { Uuid = userModel.Uuid });
                        if (queryResponseFromUpdate.HasSuccess)
                        {
                            return JsonApiResult(new List<ApiDataModel> { new ApiDataModel { Id = userModel.Uuid, Attributes = registerModel } }, HttpStatusCode.OK);
                        }
                        else
                        {
                            throw new HttpStatusException(HttpStatusCode.InternalServerError, ApiErrorModel.ERROR_CODES.INTERNAL, "error while update the user");
                        }
                    }
                }

            }
            else//user existert nicht
            {
                DbTransaction transaction = await _backendModule.CreateTransaction();


                UserModel userModel = new UserModel(registerModel);
                string actCode = userModel.GenerateCode(BackendAPIDefinitionsProperties.RegisterActivationCodeLen);

                string base64 = _auth.EncodeJWT(new UserModel() { User = registerModel.EMail, FirstName = registerModel.FirstName, LastName = registerModel.LastName }, timeExp, true);

                userModel.ActivationCode = actCode;
                userModel.ActivationToken = base64;
                userModel.ActivationTokenExpires = timeExp;
                userModel.UserTypeUuid = Const.GetConst().DefaultUserType;
                userModel.Active = false;
                userModel.Password = await _encryptionHandler.MD5Async(userModel.Password);
                userModel.Password = userModel.Password.ToLower();
                var insertResponse = await _backendModule.Insert(userModel, transaction);
                if (insertResponse.HasErrors )
                {
                    _backendModule.Rollback(transaction);
                }
                else
                {
                    _backendModule.Commit(transaction);
                    userModel.SendActivationMail(_mailHandler, fromMail, registrationActivationUrl);
                    registerModel.ActivateTokenBase64 = base64;
                    return JsonApiResult(new List<ApiDataModel> { new ApiDataModel { Id = new Guid((string)insertResponse.LastInsertedId), Attributes = registerModel } }, HttpStatusCode.OK);
                }

            }
            return JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_FORBIDDEN, Detail = "forbidden" }
            }, HttpStatusCode.Forbidden, "an error occurred", "unknown error", methodInfo);
        }

        [AllowAnonymous]
        [HttpPost("activation/{base64}")]
        public async Task<ActionResult> Activation(string base64, [FromBody] UserActivationDataTransferModel userActivationDataTransferModel)
        {
            MethodDescriptor methodInfo = _webHostEnvironment.IsDevelopment() ? new MethodDescriptor { c = this.GetType().Name, m = MethodBase.GetCurrentMethod().Name } : null;
            UserModel userModel = new UserModel();
            userModel.Active = false;
            userModel.ActivationToken = base64;
            userModel.ActivationCode = userActivationDataTransferModel.ActivationCode;

            var userSearchResponse = await _backendModule.Select(userModel, userModel);
            if (!userSearchResponse.HasData)
            {
                return JsonApiErrorResult(new List<ApiErrorModel> {
                            new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_FORBIDDEN, Detail = "forbidden" }
                        }, HttpStatusCode.Forbidden, "an error occurred", "userSearchResponse.HasData == false", methodInfo);

            }
            JWTModel data = _auth.DecodeJWT(base64);
            if (data == null)
            {

                return JsonApiErrorResult(new List<ApiErrorModel> {
                            new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, Detail = "bad request" }
                        }, HttpStatusCode.BadGateway, "an error occurred", "JWTModel is null", methodInfo);
            }
            userModel = userSearchResponse.FirstRow;
            userModel.ActivationTokenExpires = (new UserModel(data.UserModel)).ActivationTokenExpires;
            if (DateTime.Now >= userModel.ActivationTokenExpires)//token abgelaufen
            {
                return JsonApiErrorResult(new List<ApiErrorModel> {
                            new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_FORBIDDEN, Detail = "forbidden" }
                        }, HttpStatusCode.Forbidden, "an error occurred", "DateTime.Now >= userModel.ActivationTokenExpires", methodInfo);
            }
            //token nicht abgelaufen
            userModel.Active = true;
            var queryResponseFromUpdate = await _backendModule.Update(userModel, new UserModel { Uuid = userModel.Uuid });
            if (!queryResponseFromUpdate.HasSuccess)
            {
                throw new HttpStatusException(HttpStatusCode.InternalServerError, ApiErrorModel.ERROR_CODES.INTERNAL, "error while update the user");
            }
            var bM = _serviceProvider.GetModule<UserRelationToRoleModel>();
            var roleInsertRepsonse = await bM.Insert(new UserRelationToRoleModel { RoleUuid = Guid.Parse("8937df9c-fbef-11ed-8f81-7085c294413b"), UserUuid = userModel.Uuid });
            string fromMail = _configuration.GetValue<string>("NoReplyEmail");
            string name = _configuration.GetValue<string>("WebPageName");
            userModel.SendActivationComplededMail(_mailHandler, fromMail, name);
            return JsonApiResult(new List<ApiDataModel> { new ApiDataModel { Id = userModel.Uuid, Attributes = userModel } }, HttpStatusCode.OK);

        }
        [Authorize(Policy = "Administrator")]
        [HttpGet("test")]
        public async Task<ActionResult<List<UserModel>>> GetAllUsersDapperTest()
        {
            var data = await ((UserModule)_backendModule).GetAllUsers();
            if (data == null || data.Count() == 0)
                return NotFound();
            return Ok(data.ToList());
        }

        public override WebApiFunction.Application.Controller.Modules.AbstractBackendModule<UserModel> GetConcreteModule()
        {
            return ((UserModule)_backendModule);
        }

    }
}
