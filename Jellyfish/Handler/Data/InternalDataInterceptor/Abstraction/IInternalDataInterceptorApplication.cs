using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiFunction.Application.Model.Database.MySQL.Jellyfish;

namespace JellyFish.Handler.Data.InternalDataInterceptor.Abstraction
{
    public interface IInternalDataInterceptorApplication
    {
        public List<IInternalDataInterceptorApplicationInvoker> Invoker { get; }
        Task ReceiveMessage(params MessageDTO[] data);
        Task SendMessage(params MessageDTO[] data);
        Task CreateFriendRequest(params UserFriendshipRequestDTO[] data);
        Task ReceiveFriendRequest(params UserFriendshipRequestDTO[] data);
        Task ReceiveAcceptFriendRequest(UserDTO data);
        public void Add(IInternalDataInterceptorApplicationInvoker invoker);
        public void Remove(IInternalDataInterceptorApplicationInvoker invoker);
    }
}
