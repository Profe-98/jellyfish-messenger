using JellyFish.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace JellyFish.Model
{
    public abstract class ViewTemplateModel : BaseViewModel
    {
        public string Title { get; set; }
        public Microsoft.Maui.Controls.View ContentView { get; set; }
        public object ContentViewBindingContext { get; set; }
        private bool _isSelected = false;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }
        public int NotificationCount { get; set; }  
        public ICommand RefreshDataCommand { get; set; }    
        public ViewTemplateModel()
        {

        }
    }

    public class ChatsViewTemplate : ViewTemplateModel
    {
    }
    public class StatusViewTemplate : ViewTemplateModel
    {
    }
    public class CallViewTemplate: ViewTemplateModel
    {
    }

}
