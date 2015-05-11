using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit;
using NUnit.Framework;

namespace ForestCitizens
{
    [TestFixture]
    public class Forest_should
    {
        IForest testSetup()
        {
            ILoader loader = new Loader();
            return loader.GetForest("TestMaps/map_original.txt", "citizens.txt");
        }

        [Test]
        public void place_citizen()
        {
            IForest forest = testSetup();
            forest.PlaceCitizen(new Citizen("testC", new Dictionary<string, Point>().ToLookup(x => x.Key, x => x.Value)), new Point(2, 1));
            Assert.IsTrue(forest.Citizens.Any(x => x.Name == "testC" && x.Location == new Point(2, 1)));
        }

        [Test]
        public void find_citizen_by_name()
        {
            IForest forest = testSetup();
            forest.PlaceCitizen(new Citizen("testC", new Dictionary<string, Point>().ToLookup(x => x.Key, x => x.Value)), new Point(2, 1));
            Assert.IsTrue(forest.GetCitizenByName("testC").Location == new Point(2, 1));
        }

        [Test]
        public void move_citizen()
        {
            IForest forest = testSetup();
            var citizen = new Citizen("testC", new Dictionary<string, Point>().ToLookup(x => x.Key, x => x.Value));
            forest.PlaceCitizen(citizen, new Point(2, 1));
            forest.MoveCitizen(citizen, new Point(1, 0));
            Assert.AreEqual(citizen.Location, new Point(3, 1));
        }

        [Test]
        public void not_move_citizen_to_block()
        {
            IForest forest = testSetup();
            var citizen = new Citizen("testC", new Dictionary<string, Point>().ToLookup(x => x.Key, x => x.Value));
            forest.PlaceCitizen(citizen, new Point(2, 1));
            forest.MoveCitizen(citizen, new Point(0, 1));
            Assert.AreNotEqual(citizen.Location, new Point(3, 1));
        }

        [Test]
        public void delete_citizen()
        {
            IForest forest = testSetup();
            var citizen = new Citizen("testC", new Dictionary<string, Point>().ToLookup(x => x.Key, x => x.Value));
            forest.PlaceCitizen(citizen, new Point(2, 1));
            forest.DeleteCitizen(citizen);
            Assert.IsFalse(forest.Citizens.Any(x => x.Name == "testC" && x.Location == new Point(2, 1)));
        }
    }
}
