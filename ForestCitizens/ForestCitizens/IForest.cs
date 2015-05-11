using System;
using System.Collections.Generic;
using System.Drawing;

namespace ForestCitizens
{
    public interface IForest
    {
        Cell[][] Map { get; }
        List<ICitizen> Citizens { get; }

        void PlaceCitizen(ICitizen citizen, Point location);
        bool MoveCitizen(ICitizen citizen, Point vector);
        void DeleteCitizen(ICitizen citizen);
        ICitizen GetCitizenByName(string name);
    }
}
