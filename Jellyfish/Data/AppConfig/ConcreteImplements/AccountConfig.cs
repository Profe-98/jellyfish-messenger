using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JellyFish.Attribute;
using JellyFish.Data.AppConfig.Abstraction;

namespace JellyFish.Data.AppConfig.ConcreteImplements
{
    public class AccountConfig : AbstractApplicationConfig
    {
        [PropertyUiDisplayText("Username")]
        public string UserName { get; set; }
        [PropertyUiDisplayText("Password")]
        public string Password { get; set; }
        [PropertyUiDisplayText("Remember me")]
        public bool RemeberMe { get; set; }  
        public AccountConfig() 
        {

        }  
    }
}
