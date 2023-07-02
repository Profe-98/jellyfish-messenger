using JellyFish.Handler.Data.InternalDataInterceptor.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JellyFish.Handler.Data.InternalDataInterceptor.Invoker
{
    public class SqlLiteDatabaseHandlerInvoker : IInternalDataInterceptorInvoker
    {
        public SqlLiteDatabaseHandlerInvoker() 
        {

        }
        public Task Invoke(params object[] data)
        {
            return Task.CompletedTask;
        }
    }
}
