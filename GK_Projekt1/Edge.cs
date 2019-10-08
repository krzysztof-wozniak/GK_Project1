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

        public Polygon Polygon { get => polygon; set => polygon = value; }
        public Vertice Vertice1 { get => vertice1; set => vertice1 = value; }
        public Vertice Vertice2 { get => vertice2; set => vertice2 = value; }

        public Edge(Vertice v1, Vertice v2, Polygon polygon)
        {
            this.vertice1 = v1;
            this.vertice2 = v2;
            this.polygon = polygon;
        }


        //public static bool operator==(Edge e1, Edge e2)
        //{
        //    if (e1 == null || e2 == null)
        //        return false;
        //    if ((e1.vertice1 == e2.vertice1 && e1.vertice2 == e2.vertice2) || e1.vertice2 == e2.vertice1 && e1.vertice1 == e2.vertice2)
        //        return true;
        //    return false;
        //}

        //public static bool operator !=(Edge e1, Edge e2)
        //{
        //    return !(e1 == e2);
        //}

        public static bool CompareEdges(Edge e1, Edge e2)
        {
            if (e1 == null || e2 == null)
                return false;
            if ((e1.vertice1 == e2.vertice1 && e1.vertice2 == e2.vertice2) || e1.vertice2 == e2.vertice1 && e1.vertice1 == e2.vertice2)
                return true;
            return false;
        }

        //public override bool Equals(object obj)
        //{
        //    if (obj == null || GetType() != obj.GetType())
        //        return false;
        //    Edge e = (Edge)obj;
        //    if ((vertice1 == e.vertice1 && vertice2 == e.vertice2) || vertice2 == e.vertice1 && vertice1 == e.vertice2)
        //        return true;
        //    return false;
        //}



    }
}
