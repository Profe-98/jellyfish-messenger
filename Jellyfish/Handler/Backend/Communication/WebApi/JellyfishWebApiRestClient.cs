using JellyFish.Handler.Backend.Communication.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiFunction.Application.Model.Database.MySQL.Jellyfish;

namespace JellyFish.Handler.Backend.Communication.WebApi
{
    public class JellyfishWebApiRestClient : WebApiRestClient
    {

        public JellyfishWebApiRestClient()
        {

        }
        public override Task<AuthModel> Authentificate(string userName, string password, CancellationToken cancellationToken)
        {
            var result =base.Authentificate(userName, password, cancellationToken);
            return result;
        }
    }
}
