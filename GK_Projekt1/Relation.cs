using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK_Projekt1
{
    public abstract class Relation
    {
        protected Vertice edge1Vertice1;
        protected Vertice edge1Vertice2;
        protected Vertice edge2Vertice1;
        protected Vertice edge2Vertice2;
        protected int index;

        public int Index { get => index; }

        public Vertice E1V1 { get => edge1Vertice1; }
        public Vertice E1V2 { get => edge1Vertice2; }
        public Vertice E2V1 { get => edge2Vertice1; }
        public Vertice E2V2 { get => edge2Vertice2; }



        public Relation(Vertice v1, Vertice v2, Vertice v3, Vertice v4, int index)
        {
            edge1Vertice1 = v1;
            edge1Vertice2 = v2;
            edge2Vertice1 = v3;
            edge2Vertice2 = v4;
            this.index = index;
        }

        public int ChangeIndex(int i) => this.index = i;
        public bool IsVerticeInRelation(Vertice v)
        {
            if (edge1Vertice1 == v || edge1Vertice2 == v || edge2Vertice1 == v || edge2Vertice2 == v)
                return true;
            return false;
        }

        public bool IsEdgeInRelation(Edge e)
        {
            Vertice v1 = e.Vertice1;
            Vertice v2 = e.Vertice2;
            if (edge1Vertice1 == v1 && edge1Vertice2 == v2)
                return true;
            if (edge2Vertice1 == v1 && edge2Vertice2 == v2)
                return true;
            return false;
            
        }

        public abstract bool IsEquality();
    }
}
