using System;
using ForestSolver;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class ForestTests
    {
        [TestMethod]
        public void LifeAddHp()
        {
            var keeper = new ForestKeeper("aa", new Point(0,0), 2, 'A');
            var field = new ICell[2, 2];
            field[0,1] = new Life();
            field[0, 1].MakeTurn(keeper, new Point(0, 1), ref field[0, 1]);
            Assert.AreEqual(3, keeper.hp);
        }

        [TestMethod]
        public void LifeTransformation()
        {
            var keeper = new ForestKeeper("aa", new Point(0, 0), 2, 'A');
            var field = new ICell[2, 2];
            field[0, 1] = new Life();
            field[0, 1].MakeTurn(keeper, new Point(0, 1), ref field[0, 1]);
            Assert.IsTrue(field[0, 1].GetType() == typeof(Path));
        }

        [TestMethod]
        public void CanMoveToLife()
        {
            var keeper = new ForestKeeper("aa", new Point(0, 0), 2, 'A');
            var field = new ICell[2, 2];
            field[0, 1] = new Life();
            var ans = field[0, 1].MakeTurn(keeper, new Point(0, 1), ref field[0, 1]);
            Assert.IsTrue(ans);
            Assert.AreEqual(0,keeper.position.x);
            Assert.AreEqual(1, keeper.position.y);
        }

        [TestMethod]
        public void CanMoveToPath()
        {
            var keeper = new ForestKeeper("aa", new Point(0, 0), 2, 'A');
            var field = new ICell[2, 2];
            field[0, 1] = new Path();
            var ans = field[0, 1].MakeTurn(keeper, new Point(0, 1), ref field[0, 1]);
            Assert.IsTrue(ans);
            Assert.AreEqual(0, keeper.position.x);
            Assert.AreEqual(1, keeper.position.y);
        }

        [TestMethod]
        public void CanMoveToTrap()
        {
            var keeper = new ForestKeeper("aa", new Point(0, 0), 2, 'A');
            var field = new ICell[2, 2];
            field[0, 1] = new Trap();
            var ans = field[0, 1].MakeTurn(keeper, new Point(0, 1), ref field[0, 1]);
            Assert.IsTrue(ans);
            Assert.AreEqual(0, keeper.position.x);
            Assert.AreEqual(1, keeper.position.y);
        }

        [TestMethod]
        public void TrapSubtractHp()
        {
            var keeper = new ForestKeeper("aa", new Point(0, 0), 2, 'A');
            var field = new ICell[2, 2];
            field[0, 1] = new Trap();
            field[0, 1].MakeTurn(keeper, new Point(0, 1), ref field[0, 1]);
            Assert.AreEqual(1, keeper.hp);
        }

        [TestMethod]
        public void CanMoveToWall()
        {
            var keeper = new ForestKeeper("aa", new Point(0, 0), 2, 'A');
            var field = new ICell[2, 2];
            field[0, 1] = new Wall();
            var ans = field[0, 1].MakeTurn(keeper, new Point(0, 1), ref field[0, 1]);
            Assert.IsFalse(ans);
            Assert.AreEqual(0, keeper.position.x);
            Assert.AreEqual(0, keeper.position.y);
        }

        [TestMethod]
        public void SimpleMap()
        {
            RunAi("test1.txt", new Point(3, 3), new Point(1, 1));
        }

        [TestMethod]
        public void HardMap()
        {
            RunAi("test2.txt", new Point(2, 3), new Point(10, 3));
        }

        [TestMethod]
        public void SampleMap()
        {
            RunAi("test3.txt", new Point(3, 3), new Point(1, 1));
        }

        [TestMethod]
        public void Map()
        {
            RunAi("test5.txt", new Point(8, 1), new Point(1, 8));
        }

        private static void RunAi(string sourse, Point target, Point keeperPosition)
        {
            string source = sourse;
            var map = FileReader.GetField(source);
            var forest = new Forest(map);
            target = target;
            ForestKeeper keeper = forest.MakeNewKeeper("Thranduil", 'A', keeperPosition, target);
            var keeperAi = new KeeperAI(keeper, forest.keepers[keeper]);
            while (keeper.position != forest.keepers[keeper])
            {
                keeperAi.Go(forest.Move);
            }
            Assert.AreEqual(keeper.position, new Point(target.y, target.x));
        }
    }
}
