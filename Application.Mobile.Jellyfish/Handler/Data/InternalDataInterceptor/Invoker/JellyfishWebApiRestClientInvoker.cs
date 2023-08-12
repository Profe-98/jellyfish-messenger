using Application.Mobile.Jellyfish.Controls;
using Application.Mobile.Jellyfish.Data.WebApi;
using Application.Mobile.Jellyfish.Handler.Backend.Communication.WebApi;
using Application.Mobile.Jellyfish.Handler.Data.InternalDataInterceptor.Abstraction;
using Application.Shared.Kernel.Application.Model.DataTransferObject.ConcreteImplementation.Jellyfish;

namespace Application.Mobile.Jellyfish.Handler.Data.InternalDataInterceptor.Invoker
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
