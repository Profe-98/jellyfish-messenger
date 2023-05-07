﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Communication = Microsoft.Maui.ApplicationModel.Communication;

namespace JellyFish.Handler.Device.Media.Contact
{
    public interface IDeviceContactHandler
    {
        public Task<List<Model.Contact>> GetDeviceContacts();
    }
}
