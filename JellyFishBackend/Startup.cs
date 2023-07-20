using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Abstractions;
using System.Linq;
using System.Reflection;
using System;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using Microsoft.AspNetCore.HttpOverrides;
using System.Linq.Expressions;
using System.IO;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net;
using Microsoft.AspNetCore.Http;
using WebApiFunction.Mail;
using WebApiFunction.Data.Web.MIME;
using WebApiFunction.Application.Controller.Modules;
using WebApiFunction.Application.Model.Internal;
using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Ampq.Rabbitmq.Data;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Antivirus;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Application.Model.DataTransferObject.Helix.Frontend.Transfer;
using WebApiFunction.Application.Model.DataTransferObject;
using WebApiFunction.Application.Model;
using WebApiFunction.Configuration;
using WebApiFunction.Collections;
using WebApiFunction.Data;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiFunction.Database;
using WebApiFunction.Web.AspNet.Filter;
using WebApiFunction.Formatter;
using WebApiFunction.LocalSystem.IO.File;
using WebApiFunction.Log;
using WebApiFunction.Metric;
using WebApiFunction.Metric.Influxdb;
using WebApiFunction.MicroService;
using WebApiFunction.Network;
using WebApiFunction.Security;
using WebApiFunction.Security.Encryption;
using WebApiFunction.Threading;
using WebApiFunction.Threading.Service;
using WebApiFunction.Threading.Task;
using WebApiFunction.Utility;
using WebApiFunction.Web;
using WebApiFunction.Web.AspNet;
using WebApiFunction.Web.Authentification;
using WebApiFunction.Web.Http.Api.Abstractions.JsonApiV1;
using WebApiFunction.Web.Http;
using WebApiFunction.Web.AspNet.Healthcheck;
using WebApiFunction.Application;
using WebApiFunction.Web.Authentification.JWT;
using WebApiFunction.Database.Dapper.Converter;
using Microsoft.AspNetCore.SignalR;
using WebApiFunction.Web.Websocket.SignalR.HubService;
using WebApiFunction.Startup;
using WebApiApplicationServiceV2;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using JellyFishBackend.Middleware;
using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;
using JellyFishBackend.SignalR.Hub;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Formatters;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Hosting;
using WebApiFunction.Web.AspNet.Controller;
using Microsoft.OpenApi.Models;
using WebApiFunction.Web.AspNet.Swagger.OperationFilter;
using WebApiFunction.Web.AspNet.Swagger.SignalR;

namespace JellyFishBackend
{
    public class Startup : IWebApiStartup
    {
        readonly string AllowOrigin = "api-gateway";
        public IConfiguration Configuration { get; }
        public static string[] DatabaseEntityNamespaces { get; } = new string[] { "WebApiFunction.Application.Model.Database.MySQL.Jellyfish", "WebApiFunction.Application.Model.Database.MySQL.Table", "WebApiFunction.Application.Model.Database.MySQL.View" };

