using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWRandomizerEditor.LEV.Blocks;

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

        public Point(int index, int height = MapObjBlock.TileHeight, int width = MapObjBlock.TileWidth)
        {
            this.Row = index / width;
            this.Column = index % width;
        }

        public int X => Column;
        public int Y => Row;

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
