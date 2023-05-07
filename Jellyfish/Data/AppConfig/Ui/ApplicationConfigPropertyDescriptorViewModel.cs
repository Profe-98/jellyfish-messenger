using JellyFish.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JellyFish.Data.AppConfig.Ui
{
    public class ApplicationConfigPropertyDescriptorViewModel : BaseViewModel
    {
        public string DisplayName { get; set; }
        public PropertyInfo PropertyInfo { get; set; }
        public Type PropertyType { get; set; }
        public Type PropertyConfigType { get; set; }
        public bool IsReadOnly { get; set; }
        public object Value { get; set; }
    }
}
