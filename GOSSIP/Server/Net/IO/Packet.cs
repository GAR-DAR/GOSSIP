using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Net.IO
{
    public class Packet<T>
    {
        public Guid PacketId { get; set; }
        public T Data { get; set; }
    }
}