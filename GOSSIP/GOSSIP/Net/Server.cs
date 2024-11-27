using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;
using GOSSIP.Net.IO;
using GOSSIP.Models;

namespace GOSSIP.Net
{
    public class Server
    {
        TcpClient _client;
       
        public PacketReader packetReader;

        public event Action connectedEvent;

        public event Action userModelEvent;

        public event Action userDisonnectedEvent;


        public Server()
        {
            _client = new TcpClient();
        }


        public void ConnectToServer()
        {
            _client.Connect("172.22.237.81", 7891);
            packetReader = new PacketReader(_client.GetStream()); //just constructor, nothing more

           
            ReadPackets();
        }

        public void ConnectToServer(UserModel user) 
        {
            if (!_client.Connected)
            {
                _client.Connect("172.22.237.81", 7891);
                packetReader = new PacketReader(_client.GetStream()); //just constructor, nothing more

                if (user != null)
                {
                    var connectionPacket = new PacketBuilder<UserModel>();

                    connectionPacket.WriteOpCode(0);

                    connectionPacket.WriteMessage(username);

                    connectionPacket.GetPacketBytes(0, user); // 0 is the signal for

                    SendUserModel

                    _client.Client.Send(connectionPacket.GetPacketBites());
                }

                ReadPackets();
                 
            }




        }

        private void ReadPackets()
        {
           Task.Run(() => // This is a new thread for proper synchronization
           {
               while (true)
               {
                   var opcode = packetReader.ReadByte();
                   switch (opcode)
                   {
                       case 1:
                           connectedEvent?.Invoke();
                           break;
                       case 5:
                           messageReceivedEvent?.Invoke();
                           break;
                       case 10:
                           userDisonnectedEvent?.Invoke();
                           break;
                       default:
                           Console.WriteLine("Unknown opcode");
                           break;
                   }

               }
           });
        }

        public void SendMessageToServer(string message)
        {
            var messagePacket = new PacketBuilder();
            messagePacket.WriteOpCode(5);
            messagePacket.WriteMessage(message);
            _client.Client.Send(messagePacket.GetPacketBites());
        }
    } 
}
