using Microsoft.AspNetCore.Mvc.Infrastructure;
using Application.Shared.Kernel.MicroService;
using Application.Shared.Kernel.Web.Authentification;
using Application.Shared.Kernel.Web.Websocket.SignalR.HubService;
using Microsoft.AspNetCore.Authorization;
using Application.Web.Api.JellyFishBackend.Middleware;
using Swashbuckle.AspNetCore.Swagger;
using Application.Shared.Kernel.Web.AspNet.Controller;
using Microsoft.OpenApi.Models;
using Application.Shared.Kernel.Web.AspNet.Swagger.OperationFilter;
using Application.Shared.Kernel.Web.AspNet.Swagger.SignalR;
using Microsoft.AspNetCore.HttpLogging;
using Application.Shared.Kernel.Configuration.Const;
using Application.Shared.Kernel.Configuration.Extension;
using Application.Shared.Kernel.Configuration.Service;
using Application.Shared.Kernel.Configuration.Model.ConcreteImplementation;
using Application.Shared.Kernel.Configuration;
using Application.Shared.Kernel.Configuration.Model.Abstraction;
using Application.Shared.Kernel.Infrastructure.Log;
using Application.Shared.Kernel.Infrastructure.Database;
using Application.Shared.Kernel.Web.AspNet.Startup;
using Application.Shared.Web.Api.Shared;

namespace Application.Web.Api.JellyFishBackend
{
    public class Startup : WebApiStartup
    {
        readonly string AllowOrigin = "api-gateway";
        public static string[] DatabaseEntityNamespaces { get; } = new string[] { "Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.Jellyfish.Table",
                            "Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.View",
                            "Application.Shared.Kernel.Application.Model.Database.MySQL.Schema.ApiGateway.Table",
                            "Application.Shared.Kernel.Application.Model.DataTransferObject.ConcreteImplementation.Jellyfish" };

        public Startup(IConfiguration configuration, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env) : base(configuration, env)
        {
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddW3CLogging(logging =>
            {
                // Log all W3C fields
                logging.LoggingFields = W3CLoggingFields.All;

                logging.AdditionalRequestHeaders.Add("x-forwarded-for");
                logging.AdditionalRequestHeaders.Add("x-client-ssl-protocol");
                logging.FileSizeLimit = 5 * 1024 * 1024;//
                logging.RetainedFileCountLimit = 2;
                string path = Path.Combine(Environment.CurrentDirectory, "logs");
                logging.RetainedFileCountLimit += 1;
                logging.FileName = "log-data";
                logging.LogDirectory = path;
                logging.FlushInterval = TimeSpan.FromSeconds(2);
            });
            services.AddRouting(x => { x.LowercaseUrls = true; });
            services.AddWebApi(Configuration, DatabaseEntityNamespaces);

            var sp  = services.BuildServiceProvider();
            services.AddAuthentication("Base").
                AddScheme<BasicAuthenticationOptions, AuthentificationHandler>("Base", null);
            services.AddSingleton<IAuthorizationMiddlewareResultHandler, AuthorizationResultMiddleware>();
            services.AddAuthorization(options =>
            {
                //User Policy: Any user with the role root
                options.AddPolicy("Root", policy =>
                                  policy.RequireClaim(BackendAPIDefinitionsProperties.Claim.ClaimTypeUserRole, "root"));
                //User Policy: Any user with the role admin
                options.AddPolicy("Administrator", policy =>
                                  policy.RequireClaim(BackendAPIDefinitionsProperties.Claim.ClaimTypeUserRole, "admin"));
                //User Policy: Any registered User that confirms his registration
                options.AddPolicy("User", policy =>
                                  policy.RequireClaim(BackendAPIDefinitionsProperties.Claim.ClaimTypeUserRole, "user"));
            });
#if DEBUG
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    
                    Version = "v1",
                    Title = "Application.Mobile.Jellyfish API",
                    Description = "API for the Application.Mobile.Jellyfish Mobile App",
                    TermsOfService = new Uri("https://jellyfish.mail.net/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Mika Roos",
                        Url = new Uri("https://jellyfish.mail.net/contact")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "GPL2",
                        Url = new Uri("https://jellyfish.mail.net/license")
                    }
                });
                options.OperationFilter<AddAuthorizationHeaderOperationFilter>();
                options.OperationFilter<OpenApiIgnoreMethodParameterOperationFilter>();
                options.DocumentFilter<SignalRDocumentationFilter>();
            });
