using System;
using System.Collections.Generic;

namespace ForestSolver
{
    public class ConsoleBlackAndWhiteVisualizer : IVisualizer
    {
        readonly private Dictionary<Type, char> picturesDictionary = new Dictionary<Type, char>()
        {
            {typeof(Life), '♥'}, 
            {typeof(Path), '*' }, 
            {typeof(Trap), '♫'}, 
            {typeof(Wall), '█'}
        };
        
        public void DrawForest(Forest forest)
        {
            Console.Clear();
            for (int i = 0; i < forest.Field.GetLength(0); i++)
            {
                for (int j = 0; j < forest.Field.GetLength(1); j++)
                    Console.Write(picturesDictionary[(forest.Field[i, j]).GetType()]);
                Console.WriteLine();
            }
            foreach (var keeper in forest.keepers.Keys)
            {
                Console.SetCursorPosition(keeper.position.y, keeper.position.x);
                Console.Write(keeper.id); //picturesDictionary[keeper.GetType()]);
            }
            Console.SetCursorPosition(0, forest.Field.GetLength(0));
            Console.WriteLine("\n█ — заросли\n♥ - жизнь\n♫ - ловушка");
            foreach (var keeper in forest.keepers.Keys)
            {
                Console.WriteLine("{0} - лесной житель {1} ({2} жизни)", keeper.id, keeper.name, keeper.hp);
            }
        }
    }
}
