using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Mobile.Jellyfish.Handler.Data;
using Application.Mobile.Jellyfish.Data.SqlLite.Schema;
using Application.Mobile.Jellyfish.ViewModel;
using Application.Mobile.Jellyfish.Handler.Device.Vibrate;
using Application.Mobile.Jellyfish.Handler.Device.Media.Camera;
using Application.Mobile.Jellyfish.Handler.Device.Media.Contact;
using Application.Mobile.Jellyfish.Handler.Device.Media.Audio;
using Application.Mobile.Jellyfish.Handler.Device.Media.Communication;
using Application.Mobile.Jellyfish.Handler.Device.Notification;
using Application.Mobile.Jellyfish.Handler.Device.Filesystem;
using Application.Mobile.Jellyfish.Handler.Device.Sensor;
using Application.Mobile.Jellyfish.Handler.Device.ClipBoard;
using Application.Mobile.Jellyfish.Handler.Device.Network;
using Application.Mobile.Jellyfish.Handler.Backend.Communication.WebApi;
using Application.Mobile.Jellyfish.Handler.Backend.Communication.SignalR;
using Microsoft.Maui.Handlers;
using Application.Mobile.Jellyfish.Handler.AppConfig;
using Application.Mobile.Jellyfish.Handler.Data.InternalDataInterceptor.Abstraction;
using Application.Mobile.Jellyfish.Handler.Data.InternalDataInterceptor.Invoker;
using Application.Mobile.Jellyfish.Handler.Device.Media;
using CommunityToolkit.Maui;
using Application.Mobile.Jellyfish.Handler.Data.InternalDataInterceptor;
using Microsoft.Extensions.DependencyInjection;
#if ANDROID
using Application.Mobile.Jellyfish.Handler.Data.InternalDataInterceptor.Invoker.Notification.Android;
#elif IOS
using Application.Mobile.Jellyfish.Handler.Data.InternalDataInterceptor.Invoker.Notification.iOS;
#endif

namespace Application.Mobile.Jellyfish.ApplicationSpecific
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddSqlLiteDatabase(this IServiceCollection services, string databasePath, SQLite.SQLiteOpenFlags flags)
        {
#if IOS
            SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_sqlite3());
#elif ANDROID
        SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
#endif
            var initSqlLiteHandle = new JellyfishSqlliteDatabaseHandler();//Abstrakten Typ angeben, da dieser BaseType aller konkreten DB-Entities ist und mittels des BaseTypes alle Typen innerhalb der Assembly gesucht werden
            initSqlLiteHandle.Init(databasePath, flags);
            services.AddSingleton(initSqlLiteHandle);
            return services;
        }
        public static IServiceCollection AddViewModels(this IServiceCollection services)
        {
            services.AddSingleton<ChatsPageViewModel>();
            services.AddSingleton<StatusPageViewModel>();
            services.AddSingleton<CallsPageViewModel>();
            services.AddSingleton<MainPageViewModel>();
            services.AddSingleton<LoginPageViewModel>();
            services.AddSingleton<ResetPasswordContentPageViewModel>();
            services.AddTransient<ChatPageViewModel>();
            services.AddSingleton<UserSelectionPageViewModel>();
            services.AddSingleton<ProfilePageViewModel>();
            services.AddSingleton<CameraHandlerPageViewModel>();
            services.AddSingleton<RegisterContentPageViewModel>();
            services.AddSingleton<SettingsPageViewModel>();
            return services;
        }
        public static IServiceCollection AddDeviceHandlers(this IServiceCollection services,ApplicationConfigHandler applicationHandler)
        {
            services.AddSingleton<ApplicationResourcesHandler>();
            services.AddSingleton<FileHandler>(new FileHandler(() => { }, () => { }));
            services.AddSingleton<VibrateHandler>(new VibrateHandler(() => { }, () => { }));
            services.AddSingleton<CameraHandler>(new CameraHandler(() => { }, () => { }));
            services.AddSingleton<DeviceContactHandler>();
            //services.AddSingleton<AbstractAudioPlayerHandler>();
            //services.AddSingleton<AbstractAudioRecorderHandler>();
            services.AddSingleton<DeviceCommunicationHandler>(new DeviceCommunicationHandler(() => { }, () => { }));
            services.AddSingleton<NotificationHandler>(new NotificationHandler(() => { }, () => { }));
            services.AddSingleton<GpsHandler>(new GpsHandler(() => { }, () => { }));
            services.AddSingleton<ClipBoardHandler>(new ClipBoardHandler());
            services.AddSingleton<NetworkingHandler>(new NetworkingHandler(() => { }, () => { }));

            var jellyfishBackendClient = new JellyfishWebApiRestClient(applicationHandler);
            string loginSessionEndpoint = WebApiEndpointStruct.LoginSessionEndpoint;
            string logoutSessionEndpoint = WebApiEndpointStruct.LogoutSessionEndpoint;
            string validateSessionEndpoint = WebApiEndpointStruct.ValidateSessionEndpoint;
            string refreshSessionEndpoint = WebApiEndpointStruct.RefreshSessionEndpoint;
            string connectionTestEndpoint = WebApiEndpointStruct.ConnectionTestEndpoint;
            string protocolApi = applicationHandler.ApplicationConfig.NetworkConfig.WebApiHttpClientTransportProtocol == Data.AppConfig.ConcreteImplements.NetworkConfig.HTTP_TRANSPORT_PROTOCOLS.HTTP ? "http://" : "https://";
            string baseUrl =
                protocolApi +
                applicationHandler.ApplicationConfig.NetworkConfig.WebApiBaseUrl + ":" +
                applicationHandler.ApplicationConfig.NetworkConfig.WebApiBaseUrlPort +
                applicationHandler.ApplicationConfig.NetworkConfig.WebApiPath + "/";
            jellyfishBackendClient.Init(baseUrl,
                loginSessionEndpoint,
                logoutSessionEndpoint,
                validateSessionEndpoint,
                refreshSessionEndpoint,
                connectionTestEndpoint);
            services.AddSingleton<JellyfishWebApiRestClient>(jellyfishBackendClient);

            services.AddSingleton<JellyfishWebApiRestClientInvoker>();
            services.AddSingleton<ViewModelInvoker>();
            services.AddSingleton<SqlLiteDatabaseHandlerInvoker>();
            services.AddSingleton<NotificationInvoker>();
            services.AddSingleton<Application.Mobile.Jellyfish.Handler.Data.InternalDataInterceptor.InternalDataInterceptorApplication>();

            bool containsRelevantService = services.ToList().Find(x => x.ServiceType == typeof(InternalDataInterceptorApplication)) != null;
            if (!containsRelevantService)
            {
                throw new ArgumentNullException(nameof(InternalDataInterceptorApplication) + " not found in DI or " + nameof(AddDeviceHandlers) + " is called before initialization of " + nameof(InternalDataInterceptorApplication) + "");
            }
            services.AddSingleton<InitDataInterceptorApplicationModel>();
            services.AddSingleton<SignalRClient>();




            return services;
        }

        public static IServiceCollection AddPages(this IServiceCollection services)
        {

            return services;
        }
    }

}
