using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace ForestCitizens
{
    public static class Serializer
    {
        public static void SerializeAndSend<T>(Socket socket, T e)
        {
            Console.WriteLine("Sending " + e);
            var ms = new MemoryStream();
            using (var writer = new BsonWriter(ms))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(writer, e);
            }
            socket.Send(ms.ToArray());
        }

        public static T Deserialize<T>(Socket socket)
        {
            var buffer = new byte[1 << 20];
            socket.Receive(buffer);
            var ms = new MemoryStream(buffer);
            using (var reader = new BsonReader(ms))
            {
                var serializer = new JsonSerializer();
                var e = serializer.Deserialize<T>(reader);
                Console.WriteLine("Received " + JsonConvert.SerializeObject(e));
                return e;
            }
        }
    }

    public class Hello
    {
        public bool IsVisualizator;
        public string Name;
    }

    #region Работа с игроком

    public class ClientInfo
    {
        public Point MapSize; // x - height, y - width
        public int Hp;
        public Point StartPosition;
        public Point Target;
        public int[,] VisibleMap; // видимая часть карты в начале игры.
    }

    public class Move
    {
        public int Direction;
    }

    public class MoveResultInfo
    {
        public int Result; // 2 -- GameOver.
        public int[,] VisibleMap;
    }

    #endregion

    #region Работа с визуализатором


    public class WorldInfo
    {
        public Player[] Players;
        public int[,] Map;
    }

    public class Answer
    {
        public int AnswerCode;
    }

    public class LastMoveInfo
    {
        public bool GameOver;
        public Tuple<Point, int>[] ChangedCells;
        public Tuple<int, Point, int>[] PlayersChangedPosition; // <id, new position, new hp>
    }


    #endregion


    public class Point
    {
        protected bool Equals(Point other)
        {
            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X*397) ^ Y;
            }
        }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X;
        public int Y;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Point) obj);
        }
    }

    public class Player
    {
        public Player(int id, string nick, Point startPos, Point target, int hp)
        {
            Id = id;
            Nick = nick;
            StartPosition = startPos;
            Target = target;
            Hp = hp;
        }

        public int Id;
        public string Nick;
        public int Hp;
        public Point StartPosition;
        public Point Target;
    }
}
