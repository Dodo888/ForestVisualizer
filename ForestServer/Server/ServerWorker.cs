using System;
using System.Collections.Generic;
using System.Linq;
using ForestSolver;
using ForestSolverPackages;
using Point = ForestSolver.Point;

namespace Server
{
    class ServerWorker
    {
        public readonly Forest Forest;
        private readonly List<Tuple<Point, Point>> patFirstPos;
        public readonly int MaxPaticipants;
        private int nextId;
        private readonly List<ForestKeeper> keepers;
        public bool IsOver;
        private readonly Dictionary<Type, int> cellsNum = new Dictionary<Type, int>
            {
                {typeof (Path), 1},
                {typeof (Wall), 2},
                {typeof (Trap), 3},
                {typeof (Life), 4}
            };

        public ServerWorker(Forest forest, List<Tuple<Point, Point>> patFirstPos)
        {
            Forest = forest;
            this.patFirstPos = patFirstPos;
            nextId = 0; 
            MaxPaticipants = patFirstPos.Count;
            keepers = new List<ForestKeeper>();
            IsOver = false;
        }

        public Tuple<Player, ForestKeeper> AddClient(string name)
        {
            var startPosition = patFirstPos[0].Item1;
            var destination = patFirstPos[0].Item2;
            var keeper = Forest.MakeNewKeeper(name, nextId, startPosition, destination);
            keepers.Add(keeper);
            var player = new Player(nextId, name, startPosition.ConvertToNetPoint(), destination.ConvertToNetPoint(), 2);
            patFirstPos.RemoveAt(0);
            nextId = (char)(nextId + 1);
            return Tuple.Create(player, keeper);
        }

        public bool Move(int direction, ForestKeeper keeper)
        {
            var dicts = new Dictionary<int, Func<DeltaPoint>>
            {
                {0, DeltaPoint.GoUp},
                {1, DeltaPoint.GoRight},
                {2, DeltaPoint.GoDown},
                {3, DeltaPoint.GoLeft}
            };
            var canMove = Forest.Move(keeper, dicts[direction]());
            if (keeper.position == new Point(Forest.keepers[keeper].x, Forest.keepers[keeper].y))
            {
                IsOver = true;
            }
            return canMove;
        }

        public int[,] GetVisibleMap(ForestKeeper keeper)
        {
            var visibleMap = new int[Forest.fogOfWar * 2 + 1, Forest.fogOfWar * 2 + 1];
            for (int i = 0; i < visibleMap.GetLength(0); i++)
                for (int j = 0; j < visibleMap.GetLength(1); j++)
                {
                    if (keeper.position.x - Forest.fogOfWar + i < 0 ||
                        keeper.position.x - Forest.fogOfWar + i >= Forest.Field.GetLength(0) ||
                        keeper.position.y - Forest.fogOfWar + j < 0 ||
                        keeper.position.y - Forest.fogOfWar + j >= Forest.Field.GetLength(1))
                        visibleMap[i, j] = 0;
                    else
                        visibleMap[i, j] = cellsNum[
                            Forest.Field[
                                i + keeper.position.x - Forest.fogOfWar,
                                j + keeper.position.y - Forest.fogOfWar].GetType()];
                }
            return visibleMap;
        }

        public int[,] GetMap()
        {
            var map = new int[Forest.Field.GetLength(0), Forest.Field.GetLength(1)];
            for (int i = 0; i < map.GetLength(0); i++)
                for (int j = 0; j < map.GetLength(1); j++)
                    map[i, j] = cellsNum[Forest.Field[i, j].GetType()];
            return map;
        }

        public Tuple<ForestSolverPackages.Point, int>[] GetChangedCells(Tuple<int, ForestSolverPackages.Point, int>[] changedPositions)
        {
            return
                changedPositions.Select(
                    x => Tuple.Create(x.Item2, cellsNum[Forest.Field[x.Item2.X, x.Item2.Y].GetType()])).ToArray();
        }
    }
}
