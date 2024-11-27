using Server.Models;
using Server.Net.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


// 1 - 

namespace Server
{
    public class Client
    {
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }
        public UserModel userModel { get; set; }

        PacketReader _packetReader;

        public Client(TcpClient client)
        {
            ClientSocket = client;
            UID = Guid.NewGuid();

            _packetReader = new PacketReader(ClientSocket.GetStream());

            var userPacket = _packetReader.ReadPacket<UserModel>();
            userModel = userPacket.Data;

            Console.WriteLine($"{DateTime.Now} - Client {userModel.Username} connected :)");

            Task.Run(() => Process());
        }


        void Process()
        {
            while (true)
            {
                try
                {
                    //var opcode = _packetReader.ReadByte(); 
                    //switch (opcode)
                    //{
                    //    case 5:
                    //        var message = _packetReader.ReadMessage();
                    //        Console.WriteLine($"{DateTime.Now} - {userModel.Username} sent: {message}");
                    //        Program.BroadcastMessage(message);
                    //        break;
                    //}
                }
                catch (Exception)
                {
                    Console.WriteLine($"{DateTime.Now} - Client {userModel.Username} disconnected :(");
                    //Program.BroadcastDisconnect(UID.ToString());
                    ClientSocket.Close();
                    break;
                }
            }
        }
    }
}
