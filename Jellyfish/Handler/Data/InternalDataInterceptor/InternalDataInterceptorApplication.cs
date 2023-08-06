using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JellyFish.Data.WebApi;
using JellyFish.Handler.Data.InternalDataInterceptor.Abstraction;
using JellyFish.Handler.Data.InternalDataInterceptor.Invoker;
#if ANDROID
using JellyFish.Handler.Data.InternalDataInterceptor.Invoker.Notification.Android;
#else
using JellyFish.Handler.Data.InternalDataInterceptor.Invoker.Notification.iOS;
#endif
using WebApiFunction.Application.Model.Database.MySQL.Jellyfish;
using WebApiFunction.Application.Model.Database.MySQL.Jellyfish.DataTransferObject;

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
        public IInternalDataInterceptorApplicationInvoker Get<T>() where T : IInternalDataInterceptorApplicationInvoker
        {
            var foundItem = Invoker.Find(x => x.GetType() == typeof(T));
            return foundItem;
        }
        private async Task<DataInterceptorApplicationInvokerResponseModel> ExecAction<T>(Func<T[], Task> func, T[] param) where T : class, new()
        {
            DataInterceptorApplicationInvokerResponseModel dataInterceptorApplicationInvokerResponseModel = new DataInterceptorApplicationInvokerResponseModel();
            dataInterceptorApplicationInvokerResponseModel.Start();
            try
            {
                await func(param);

                dataInterceptorApplicationInvokerResponseModel.IsSuccess = true;
            }
            catch (Exception ex)
            {

                dataInterceptorApplicationInvokerResponseModel.Exception = ex;
            }
            dataInterceptorApplicationInvokerResponseModel.Stop();
            return dataInterceptorApplicationInvokerResponseModel;
        }
        #region BusinessContext

        public async Task<InternalDataInterceptorApplicationInvokeResponseModel> ReceiveMessage(params MessageDTO[] data)
        {
            var response = new InternalDataInterceptorApplicationInvokeResponseModel(Invoker);
            foreach (var item in Invoker)
            {
                response.ExecResponseDictionary[item] = await ExecAction<MessageDTO>(item.ReceiveMessage, data);
            }
            return response;
        }

        public async Task<InternalDataInterceptorApplicationInvokeResponseModel> SendMessage(params MessageDTO[] data)
        {
            var response = new InternalDataInterceptorApplicationInvokeResponseModel(Invoker);
            foreach (var item in Invoker)
            {
                response.ExecResponseDictionary[item] = await ExecAction<MessageDTO>(item.SendMessage, data);
            }
            return response;
        }

        public async Task<InternalDataInterceptorApplicationInvokeResponseModel> CreateFriendRequest(params UserFriendshipRequestDTO[] data)
        {
            var response = new InternalDataInterceptorApplicationInvokeResponseModel(Invoker);
            foreach (var item in Invoker)
            {
                response.ExecResponseDictionary[item] = await ExecAction<UserFriendshipRequestDTO>(item.CreateFriendRequest,data);
            }
            return response;
        }
        public async Task<InternalDataInterceptorApplicationInvokeResponseModel> ReceiveAcceptFriendRequest(params UserDTO[] data)
        {
            var response = new InternalDataInterceptorApplicationInvokeResponseModel(Invoker);
            foreach (var item in Invoker)
            {
                response.ExecResponseDictionary[item] = await ExecAction<UserDTO>(item.ReceiveAcceptFriendRequest, data);
            }
            return response;
        }

        public async Task<InternalDataInterceptorApplicationInvokeResponseModel> ReceiveFriendRequest(params UserFriendshipRequestDTO[] data)
        {
            var response = new InternalDataInterceptorApplicationInvokeResponseModel(Invoker);
            foreach (var item in Invoker)
            {
                response.ExecResponseDictionary[item] = await ExecAction<UserFriendshipRequestDTO>(item.ReceiveFriendRequest, data);
            }
            return response;
        }

        public async Task<UserDTO> GetOwnProfile(CancellationToken cancellationToken)
        {
            UserDTO response = null;
            JellyfishWebApiRestClientInvoker webApiInvoker = (JellyfishWebApiRestClientInvoker)this.Get<JellyfishWebApiRestClientInvoker>();
            var requestResponse = await webApiInvoker.GetOwnProfile(cancellationToken);
            if (requestResponse.IsSuccess)
            {
                response = requestResponse.ApiResponseDeserialized.data.First().attributes;
            }
            return response;
        }

        public async Task<List<UserFriendshipUserModelDTO>> GetFriendshipRequests(CancellationToken cancellationToken)
        {
            var response = new List<UserFriendshipUserModelDTO>();
            JellyfishWebApiRestClientInvoker webApiInvoker = (JellyfishWebApiRestClientInvoker)this.Get<JellyfishWebApiRestClientInvoker>();

            var requestResponse = await webApiInvoker.GetFriendshipRequests(cancellationToken);
            if (requestResponse.IsSuccess)
            {
                foreach (var rq in requestResponse.ApiResponseDeserialized.data)
                {
                    response.Add(rq.attributes);
                }
            }
            return response;
        }
        public async Task<WebApiHttpRequestResponseModel<UserDTO>> GetFriends(CancellationToken cancellationToken)
        {
            var response = new List<UserFriendshipUserModelDTO>();
            JellyfishWebApiRestClientInvoker webApiInvoker = (JellyfishWebApiRestClientInvoker)this.Get<JellyfishWebApiRestClientInvoker>();

            var requestResponse = await webApiInvoker.GetFriends(cancellationToken);
            return requestResponse;
        }

        public async Task<WebApiHttpRequestResponseModel<UserDTO>> AcceptFriendRequest(Guid requestUuid, CancellationToken cancellationToken)
        {
            JellyfishWebApiRestClientInvoker webApiInvoker = (JellyfishWebApiRestClientInvoker)this.Get<JellyfishWebApiRestClientInvoker>();
            return await webApiInvoker.AcceptFriendshipRequests(requestUuid, cancellationToken);
        }

        public async Task<WebApiHttpRequestResponseModel<UserDTO>> SearchUser(string searchUser, CancellationToken cancellationToken)
        {
            JellyfishWebApiRestClientInvoker webApiInvoker = (JellyfishWebApiRestClientInvoker)this.Get<JellyfishWebApiRestClientInvoker>();
            return await webApiInvoker.SearchUser(searchUser, cancellationToken);
        }

        #endregion

    }
}
