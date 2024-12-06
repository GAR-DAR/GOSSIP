using GOSSIP.Net.IO;
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

        public byte Signal { get; set; } = 255;

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

        public byte ReadSignal()
        {
            if (_networkStream.CanRead && _networkStream.DataAvailable)
            {
                Signal = (byte)_networkStream.ReadByte();
            }
            return Signal;
        }

        public byte[] ReadRawPacket()
        {
            if (!_networkStream.CanRead)
            {
                //Console.WriteLine("Cannot read");
                return null;
            }

            if (!_networkStream.DataAvailable)
            {
                //Console.WriteLine("No data available");
                return null;
            }

            var lengthBuffer = new byte[4];
            _networkStream.Read(lengthBuffer, 0, 4);
            int length = BitConverter.ToInt32(lengthBuffer, 0);

            var dataBuffer = new byte[length];
            _networkStream.Read(dataBuffer, 0, length);

            return dataBuffer;
        }

        public T DeserializePacket<T>(byte[] dataBuffer)
        {
            var json = Encoding.UTF8.GetString(dataBuffer);
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (JsonReaderException ex)
            {
                throw;
            }
        }


        public void ClearStream()
        {
            _networkStream.Flush();
        }

    }

}
