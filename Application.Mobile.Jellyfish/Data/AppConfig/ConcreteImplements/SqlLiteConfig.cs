using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Mobile.Jellyfish.Attribute;
using Application.Mobile.Jellyfish.Data.AppConfig.Abstraction;

namespace Application.Mobile.Jellyfish.Data.AppConfig.ConcreteImplements
{
    public class SqlLiteConfig : AbstractApplicationConfig
    {

        public bool SchemaCreated { get; set; }
        public string DatabasePath { get; set; }
    }
}
