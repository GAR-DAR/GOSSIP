using Server.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Net.IO
{
    public class PacketBuilder<T>
    {
        private Packet<T> _packet;

        public PacketBuilder()
        {
            _packet = new Packet<T>();
        }

        public void WriteData(T data)
        {
            _packet.Data = data;
        }

        public string GetPacketJson()
        {
            return JsonConvert.SerializeObject(_packet);
        }
    }
}