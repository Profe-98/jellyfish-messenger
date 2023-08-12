using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Application.Mobile.Jellyfish.ViewModel
{
    public class AppShellViewModel : BaseViewModel
    {
        public ICommand BackButtonCommand { get; private set; }
        public AppShellViewModel()
        {
            BackButtonCommand = new RelayCommand(() => { 
            
            });
        }
    }
}
