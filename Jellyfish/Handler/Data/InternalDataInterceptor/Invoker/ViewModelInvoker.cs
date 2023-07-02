using JellyFish.Controls;
using JellyFish.Handler.Data.InternalDataInterceptor.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiFunction.Application.Model.Database.MySQL.Jellyfish;

namespace JellyFish.Handler.Data.InternalDataInterceptor.Invoker
{
    public class ViewModelInvoker : IInternalDataInterceptorInvoker
    {
        public ViewModelInvoker()
        {
            
        }
        public Task Invoke(params object[] data)
        {
            List<MessageDTO> messages = (List<MessageDTO>)data[0];
            messages.ForEach(message =>
            {
                MainThread.InvokeOnMainThreadAsync(() =>
                {

                    NotificationHandler.ToastNotify(message.Text);
                });
            });
            return Task.CompletedTask;
        }
    }
}
