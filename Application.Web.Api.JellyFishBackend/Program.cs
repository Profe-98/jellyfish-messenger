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
                    try
                    {
                        var transformToValidUriStr = envVarAspNetCoreUrls.Replace("+","tmphostname");
                        if (Uri.TryCreate(transformToValidUriStr, UriKind.RelativeOrAbsolute, out Uri result))
                        {
                            if(result.Port == 443 && !String.IsNullOrWhiteSpace(envVarAspNetCoreSslPfxPath))
                            {
                                webBuilder.ConfigureKestrel(opt =>
                                {
                                    Console.WriteLine("[SSL]: Init ssl encryption process...");
                                    if (File.Exists(envVarAspNetCoreSslPfxPath))
                                    {
                                        try
                                        {

                                            Console.WriteLine("[SSL]: Following pfx-file found: " + envVarAspNetCoreSslPfxPath + "");
                                            var port = 443;
                                            // The password you specified when exporting the PFX file using OpenSSL.
                                            // This would normally be stored in configuration or an environment variable;
                                            // I've hard-coded it here just to make it easier to see what's going on.

                                            Console.WriteLine("[SSL]: try to read pfx and setup encryption");
                                            opt.Listen(IPAddress.Any, port, listenOptions => {
                                                listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
                                                // Configure Kestrel to use a certificate from a local .PFX file for hosting HTTPS
                                                if(String.IsNullOrWhiteSpace(envVarAspNetCoreSslPfxPassword))
                                                {

                                                    Console.WriteLine("[SSL-WARN]: try to open pfx without password, environment var 'ASPNETCORE_SSL_PFX_PASSWORD' doesnt filled with any string");
                                                }
                                                listenOptions.UseHttps(envVarAspNetCoreSslPfxPath, envVarAspNetCoreSslPfxPassword);
                                            });
                                            Console.WriteLine("[SSL]: kestrel is now listening on port 443 with ssl encryption");
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine("[SSL-ERR]: exception happend by reading pfx");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("[SSL-ERR]: pfx-file not found: " + envVarAspNetCoreSslPfxPath + "");
                                    }




                                });
                                


                            }
                            else
                            {

                                Console.WriteLine("[SSL]: start kestrel without ssl encryption");
                            }
                        }
                    }
                    catch 
                    {

                    }
                    webBuilder.UseStartup<Startup>();
                })
            ;
    }
}
