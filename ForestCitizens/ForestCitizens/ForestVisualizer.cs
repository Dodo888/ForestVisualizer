using System;
using System.Collections.Generic;
using System.Linq;

namespace ForestCitizens
{
    internal class ForestVisualizer : IForestVisualizer
    {
        private IForest forest;

        private Dictionary<Type, char> chars = new Dictionary<Type, char>
        {
            {typeof (Terrain), ' '},
            {typeof (Block), '█'},
            {typeof (Trap), 'ￓ'},
            {typeof (Life), '♥'},
        };

        public ForestVisualizer(IForest forest)
        {
            this.forest = forest;
        }

        public void Display()
        {
            var lines = forest.Map.Select(x => x.Select(y => chars[y.GetType()]).ToArray()).ToArray();
            foreach (var citizen in forest.Citizens)
                lines[citizen.Location.X][citizen.Location.Y] = citizen.Name[0];
            foreach (var line in lines.Select(x => string.Join("", x)))
            {
                Console.WriteLine(line);
            }

            Console.WriteLine();
            Console.WriteLine("Legend:");
            Console.WriteLine("\t█ - Block");
            Console.WriteLine("\t♥ - Life");
            Console.WriteLine("\tￓ - It's a trap!");
            Console.WriteLine();
            Console.WriteLine("Citizens:");
            foreach (var citizen in forest.Citizens)
            {
                Console.WriteLine("\t{0} - Name: {1}, Location: {2}, Lifes Count: {3}",
                    citizen.Name[0], citizen.Name, citizen.Location, citizen.LifesCount);
            }
        }

        public void RunForestVisualization()
        {
            while (true)
            {
                Console.Clear();
                Display();
                try
                {
                    var key = Console.ReadKey().Key.ToString();
                    foreach (var citizen in forest.Citizens)
                    {
                        try
                        {
                            forest.MoveCitizen(citizen, citizen.KeySet[key].FirstOrDefault());
                        }
                        catch (NullReferenceException)
                        {
                            citizen.Ai.Move();
                        }
                    }
                }
                catch (DivideByZeroException)
                {
                    Console.Clear();
                    break;
                }
                catch (InvalidOperationException)
                {
                    // Nothing to do here
                }
            }
            Console.WriteLine("There's no citizens left in the forest.");
        }
    }
}
