using Application.Mobile.Jellyfish.Data.WebApi;
using Application.Shared.Kernel.Application.Model.DataTransferObject.ConcreteImplementation.Jellyfish;

namespace Application.Mobile.Jellyfish.Handler.Data.InternalDataInterceptor.Abstraction
{
    public interface IInternalDataInterceptorApplication
    {
        public List<IInternalDataInterceptorApplicationInvoker> Invoker { get; }
        public Task<InternalDataInterceptorApplicationInvokeResponseModel> ReceiveMessage(params MessageDTO[] data);
        public Task<InternalDataInterceptorApplicationInvokeResponseModel> SendMessage(params MessageDTO[] data);
        public Task<InternalDataInterceptorApplicationInvokeResponseModel> CreateFriendRequest(params UserFriendshipRequestDTO[] data);
        public Task<InternalDataInterceptorApplicationInvokeResponseModel> ReceiveFriendRequest(params UserFriendshipRequestDTO[] data);
        public Task<InternalDataInterceptorApplicationInvokeResponseModel> ReceiveAcceptFriendRequest(params UserDTO[] data);

        public Task<UserDTO> GetOwnProfile(CancellationToken cancellationToken);
        public Task<List<UserFriendshipUserModelDTO>> GetFriendshipRequests(CancellationToken cancellationToken);
        public Task<WebApiHttpRequestResponseModel<UserDTO>> SearchUser(string searchUser,CancellationToken cancellationToken);
        public Task<WebApiHttpRequestResponseModel<UserDTO>> AcceptFriendRequest(Guid requestUuid,CancellationToken cancellationToken);
        public void Add(IInternalDataInterceptorApplicationInvoker invoker);
        public void Remove(IInternalDataInterceptorApplicationInvoker invoker);
        public IInternalDataInterceptorApplicationInvoker Get<T>() where T: IInternalDataInterceptorApplicationInvoker;
    }
}
