using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GOSSIP.Net.IO
{
    public class PacketReader
    {
        private readonly NetworkStream _networkStream;

        public PacketReader(NetworkStream networkStream)
        {
            _networkStream = networkStream;
        }

        public Packet<T> ReadPacket<T>()
        {
            var lengthBuffer = new byte[4];
            _networkStream.Read(lengthBuffer, 0, 4);

            int length = BitConverter.ToInt32(lengthBuffer, 0);
            var dataBuffer = new byte[length];

            _networkStream.Read(dataBuffer, 0, length);

            var json = Encoding.UTF8.GetString(dataBuffer);
            return JsonConvert.DeserializeObject<Packet<T>>(json);
        }
    }
}
