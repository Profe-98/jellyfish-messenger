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
using System.Runtime.CompilerServices;
using JellyFish.Service;
using JellyFish.ViewModel;
using JellyFish.View;
using JellyFish.Controls;
using System.Reflection;

namespace JellyFish.Handler.Device.Media.Contact
{
    public class DeviceContactHandler : AbstractDeviceActionHandler<Permissions.ContactsRead>
    {

        public async Task<List<Microsoft.Maui.ApplicationModel.Communication.Contact>> GetDeviceContacts()
        {
            bool permissions = await AreRequiredPermissionsGranted();
            if (!permissions)
            {
                return null;
            }
            List<Microsoft.Maui.ApplicationModel.Communication.Contact> list = new List<Microsoft.Maui.ApplicationModel.Communication.Contact>();
            var contactsFromPhone = await Microsoft.Maui.ApplicationModel.Communication.Contacts.GetAllAsync();
            if (contactsFromPhone != null)
            {
                foreach (var contact in contactsFromPhone)
                {
                    list.Add(contact);
                }
            }
            return list;
        }
        public async Task OpenUserSelectionHandler(NavigationService navigationService, bool multiselection, string messageBusQueue)
        {
            try
            {
                bool permissions = await AreRequiredPermissionsGranted();
                if (!permissions)
                {
                    NotificationHandler.ToastNotify("Abort: No permissions");
                    return;
                }
                var vm = new UserSelectionPageViewModel(navigationService, messageBusQueue);
                vm.IsFriendMultiSelectionEnabled = multiselection;
                var page = new UserSelectionPage(vm);
                page.BindingContext = vm;
                await navigationService.PushAsync(page);
            }
            catch (Exception ex)
            {
                NotificationHandler.ToastNotify("Error: " + ex.Message + "");
            }


        }
        public override void SetUserDeniedAction()
        {
            UserDeniedAction = () => { };
        }

        public override void SetUserRationalAction()
        {
            UserRationalAction = () => { };
        }
    }
}
