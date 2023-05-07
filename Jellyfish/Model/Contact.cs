using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Communication = Microsoft.Maui.ApplicationModel.Communication;

namespace JellyFish.Model
{
    public class Contact
    {
        public string Id { get; set; }
        public string DisplayName { private set; get; }
        public string NamePrefix { get; set; }
        public string GivenName { get; set; }
        public string MiddleName { get; set; }
        public string FamilyName { get; set; }
        public string NameSuffix { get; set; }
        public List<ContactPhone> Phones { get; set; }
        public List<ContactEmail> Emails { get; set; }
        public byte[] ProfilePicture { get; set; }//Profilbild aus Cache, Cache wird SQLLite mittels Blob

        public Contact()
        {

        }
        public static Contact Create(Communication.Contact contact)
        {
            Contact responseValue = new Contact();
            responseValue.Id = contact.Id;
            responseValue.DisplayName = contact.DisplayName;
            responseValue.NamePrefix = contact.NamePrefix;
            responseValue.GivenName = contact.GivenName;
            responseValue.MiddleName = contact.MiddleName;
            responseValue.FamilyName = contact.FamilyName;
            responseValue.NameSuffix = contact.NameSuffix;
            responseValue.Phones = contact.Phones;
            responseValue.Emails = contact.Emails;
            return responseValue;
        }
    }
}
