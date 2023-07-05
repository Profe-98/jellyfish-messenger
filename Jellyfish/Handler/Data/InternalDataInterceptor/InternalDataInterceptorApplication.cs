using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JellyFish.Handler.Data.InternalDataInterceptor.Abstraction;
using WebApiFunction.Application.Model.Database.MySQL.Jellyfish;

namespace JellyFish.Handler.Data.InternalDataInterceptor
{
    public class InternalDataInterceptorApplication : IInternalDataInterceptorApplication
    {
        private List<IInternalDataInterceptorApplicationInvoker> _interceptors;
        public List<IInternalDataInterceptorApplicationInvoker> Invoker { get => _interceptors; private set => _interceptors = value; }

        List<IInternalDataInterceptorApplicationInvoker> IInternalDataInterceptorApplication.Invoker => throw new NotImplementedException();

        public InternalDataInterceptorApplication()
        {
            Invoker = new List<IInternalDataInterceptorApplicationInvoker>();  
        }

        public void Add(IInternalDataInterceptorApplicationInvoker invoker)
        {
            if ((Invoker.Find(x => x.Equals(invoker)) != null))
                return;
            Invoker.Add(invoker);
        }
        public void Remove(IInternalDataInterceptorApplicationInvoker invoker)
        {
            if (Invoker.Find(x=> x.Equals(invoker)) == null)
                return;
            Invoker.Remove(invoker);
        }

        public Task ReceiveMessage(params MessageDTO[] data)
        {
            foreach (var item in Invoker)
            {
                item.ReceiveMessage(data);
            }
            return Task.CompletedTask;
        }

        public Task SendMessage(params MessageDTO[] data)
        {
            foreach (var item in Invoker)
            {
                item.SendMessage(data);
            }
            return Task.CompletedTask;
        }

        public Task CreateFriendRequest(params UserFriendshipRequestDTO[] data)
        {
            foreach (var item in Invoker)
            {
                item.CreateFriendRequest(data);
            }
            return Task.CompletedTask;
        }
        public Task ReceiveAcceptFriendRequest(UserDTO data)
        {
            throw new NotImplementedException();
        }

        public Task ReceiveFriendRequest(params UserFriendshipRequestDTO[] data)
        {
            foreach (var item in Invoker)
            {
                item.ReceiveFriendRequest(data);
            }
            return Task.CompletedTask;
        }

    }
}
