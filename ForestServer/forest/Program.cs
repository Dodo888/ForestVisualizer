using System;
using System.Threading;

namespace ForestSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            string source = "input1.txt";
            var map = FileReader.GetField(source);
            var forest = new Forest(map);
            var visualizer = new ConsoleBlackAndWhiteVisualizer();
            var keeper = forest.MakeNewKeeper("Thranduil", 'A', new Point(2, 1), new Point(3, 3));
            var keeperAi = new KeeperAI(keeper, forest.keepers[keeper]);
            visualizer.DrawForest(forest);
            while (keeper.position != forest.keepers[keeper])
            {
                keeperAi.Go(forest.Move);
                Thread.Sleep(1000);
                Console.Clear();
                visualizer.DrawForest(forest);
            }
            Console.WriteLine("цель достигнута");
        }
    }
}
