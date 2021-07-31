using Iot.TcpServer.Entities;
using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iot.TcpServer.Concrete
{
    public class ClinetSession : TcpSession
    {
        private readonly Server _server;
        public ClinetSession(Server server):base(server)
        {
            _server = server;
        }
        protected override void OnConnecting()
        {
            Console.WriteLine(Id +" conected");
        }
        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            byte[] buf = new byte[(int)size];
            Array.Copy(buffer, buf, buf.Length);

            TcpData data = new TcpData
            {
                SessionId = Id,
                Ip=Socket.RemoteEndPoint.ToString(),
                Buffer=buf
            };
            _server.AddQueue(data);
        }
    }
}
