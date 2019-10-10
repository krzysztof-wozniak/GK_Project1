using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK_Projekt1
{
    public class Polygon
    {
        private List<Vertice> vertices = new List<Vertice>();
        //private List<Edge> edges = new List<Edge>();
        private int index;
        private bool fullPolygon = false;

        public int Index
        {
            get => index;
        }

        public List<Vertice> Vertices
        {
            get
            {
                return vertices;
            }
            set
            {
                vertices = value;
            }
        }

        //public List<Edge> Edges
        //{
        //    get
        //    {
        //        return edges;
        //    }
        //}     
        
        public int VerticeCount
        {
            get => vertices.Count;
        }

        //public int EdgeCount
        //{
        //    get => edges.Count;
        //}


        public Vertice this[int key]
        {
            get
            {
                return this.vertices[key];
            }
            set
            {
                vertices[key] = value;
            }
        }

        public void DeleteVertice(Vertice v)
        {
            if (v.Index < 0 || v.Index >= vertices.Count || v.Polygon != this)
                return;
            for (int i = v.Index + 1; i < v.Polygon.VerticeCount; i++)
            {
                v.Polygon.vertices[i].Index--;
            }
            
            v.Polygon.vertices.RemoveAt(v.Index);
        }

        /// <summary>
        /// Adds a vertice to the end of the polygon.
        /// </summary>
        /// <param name="v"></param>
        public void AddVertice(Vertice v)
        {
            vertices.Add(v);
        }

        public Vertice GetFirstVertice()
        {
            return vertices[0];
        }

        public Vertice GetLastVertice()
        {
            return vertices[vertices.Count - 1];
        }

        public void ChangeIndex(int index)
        {
            this.index = index;
        }

        public Polygon(int index)
        {
            this.index = index;
        }

        public List<Edge> GetEdges()
        {
            List<Edge> edges = new List<Edge>();
            if (vertices.Count < 3)
                return edges;
            var iterator = vertices.GetEnumerator();
            iterator.MoveNext();
            Vertice v1 = iterator.Current;
            Vertice v2;

            while(iterator.MoveNext())
            {
                v2 = iterator.Current;
                edges.Add(new Edge(v1, v2, v1.Polygon));
                v1 = v2;
            }
            if(fullPolygon == true)
                edges.Add(new Edge(vertices[vertices.Count - 1], vertices[0], v1.Polygon));


            return edges;
        }

        public void FinishDrawing()
        {
            fullPolygon = true;
        }

        public void AddMidPoint(Vertice PreviousVertice, Point Point)
        {
            Polygon polygon = PreviousVertice.Polygon;
            Vertice newVertice = new Vertice(Point, polygon, PreviousVertice.Index + 1);
            
            for (int i = PreviousVertice.Index + 1; i < polygon.VerticeCount; i++)
            {
                polygon.vertices[i].Index++;
            }
            vertices.Insert(PreviousVertice.Index + 1, newVertice);
        }
    }
}
