using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForestSolver
{
    public class Life : ICell
    {
        public bool MakeTurn(ForestKeeper keeper, Point position, ref ICell cell)
        {
            keeper.hp += 1;
            keeper.position = position;
            cell = new Path();
            return true;
        }
    }
}
