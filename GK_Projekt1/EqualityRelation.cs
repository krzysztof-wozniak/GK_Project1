using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK_Projekt1
{
    public class EqualityRelation : Relation
    {
        private double length;

        public EqualityRelation(Vertice v1, Vertice v2, Vertice v3, Vertice v4, int index) : base(v1, v2, v3, v4, index)
        {
            this.length = Math.Sqrt(Math.Pow(v1.Point.X - v2.Point.X, 2) + Math.Pow(v1.Point.Y - v2.Point.Y, 2));
        }

        public EqualityRelation(Edge e1, Edge e2, int index) : base(e1.Vertice1, e1.Vertice2, e2.Vertice1, e2.Vertice2, index)
        {
            this.length = Math.Sqrt(Math.Pow(e1.Vertice1.Point.X - e1.Vertice2.Point.X, 2) + Math.Pow(e1.Vertice1.Point.Y - e1.Vertice2.Point.Y, 2));
        }

        public override bool IsEquality()
        {
            return true;
        }

        public double Length
        {
            get => length;
        }

        public void UpdateLength()
        {
            length = edge1Vertice1.Point.DistanceToPoint(edge1Vertice2.Point);
        }
    }
}
