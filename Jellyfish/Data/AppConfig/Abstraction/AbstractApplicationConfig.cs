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

namespace JellyFish.Data.AppConfig.Abstraction
{
    [Serializable]
    public abstract class AbstractApplicationConfig : INotifyPropertyChanged
    {

        public event EventHandler SafeEvent;
        public event PropertyChangedEventHandler PropertyChanged;
        protected AbstractApplicationConfig()
        {
            SetDefaults();
        }
        public virtual void SetDefaults()
        {

        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            if (SafeEvent != null)
            {
                SafeEvent.Invoke(this, new PropertyChangedEventArgs(name));
            }
            OnSafeEventTriggered(this, new PropertyChangedEventArgs(name));
        }
        protected void OnSafeEventTriggered(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {

        }


        public ObservableCollection<ApplicationConfigPropertyDescriptorViewModel> GetInstanceValuesWithUiDisplayNames()
        {
            ObservableCollection<ApplicationConfigPropertyDescriptorViewModel> propValues = new ObservableCollection<ApplicationConfigPropertyDescriptorViewModel>();   
            var props = this.GetType().GetProperties();
            if(props.Length > 0)
            {
                    foreach (var prop in props)
                    {
                        bool isPub = prop.GetAccessors()?.ToList().Find(x => x.IsPublic) != null;

                        if (prop.CanRead && isPub)
                        {
                            var att = prop.GetCustomAttribute<PropertyUiDisplayTextAttribute>();
                            if (att != null)
                            {
                                string propUiDisplayName = att.DisplayName;
                                object currentValue = prop.GetValue(this, null);
                                propValues.Add(new ApplicationConfigPropertyDescriptorViewModel
                                {
                                    DisplayName = propUiDisplayName,
                                    PropertyInfo = prop,
                                    PropertyConfigType = this.GetType(),
                                    PropertyType = prop.DeclaringType,
                                    IsReadOnly=att.IsReadonly,
                                    Value = currentValue,
                                });
                            }
                        }
                    }
                
            }
            return propValues;
        }
    }
}
