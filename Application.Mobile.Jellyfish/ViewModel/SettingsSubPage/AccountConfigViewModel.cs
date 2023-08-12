using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Mobile.Jellyfish.Attribute;
using Application.Mobile.Jellyfish.Data.AppConfig.Abstraction;
using Application.Mobile.Jellyfish.Validation;
using Application.Mobile.Jellyfish.ViewModel.SettingsSubPage;

namespace Application.Mobile.Jellyfish.Data.AppConfig.ConcreteImplements
{
    public class AccountConfigViewModel : AbstractConfigViewModel<AccountConfig>
    {
        [PropertyUiDisplayText("Username","Username...")]
        public ValidatableObject<string> UserName { get; set; } = new ValidatableObject<string>();
        [PropertyUiDisplayText("Password", "Password...",true,false)]
        public ValidatableObject<string> Password { get; set; } = new ValidatableObject<string>();
        [PropertyUiDisplayText("Save Login")]
        public ValidatableObject<bool> RemeberMe { get; set; } = new ValidatableObject<bool>();

        public AccountConfigViewModel(AccountConfig config) : base(config)
        {
        }

        public override void MapConfigDataWithDisplayData()
        {
            UserName.Value = Config.UserName;
            Password.Value = Config.Password;
            RemeberMe.Value = Config.RemeberMe;
        }

        public override void Safe()
        {
            Config.UserName = UserName.Value;
            Config.Password = Password.Value;
            Config.RemeberMe = RemeberMe.Value;
        }

        public override void AddValidations()
        {
            UserName.Validations.Add(new IsNotNullOrEmptyRule() { ValidationMessage= "Username entry is required." });
            Password.Validations.Add(new IsNotNullOrEmptyRule() { ValidationMessage= "Password entry is required." });
        }
    }
}
