using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GOSSIP.Net.IO
{
    public class PacketBuilder<T> //where T : class
    {
        private Packet<T> _packet;
        private byte _signal;

        public PacketBuilder()
        {
            _packet = new Packet<T>();
        }

        public byte[] GetPacketBytes(SignalsEnum signal)
        {
            _signal = (byte)signal;
            var packetBytes = new byte[1];
            packetBytes[0] = _signal;
            return packetBytes;
        }

        public byte[] GetPacketBytes(SignalsEnum signal, T data)
        {
            _signal = (byte)signal;
            _packet.Data = data;

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            var json = JsonConvert.SerializeObject(_packet, settings);
            var dataBuffer = Encoding.UTF8.GetBytes(json);

            var lengthBuffer = BitConverter.GetBytes(dataBuffer.Length);

            var packetBytes = new byte[1 + lengthBuffer.Length + dataBuffer.Length];

            packetBytes[0] = _signal;

            Buffer.BlockCopy(lengthBuffer, 0, packetBytes, 1, lengthBuffer.Length);
            Buffer.BlockCopy(dataBuffer, 0, packetBytes, 1 + lengthBuffer.Length, dataBuffer.Length);

            return packetBytes;
        }
    }
} 