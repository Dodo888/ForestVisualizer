using System;

namespace ForestSolver
{
    public class ForestKeeper
    {
        public int hp;
        public Point position;
        readonly public string name;
        readonly public int id;

        public ForestKeeper(string name, Point position, int hp, int id)
        {
            this.position = position;
            this.hp = hp;
            this.name = name;
            this.id = id;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(ForestKeeper))
                throw new InvalidCastException("obj is not ForestKeeper");
            return id.Equals(((ForestKeeper)obj).id);
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }
    }
}
