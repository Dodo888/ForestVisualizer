using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ForestSolver;
using ForestSolverPackages;
using log4net;
using log4net.Config;
using Point = ForestSolverPackages.Point;

namespace Server
{
    class PlayerBot
    {
        public readonly TcpClient Client;
        public readonly ForestKeeper Keeper;
        public PlayerBot(TcpClient client, ForestKeeper keeper)
        {
            Client = client;
            Keeper = keeper;
        }
    }
    class ServersConnection
    {
        private readonly TcpListener listener;
        private readonly ServerWorker serverWorker;
        private static readonly ILog Log = LogManager.GetLogger(typeof(ServersConnection));
        private TcpClient visualizer;
        private readonly Dictionary<TcpClient, string> clients;
        private readonly List<PlayerBot> players;

        public ServersConnection(ServerWorker serverWorker, IPAddress address, int port)
        {
            XmlConfigurator.Configure();
            this.serverWorker = serverWorker;
            ThreadPool.SetMinThreads(8, 8);
            listener = new TcpListener(address, port);
            clients = new Dictionary<TcpClient, string>();
            players = new List<PlayerBot>();
        }

        public void Start()
        {
            listener.Start();
            Log.InfoFormat("Server start!");
            Begin();
            Log.InfoFormat("All paticipants connected");
            foreach (var client in clients)
            {
                InitializeClient(client.Key, client.Value);
            }
            InitializeVisualizer();
            Log.InfoFormat("All clients initialize");
            RunGame();
            Log.InfoFormat("Game over");
        }

        private void InitializeVisualizer()
        {
            var playersForVis =
                players.Select(
                    x =>
                        new Player(x.Keeper.Id, x.Keeper.Name, x.Keeper.Position.ConvertToNetPoint(),
                            x.Keeper.Destination.ConvertToNetPoint(), x.Keeper.Hp)).ToArray();
            var map = serverWorker.GetMap();
            var worldInfo = new WorldInfo {Map = map, Players = playersForVis};
            JSon.Write(worldInfo, visualizer.GetStream());
        }

        private TcpClient WaitClientConnect(out Hello packet)
        {
            var client = listener.AcceptTcpClient();
            NetworkStream stream = client.GetStream();
            packet = JSon.Read<Hello>(stream);
            return client;
        }

        private void Begin()
        {
            int paticipantsCount = 0;
            while (paticipantsCount < serverWorker.MaxPaticipants || visualizer == null)
            {
                Hello packet;
                try
                {
                    TcpClient client = WaitClientConnect(out packet);
                    if (packet.IsVisualizator)
                        visualizer = client;
                    else
                    {
                        if (paticipantsCount < serverWorker.MaxPaticipants)
                            clients.Add(client, packet.Name);
                        paticipantsCount++;
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private void InitializeClient(TcpClient client, string name)
        {
            var rr = CreateClientInfo(name);
            var clientInfo = rr.Item1;
            var keeper = rr.Item2;
            JSon.Write(clientInfo, client.GetStream());
            players.Add(new PlayerBot(client, keeper));
        }

        private void RunGame()
        {
            while (true)
            {
                var visStream = visualizer.GetStream();
                var ans = JSon.Read<Answer>(visStream);
                if (ans.AnswerCode == 0)
                {
                    foreach (var playerBot in players)
                        RunOneStep(playerBot);
                }
                var lastMoveInfo = CreateLastMoveInfo();
                JSon.Write(lastMoveInfo, visStream);
                if (serverWorker.IsOver)
                {
                    foreach (var player in players)
                    {
                        player.Client.Close();
                    }
                    listener.Stop();
                    break;
                }
            }
        }

        private LastMoveInfo CreateLastMoveInfo()
        {
            var changedPositions = players
                .Select(x => Tuple.Create(x.Keeper.Id, x.Keeper.Position.ConvertToNetPoint(), x.Keeper.Hp))
                .ToArray();
            var changedCells = serverWorker.GetChangedCells(changedPositions);
            var lastMoveInfo = new LastMoveInfo
            {
                ChangedCells = changedCells,
                GameOver = serverWorker.IsOver,
                PlayersChangedPosition = changedPositions
            };
            return lastMoveInfo;
        }

        private void RunOneStep(PlayerBot player)
        {
            try
            {

                Log.InfoFormat("{0} position {1} {2}", player.Keeper.Name, player.Keeper.Position.X,
                    player.Keeper.Position.Y);
                var move = JSon.Read<Move>(player.Client.GetStream());
                Log.InfoFormat("{0} move {1}", player.Keeper.Name, move.Direction);
                var res = 1;
                if (serverWorker.Move(move.Direction, player.Keeper))
                    res = 0;
                if (serverWorker.IsOver)
                    res = 2;
                var visibleMap = serverWorker.GetVisibleMap(player.Keeper);
                var result = new MoveResultInfo
                {
                    Result = res,
                    VisibleMap = visibleMap
                };
                JSon.Write(result, player.Client.GetStream());
                Log.InfoFormat("{0} can move {1}", player.Keeper.Name, result.Result == 0);
            }
            catch (Exception)
            {
                players.Remove(player);
                player.Client.Close();
            }
        }

        private Tuple<ClientInfo, ForestKeeper> CreateClientInfo(string name)
        {
            var a = serverWorker.AddClient(name);
            var player = a.Item1;
            var keeper = a.Item2;
            var visibleMap = serverWorker.GetVisibleMap(keeper);
            var clientInfo = new ClientInfo
            {
                MapSize = new Point(serverWorker.Forest.Field.GetLength(0), serverWorker.Forest.Field.GetLength(1)),
                Hp = player.Hp,
                StartPosition = player.StartPosition,
                Target = player.Target,
                VisibleMap = visibleMap
            };
            return Tuple.Create(clientInfo, keeper);
        }
    }
}
