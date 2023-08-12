using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Mobile.Jellyfish.Data.AppConfig.Abstraction;
using Application.Mobile.Jellyfish.ViewModel.SettingsSubPage;

namespace Application.Mobile.Jellyfish.Data.AppConfig.ConcreteImplements
{
    public class ChatConfigViewModel : AbstractConfigViewModel<ChatConfig>
    {
        public ChatConfigViewModel(ChatConfig config) : base(config)
        {
        }

        public override void AddValidations()
        {

        }

        public override void MapConfigDataWithDisplayData()
        {
        }

        public override void Safe()
        {

        }
    }
}
