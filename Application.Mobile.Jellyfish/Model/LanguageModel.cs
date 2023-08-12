using Application.Mobile.Jellyfish.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mobile.Jellyfish.Model
{
    public class LanguageModel : BaseViewModel
    {
        public string PhonePrefix
        {
            get;
            set;
        }
        public string Country
        {
            get;
            set;
        }
    }
}
