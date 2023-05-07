using JellyFish.Handler.Backend.Communication.WebApi.Abstraction;
using JellyFish.Handler.Data;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WebApiFunction.Application.Model.Database.MySql.Entity;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Data.Web.Api.Abstractions;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;

namespace JellyFish.Handler.Backend.Communication.WebApi
{
    public class WebApiRestClient : AbstractRestClient
    {
        public AuthModel CurrentWebApiSession = null;
        public string LoginSessionEndpoint { get; private set; }
        public string LogoutSessionEndpoint { get; private set; }
        public string RefreshSessionEndpoint { get; private set; }
        public bool RefreshLogin { get; private set; }
        public string User { get; private set; }
        public string Password { get; private set; }
        public WebApiRestClient(string user,string password,string loginSessionEndpoint, string logoutSessionEndpoint,string refreshSessionEndpoint,bool sessionAutoRefresh  =true)
        {
            LoginSessionEndpoint = loginSessionEndpoint;
            LogoutSessionEndpoint = logoutSessionEndpoint;
            RefreshSessionEndpoint = refreshSessionEndpoint;
            User = user;
            Password = password;
            RefreshLogin = sessionAutoRefresh;
        }
        public async Task<AuthModel> Authentificate(CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(User) || string.IsNullOrEmpty(Password))
            {
                return null;
            }
            var postData = new { user = User, password = Password };
            var resp = await Request<object, AuthModel>(LoginSessionEndpoint, Method.Post, cancellationToken, postData, null, null);
            if (resp != null)
            {
                CurrentWebApiSession = resp;
                return resp;
            }
            return null;
        }
        public string BuildRefreshTokenUrl()
        {
            return RefreshSessionEndpoint + CurrentWebApiSession.RefreshToken;
        }
        public async Task<AuthModel> RefreshToken(CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(User) || string.IsNullOrEmpty(Password))
            {
                return null;
            }
            string refreshTokenUrl = BuildRefreshTokenUrl();
            var resp = await Request<object, AuthModel>(RefreshSessionEndpoint, Method.Post, cancellationToken, null, null, null);
            if (resp != null)
            {
                CurrentWebApiSession = resp;
                return resp;
            }
            return null;
        }

        public async Task<System.Net.HttpStatusCode> Logout(CancellationToken cancellationToken)
        {
            if(CurrentWebApiSession != null && !CurrentWebApiSession.IsTokenExpired && !CurrentWebApiSession.IsRefreshTokenExpired)
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
            if (CurrentWebApiSession != null && !CurrentWebApiSession.IsTokenExpired && !CurrentWebApiSession.IsRefreshTokenExpired)
            {
                string bearerStr = "Bearer " + CurrentWebApiSession.Token + "";
                headers.Add(new KeyValuePair<string, string>("Authorization", bearerStr));
            }
        }
        public async Task<T> Request<T2,T>(string url,RestSharp.Method method, CancellationToken cancellationToken, T2 bodyObject = default, List<KeyValuePair<string, string>> query = null, List<KeyValuePair<string, string>> headers = null)
        where T :class,new()
            where T2 : class,new()
        {
            T responseModel = default;
            string body = null;
            ExtendedJsonHandler jsonHandler = new ExtendedJsonHandler();
            if (bodyObject != null)
            {
                body = jsonHandler.JsonSerialize(bodyObject);

            }
            AppHeaderWithSessionToken(ref headers);
            var requ = CreateRequest(url, method, ContentType.Json, body,query,headers);
            var resp = await Send(requ, cancellationToken);
            if(resp != null && resp.Content != null)
            {
                string responseJson = resp.Content;
                responseModel = jsonHandler.JsonDeserialize<T>(responseJson);
            }
            return responseModel;
        }
    }
}
