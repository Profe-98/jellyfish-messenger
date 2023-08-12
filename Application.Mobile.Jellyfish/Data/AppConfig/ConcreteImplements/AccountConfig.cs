using System;
using Application.Mobile.Jellyfish.Data.AppConfig.Abstraction;
using Application.Mobile.Jellyfish.Model;
using Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.Jellyfish.Table;

namespace Application.Mobile.Jellyfish.Data.AppConfig.ConcreteImplements
{
    public class AccountConfig : AbstractApplicationConfig
    {
        public User User { get; set; }
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
