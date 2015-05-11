using System;
using System.Drawing;
using System.Linq;

namespace ForestCitizens
{
    public interface ICitizen
    {
        Point Target { get; set; }
        Point Location { get; set; }
        int LifesCount { get; set; }
        string Name { get; }
        ILookup<string, Point> KeySet { get; }
        IAi Ai { get; set; }
    }
}

