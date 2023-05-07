using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JellyFish.Handler.Data;
using JellyFish.Data.SqlLite.Schema;
using JellyFish.ViewModel;
using JellyFish.Handler.Device.Vibrate;
using JellyFish.Handler.Device.Media.Camera;
using JellyFish.Handler.Device.Media.Contact;
using JellyFish.Handler.Device.Media.Audio;
using JellyFish.Handler.Device.Media.Communication;
using JellyFish.Handler.Device.Notification;
using JellyFish.Handler.Device.Filesystem;
using JellyFish.Handler.Device.Sensor;
using JellyFish.Handler.Device.ClipBoard;
using JellyFish.Handler.Device.Network;

namespace JellyFish.ApplicationSpecific
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
            var initSqlLiteHandle = new SqlLiteDatabaseHandler<AbstractEntity>();//Abstrakten Typ angeben, da dieser BaseType aller konkreten DB-Entities ist und mittels des BaseTypes alle Typen innerhalb der Assembly gesucht werden
            initSqlLiteHandle.Init(databasePath, flags);
            var type = initSqlLiteHandle.GetType();
            services.AddSingleton(type);
            return services;
        }
        public static IServiceCollection AddViewModels(this IServiceCollection services)
        {
            services.AddSingleton<ChatsPageViewModel>();
            services.AddSingleton<StatusPageViewModel>();
            services.AddSingleton<CallsPageViewModel>();
            services.AddSingleton<MainPageViewModel>();
            services.AddSingleton<LoginPageViewModel>();
            services.AddSingleton<RegisterContentPageViewModel>();
            services.AddSingleton<ResetPasswordContentPageViewModel>();
            services.AddTransient<ChatPageViewModel>();
            services.AddSingleton<ContactsPageViewModel>();
            services.AddSingleton<ProfilePageViewModel>();
            services.AddSingleton<CameraHandlerPageViewModel>();
            return services;
        }
        public static IServiceCollection AddDeviceHandlers(this IServiceCollection services)
        {
            services.AddSingleton<FileHandler>();
            services.AddSingleton<VibrateHandler>();
            services.AddSingleton<CameraHandler>();
            services.AddSingleton<DeviceContactHandler>();
            //services.AddSingleton<AbstractAudioPlayerHandler>();
            //services.AddSingleton<AbstractAudioRecorderHandler>();
            services.AddSingleton<DeviceCommunicationHandler>();
            services.AddSingleton<NotificationHandler>();
            services.AddSingleton<GpsHandler>();
            services.AddSingleton<ClipBoardHandler>();
            services.AddSingleton<NetworkingHandler>();
            return services;
        }
        public static IServiceCollection AddPages(this IServiceCollection services)
        {

            return services;
        }
    }

}
