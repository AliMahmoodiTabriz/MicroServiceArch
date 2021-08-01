using Iot.Grpc;
using Iot.TcpServer.Entities;
using NetCoreServer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iot.TcpServer.Concrete
{
    public class Server : NetCoreServer.TcpServer
    {
        private ConcurrentQueue<TcpBuffers> _tcpDatas;
        public Server(string address, int port) : base(address, port)
        {
            _tcpDatas = new ConcurrentQueue<TcpBuffers>();
            Start();
        }

        protected override TcpSession CreateSession()
        {
            ClinetSession session = new ClinetSession(this);
            return session;
        }

        public void AddQueue(TcpBuffers tcpData)
        {
            _tcpDatas.Enqueue(tcpData);
        }

        public async Task<TcpBuffers> GetData()
        {
            if (!(_tcpDatas.Count > 0))
                return null;

            return await Task.Run(() =>
            {
                if(_tcpDatas.TryDequeue(out TcpBuffers data))
                {
                    return data;
                }
                return null;
            });
        }
        public async Task<bool> Send(TcpBuffers tcpData)
        {
            return await Task.Run(() =>
            {
                TcpSession session = FindSession(Guid.Parse(tcpData.SessionId));
                if(session !=null && session.IsConnected)
                {
                    return session.SendAsync(tcpData.Buffer.ToByteArray());
                }
                else
                {
                    return false;
                }
            });
        }
    }
}
