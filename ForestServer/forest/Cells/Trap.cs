namespace ForestSolver
{
    public class Trap : ICell
    {
        public bool MakeTurn(ForestKeeper keeper, Point position, ref ICell cell)
        {
            keeper.hp -= 1;
            keeper.position = position;
            cell = this;
            return true;
        }
    }
}
