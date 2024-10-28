using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWRandomizerEditor.LEV
{
    public class Point
    {
        public int x;
        public int y;
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Point operator +(Point a, Point b)
        {
            return new Point(a.x + b.x, a.y + b.y);
        }

        public static Point operator -(Point a, Point b)
        {
            return new Point(a.x - b.x, a.y - b.y);
        }

        public double DistanceTo(Point other)
        {
            return DistanceBetween(this, other);
        }

        public static double DistanceBetween(Point a, Point b)
        {
            return Math.Sqrt( (a.x - b.x) ^ 2+ (a.y - b.y) ^ 2 );
        }

        public int[] ToArray() { return new int[] { x, y }; }
        
        public override string ToString()
        {
            return $"({x}, {y})";
        }

    }
}
