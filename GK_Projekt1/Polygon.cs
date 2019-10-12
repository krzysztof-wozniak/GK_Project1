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
        private List<Edge> edges = new List<Edge>();
        private int index;
        private bool fullPolygon = false;

        public List<Relation> relations = new List<Relation>();



        public Polygon(int index)
        {
            this.index = index;
        }



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

        public List<Edge> Edges
        {
            get
            {
                return edges;
            }
        }

        public int VerticeCount
        {
            get => vertices.Count;
        }

        public int EdgeCount
        {
            get => edges.Count;
        }


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
            UpdateEdges();
        }

        public void UpdateEdges()
        {
            List<Edge> edges = new List<Edge>();
            var iterator = vertices.GetEnumerator();
            iterator.MoveNext();
            Vertice v1 = iterator.Current;
            Vertice v2;
            while (iterator.MoveNext())
            {
                v2 = iterator.Current;
                edges.Add(new Edge(v1, v2, v1.Polygon));
                v1 = v2;
            }
            if(fullPolygon == true)
                edges.Add(new Edge(v1, vertices[0], v1.Polygon));
            this.edges = edges;
        }

        /// <summary>
        /// Adds a vertice to the end of the polygon.
        /// </summary>
        /// <param name="v"></param>
        public void AddVertice(Vertice v)
        {
            vertices.Add(v);
            UpdateEdges();
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


        public List<Edge> GetEdges()
        {
            return edges;
        }

        public void FinishDrawing()
        {
            fullPolygon = true;
            UpdateEdges();
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
            UpdateEdges();
        }

        public bool IsVerticeInRelation(Vertice v)
        {
            foreach(Relation rel in relations)
            {
                if (rel.IsVerticeInRelation(v))
                    return true;
            }
            return false;
        }

        public bool IsEdgeInRelation(Edge e)
        {
            foreach (Relation rel in relations)
            {
                if (rel.IsEdgeInRelation(e))
                    return true;
            }
            return false;
        }

        public bool IsEdgeInRelation(Vertice v1, Vertice v2)
        {
            return v1.Polygon.IsEdgeInRelation(new Edge(v1, v2, v1.Polygon));
        }

        public void AddEqualityRelation(Edge e1, Edge e2)
        {
            relations.Add(new EqualityRelation(e1, e2));
        }

        public void DeleteLastRelation()
        {
            if (relations.Count == 0)
                return;
            relations.RemoveAt(relations.Count - 1);
        }


        public List<Vertice> GetVerticesToMove(Vertice v, Vertice vEnd, Direction direction)
        {
            Vertice iterator = v;
            List<Vertice> list = new List<Vertice>();
            if(direction == Direction.Forward)
            {
                while(iterator != vEnd)
                {
                    list.Add(iterator);
                    Vertice nextV = iterator.GetNextVertice();
                    if (!iterator.Polygon.IsEdgeInRelation(iterator, nextV))
                    {
                        //if (nextV == vEnd)
                        //    return null;
                        return list;
                    }
                    iterator = nextV;
                }
                return null;
            }
            if(direction == Direction.Backward)
            {
                while (iterator != vEnd)
                {
                    list.Add(iterator);
                    Vertice prevV = iterator.GetPreviousVertice();
                    if (!iterator.Polygon.IsEdgeInRelation(prevV, iterator))
                    {
                        //if (prevV == vEnd)
                        //    return null;
                        return list;
                    }
                    iterator = prevV;
                }
                return null;
            }
            return null;
        }


    }
}
