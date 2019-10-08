using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK_Projekt1
{
    public static class PointExtender
    {
        public static double DistanceToPoint(this Point a, Point b)
        {
            int dx = a.X - b.X;
            int dy = a.Y - b.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static double DistanceToEdge(this Point p, Point p1, Point p2)
        {
            double A = p2.X - p1.X, B = p2.Y - p1.Y;
            Point p3;
            var u = (A * (p.X - p1.X) + B * (p.Y - p1.Y)) / (Math.Pow(A, 2) + Math.Pow(B, 2));
            if (u <= 0)
            {
                p3 = p1;
            }
            else if (u >= 1)
            {
                p3 = p2;
            }
            else
            {
                p3 = new Point((int)(p1.X + u * A), (int)(p1.Y + u * B));
            }
            return Math.Sqrt(Math.Pow(p.X - p3.X, 2) + Math.Pow(p.Y - p3.Y, 2));
        }

        public static double DistanceToEdge(this Point p, Edge e)
        {
            return p.DistanceToEdge(e.Vertice1.Point, e.Vertice2.Point);
        }
    }
}
