using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK_Projekt1
{
    public class PerpendicularRelation : Relation
    {


        public PerpendicularRelation(Vertice v1, Vertice v2, Vertice v3, Vertice v4, int index) : base(v1, v2, v3, v4, index)
        {

        }

        public PerpendicularRelation(Edge e1, Edge e2, int index) : base(e1.Vertice1, e1.Vertice2, e2.Vertice1, e2.Vertice2, index)
        {

        }

        public override bool IsEquality()
        {
            return false;
        }
        
        
    }
}
