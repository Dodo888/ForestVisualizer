using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using ForestSolver;
using ForestSolverPackages;
using log4net;
using log4net.Config;

namespace Client
{
    class ClientConnection
    {
        private readonly ClientWorker clientWorker;
        private readonly TcpClient server;
        private readonly string name;
        private readonly ILog log = LogManager.GetLogger(typeof(ClientConnection));
        private bool isGameOver;
        private readonly IPAddress address;
        private readonly int port;

        public ClientConnection(ClientWorker clientWorker, string name, IPAddress address, int port)
        {
            XmlConfigurator.Configure();
            this.clientWorker = clientWorker;
            server = new TcpClient();
            this.name = name;
            isGameOver = false;
            this.address = address;
            this.port = port;
        }

        public void Begin()
        {
            log.Info("Start!!!");
            Initialize();
            RunGame();
        }

        private void Initialize()
        {
            server.Connect(address, port);
            log.Info("Connected to server");
            var stream = server.GetStream();
            var helloPacket = new Hello
            {
                IsVisualizator = false,
                Name = name
            };
            JSon.Write(helloPacket, stream);
            var clientInfo = JSon.Read<ClientInfo>(stream);
            clientWorker.Initialise(clientInfo, name);
        }

        private void RunGame()
        {
            while (!isGameOver)
                clientWorker.Move(TryMove);
        }


        private bool TryMove(ForestKeeper keeper, DeltaPoint point)
        {
            var stream = server.GetStream();
            var directoins = new Dictionary<DeltaPoint, int>
            {
                { DeltaPoint.GoUp(), 0 }, 
                { DeltaPoint.GoRight(), 1 }, 
                { DeltaPoint.GoDown(), 2 }, 
                { DeltaPoint.GoLeft(), 3 }
            };
            var move = new Move {Direction = directoins[point]};
            log.InfoFormat("{2} position {0} {1}", keeper.Position.X, keeper.Position.Y, name);
            log.InfoFormat("{1} tryed to {0}", move.Direction, name);
            JSon.Write(move, stream);
            var resultInfo = JSon.Read<MoveResultInfo>(stream);
            log.InfoFormat("{1} can move {0}", resultInfo.Result == 0, name);
            if (resultInfo.Result == 2)
            {
                isGameOver = true;
                log.Info("Game over!");
            }
            if (resultInfo.Result == 0)
                clientWorker.ChangePosition(point);
            return resultInfo.Result == 0;
        }

    }
}
