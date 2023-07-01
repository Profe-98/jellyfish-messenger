using JellyFish.Data.WebApi;
using JellyFish.Handler.Backend.Communication.WebApi.Abstraction;
using JellyFish.Handler.Data;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using WebApiFunction.Application.Model.Database.MySQL.Jellyfish;
using WebApiFunction.Application.Model.DataTransferObject.Helix.Frontend.Transfer;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Data.Web.Api.Abstractions;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiFunction.Web.Authentification;

namespace JellyFish.Handler.Backend.Communication.WebApi
{
    public class WebApiRestClient : AbstractRestClient
    {
        private bool _isInit;
        public AuthModel CurrentWebApiSession = null;
        public string BaseUrl { get; set; }
        public string LoginSessionEndpoint { get; private set; }
        public string LogoutSessionEndpoint { get; private set; }
        public string RefreshSessionEndpoint { get; private set; }
        public string ValidateSessionEndpoint { get; private set; }
        public string ConnectionTestEndpoint { get; private set; }
        public bool RefreshLogin { get; private set; }
        public uint MaxRequestRetries { get; private set; } = 3;
        public string User { get; private set; }
        public string Password { get; private set; }
        public WebApiRestClient()
        {
        }
        public void Init(string baseUrl,string loginSessionEndpoint, string logoutSessionEndpoint, string validateSessionEndpoint, string refreshSessionEndpoint,string connectionTestEndpoint)
        {
            BaseUrl = baseUrl;
                LoginSessionEndpoint = BuildUrl(loginSessionEndpoint);
                LogoutSessionEndpoint = BuildUrl(logoutSessionEndpoint);
                RefreshSessionEndpoint = BuildUrl(refreshSessionEndpoint);
            ValidateSessionEndpoint = BuildUrl(validateSessionEndpoint);
            ConnectionTestEndpoint = BuildUrl(connectionTestEndpoint);
            _isInit =true;   
        }
        private void SetCredentials(string user, string password, bool sessionAutoRefresh = true)
        {
            User = user;
            Password = password;
            RefreshLogin = sessionAutoRefresh;
        }
        public string BuildUrl(string endPoint)
        {
            return BaseUrl + endPoint;
        }
        public bool IsInit
        {
            get { return _isInit; }
        }
        public virtual async Task<AuthModel> Authentificate(string userName, string password, CancellationToken cancellationToken)
        {
            if (!IsInit)
            {
                throw new InvalidOperationException("please initialize the handler correctly via method: " + nameof(Init) + "");
            }
            SetCredentials(userName, password);
            var postData = new { user = userName, password = password };
            RestResponse resp = await Request<AuthModel>(LoginSessionEndpoint, Method.Post, cancellationToken, postData, null, null, true);
            if (resp != null)
            {
                AuthModel response;
                ExtendedJsonHandler jsonHandler = new ExtendedJsonHandler();
                response = jsonHandler.JsonDeserialize<AuthModel>(resp.Content);
                return response;
            }
            return null;
        }
        public async Task<WebApiHttpRequestResponseModel<WebApiFunction.Application.Model.Database.MySQL.Jellyfish.UserModel>> Activate(string base64Token,UserActivationDataTransferModel userActivationDataTransferModel, CancellationToken cancellationToken)
        {
            if (!IsInit)
            {
                throw new InvalidOperationException("please initialize the handler correctly via method: " + nameof(Init) + "");
            }
            var resp = await Request<object, WebApiFunction.Application.Model.Database.MySQL.Jellyfish.UserModel>(WebApiEndpointStruct.RegisterActivationEndpoint+"/"+base64Token, Method.Post, cancellationToken, userActivationDataTransferModel, null, null, true);
            if (resp != null)
            {
                return resp;
            }
            return null;
        }
        public async Task<WebApiHttpRequestResponseModel<RegisterDataTransferModel>> Register(RegisterDataTransferModel registerDataTransferModel, CancellationToken cancellationToken)
        {
            if (!IsInit)
            {
                throw new InvalidOperationException("please initialize the handler correctly via method: " + nameof(Init) + "");
            }
            var resp = await Request<object, RegisterDataTransferModel>(WebApiEndpointStruct.RegisterEndpoint, Method.Post, cancellationToken, registerDataTransferModel, null, null, true);
            if (resp != null)
            {
                return resp;
            }
            return null;
        }
        /// <summary>
        /// Sends the confirmation secure code from users mail to backend to confirm the password request action
        /// </summary>
        /// <param name="passwordResetRequestDataTransferModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<WebApiHttpRequestResponseModel<WebApiResponseModel>> ResetPasswordConfirmation(PasswordResetConfirmationCodeDataTransferModel passwordResetRequestDataTransferModel, CancellationToken cancellationToken)
        {
            if (!IsInit)
            {
                throw new InvalidOperationException("please initialize the handler correctly via method: " + nameof(Init) + "");
            }
            var resp = await Request<object, WebApiResponseModel>(WebApiEndpointStruct.UserPasswordCodeConfirmationEndpoint, Method.Post, cancellationToken, passwordResetRequestDataTransferModel, null, null, true);
            if (resp != null)
            {
                return resp;
            }
            return null;
        }
        /// <summary>
        /// The action that starts the password reset action
        /// </summary>
        /// <param name="passwordResetRequestDataTransferModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<WebApiHttpRequestResponseModel<WebApiResponseModel>> ResetPasswordRequest(PasswordResetRequestDataTransferModel passwordResetRequestDataTransferModel, CancellationToken cancellationToken)
        {
            if (!IsInit)
            {
                throw new InvalidOperationException("please initialize the handler correctly via method: " + nameof(Init) + "");
            }
            var resp = await Request<object, WebApiResponseModel>(WebApiEndpointStruct.UserPasswordResetRequestEndpoint, Method.Post, cancellationToken, passwordResetRequestDataTransferModel, null, null, true);
            if (resp != null)
            {
                return resp;
            }
            return null;
        }
        /// <summary>
        /// Reset the password, previously the password reset must be requested and confirmed by secure code 
        /// </summary>
        /// <param name="passwordResetDataTransferModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<WebApiHttpRequestResponseModel<WebApiResponseModel>> ResetPassword(PasswordResetDataTransferModel passwordResetDataTransferModel, CancellationToken cancellationToken)
        {
            if (!IsInit)
            {
                throw new InvalidOperationException("please initialize the handler correctly via method: " + nameof(Init) + "");
            }
            var resp = await Request<object, WebApiResponseModel>(WebApiEndpointStruct.UserPasswordResetEndpoint, Method.Post, cancellationToken, passwordResetDataTransferModel, null, null, true);
            if (resp != null)
            {
                return resp;
            }
            return null;
        }
        public string BuildRefreshTokenUrl()
        {
            if (!IsInit)
            {
                throw new InvalidOperationException("please initialize the handler correctly via method: " + nameof(Init) + "");
            }
            return RefreshSessionEndpoint + CurrentWebApiSession.RefreshToken;
        }
        public async Task<AuthModel> RefreshToken(CancellationToken cancellationToken)
        {
            if (!IsInit)
            {
                throw new InvalidOperationException("please initialize the handler correctly via method: " + nameof(Init) + "");
            }
            if (string.IsNullOrEmpty(User) || string.IsNullOrEmpty(Password))
            {
                return null;
            }
            string refreshTokenUrl = BuildRefreshTokenUrl();
            var resp = await Request<object, AuthModel>(RefreshSessionEndpoint, Method.Post, cancellationToken, null, null, null,true);
            if (resp != null)
            {
                var d = resp.ApiResponseDeserialized;
                CurrentWebApiSession =d.data.First().attributes;
                return d.data.First().attributes;
            }
            return null;
        }
        public async Task<bool> ConnectionTest(CancellationToken cancellationToken)
        {
            if (!IsInit)
            {
                throw new InvalidOperationException("please initialize the handler correctly via method: " + nameof(Init) + "");
            }


            var resp = await Request<object>(ConnectionTestEndpoint, Method.Get, cancellationToken, null, null, null, true);
            if (resp != null)
            {

                return resp.IsSuccessStatusCode;
            }
            return false;
        }
        public async Task<bool> ValidateToken(CancellationToken cancellationToken,string token =null)
        {
            if (!IsInit)
            {
                throw new InvalidOperationException("please initialize the handler correctly via method: " + nameof(Init) + "");
            }
            if (string.IsNullOrEmpty(token) && CurrentWebApiSession==null)
            {
                return false;
            }
            if(token==null&& CurrentWebApiSession.Token!=null)
            {
                token = CurrentWebApiSession.Token; 
            }
            var body = new AuthentificationTokenModel { Token = token };
            var resp = await Request<object>(ValidateSessionEndpoint, Method.Post, cancellationToken, body, null, null, true);
            if (resp != null)
            {

                return resp.IsSuccessStatusCode;
            }
            return false;
        }

