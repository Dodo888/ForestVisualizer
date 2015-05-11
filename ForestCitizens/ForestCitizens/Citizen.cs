using System.Drawing;
using System.Linq;
using System.Security.Authentication.ExtendedProtection.Configuration;

namespace ForestCitizens
{
    public class Citizen : ICitizen
    {
        public Point Target { get; set; }
        public Point Location { get; set; }
        public int LifesCount { get; set; }
        public string Name { get; private set; }
        public ILookup<string, Point> KeySet { get; private set; } 
        public IAi Ai { get; set; }

        public Citizen(string name, ILookup<string, Point> keySet)
        {
            Name = name;
            KeySet = keySet;
        }

        public Citizen(string name, ILookup<string, Point> keySet, int lifesCount, Point location, Point target)
        {
            Name = name;
            KeySet = keySet;
            LifesCount = lifesCount;
            Location = location;
            Target = target;
        }

        public Citizen(string name, ILookup<string, Point> keySet, int lifesCount, Point location)
        {
            Name = name;
            KeySet = keySet;
            LifesCount = lifesCount;
            Location = location;
        }
    }
}
