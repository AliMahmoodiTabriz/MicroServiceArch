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
        private ConcurrentQueue<TcpData> _tcpDatas;
        public Server(string address, int port) : base(address, port)
        {
            _tcpDatas = new ConcurrentQueue<TcpData>();
            Start();
        }

        protected override TcpSession CreateSession()
        {
            ClinetSession session = new ClinetSession(this);
            return session;
        }

        public void AddQueue(TcpData tcpData)
        {
            _tcpDatas.Enqueue(tcpData);
        }

        public async Task<TcpData> GetData()
        {
            if (!(_tcpDatas.Count > 0))
                return null;

            return await Task.Run(() =>
            {
                if(_tcpDatas.TryDequeue(out TcpData data))
                {
                    return data;
                }
                return null;
            });
        }
        public async Task<bool> Send(TcpData tcpData)
        {
            return await Task.Run(() =>
            {
                TcpSession session = FindSession(tcpData.SessionId);
                if(session !=null && session.IsConnected)
                {
                    return session.SendAsync(tcpData.Buffer);
                }
                else
                {
                    return false;
                }
            });
        }
    }
}
