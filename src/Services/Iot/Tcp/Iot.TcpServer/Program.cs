using Iot.TcpServer.Concrete;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Iot.TcpServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostConfig, service) => 
                {
                    service.AddHostedService<ConsoleHostedService>();
                    service.AddSingleton(opt =>
                    {
                        return new Server(IPAddress.Any.ToString(),9800);
                    });
                }).RunConsoleAsync();
        }
    }

    internal sealed class ConsoleHostedService : IHostedService
    {
        private Server _server;
        private IHostApplicationLifetime _applicationLifetime;

        public ConsoleHostedService(Server server, IHostApplicationLifetime applicationLifetime)
        {
            _server = server;
            _applicationLifetime = applicationLifetime;
        }
        private async Task StartGetData()
        {
            while (true)
            {
                var data =await _server.GetData();
                if(data!=null)
                {

                }
                else
                    Thread.Sleep(20);
            }
        }

        private async Task StartSetData()
        {

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _applicationLifetime.ApplicationStarted.Register(() => {
                try
                {
                    Task.Run(async () => {

                        await StartGetData();
                    });
                }
                catch (Exception)
                {

                    throw;
                }
            
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
