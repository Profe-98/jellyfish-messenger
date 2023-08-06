using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiFunction.Application.Model.Database.MySQL.Jellyfish;
using WebApiFunction.Application.Model.Database.MySQL.Jellyfish.DataTransferObject;

namespace JellyFish.Handler.Data.InternalDataInterceptor.Abstraction
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
