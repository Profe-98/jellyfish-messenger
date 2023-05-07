using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JellyFish.Attribute;
using JellyFish.Data.AppConfig.Abstraction;

namespace JellyFish.Data.AppConfig.ConcreteImplements
{
    public class SqlLiteConfig : AbstractApplicationConfig
    {

        public bool SchemaCreated { get; set; }
    }
}
