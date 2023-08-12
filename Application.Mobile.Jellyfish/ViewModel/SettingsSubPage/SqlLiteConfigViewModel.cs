using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Mobile.Jellyfish.Attribute;
using Application.Mobile.Jellyfish.Data.AppConfig.Abstraction;
using Application.Mobile.Jellyfish.ViewModel.SettingsSubPage;

namespace Application.Mobile.Jellyfish.Data.AppConfig.ConcreteImplements
{
    public class SqlLiteConfigViewModel : AbstractConfigViewModel<SqlLiteConfig>
    {
        public SqlLiteConfigViewModel(SqlLiteConfig config) : base(config)
        {
        }

        public bool SchemaCreated { get; set; }

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