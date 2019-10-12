using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace GK_Projekt1
{
    public class Edge
    {
        private Vertice vertice1;
        private Vertice vertice2;
        private Polygon polygon;
        private double length;

        public Polygon Polygon { get => polygon;}
        public Vertice Vertice1 { get => vertice1; }
        public Vertice Vertice2 { get => vertice2; }
        public double Length { get => length; }

        public Edge(Vertice v1, Vertice v2, Polygon polygon)
        {
            this.vertice1 = v1;
            this.vertice2 = v2;
            this.polygon = polygon;
            this.length = Math.Sqrt(Math.Pow(v1.Point.X - v2.Point.X, 2) + Math.Pow(v1.Point.Y - v2.Point.Y, 2));
        }

        
        public static bool CompareEdges(Edge e1, Edge e2)
        {
            if (e1 == null || e2 == null)
                return false;
            if ((e1.vertice1 == e2.vertice1 && e1.vertice2 == e2.vertice2) || e1.vertice2 == e2.vertice1 && e1.vertice1 == e2.vertice2)
                return true;
            return false;
        }
        


    }
}
