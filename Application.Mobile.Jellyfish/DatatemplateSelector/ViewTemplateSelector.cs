﻿using Application.Mobile.Jellyfish.Model;
using Application.Mobile.Jellyfish.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mobile.Jellyfish.DatatemplateSelector
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
