using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JellyFish.Attribute;
using JellyFish.Data.AppConfig.Abstraction;
using WebApiFunction.Application.Model.Database.MySQL.Jellyfish;

namespace JellyFish.Data.AppConfig.ConcreteImplements
{
    public class AccountConfig : AbstractApplicationConfig
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool RemeberMe { get; set; }
        public string RegisterBase64Token { get; set; }
        public AuthModel UserSession { get; set; }    
        public AccountConfig() 
        {

        }
        public override void SetDefaults()
        {
            UserName = String.Empty;
            Password = String.Empty;
            RemeberMe = false;
            RegisterBase64Token = null;
            base.SetDefaults();
        }
    }
}
