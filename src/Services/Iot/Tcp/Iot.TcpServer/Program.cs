using Grpc.Core;
using Iot.Grpc;
using Iot.TcpServer.Concrete;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using static Iot.Grpc.TcpBufferService;

namespace Iot.TcpServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostConfig, service) => 
                {
                    service.AddGrpcClient<TcpBufferServiceClient>(opt=> {
                        opt.Address = new Uri("http://127.0.0.1:6300");
                    });
                    service.AddHostedService<ConsoleHostedService>();
                    service.AddSingleton(opt =>
                    {
                        return new Concrete.Server(IPAddress.Any.ToString(),9800);
                    });
                }).RunConsoleAsync();
        }
    }

    internal sealed class ConsoleHostedService : IHostedService
    {
        private Concrete.Server _server;
        private IHostApplicationLifetime _applicationLifetime;
        private TcpBufferServiceClient _tcpBufferService;

        public ConsoleHostedService(Concrete.Server server, IHostApplicationLifetime applicationLifetime, TcpBufferServiceClient tcpBufferService)
        {
            _server = server;
            _applicationLifetime = applicationLifetime;
            _tcpBufferService = tcpBufferService;
        }
        private async Task StartGetData()
        {
            try
            {
                using (var caller = _tcpBufferService.GetTcpBuffer(new Google.Protobuf.WellKnownTypes.Empty()))
                {
                    await foreach (var data in caller.ResponseStream.ReadAllAsync())
                    {
                        await _server.Send(data);
                    }
                }
            }
            catch (Exception ex)
            {
                Thread.Sleep(200);
                StartGetData();
            }
        }

        private async Task StartSetData()
        {
            try
            {
                using (var caller = _tcpBufferService.SetTcpBuffer())
                {
                    while (true)
                    {
                        var data = await _server.GetData();
                        if (data != null)
                        {
                            await caller.RequestStream.WriteAsync(data);
                        }
                        else
                            Thread.Sleep(20);
                    }
                }

            }
            catch (Exception ex)
            {
                Thread.Sleep(200);
                StartSetData();
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _applicationLifetime.ApplicationStarted.Register(() => {
                try
                {
                    //Task.Run(async () => {

                    //    await StartGetData();
                    //});
                    Task.Run(async () => {

                        await StartSetData();
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
