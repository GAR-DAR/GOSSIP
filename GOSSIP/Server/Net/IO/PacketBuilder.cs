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
        private byte _signal;

        public PacketBuilder()
        {
            _packet = new Packet<T>();
        }

        public byte[] GetPacketBytes(byte signal, T data)
        {

            _signal = signal;
            _packet.Data = data;

            var json = JsonConvert.SerializeObject(_packet);
            var dataBuffer = Encoding.UTF8.GetBytes(json); //kdfkdmjaflkdmfkldmfkldmfkldmfkfdmkfmd;lamk

            var lengthBuffer = BitConverter.GetBytes(dataBuffer.Length); //18 0 00 
            lengthBuffer[1] = _signal;

            var packetBytes = new byte[lengthBuffer.Length + dataBuffer.Length]; //18 000 kdfkdmjaflkdmfkldmfkldmfkldmfkfdmkfmd;lamk
            Buffer.BlockCopy(lengthBuffer, 0, packetBytes, 0, lengthBuffer.Length);
            Buffer.BlockCopy(dataBuffer, 0, packetBytes, lengthBuffer.Length, dataBuffer.Length);

            return packetBytes;
        }


    }
} 