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
    }
}
