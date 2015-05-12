using System;
using System.Collections.Generic;
using System.Linq;

namespace ForestSolver
{
    public enum MyColour
    {
        White,
        Black

    }
    public class CellWithColor
    {
        public ICell cell { get; private set; }
        public MyColour color { get; set; }

        public CellWithColor(ICell cell, MyColour color)
        {
            this.cell = cell;
            this.color = color;
        }

    }
    public class KeeperAI
    {
        private ExpansibleList<CellWithColor> field;
        private ForestKeeper keeper;
        private Point target;

        public KeeperAI(ForestKeeper keeper, Point target)
        {
            field = new ExpansibleList<CellWithColor>(() => new CellWithColor(new Path(), MyColour.White));
            field[target.x, target.y] = new CellWithColor(new Path(), MyColour.White);
            field[keeper.position.x, keeper.position.y] = new CellWithColor(new Path(), MyColour.White);
            this.keeper = keeper;
            this.target = target;
        }

        public void Go(Func<ForestKeeper, DeltaPoint , bool> tryMove)
        {
            DeltaPoint deltaPoint;
            Point position;
            position = keeper.position;
            deltaPoint = AlmostBFS();
            if (!tryMove(keeper, deltaPoint))
            {
                var newPosition = position.Add(deltaPoint);
                field[newPosition.x, newPosition.y] = new CellWithColor(new Wall(), MyColour.White);
            }
        }

        private DeltaPoint AlmostBFS()
        {
            var queue = new Queue<Point>();
            queue.Enqueue(target);

            for(int i = 0; i < field.rowCount; i++)
                for (int j = 0; j < field.columnCount; j++)
                    field[i,j].color = MyColour.White;
            field[target.x, target.y].color = MyColour.Black;
            var neighbours = new[] {DeltaPoint.GoDown(), DeltaPoint.GoLeft(), DeltaPoint.GoRight(), DeltaPoint.GoUp()};
            while (queue.Count() != 0)
            {
                var position = queue.Dequeue();
                for (int i = 0; i < neighbours.Length; i++)
                {
                    var newPosition = position.Add(neighbours[i]);
                    if (newPosition.x < 0 || newPosition.y < 0)
                        continue;
                    if (newPosition == keeper.position)
                        return neighbours[i].Reverse();
                    if (newPosition.x >= field.rowCount || newPosition.y >= field.columnCount)
                        field[newPosition.x, newPosition.y] = new CellWithColor(new Path(), MyColour.White);
                    if (field[newPosition.x, newPosition.y].color == MyColour.White &&
                        field[newPosition.x, newPosition.y].cell.GetType() != typeof (Wall))
                    {
                        queue.Enqueue(newPosition);
                        field[newPosition.x, newPosition.y].color = MyColour.Black;
                    }
                }
            }
            throw new Exception("невозможно прийти");
        }
    }

    public class ExpansibleList<T>
    {
        private List<List<T>> arr;
        private Func<T> defaultElement;
        public int rowCount { get; private set; }
        public int columnCount { get; private set; }

        public ExpansibleList(Func<T> defaultElement)
        {
            this.defaultElement = defaultElement;
            arr = new List<List<T>>();
            rowCount = 0;
            columnCount = 0;
        }

        public T this[int i, int j]
        {
            get
            {
                if (i < 0 || j < 0)
                    throw new IndexOutOfRangeException("the index is negative");
                if (i >= rowCount || j >= columnCount)
                    return defaultElement();
//                    throw new IndexOutOfRangeException("the index is out of bounds");
                return arr[i][j];
            }
            set
            {
                if (i < 0 || j < 0)
                    throw new IndexOutOfRangeException("the index is negative");
                while (rowCount <= i)
                {
                    arr.Add(new List<T>());
                    rowCount++;
                }
                foreach (var row in arr)
                    while (row.Count <= j || row.Count < columnCount)
                    {
                        row.Add(defaultElement());
                    }
                if (columnCount <= j)
                    columnCount = j + 1;
                arr[i][j] = value;
            }
        }
    }
}
