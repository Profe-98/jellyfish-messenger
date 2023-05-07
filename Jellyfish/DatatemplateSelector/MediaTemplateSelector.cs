using JellyFish.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace JellyFish.DatatemplateSelector
{

    public class MediaTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ImageTemplate { get; set; }
        public DataTemplate VideoTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var msg = (CameraMediaModel)item;
            var selectedTemplate = msg.IsImage? ImageTemplate : VideoTemplate;
            if (selectedTemplate == null)
            {
                throw new ArgumentNullException();
            }
            return selectedTemplate;
        }
    }
}
