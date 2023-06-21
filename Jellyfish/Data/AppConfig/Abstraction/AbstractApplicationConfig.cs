using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Reflection;
using JellyFish.Attribute;
using System.Collections.ObjectModel;
using JellyFish.Data.AppConfig.Ui;
using WebApiFunction.Configuration;

namespace JellyFish.Data.AppConfig.Abstraction
{
    [Serializable]
    public abstract class AbstractApplicationConfig : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected AbstractApplicationConfig()
        {
            SetDefaults();
        }
        public virtual void SetDefaults()
        {

        }

        
    }
}
