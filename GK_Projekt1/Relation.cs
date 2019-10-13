using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK_Projekt1
{
    public abstract class Relation
    {
        protected Vertice Edge1Vertice1;
        protected Vertice Edge1Vertice2;
        protected Vertice Edge2Vertice1;
        protected Vertice Edge2Vertice2;

       
        public Relation(Vertice v1, Vertice v2, Vertice v3, Vertice v4)
        {
            Edge1Vertice1 = v1;
            Edge1Vertice2 = v2;
            Edge2Vertice1 = v3;
            Edge2Vertice2 = v4;
        }

        public bool IsVerticeInRelation(Vertice v)
        {
            if (Edge1Vertice1 == v || Edge1Vertice2 == v || Edge2Vertice1 == v || Edge2Vertice2 == v)
                return true;
            return false;
        }

        public bool IsEdgeInRelation(Edge e)
        {
            Vertice v1 = e.Vertice1;
            Vertice v2 = e.Vertice2;
            if (Edge1Vertice1 == v1 && Edge1Vertice2 == v2)
                return true;
            if (Edge2Vertice1 == v1 && Edge2Vertice2 == v2)
                return true;
            return false;
            
        }

        public abstract bool IsEquality();
    }
}
