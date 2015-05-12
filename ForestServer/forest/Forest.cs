using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace ForestSolver
{
    public class Forest
    {
        public Dictionary<ForestKeeper, Point> keepers;
        public ICell[,] Field;
        public int fogOfWar = 0;
        public Forest(ICell[,] field)
        {
            Field = field;
            keepers = new Dictionary<ForestKeeper, Point>();
        }

        public bool Move(ForestKeeper keeper, DeltaPoint deltas)
        {
//            if (keeper.position == keepers[keeper])
//                throw new Exception("done!");
            var newPosition = keeper.position.Add(deltas);
            var canMove = Field[newPosition.x, newPosition.y].MakeTurn(keeper, newPosition, ref Field[newPosition.x, newPosition.y]);
            if (keeper.hp <= 0)
                keepers.Remove(keeper);
            return canMove;
        }

        public ForestKeeper MakeNewKeeper(string name, int id, Point position, Point destination)
        {
            var keeper = new ForestKeeper(name, new Point(position.y - 1, position.x), 2, id);
            keepers.Add(keeper, new Point(destination.y, destination.x));
            if (!Move(keeper, DeltaPoint.GoRight()))
                throw new Exception("нельзя сюда поставить");
            return keeper;
        }
    }
}
