using JellyFish.Handler.Data.InternalDataInterceptor.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiFunction.Application.Model.Database.MySQL.Jellyfish;

namespace JellyFish.Handler.Data.InternalDataInterceptor.Invoker.Notification
{
    public class PlatformIndependentNotificationInvoker : IInternalDataInterceptorApplicationInvoker
    {
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
            throw new NotImplementedException();
        }

        public Task SendMessage(params MessageDTO[] data)
        {
            throw new NotImplementedException();
        }
    }
}
