using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit;
using NUnit.Framework;
using System.Drawing;

namespace ForestCitizens
{
    [TestFixture]
    class Ai_should
    {
        private ICitizen GetFinalCitizen(string map, Point location, Point target)
        {
            var forest = new Loader().GetForest(map);
            forest.Citizens.Add(new Citizen("A", null, 4, location, target));
            forest.Citizens[0].Ai = new Ai(forest, forest.Citizens[0]);
            for (int i = 0; i < forest.Map.Length * forest.Map[0].Length; i++)
                forest.Citizens[0].Ai.Move();
            return forest.Citizens[0];
        }

        [Test]
        public void find_simple_path()
        {
            var citizen = GetFinalCitizen("TestMaps/map_original.txt", new Point(6, 6), new Point(1, 1));
            Assert.AreEqual(citizen.Target, citizen.Location);
        }

        [Test]
        public void find_path()
        {
            var citizen = GetFinalCitizen("TestMaps/map1.txt", new Point(8, 6), new Point(1, 2));
            Assert.AreEqual(citizen.Target, citizen.Location);
        }

        [Test]
        public void not_find_path()
        {
            var citizen = GetFinalCitizen("TestMaps/map2.txt", new Point(8, 6), new Point(1, 2));
            Assert.AreNotEqual(citizen.Target, citizen.Location);
        }

        [Test]
        public void find_with_cycles()
        {
            var citizen = GetFinalCitizen("TestMaps/map3.txt", new Point(8, 6), new Point(1, 2));
            Assert.AreEqual(citizen.Target, citizen.Location);
        }
    }
}
