using JellyFish.Data.AppConfig.Abstraction;
using JellyFish.Data.AppConfig.Ui;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace JellyFish.ViewModel.SettingsSubPage
{
    public class SettingsPageGenericViewModel<T> : AbstractSettingsPageGenericViewModel
        where T : AbstractApplicationConfig
    {
        public SettingsPageGenericViewModel(string pageTitle,T configInstance) : base(pageTitle) 
        {
            configInstance = (configInstance == null ? Activator.CreateInstance<T>() : configInstance);
            var propVals = configInstance.GetInstanceValuesWithUiDisplayNames();
            PropertyValues = propVals;
            OnPropertyChanged(nameof(PropertyValues));

            SaveConfigCommand = new RelayCommand(SaveConfigAction);
        }   
        public void SaveConfigAction()
        {

        }
    }
}
