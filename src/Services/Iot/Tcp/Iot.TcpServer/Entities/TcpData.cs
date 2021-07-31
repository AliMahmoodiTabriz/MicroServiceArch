using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iot.TcpServer.Entities
{
    public class TcpData
    {
        public Guid SessionId { get; set; }

        public string Ip { get; set; }
        public byte[] Buffer { get; set; }
        public byte[] Data { get; set; }
    }
}
