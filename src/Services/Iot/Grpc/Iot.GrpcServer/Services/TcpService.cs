using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Iot.Grpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Iot.GrpcServer.Services
{
    public class TcpService:TcpBufferService.TcpBufferServiceBase
    {

        public override Task GetTcpBuffer(Empty request, IServerStreamWriter<TcpBuffers> responseStream, ServerCallContext context)
        {
            return base.GetTcpBuffer(request, responseStream, context);
        }

        public async override Task<Empty> SetTcpBuffer(IAsyncStreamReader<TcpBuffers> requestStream, ServerCallContext context)
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {
                try
                {
                    if(await requestStream.MoveNext())
                    {
                        var data = requestStream.Current;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($" SetTcpBuffer {ex.Message} ");
                }
            }

            return await Task.FromResult(new Empty());
        }
    }
}
