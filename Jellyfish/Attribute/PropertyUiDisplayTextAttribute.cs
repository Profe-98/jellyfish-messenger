using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JellyFish.Attribute
{
    [System.AttributeUsage(AttributeTargets.Property| AttributeTargets.Field, AllowMultiple =false,Inherited =false)]
    public class PropertyUiDisplayTextAttribute : System.Attribute
    {
        public string DisplayName { get; private set; }
        public bool IsReadonly{ get; private set; }

        public PropertyUiDisplayTextAttribute(string displayName,bool readonlyInUi = false) {

            DisplayName = displayName;
            IsReadonly = readonlyInUi;
        }

    }
}
