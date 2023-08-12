using System;
using System.ComponentModel;

namespace Application.Mobile.Jellyfish.Data.AppConfig.Abstraction
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
