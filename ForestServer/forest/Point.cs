using System;

namespace ForestSolver
{
    public class Point
    {
        public readonly int x;
        public readonly int y;
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        static public ForestSolver.Point ConvertFromNetPoint(ForestSolverPackages.Point p)
        {
            var t = new Point(p.X, p.Y);
            return t;
        }

        public Point Add(Point other)
        {
            return new Point(this.x + other.x, this.y + other.y); 
        }

        public Point Add(DeltaPoint other)
        {
            return new Point(this.x + other.deltax, this.y + other.deltay);
        }

        public Point Reverse()
        {
            return new Point(y, x);
        }

        public Point Sub(Point other)
        {
            return new Point(this.x - other.x, this.y - other.y);
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Point))
                throw new InvalidCastException();
            var o = (Point) obj;
            return this.x == o.x && this.y == o.y;
        }

        static public bool operator ==(Point one, Point two)
        {
            return one.Equals(two);
        }

        static public bool operator !=(Point one, Point two)
        {
            return !(one == two);
        }

        public override string ToString()
        {
            return String.Format("({0}, {1})", x, y);
        }

        public ForestSolverPackages.Point ConvertToNetPoint()
        {
            return new ForestSolverPackages.Point(x, y);
        }
    }

    public class DeltaPoint
    {
        public readonly int deltax;
        public readonly int deltay;

        private DeltaPoint(int x, int y)
        {
            deltax = x;
            deltay = y;
        }

        public static DeltaPoint GoLeft()
        {
            return new DeltaPoint(-1, 0);
        }

        public static DeltaPoint GoRight()
        {
            return new DeltaPoint(1, 0);
        }

        public static DeltaPoint GoDown()
        {
            return new DeltaPoint(0, 1);
        }

        public static DeltaPoint GoUp()
        {
            return new DeltaPoint(0, -1);
        }

        public DeltaPoint Reverse()
        {
            return new DeltaPoint(-this.deltax, -this.deltay);
        }

        public override bool Equals(object obj)
        {
            var oth = (DeltaPoint) obj;
            return deltax == oth.deltax && deltay == oth.deltay;
        }

        public override int GetHashCode()
        {
            return deltax*2 + deltay*3;
        }
    }
}