        public Startup(IConfiguration configuration, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env)
        {
            #region Initial Configurations
            ApiSecurityConfigurationModel initialApiSecurityConfigurationModel = new ApiSecurityConfigurationModel();
            initialApiSecurityConfigurationModel.ApiContentType = GeneralDefs.ApiContentType;
            initialApiSecurityConfigurationModel.Jwt = new ApiSecurityConfigurationModel.JsonWebTokenModel();
            initialApiSecurityConfigurationModel.Jwt.JwtBearerSecretStr = "this is my custom Secret key for authnetication";
            initialApiSecurityConfigurationModel.SiteProtect = new ApiSecurityConfigurationModel.SiteProtectModel();
            initialApiSecurityConfigurationModel.SiteProtect.MaxHttpContentLen =0;
            initialApiSecurityConfigurationModel.SiteProtect.MaxHttpHeaderFieldLen =255;
            initialApiSecurityConfigurationModel.SiteProtect.MaxHttpHeaderFieldValueLen =255;
            initialApiSecurityConfigurationModel.SiteProtect.MaxHttpRequUriLen =255;

            AntivirusConfigurationModel initialAntiVirusConfigurationModel = new AntivirusConfigurationModel();
            initialAntiVirusConfigurationModel.DeleteInfectedFilesPermantly =true;
            initialAntiVirusConfigurationModel.Host = "localhost";
            initialAntiVirusConfigurationModel.Port=3310;

            LogConfigurationModel initialLogConfigurationModel = new LogConfigurationModel();
            initialLogConfigurationModel.LogdateFormat = "yyyy-MM-dd";
            initialLogConfigurationModel.LogtimeFormat = "HH:mm:ss";
            initialLogConfigurationModel.LogLevel = General.MESSAGE_LEVEL.LEVEL_INFO;
            initialLogConfigurationModel.UserInterfaceDateFormat = "yyyy-MM-dd";
            initialLogConfigurationModel.UserInterfaceTimeFormat = "HH:mm:ss";

            MailConfigurationModel initialMailConfigurationModel = new MailConfigurationModel();

            initialMailConfigurationModel.EmailAttachmentPath ="";
            //IMAP
            initialMailConfigurationModel.ImapSettings =new MailConfigurationModel.MailSettingsModel();
            initialMailConfigurationModel.ImapSettings.User = "mail@roos-it.net";
            initialMailConfigurationModel.ImapSettings.Password = "aChJz8nPf5dXZsa";
            initialMailConfigurationModel.ImapSettings.Server = "imap.strato.de";
            initialMailConfigurationModel.ImapSettings.Port =993;
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
            initialDatabaseConfigurationModel.Host = "localhost";
            initialDatabaseConfigurationModel.Port = 3306;
            initialDatabaseConfigurationModel.Database = "jellyfish";
            initialDatabaseConfigurationModel.User = "jellyfish";
            initialDatabaseConfigurationModel.Password = "meinDatabasePassword!";
            initialDatabaseConfigurationModel.Timeout = 300;
            initialDatabaseConfigurationModel.ConvertZeroDateTime = true;
            initialDatabaseConfigurationModel.OldGuids = true;

            DatabaseConfigurationModel initialNodeManagerDatabaseConfigurationModel = new DatabaseConfigurationModel();
            initialNodeManagerDatabaseConfigurationModel.Host = "localhost";
            initialNodeManagerDatabaseConfigurationModel.Port = 3306;
            initialNodeManagerDatabaseConfigurationModel.Database = "rest_api";
            initialNodeManagerDatabaseConfigurationModel.User = "rest";
            initialNodeManagerDatabaseConfigurationModel.Password = "meinDatabasePassword!";
            initialNodeManagerDatabaseConfigurationModel.Timeout = 300;
            initialNodeManagerDatabaseConfigurationModel.ConvertZeroDateTime = true;
            initialNodeManagerDatabaseConfigurationModel.OldGuids = true;

            AmpqConfigurationModel initialRabbitMqConfigurationModel = new AmpqConfigurationModel();
            initialRabbitMqConfigurationModel.Host = "localhost";
            initialRabbitMqConfigurationModel.Port = 5672;
            initialRabbitMqConfigurationModel.User = "jellyfish";
            initialRabbitMqConfigurationModel.Password = "admin1234";
            initialRabbitMqConfigurationModel.VirtualHost = "webapi";
            initialRabbitMqConfigurationModel.HeartBeatMs = 30000;

            SignalRConfigurationModel initialSignalRConfigurationModel = new SignalRConfigurationModel();
            initialSignalRConfigurationModel.UseLocalHub  = false;
            initialSignalRConfigurationModel.DebugErrorsDetailedClientside  = false;
            initialSignalRConfigurationModel.TimoutTimeSec  = 15;
            initialSignalRConfigurationModel.KeepaliveTimeout  = 15;
            initialSignalRConfigurationModel.ClientTimeoutSec  = 30;
            initialSignalRConfigurationModel.HandshakeTimeout  = 5;
            initialSignalRConfigurationModel.MaximumParallelInvocationsPerClient  = 1;

            CacheConfigurationModel initialCacheConfigurationModel = new CacheConfigurationModel();
            initialCacheConfigurationModel.Hosts = new CacheHostConfigurationModel[] {
                new CacheHostConfigurationModel{ Host="10.0.0.77",Port=7300,Timeout=20, User="",Password="test"},
                new CacheHostConfigurationModel{Host="10.0.0.77",Port=7301,Timeout=20, User="",Password="test"},
                new CacheHostConfigurationModel{Host="10.0.0.77",Port=7302,Timeout=20, User="",Password="test"},
                new CacheHostConfigurationModel{Host="10.0.0.77",Port=7303,Timeout=20, User="",Password="test"},
                new CacheHostConfigurationModel{Host="10.0.0.77",Port=7304,Timeout=20, User="",Password="test"},
                new CacheHostConfigurationModel{Host="10.0.0.77",Port=7305,Timeout=20, User="",Password="test"},
            };

            WebApiConfigurationModel initialWebApiConfigurationModel = new WebApiConfigurationModel();
            initialWebApiConfigurationModel.Encoding = "UTF-8";


            //merged alle config in eine json namens: appservice.json
            AppServiceConfigurationModel initialAppServiceConfigurationModel = new AppServiceConfigurationModel(env.ContentRootPath);
            initialAppServiceConfigurationModel.AntivirusConfigurationModel = initialAntiVirusConfigurationModel;
            initialAppServiceConfigurationModel.DatabaseConfigurationModel = initialDatabaseConfigurationModel;
            initialAppServiceConfigurationModel.NodeManagerDatabaseConfigurationModel = initialNodeManagerDatabaseConfigurationModel;
            initialAppServiceConfigurationModel.ApiSecurityConfigurationModel =initialApiSecurityConfigurationModel;
            initialAppServiceConfigurationModel.WebApiConfigurationModel = initialWebApiConfigurationModel;
            initialAppServiceConfigurationModel.LogConfigurationModel = initialLogConfigurationModel;
            initialAppServiceConfigurationModel.MailConfigurationModel = initialMailConfigurationModel;
            initialAppServiceConfigurationModel.CacheConfigurationModel = initialCacheConfigurationModel;
            initialAppServiceConfigurationModel.RabbitMqConfigurationModel = initialRabbitMqConfigurationModel;
            initialAppServiceConfigurationModel.SignalRHubConfigurationModel = initialSignalRConfigurationModel;

            #endregion Initial Configurations
            Console.WriteLine("ContentRootPath: "+ env.ContentRootPath + "");
            string basePath = Path.Combine(env.ContentRootPath, "Config");
            string appsettingsFile = Path.Combine(basePath, "appsettings.json");
            var configurationBuilder = new ConfigurationBuilder().
                SetBasePath(env.ContentRootPath).
                AddCustomWebApiConfig<AppServiceConfigurationModel>(basePath, initialAppServiceConfigurationModel).
                AddJsonFile(appsettingsFile).
                
                AddEnvironmentVariables();
#if DEBUG
            var envVars = Environment.GetEnvironmentVariables();
            Console.WriteLine("!!! Environment Vars output only in DEBUG Mode !!!");
            foreach (var envVar in envVars.Keys)
            {
                Console.WriteLine("ENV: "+ envVar + ":" + envVars[envVar].ToString());
            }
            Console.WriteLine("!!! Environment Vars output only in DEBUG Mode !!!");
#endif
            var config = configurationBuilder.AddConfiguration(configuration).Build();
            Configuration = config;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
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
                    Title = "Jellyfish API",
                    Description = "API for the Jellyfish Mobile App",
                    TermsOfService = new Uri("https://jellyfish.roos-it.net/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Mika Roos",
                        Url = new Uri("https://jellyfish.roos-it.net/contact")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "GPL2",
                        Url = new Uri("https://jellyfish.roos-it.net/license")
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
            ISingletonNodeDatabaseHandler databaseHandler = serviceProvider.GetService<ISingletonNodeDatabaseHandler>();
            INodeManagerHandler nodeManager = serviceProvider.GetService<INodeManagerHandler>();
            IAppconfig appConfig = serviceProvider.GetService<IAppconfig>();

            app.UseEndpoints(endpoints =>
            {

                //endpoints.MapHub<MessengerHub>("/messenger");
                if (appConfig.AppServiceConfiguration.SignalRHubConfigurationModel != null && appConfig.AppServiceConfiguration.SignalRHubConfigurationModel.UseLocalHub)
                    endpoints.RegisterSignalRHubs(serviceProvider);//signalr init before register backend for route register
                endpoints.RegisterBackend(nodeManager, serviceProvider, env, databaseHandler, actionDescriptorCollectionProvider, Configuration, DatabaseEntityNamespaces,HubServiceExtensions.RegisteredHubServices);

            });
        }
    }
}
