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
    public abstract class AbstractSettingsPageGenericViewModel: BaseViewModel
    {
        public string PageTitle { get;private set; }
        private ObservableCollection<ApplicationConfigPropertyDescriptorViewModel> _propertyValues;
        public ObservableCollection<ApplicationConfigPropertyDescriptorViewModel> PropertyValues
        {
            get
            {
                return _propertyValues;
            }
            protected set {
                _propertyValues = value; 
                OnPropertyChanged(nameof(PropertyValues));
            }
        }
        public bool HasProps
        {
            get => PropertyValues != null && PropertyValues.Count != 0;
        }
        public ICommand SaveConfigCommand;

        public void RefreshUi()
        {
            OnPropertyChanged(nameof(PropertyValues));
        }
        public AbstractSettingsPageGenericViewModel(string pageTitle)
        {
            PageTitle = pageTitle;  
        }
    }
}