        public async Task<System.Net.HttpStatusCode> Logout(CancellationToken cancellationToken)
        {
            if (!IsInit)
            {
                throw new InvalidOperationException("please initialize the handler correctly via method: " + nameof(Init) + "");
            }
            if (CurrentWebApiSession != null && !CurrentWebApiSession.IsTokenExpired && !CurrentWebApiSession.IsRefreshTokenExpired)
            {
                var headers = new List<KeyValuePair<string,string>>();

                AppHeaderWithSessionToken(ref headers);
                var requ = CreateRequest(LogoutSessionEndpoint,Method.Post,ContentType.Json,null,null, headers);
                var resp = await Send(requ,cancellationToken);
                return resp.StatusCode;
            }
            return System.Net.HttpStatusCode.Forbidden;
        }
        public void AppHeaderWithSessionToken(ref List<KeyValuePair<string,string>> headers)
        {
            if (!IsInit)
            {
                throw new InvalidOperationException("please initialize the handler correctly via method: " + nameof(Init) + "");
            }
            if (CurrentWebApiSession != null && !CurrentWebApiSession.IsTokenExpired && !CurrentWebApiSession.IsRefreshTokenExpired)
            {
                string bearerStr = "Bearer " + CurrentWebApiSession.Token + "";
                headers.Add(new KeyValuePair<string, string>("Authorization", bearerStr));
            }
        }
        public async Task<RestResponse> Request<T2>(string url, RestSharp.Method method, CancellationToken cancellationToken, object bodyObject = null, List<KeyValuePair<string, string>> query = null, List<KeyValuePair<string, string>> headers = null,bool donttryagain = true)
            where T2 : class, new()
        {
            if (!IsInit)
            {
                throw new InvalidOperationException("please initialize the handler correctly via method: " + nameof(Init) + "");
            }
            string body = null;
            ExtendedJsonHandler jsonHandler = new ExtendedJsonHandler();
            if (bodyObject != null)
            {
                body = jsonHandler.JsonSerialize(bodyObject);

            }
            int retries = 0;
            bool reauth = false;
            RestResponse response = null;   
            do
            {
                AppHeaderWithSessionToken(ref headers);
                url = url.ToLower().StartsWith("http") ? url : BuildUrl(url);
                var requ = CreateRequest(url, method, ContentType.Json, body, query, headers);
                response = await Send(requ, cancellationToken);
                if (!response.IsSuccessStatusCode )
                {
                    if(this.RefreshLogin)
                    {
                        if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            if(!reauth)
                            {
                                var responseAuth = await Authentificate(User,Password,cancellationToken);
                                if (responseAuth != null)
                                {
                                    CurrentWebApiSession = responseAuth;


                                }
                                reauth = true;
                            }
                            else//da bereits die erste auth fehlgeschlagen ist werden die nächsten folgen d.h. do-while abort
                            {
                                donttryagain = true;    
                            }
                        }
                        
                    }
                    if (!reauth)
                    {
                        if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.Unauthorized)
                            donttryagain = true;
                    }

                    retries++;
                }
                else
                {
                    donttryagain = true;
                }
            }
            while ((retries < MaxRequestRetries && !donttryagain) || ((response!=null&&!response.IsSuccessStatusCode) && !donttryagain));
            return response;
        }
        public async Task<WebApiHttpRequestResponseModel<T>> Request<T2,T>(string url,RestSharp.Method method, CancellationToken cancellationToken, T2 bodyObject = default, List<KeyValuePair<string, string>> query = null, List<KeyValuePair<string, string>> headers = null,bool donttryagain = true)
        where T : class
            where T2 : class,new()
        {
            ExtendedJsonHandler jsonHandler = new ExtendedJsonHandler();
            WebApiHttpRequestResponseModel<T> responseModel = new WebApiHttpRequestResponseModel<T>() ;
            var resp = await Request<T2>(url,method,cancellationToken,bodyObject,query,headers, donttryagain);
            responseModel.DefaultResponse = resp;
            if (resp != null && resp.Content != null)
            {
                string responseJson = resp.Content;

                var baseData = jsonHandler.JsonDeserialize<WebApiResponseModel.RootObject>(responseJson);
                WebApiResponseModel.RootObject<T> responseModelData = new WebApiResponseModel.RootObject<T>();
                var respp = JsonNode.Parse(responseJson).AsObject();
                if (respp != null && respp.ContainsKey("data"))
                {
                    var tttp= respp["data"];
                    var typ = tttp.GetType();
                    if (typ == typeof(JsonArray))
                    {
                        responseModelData.data = tttp.Deserialize<List<WebApiResponseModel.Data<T>>>().ToArray();
                    }
                    else if (typ == typeof(JsonObject))
                    {

                        var tt = tttp.Deserialize<WebApiResponseModel.Data<T>>();
                        responseModelData.data = new List<WebApiResponseModel.Data<T>>() { tt }.ToArray();
                    }
                    else
                    {
                        responseModelData.data = null;
                    }
                }
                responseModelData.meta = baseData.meta;
                responseModelData.jsonapi = baseData.jsonapi;
                responseModel.ApiResponseDeserialized = responseModelData;
            }
            return responseModel;
        }
    }
}
