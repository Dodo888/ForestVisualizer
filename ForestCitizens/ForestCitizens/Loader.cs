using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace ForestCitizens
{
    public class Loader : ILoader
    {
        private Dictionary<char, Cell> cellsDictionary = new Dictionary<char, Cell>
        {
            {'0', new Terrain()},
            {'1', new Block()},
            {'K', new Trap()},
            {'L', new Life()}
        };

        public IForest GetForest(string[] map, List<ICitizen> citizens)
        {
            var cells = new Cell[map.Length][];
            for (int i = 0; i < map.Length; i++)
                cells[i] = map[i].Select(x => cellsDictionary[x]).ToArray();
            return new Forest(cells, citizens);
        }

        public IForest GetForest(string mapFile, string citizensFile)
        {
            string[] lines = File.ReadAllLines(mapFile);
            var cells = new Cell[lines.Length][];
            for (int i = 0; i < lines.Length; i++)
                cells[i] = lines[i].Select(x => cellsDictionary[x]).ToArray();

            lines = File.ReadAllLines(citizensFile);
            var citizens = new List<ICitizen>();
            foreach (var line in lines)
            {
                if (line.StartsWith("//")) continue;
                var citizenInfo = line.Split();
                var locationInfo = citizenInfo[1].Split(',').Select(int.Parse).ToList();
                var location = new Point(locationInfo[0], locationInfo[1]);
                var keySetInfo = citizenInfo[3].Split(',');
                try
                {
                    
                    var keySet = new Dictionary<string, Point>
                    {
                        {keySetInfo[0], new Point(0, -1)},
                        {keySetInfo[1], new Point(0, 1)},
                        {keySetInfo[2], new Point(-1, 0)},
                        {keySetInfo[3], new Point(1, 0)}
                    }.ToLookup(x => x.Key, x => x.Value);
                    citizens.Add(new Citizen(citizenInfo[0], keySet, int.Parse(citizenInfo[2]), location));
                }
                catch (Exception)
                {
                    var target = keySetInfo.Select(int.Parse).ToList();
                    citizens.Add(new Citizen(citizenInfo[0],
                        null, int.Parse(citizenInfo[2]),
                        location,
                        new Point(target[0], target[1])));
                }
            }
            var forest = new Forest(cells, citizens);
            foreach (var citizen in forest.Citizens)
            {
                if (citizen.Target != null)
                {
                    citizen.Ai = new Ai(forest, citizen);
                }
            }

            return new Forest(cells, citizens);
        }

        public IForest GetForest(string mapFile)
        {
            string[] lines = File.ReadAllLines(mapFile);
            var cells = new Cell[lines.Length][];
            for (int i = 0; i < lines.Length; i++)
                cells[i] = lines[i].Select(x => cellsDictionary[x]).ToArray();
            return new Forest(cells);
        }
    }
}