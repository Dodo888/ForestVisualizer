using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting;

namespace ForestCitizens
{
    internal class MainClass
    {
        public static void Main(string[] args)
        {
            ILoader loader = new Loader();
            IForest forest = loader.GetForest("map.txt", "citizens.txt");
            IForestVisualizer visualizer = new FormForestVisualizer(forest);
            visualizer.RunForestVisualization();
        }
    }
}
