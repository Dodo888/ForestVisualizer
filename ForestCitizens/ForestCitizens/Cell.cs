using System;
using System.Drawing;

namespace ForestCitizens
{
    public abstract class Cell
    {
        public abstract bool Interact(ICitizen citizen, IForest forest, Point delta);
        public abstract string GetImageString();

        protected void Move(ICitizen citizen, Point delta)
        {
            var newX = citizen.Location.X + delta.X;
            var newY = citizen.Location.Y + delta.Y;
            citizen.Location = new Point(newX, newY);
        }
    }

    public class Trap : Cell
    {
        public override bool Interact(ICitizen citizen, IForest forest, Point delta)
        {
            citizen.LifesCount--;    
            try
            {
                var test = 1 / citizen.LifesCount;
                Move(citizen, delta);
            }
            catch (DivideByZeroException)
            {
                forest.DeleteCitizen(citizen);
            }
            return true;
        }

        public override string GetImageString()
        {
            return "Trap";
        }
    }

    public class Life : Cell
    {
        public override bool Interact(ICitizen citizen, IForest forest, Point delta)
        {
            citizen.LifesCount++;
            var x = citizen.Location.X + delta.X;
            var y = citizen.Location.Y + delta.Y;
            forest.Map[x][y] = new Terrain();
            Move(citizen, delta);
            return true;
        }

        public override string GetImageString()
        {
            return "Life";
        }
    }

    public class Terrain : Cell
    {
        public override bool Interact(ICitizen citizen, IForest forest, Point delta)
        {
            Move(citizen, delta);
            return true;
        }

        public override string GetImageString()
        {
            return "Terrain";
        }
    }

    public class Block : Cell
    {
        public override bool Interact(ICitizen citizen, IForest forest, Point delta)
        {
            return false;
        }

        public override string GetImageString()
        {
            return "Block";
        }
    }
}
