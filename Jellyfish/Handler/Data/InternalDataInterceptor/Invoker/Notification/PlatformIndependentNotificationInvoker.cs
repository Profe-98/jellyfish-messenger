using JellyFish.Handler.Data.InternalDataInterceptor.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JellyFish.Handler.Data.InternalDataInterceptor.Invoker.Notification
{
    public class PlatformIndependentNotificationInvoker : IInternalDataInterceptorInvoker
    {
        public virtual Task Invoke(params object[] data)
        {
            return Task.CompletedTask;  
        }
    }
}
