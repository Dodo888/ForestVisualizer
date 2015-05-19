using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForestVisualizer
{
    public class Hello
    {
        public bool IsVisualizator;
        public string Name;
    }

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
}
