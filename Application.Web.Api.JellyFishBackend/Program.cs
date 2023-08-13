using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Linq.Expressions;
using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Application.Shared.Kernel.Web.AspNet.Startup;

namespace Application.Web.Api.JellyFishBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var hostBuilder = CreateHostBuilder(args);
            hostBuilder.Build().Run();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole(o => o.TimestampFormat = "[hh:mm:ss] ") ;
                    /*logging.AddJsonConsole(options =>
                    {
                        options.IncludeScopes = false;
                        options.TimestampFormat = "hh:mm:ss ";
                        options.JsonWriterOptions = new JsonWriterOptions
                        {
                            Indented = true
                        };
                    });*/
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var envVarAspNetCoreUrls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
                    var envVarAspNetCoreSslPfxPassword = Environment.GetEnvironmentVariable("ASPNETCORE_SSL_PFX_PASSWORD");
                    var envVarAspNetCoreSslPfxPath = Environment.GetEnvironmentVariable("ASPNETCORE_SSL_PFX_PATH");
                    webBuilder.ConfigureTrafficLayerSecurity(envVarAspNetCoreUrls, envVarAspNetCoreSslPfxPassword, envVarAspNetCoreSslPfxPath);
                    webBuilder.UseStartup<Startup>();
                })
            ;
    }
}