#endif

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, IServiceProvider serviceProvider)
        {

            if (env.IsDevelopment())
            {
                app.UseExceptionHandler("/error-debug");
                //app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            //app.UseHttpsRedirection();
            //app.UseStaticFiles();

            app.UseW3CLogging();
            app.UseRouting();
            app.UseCors(AllowOrigin);//must used between UseRouting & UseEndpoints
            var appConfigService = serviceProvider.GetService<IAppconfig>();
            if(appConfigService.AppServiceConfiguration!=null&& appConfigService.AppServiceConfiguration.SignalRHubConfigurationModel!=null)
            {
                app.UseWebSockets();

            }
#if DEBUG

            var swaggerOptions = new SwaggerOptions()
            {

            };
            //beim Aufbauen der Swagger Doku aktuell enorm hohe RAM Auslastung.
            //Sprich nach erstem HTTP Call auf Swagger Doku via Browser
            app.UseSwagger(swaggerOptions);
            //http://localhost:5030/swagger/v1/swagger.json
            //http://localhost:5030/swagger/index.html
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = string.Empty;

            });
#endif
            app.UseAuthentication();
            app.UseAuthorization();
            ISingletonNodeDatabaseHandler singletonNodeDatabaseHandler = serviceProvider.GetService<ISingletonNodeDatabaseHandler>();
            INodeManagerHandler nodeManager = serviceProvider.GetService<INodeManagerHandler>();
            IAppconfig appConfig = serviceProvider.GetService<IAppconfig>();

            ISingletonDatabaseHandler databaseHandler = serviceProvider.GetService<ISingletonDatabaseHandler>();
            app.UseEndpoints(endpoints =>
            {
                if (appConfig.AppServiceConfiguration.SignalRHubConfigurationModel != null && appConfig.AppServiceConfiguration.SignalRHubConfigurationModel.UseLocalHub)
                    endpoints.RegisterSignalRHubs(serviceProvider);//signalr init before register backend for route register
                endpoints.RegisterBackend(nodeManager, serviceProvider, env, singletonNodeDatabaseHandler, actionDescriptorCollectionProvider, Configuration, DatabaseEntityNamespaces, HubServiceExtensions.RegisteredHubServices);

            });
        }

        public override IMainConfigurationModel SetInitialConfiguration(string rootDir)
        {
            #region Initial Configurations
            ApiSecurityConfigurationModel initialApiSecurityConfigurationModel = new ApiSecurityConfigurationModel();
            initialApiSecurityConfigurationModel.ApiContentType = GeneralDefs.ApiContentType;
            initialApiSecurityConfigurationModel.Jwt = new ApiSecurityConfigurationModel.JsonWebTokenModel();
            initialApiSecurityConfigurationModel.Jwt.JwtBearerSecretStr = "this is my custom Secret key for authnetication";
            initialApiSecurityConfigurationModel.SiteProtect = new ApiSecurityConfigurationModel.SiteProtectModel();
            initialApiSecurityConfigurationModel.SiteProtect.MaxHttpContentLen = 0;
            initialApiSecurityConfigurationModel.SiteProtect.MaxHttpHeaderFieldLen = 255;
            initialApiSecurityConfigurationModel.SiteProtect.MaxHttpHeaderFieldValueLen = 255;
            initialApiSecurityConfigurationModel.SiteProtect.MaxHttpRequUriLen = 255;

            ClamAvConfigurationModel initialAntiVirusConfigurationModel = new ClamAvConfigurationModel();
            initialAntiVirusConfigurationModel.DeleteInfectedFilesPermantly = true;
            initialAntiVirusConfigurationModel.Host = "clam-av01";
            initialAntiVirusConfigurationModel.Port = 3310;

            LogConfigurationModel initialLogConfigurationModel = new LogConfigurationModel();
            initialLogConfigurationModel.LogdateFormat = "yyyy-MM-dd";
            initialLogConfigurationModel.LogtimeFormat = "HH:mm:ss";
            initialLogConfigurationModel.LogLevel = General.MESSAGE_LEVEL.LEVEL_INFO;
            initialLogConfigurationModel.UserInterfaceDateFormat = "yyyy-MM-dd";
            initialLogConfigurationModel.UserInterfaceTimeFormat = "HH:mm:ss";

            MailConfigurationModel initialMailConfigurationModel = new MailConfigurationModel();

            initialMailConfigurationModel.EmailAttachmentPath = "";
            //IMAP
            initialMailConfigurationModel.ImapSettings = new MailConfigurationModel.MailSettingsModel();
            initialMailConfigurationModel.ImapSettings.User = "mail@mail.net";
            initialMailConfigurationModel.ImapSettings.Password = "aChJz8nPf5dXZsa";
            initialMailConfigurationModel.ImapSettings.Server = "imap.strato.de";
            initialMailConfigurationModel.ImapSettings.Port = 993;
            initialMailConfigurationModel.ImapSettings.SecureSocketOptions = MailKit.Security.SecureSocketOptions.Auto;
            initialMailConfigurationModel.ImapSettings.LoggerFolderPath = Path.Combine(Environment.CurrentDirectory, "imap_logs");
            initialMailConfigurationModel.ImapSettings.Timeout = 10000;
            initialMailConfigurationModel.SmtpSettings = new MailConfigurationModel.MailSettingsModel();
            initialMailConfigurationModel.SmtpSettings.Server = "smtp.strato.log";
            initialMailConfigurationModel.SmtpSettings.Port = 465;
            initialMailConfigurationModel.SmtpSettings.SecureSocketOptions = MailKit.Security.SecureSocketOptions.Auto;
            initialMailConfigurationModel.SmtpSettings.LoggerFolderPath = Path.Combine(Environment.CurrentDirectory, "smtp_logs");
            initialMailConfigurationModel.SmtpSettings.Timeout = 10000;

            DatabaseConfigurationModel initialDatabaseConfigurationModel = new DatabaseConfigurationModel();
            initialDatabaseConfigurationModel.Host = "127.0.0.1";
            initialDatabaseConfigurationModel.Port = 3306;
            initialDatabaseConfigurationModel.Database = "jellyfish";
            initialDatabaseConfigurationModel.User = "jellyfish";
            initialDatabaseConfigurationModel.Password = "meinDatabasePassword!";
            initialDatabaseConfigurationModel.Timeout = 300;
            initialDatabaseConfigurationModel.ConvertZeroDateTime = true;
            initialDatabaseConfigurationModel.OldGuids = true;

            DatabaseConfigurationModel initialNodeManagerDatabaseConfigurationModel = new DatabaseConfigurationModel();
            initialNodeManagerDatabaseConfigurationModel.Host = "127.0.0.1";
            initialNodeManagerDatabaseConfigurationModel.Port = 3306;
            initialNodeManagerDatabaseConfigurationModel.Database = "api_gateway";
            initialNodeManagerDatabaseConfigurationModel.User = "rest";
            initialNodeManagerDatabaseConfigurationModel.Password = "meinDatabasePassword!";
            initialNodeManagerDatabaseConfigurationModel.Timeout = 300;
            initialNodeManagerDatabaseConfigurationModel.ConvertZeroDateTime = true;
            initialNodeManagerDatabaseConfigurationModel.OldGuids = true;

            AmpqConfigurationModel initialRabbitMqConfigurationModel = new AmpqConfigurationModel();
            initialRabbitMqConfigurationModel.Host = "rabbitmq-ampq01";
            initialRabbitMqConfigurationModel.Port = 5672;
            initialRabbitMqConfigurationModel.User = "jellyfish";
            initialRabbitMqConfigurationModel.Password = "admin1234";
            initialRabbitMqConfigurationModel.VirtualHost = "webapi";
            initialRabbitMqConfigurationModel.HeartBeatMs = 30000;

            SignalRConfigurationModel initialSignalRConfigurationModel = new SignalRConfigurationModel();
            initialSignalRConfigurationModel.UseLocalHub = false;
            initialSignalRConfigurationModel.DebugErrorsDetailedClientside = false;
            initialSignalRConfigurationModel.TimoutTimeSec = 15;
            initialSignalRConfigurationModel.KeepaliveTimeout = 15;
            initialSignalRConfigurationModel.ClientTimeoutSec = 30;
            initialSignalRConfigurationModel.HandshakeTimeout = 5;
            initialSignalRConfigurationModel.MaximumParallelInvocationsPerClient = 1;

            CacheConfigurationModel initialCacheConfigurationModel = new CacheConfigurationModel();
            initialCacheConfigurationModel.Hosts = new CacheHostConfigurationModel[] {
                new CacheHostConfigurationModel{ Host="redis-cache01",Port=6379,Timeout=20, User="",Password="test"},
                new CacheHostConfigurationModel{Host="redis-cache01",Port=6379,Timeout=20, User="",Password="test"},
                new CacheHostConfigurationModel{Host="redis-cache01",Port=6379,Timeout=20, User="",Password="test"},
                new CacheHostConfigurationModel{Host="redis-cache01",Port=6379,Timeout=20, User="",Password="test"},
                new CacheHostConfigurationModel{Host="redis-cache01",Port=6379,Timeout=20, User="",Password="test"},
                new CacheHostConfigurationModel{Host="redis-cache01",Port=6379,Timeout=20, User="",Password="test"},
            };

            WebApiConfigurationModel initialWebApiConfigurationModel = new WebApiConfigurationModel();
            initialWebApiConfigurationModel.Encoding = "UTF-8";


            //merged alle config in eine json namens: appservice.json
            MainConfigurationModel initialAppServiceConfigurationModel = new MainConfigurationModel(rootDir);
            initialAppServiceConfigurationModel.AntivirusConfigurationModel = initialAntiVirusConfigurationModel;
            initialAppServiceConfigurationModel.DatabaseConfigurationModel = initialDatabaseConfigurationModel;
            initialAppServiceConfigurationModel.NodeManagerDatabaseConfigurationModel = initialNodeManagerDatabaseConfigurationModel;
            initialAppServiceConfigurationModel.ApiSecurityConfigurationModel = initialApiSecurityConfigurationModel;
            initialAppServiceConfigurationModel.WebApiConfigurationModel = initialWebApiConfigurationModel;
            initialAppServiceConfigurationModel.LogConfigurationModel = initialLogConfigurationModel;
            initialAppServiceConfigurationModel.MailConfigurationModel = initialMailConfigurationModel;
            initialAppServiceConfigurationModel.CacheConfigurationModel = initialCacheConfigurationModel;
            initialAppServiceConfigurationModel.RabbitMqConfigurationModel = initialRabbitMqConfigurationModel;
            initialAppServiceConfigurationModel.SignalRHubConfigurationModel = initialSignalRConfigurationModel;

            #endregion Initial Configurations
            return initialAppServiceConfigurationModel;
        }

        public override IConfiguration LoadConfiguration(IConfiguration previousConfig, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env, IMainConfigurationModel cfg)
        {
            Console.WriteLine("ContentRootPath: " + env.ContentRootPath + "");
            string basePath = Path.Combine(env.ContentRootPath, "Config");
            string appsettingsFile = Path.Combine(basePath, "appsettings.json");
            var configurationBuilder = new ConfigurationBuilder().
                SetBasePath(env.ContentRootPath).
                AddCustomWebApiConfig<AbstractConfigurationModel>(basePath, (AbstractConfigurationModel)cfg).
                AddJsonFile(appsettingsFile).

                AddEnvironmentVariables();
            var config = configurationBuilder.AddConfiguration(previousConfig).Build();
#if DEBUG
            var envVars = Environment.GetEnvironmentVariables();
            Console.WriteLine("!!! Environment Vars output only in DEBUG Mode !!!");
            foreach (var envVar in envVars.Keys)
            {
                Console.WriteLine("ENV: " + envVar + ":" + envVars[envVar].ToString());
            }
            Console.WriteLine("!!! Environment Vars output only in DEBUG Mode !!!");
            Console.WriteLine("!!! Config Vars output only in DEBUG Mode !!!");
            foreach (var c in config.AsEnumerable().ToList())
            {
                Console.WriteLine("CONFIG: " + c.Key + ":" + c.Value);
            }
            Console.WriteLine("!!! Config Vars output only in DEBUG Mode !!!");
#endif
            return config;
        }
    }
}
