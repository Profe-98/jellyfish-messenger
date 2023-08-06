using JellyFish.Controls;
using JellyFish.Data.WebApi;
using JellyFish.Handler.Backend.Communication.WebApi;
using JellyFish.Handler.Data.InternalDataInterceptor.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiFunction.Application.Model.Database.MySQL.Jellyfish;
using WebApiFunction.Application.Model.Database.MySQL.Jellyfish.DataTransferObject;
using WebApiFunction.Application.Model.DataTransferObject.Jellyfish;

namespace JellyFish.Handler.Data.InternalDataInterceptor.Invoker
{
    public class JellyfishWebApiRestClientInvoker : IInternalDataInterceptorApplicationInvoker
    {
        private readonly JellyfishWebApiRestClient _jellyfishWebApiRestClient;
        public JellyfishWebApiRestClientInvoker(JellyfishWebApiRestClient jellyfishWebApiRestClient)
        {
            _jellyfishWebApiRestClient = jellyfishWebApiRestClient;
        }
        public Task CreateFriendRequest(params UserFriendshipRequestDTO[] data)
        {
            throw new NotImplementedException();
        }

        public Task ReceiveAcceptFriendRequest(params UserDTO[] data)
        {
            throw new NotImplementedException();
        }

        public Task ReceiveFriendRequest(params UserFriendshipRequestDTO[] data)
        {
            throw new NotImplementedException();
        }

        public Task ReceiveMessage(params MessageDTO[] data)
        {
            List<MessageDTO> messages =  data.ToList();
            messages.ForEach(message =>
            {
                MainThread.InvokeOnMainThreadAsync(() =>
                {

                    NotificationHandler.ToastNotify(message.Text);
                });
            });
            return Task.CompletedTask;
        }

        public Task SendMessage(params MessageDTO[] data)
        {
            throw new NotImplementedException();
        }


        public async Task<WebApiHttpRequestResponseModel<UserDTO>> GetOwnProfile(CancellationToken cancellationToken)
        {
            var result =await _jellyfishWebApiRestClient.GetOwnProfile(cancellationToken);
            return result;
        }

        public async Task<WebApiHttpRequestResponseModel<UserFriendshipUserModelDTO>> GetFriendshipRequests(CancellationToken cancellationToken)
        {
            var result =await _jellyfishWebApiRestClient.GetFriendshipRequests(cancellationToken);
            return result;
        }

        public async Task<WebApiHttpRequestResponseModel<UserDTO>> GetFriends(CancellationToken cancellationToken)
        {
            var result = await _jellyfishWebApiRestClient.GetFriends(cancellationToken);
            return result;
        }
        public async Task<WebApiHttpRequestResponseModel<UserDTO>> AcceptFriendshipRequests(Guid requestUuid, CancellationToken cancellationToken)
        {

            var result = await _jellyfishWebApiRestClient.AcceptFriendshipRequests(requestUuid,cancellationToken);
            return result;
        }
        public async Task<WebApiHttpRequestResponseModel<UserDTO>> SearchUser(string searchName, CancellationToken cancellationToken)
        {
            var result = await _jellyfishWebApiRestClient.SearchUser(searchName, cancellationToken);
            return result;
        }
    }
}
