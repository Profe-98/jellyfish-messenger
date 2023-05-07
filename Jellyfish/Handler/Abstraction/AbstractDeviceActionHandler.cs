using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls.Compatibility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.Maui.ApplicationModel.Permissions;
using Perms = Microsoft.Maui.ApplicationModel;

namespace JellyFish.Handler.Abstraction
{
    public abstract class AbstractDeviceActionHandler
    {
        //Prüft anhand aller Handler und deren benutzte Permissions, welche Permissions die App im Gesamten braucht um reibungslose zu funktionieren
        public List<T> GetAllAppRequiredPermissions<T>()
            where T : BasePermission, new()
        {
            List<T> response = new List<T>();

            var allHandlersWithDerivedDeviceActionHandler = Assembly.GetExecutingAssembly().GetTypes()?.
                ToList().FindAll(x => x.BaseType == typeof(AbstractDeviceActionHandler) || x.DeclaringType == typeof(AbstractDeviceActionHandler)).FindAll(x => x.IsGenericType);
            if (allHandlersWithDerivedDeviceActionHandler != null && allHandlersWithDerivedDeviceActionHandler.Any())
            {
                foreach (var item in allHandlersWithDerivedDeviceActionHandler)
                {
                    var genericParams = item.GetGenericArguments();
                    if (genericParams.Length > 0)
                    {
                        for (int i = 0; i < genericParams.Length; i++)
                        {
                            if (genericParams[i] == typeof(BasePermission))
                            {
                                var t = genericParams[i] as T;
                                response.Add(t);
                            }
                        }
                    }
                }
            }
            return response;
        }

        public async Task<PermissionStatus> GetPermissionState<PERM>() where PERM : BasePermission, new()
        {
            return await CheckStatusAsync<PERM>();
        }
        public async Task<bool> IsPermissionGranted<PERM>() where PERM : BasePermission, new()
        {
            var perm = await GetPermissionState<PERM>();
            //Limited by iOS
            if (perm.HasFlag(PermissionStatus.Granted | PermissionStatus.Limited) || perm == PermissionStatus.Granted)
            {
                return true;
            }
            return false;
        }
        public async Task<PermissionStatus> GetPermissionFromDeviceUser<PERM>() where PERM : BasePermission, new()
        {
            return await RequestAsync<PERM>();
        }
        public async Task<Hashtable> GetAllPermissionStates()
        {
            Hashtable hashtable = new Hashtable();
            var listOfPermissionREADypes = Assembly.GetExecutingAssembly().GetTypes()?.ToList().
                FindAll(x => x.BaseType == typeof(BasePermission) || x.DeclaringType == typeof(BasePermission));

            if (listOfPermissionREADypes != null && listOfPermissionREADypes.Any())
            {
                foreach (var type in listOfPermissionREADypes)
                {

                    var checkPermission = await (Task<PermissionStatus>)typeof(AbstractDeviceActionHandler).
                        GetMethod("GetPermissionState").MakeGenericMethod(type, type).Invoke(this, new object[] { });
                    hashtable.Add(type, checkPermission);
                }
            }
            return hashtable;
        }
    }
    public class AbstractDeviceActionHandler<PERM1, PERM2, PERM3, PERM4> : AbstractDeviceActionHandler
        where PERM1 : BasePermission, new()
        where PERM2 : BasePermission, new()
        where PERM3 : BasePermission, new()
        where PERM4 : BasePermission, new()
    {
        protected Action UserGrantedAction { get; private set; }
        protected Action UserDeniedAction { get; private set; }

        public void SetUserGrantedAction(Action action)
        {
            UserGrantedAction = action;
        }
        public void SetUserDeniedAction(Action action)
        {
            UserDeniedAction = action;
        }
        public AbstractDeviceActionHandler([CallerMemberName] string caller = "")
        {

            InitDeviceActor(null, null, caller);
        }
        public AbstractDeviceActionHandler(Action userGrantAction, Action userDeniedAction, [CallerMemberName] string caller = "")
        {
            UserGrantedAction = userGrantAction;
            UserDeniedAction = userDeniedAction;
            InitDeviceActor(userGrantAction, userDeniedAction, caller);
        }
        protected async void InitDeviceActor(Action successAction, Action userDeniedAction,string caller)
        {
            var permState = await HasRequiredPermissions();
            if (!permState)
            {
                var responseFromUser = PermissionStatus.Denied;
                /*var responseFromUser = await GetPermissionFromDeviceUser<PERM1>()
                | (typeof(PERM1) != typeof(PERM2) ? await GetPermissionFromDeviceUser<PERM2>() : PermissionStatus.Unknown)
                | (typeof(PERM1) != typeof(PERM3) ? await GetPermissionFromDeviceUser<PERM3>() : PermissionStatus.Unknown)
                | (typeof(PERM1) != typeof(PERM4) ? await GetPermissionFromDeviceUser<PERM4>() : PermissionStatus.Unknown);*/
                System.Diagnostics.Debug.WriteLine("NET 6.0, Android 12 API:33: RequestAsync Methode von NET MAUI die mittels meiner Wrapper Methode GetPermissionFromDeviceUser getriggert wird, wird PermissionException obwohl die Permission ja angefragt werden soll....");
                if (responseFromUser.HasFlag(PermissionStatus.Denied))
                {
                    if (userDeniedAction != null)
                        userDeniedAction.Invoke();
                }
            }
            else
            {
                if (successAction != null)
                    successAction.Invoke();
            }
        }
        protected async Task<bool> HasRequiredPermissions()
        {
            return (await GetPermissionState<PERM1>()
                | (typeof(PERM1) != typeof(PERM2) ? await GetPermissionState<PERM2>() : PermissionStatus.Unknown)
                | (typeof(PERM1) != typeof(PERM3) ? await GetPermissionState<PERM3>() : PermissionStatus.Unknown)
                | (typeof(PERM1) != typeof(PERM4) ? await GetPermissionState<PERM4>() : PermissionStatus.Unknown)) == PermissionStatus.Granted;
        }
    }
    public class AbstractDeviceActionHandler<PERM1> : AbstractDeviceActionHandler<PERM1, PERM1, PERM1, PERM1> where PERM1 : BasePermission, new()
    {

    }
    public class AbstractDeviceActionHandler<PERM1, PERM2> : AbstractDeviceActionHandler<PERM1, PERM2, PERM1, PERM1> where PERM1 : BasePermission, new()
        where PERM2 : BasePermission, new()
    {

    }
    public class AbstractDeviceActionHandler<PERM1, PERM2, PERM3> : AbstractDeviceActionHandler<PERM1, PERM2, PERM3, PERM1> where PERM1 : BasePermission, new()
        where PERM2 : BasePermission, new()
        where PERM3 : BasePermission, new()
    {

    }
}
