using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JellyFish.Data.AppConfig.Abstraction;
using JellyFish.ViewModel.SettingsSubPage;

namespace JellyFish.Data.AppConfig.ConcreteImplements
{
    public class NotificationConfigViewModel : AbstractConfigViewModel<NotificationConfig>
    {
        public NotificationConfigViewModel(NotificationConfig config) : base(config)
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
