using JellyFish.Model;
using JellyFish.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JellyFish.DatatemplateSelector
{
    public class ViewTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ViewHeaderTemplate { get; set; }
        
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return ViewHeaderTemplate;
        }
    }
}
