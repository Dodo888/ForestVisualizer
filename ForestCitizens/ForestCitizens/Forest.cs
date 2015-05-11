using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;

namespace ForestCitizens
{
    internal class Forest : IForest
    {
        public Cell[][] Map { get; private set; }
        public List<ICitizen> Citizens { get; private set; }

        public Forest(Cell[][] map)
        {
            Map = map;
            Citizens = new List<ICitizen>();
        }

        public Forest(Cell[][] map, List<ICitizen> citizens)
        {
            Map = map;
            Citizens = citizens;
        }

        public void PlaceCitizen(ICitizen citizen, Point location)
        {
            citizen.Location = location;
            citizen.LifesCount = 1;
            Citizens.Add(citizen);
        }

        public void PlaceCitizen(ICitizen citizen, Point location, Point target)
        {
            citizen.Location = location;
            citizen.Target = target;
            Citizens.Add(citizen);
        }

        public bool MoveCitizen(ICitizen citizen, Point vector)
        {
            var cell = Map[citizen.Location.X + vector.X][citizen.Location.Y + vector.Y];
            var interactionResult = cell.Interact(citizen, this, vector);
            var test = 1 / Citizens.Count;
            return interactionResult;
        }

        public void DeleteCitizen(ICitizen citizen)
        {
            Citizens.Remove(citizen);
        }

        public ICitizen GetCitizenByName(string name)
        {
            return Citizens.FirstOrDefault(x => x.Name == name);
        }
    }
}
