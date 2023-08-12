using Application.Shared.Kernel.Application.Model.DataTransferObject.ConcreteImplementation.Jellyfish;

namespace Application.Mobile.Jellyfish.Handler.Data.InternalDataInterceptor.Abstraction
{
    public interface IInternalDataInterceptorApplicationInvoker
    {
        Task ReceiveMessage(params MessageDTO[] data);
        Task SendMessage(params MessageDTO[] data);
        Task CreateFriendRequest(params UserFriendshipRequestDTO[] data);
        Task ReceiveFriendRequest(params UserFriendshipRequestDTO[] data);
        Task ReceiveAcceptFriendRequest(params UserDTO[] data);
    }
}
