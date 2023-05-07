using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Communication = Microsoft.Maui.ApplicationModel.Communication;
using JellyFish.Model;
using Perms = Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.Communication;
using JellyFish.Handler.Abstraction;

namespace JellyFish.Handler.Device.Media.Contact
{
    public class DeviceContactHandler : AbstractDeviceActionHandler<Permissions.ContactsRead>
    {
        public async Task<List<Model.Contact>> GetDeviceContacts()
        {
            List<Model.Contact> list = new List<Model.Contact>();
            var contactsFromPhone = await Microsoft.Maui.ApplicationModel.Communication.Contacts.GetAllAsync();
            if (contactsFromPhone != null)
            {
                foreach (var contact in contactsFromPhone)
                {
                    list.Add(Model.Contact.Create(contact));
                }
            }
            return list;
        }
    }
}
