using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWRandomizerEditor.LEV
{
    public class Point
    {
        public int Row;
        public int Column;
        public Point(int row, int column)
        {
            this.Row = row;
            this.Column = column;
        }

        public static Point operator +(Point a, Point b)
        {
            return new Point(a.Row + b.Row, a.Column + b.Column);
        }

        public static Point operator -(Point a, Point b)
        {
            return new Point(a.Row - b.Row, a.Column - b.Column);
        }

        public double DistanceTo(Point other)
        {
            return DistanceBetween(this, other);
        }

        public static double DistanceBetween(Point a, Point b)
        {
            return Math.Sqrt( (a.Row - b.Row) ^ 2+ (a.Column - b.Column) ^ 2 );
        }

        public int[] ToArray() { return new int[] { Row, Column }; }
        
        public override string ToString()
        {
            return $"({Row}, {Column})";
        }

    }
}
