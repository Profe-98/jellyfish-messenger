using Application.Mobile.Jellyfish.Handler.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mobile.Jellyfish.Handler.Device.ClipBoard
{
    public class ClipBoardHandler : AbstractDeviceActionHandler
    {
        public override Task CheckAndRequestPermission()
        {
            throw new NotImplementedException();
        }

        protected override Task<bool> AreRequiredPermissionsGranted()
        {
            throw new NotImplementedException();
        }
    }
}
