using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace HttpsTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            //Getting our cert credentials
                .ConfigureServices((context, services)=> {
                    HostConfig.CertPath = context.Configuration["CerthPath"];
                    HostConfig.CertPassword = context.Configuration["CertPassword"];
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var host = Dns.GetHostEntry("weather.io");
                    webBuilder.ConfigureKestrel(opt =>
                    {
                        opt.Listen(host.AddressList[0], 80);
                        //opt.ListenAnyIP(80);
                        opt.Listen(host.AddressList[0], 81, listOpt=>
                        {
                            listOpt.UseHttps(HostConfig.CertPath, HostConfig.CertPassword);
                        });
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }

    public static class HostConfig
    {
        public static string CertPath { get; set; }
        public static string CertPassword { get; set; }
    }
}
