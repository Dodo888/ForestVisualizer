using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ForestCitizens
{
    enum CellStatus
    {
        Blocked,
        Trap,
        Path
    }

    public class Ai : IAi
    {
        private readonly List<Point> directions = new List<Point>
        {
            new Point(0, -1),
            new Point(1, 0),
            new Point(0, 1),
            new Point(-1, 0)
        };

        private int lifesCount;
        private Point location;
        private readonly Point target;

        private int currentIndex;
        private Point currentPoint;
        private Random random = new Random();


        private readonly IForest _forest;
        private readonly ICitizen _citizen;
        private readonly List<Point> _visited;
        private readonly Dictionary<Point, Point> _paths; 
        private readonly Dictionary<Point, CellStatus> _statuses;

        public Ai(IForest forest, ICitizen citizen)
        {
            _forest = forest;
            _citizen = citizen;
            _visited = new List<Point>();
            _paths = new Dictionary<Point, Point>();
            _statuses = new Dictionary<Point, CellStatus>();
        }

        public Ai(ClientInfo info)
        {
            location = info.StartPosition;
            target = info.Target;
            lifesCount = info.Hp;

            _visited = new List<Point>();
            _paths = new Dictionary<Point, Point>();
            _statuses = new Dictionary<Point, CellStatus>();
        }

        private int GetHeuristics(Point delta)
        {
            var dx = _citizen.Target.X - _citizen.Location.X;
            var dy = _citizen.Target.Y - _citizen.Location.Y;
            var heuristics = 0;

            if (delta.X != 0 && Math.Abs(dx) >= Math.Abs(dy) && Math.Sign(dx) == delta.X)
                heuristics += 5;
            if (delta.Y != 0 && Math.Abs(dy) > Math.Abs(dx) && Math.Sign(dy) == delta.Y)
                heuristics += 5;

            if (delta.X != 0 && Math.Abs(dx) < Math.Abs(dy) && Math.Sign(dx) == delta.X)
                heuristics += 4;
            if (delta.Y != 0 && Math.Abs(dy) <= Math.Abs(dx) && Math.Sign(dy) == delta.Y)
                heuristics += 4;

            var p = new Point(_citizen.Location.X + delta.X, _citizen.Location.Y + delta.Y);
            if (_statuses.ContainsKey(p) && _statuses[p] == CellStatus.Trap)
                heuristics -= 4;

            return heuristics;
        }

        private IEnumerable<Point> GetNotWeightedMoves()
        {
            yield return new Point(-1, 0);
            yield return new Point(0, -1);
            yield return new Point(1, 0);
            yield return new Point(0, 1);
        }

        public void Move()
        {
            if (_citizen.Location == _citizen.Target)
                return;

            var moves =
                GetNotWeightedMoves()
                    .Select(x => new Point(_citizen.Location.X + x.X, _citizen.Location.Y + x.Y))
                    .Where(x => !_visited.Contains(x))
                    .OrderByDescending(GetHeuristics);

            if (!moves.Any())
            {
                var previous = _paths[_citizen.Location];
                var delta = new Point(previous.X - _citizen.Location.X, previous.Y - _citizen.Location.Y);
                _forest.MoveCitizen(_citizen, delta);
                return;
            }

            foreach (var move in moves)
            {
                var citizenLocation = _citizen.Location;
                var lifesCount = _citizen.LifesCount;
                if (_forest.MoveCitizen(_citizen, new Point(move.X - _citizen.Location.X, move.Y - _citizen.Location.Y)))
                {
                    _visited.Add(_citizen.Location);
                    _paths[move] = citizenLocation;
                    if (lifesCount > _citizen.LifesCount)
                        _statuses[move] = CellStatus.Trap;
                    else if (citizenLocation == _citizen.Location)
                        _statuses[move] = CellStatus.Blocked;
                    else _statuses[move] = CellStatus.Path;
                    break;
                }
                _visited.Add(move);
            }
        }

        private int GetNetHeuristics(Point delta)
        {
            var dx = target.X - location.X;
            var dy = target.Y - location.Y;
            var heuristics = 0;

            if (delta.X != 0 && Math.Abs(dx) >= Math.Abs(dy) && Math.Sign(dx) == delta.X)
                heuristics += 5;
            if (delta.Y != 0 && Math.Abs(dy) > Math.Abs(dx) && Math.Sign(dy) == delta.Y)
                heuristics += 5;

            if (delta.X != 0 && Math.Abs(dx) < Math.Abs(dy) && Math.Sign(dx) == delta.X)
                heuristics += 4;
            if (delta.Y != 0 && Math.Abs(dy) <= Math.Abs(dx) && Math.Sign(dy) == delta.Y)
                heuristics += 4;

            var p = new Point(location.X + delta.X, location.Y + delta.Y);
            if (_statuses.ContainsKey(p) && _statuses[p] == CellStatus.Trap)
                heuristics -= 4;

            return heuristics;
        }

        private bool backwardMotion = false;

        public Move GetMove(MoveResultInfo info)
        {
            _visited.Add(currentPoint);
            if (info.Result == 0)
            {
                if (!backwardMotion && !_paths.ContainsKey(currentPoint))
                {
                    //Console.WriteLine("Updated key");

                    _paths[currentPoint] = location;
                }
                location = currentPoint;
                currentIndex = 0;
            }

            if (location == target)
                return new Move { Direction = -1 };

            var moves =
                GetNotWeightedMoves()
                    .Select(x => new Point(location.X + x.X, location.Y + x.Y))
                    .Where(x => !_visited.Contains(x))
                    .OrderByDescending(GetNetHeuristics).ToList();

            //Console.WriteLine("----visited----");
            //foreach (var point in _visited)
            //{
            //    Console.WriteLine("(" + point.X + ", " + point.Y + ")");
            //}
            //Console.WriteLine("----endvisited----");

            //Console.WriteLine("----moves----");
            //foreach (var point in moves)
            //{
            //    Console.WriteLine("(" + point.X + ", " + point.Y + ")");
            //}
            //Console.WriteLine("----endmoves----");

            if (!moves.Any())
            {
                
                var previous = _paths[location];
                Console.WriteLine("MOVING BACK FROM: ({0}, {1}) TO ({2}, {3})", location.X, location.Y, previous.X, previous.Y);
                var delta = new Point(previous.X - location.X, previous.Y - location.Y);
                backwardMotion = true;
                return new Move {Direction = PointToInt(delta)};
            }
            backwardMotion = false;

            currentPoint = moves[0];

            return new Move {Direction = PointToInt(new Point(currentPoint.X - location.X, currentPoint.Y - location.Y))};
        }

        public Move GetInitMove()
        {
            _visited.Add(location);
            var moves =
                GetNotWeightedMoves()
                    .Select(x => new Point(location.X + x.X, location.Y + x.Y))
                    .Where(x => !_visited.Contains(x))
                    .OrderByDescending(GetNetHeuristics);
            currentPoint = moves.First();
            
            return new Move { Direction = PointToInt(new Point(currentPoint.X - location.X, currentPoint.Y - location.Y)) };
        }

        private int PointToInt(Point p)
        {
            for (int i = 0; i < directions.Count; i++)
                if (p.X == directions[i].X && p.Y == directions[i].Y)
                    return i;
            return -1;
        }
    }

    public interface IAi
    {
        void Move();
    }
}
