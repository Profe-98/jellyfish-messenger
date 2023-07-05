using JellyFish.Data.WebApi;
using JellyFish.Handler.AppConfig;
using JellyFish.Handler.Backend.Communication.SignalR;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiFunction.Application.Model.Database.MySQL.Jellyfish;
using WebApiFunction.Application.Model.DataTransferObject.Jellyfish;

namespace JellyFish.Handler.Backend.Communication.WebApi
{
    public class JellyfishWebApiRestClient : WebApiRestClient
    {
        public struct JellyfishEndpoints
        {
            public const string GetFriendEndpoint = "/user/friends";
            public const string SearchUserEndpoint = "/user/search";
        }
        private readonly ApplicationConfigHandler _applicationConfigHandler;

        public JellyfishWebApiRestClient(ApplicationConfigHandler applicationConfigHandler):base()
        {
            _applicationConfigHandler = applicationConfigHandler;   
        }
        public override RestRequest CreateRequest(string urlOrEndpoint, Method httpMethod, ContentType contentType, string body = null, List<KeyValuePair<string, string>> query = null, List<KeyValuePair<string, string>> headerValues = null)
        {
            var baseRequ = base.CreateRequest(urlOrEndpoint, httpMethod, contentType, body, query, headerValues);
            
            return baseRequ;
        }
        public override Task<AuthModel> Authentificate(string userName, string password, CancellationToken cancellationToken)
        {
            var result =base.Authentificate(userName, password, cancellationToken);
            return result;
        }
        public async Task<WebApiHttpRequestResponseModel<UserDTO>> GetFriends(CancellationToken cancellationToken)
        {
            var url = BuildUrl(JellyfishEndpoints.GetFriendEndpoint);
            var result = await this.Request<UserDTO, object>(url: url, method: RestSharp.Method.Get, cancellationToken: cancellationToken, null, null, null, false);

            return result;
        }
        public async Task<WebApiHttpRequestResponseModel<UserDTO>> SearchUser(string searchName,CancellationToken cancellationToken)
        {
            var url = BuildUrl(JellyfishEndpoints.GetFriendEndpoint);
            var result = await this.Request<UserDTO, UserSearchDTO>(url: url, method: RestSharp.Method.Get, cancellationToken: cancellationToken, new UserSearchDTO { SearchUser = searchName}, null, null, false);

            return result;
        }

    }
}
