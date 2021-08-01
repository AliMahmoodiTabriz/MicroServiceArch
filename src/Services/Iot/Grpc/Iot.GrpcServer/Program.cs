using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Iot.GrpcServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(opt=> {
                        opt.ListenAnyIP(6300, listenOptions => listenOptions.Protocols = HttpProtocols.Http2);//grpc
                        //opt.ListenAnyIP(8300, listenOptions => listenOptions.Protocols = HttpProtocols.Http1);//web api
                    }).
                    UseStartup<Startup>();
                });
    }
}
