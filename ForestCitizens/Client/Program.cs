using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ForestCitizens;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect("localhost", 6002);
            Serializer.SerializeAndSend(socket, new Hello {IsVisualizator = false, Name = args.Length == 0 ? "Benjamin" : args[0]});
            var ai = new Ai(Serializer.Deserialize<ClientInfo>(socket));
            Serializer.SerializeAndSend(socket, ai.GetInitMove());
            while (true)
            {
                var info = Serializer.Deserialize<MoveResultInfo>(socket);
                Console.WriteLine("RESULT: " + info.Result);
                Console.WriteLine(args[0]);
                if (info.Result == 2)
                    break;
                Serializer.SerializeAndSend(socket, ai.GetMove(info));
            }
            Console.WriteLine("Game is over.");
            Console.ReadKey();
        }
    }
}
