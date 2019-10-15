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
        public List<EqualityRelation> equalityRelations = new List<EqualityRelation>();


        public Polygon(int index)
        {
            this.index = index;
        }

        public bool FullPolygon { get => fullPolygon; }

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

        public int RelationCount => relations.Count;

        public int RelationIndex(Relation r) => relations.IndexOf(r);

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
            EqualityRelation r = new EqualityRelation(e1, e2, relations.Count );
            relations.Add(r);
            e1.Vertice1.RelationNextVertice = r;
            e1.Vertice2.RelationPrevVertice = r;
            e2.Vertice1.RelationNextVertice = r;
            e2.Vertice2.RelationPrevVertice = r;
        }

        public void AddPerpendicularRelation(Edge e1, Edge e2)
        {
            PerpendicularRelation r = new PerpendicularRelation(e1, e2, relations.Count);
            relations.Add(r);
            e1.Vertice1.RelationNextVertice = r;
            e1.Vertice2.RelationPrevVertice = r;
            e2.Vertice1.RelationNextVertice = r;
            e2.Vertice2.RelationPrevVertice = r;
        }
        public void DeleteLastRelation()
        {
            if (relations.Count == 0)
                return;
            Relation r = relations[relations.Count - 1];
            r.E1V1.RelationNextVertice = null;
            r.E1V2.RelationPrevVertice = null;
            r.E2V1.RelationNextVertice = null;
            r.E2V2.RelationPrevVertice = null;
            relations.RemoveAt(relations.Count - 1);
        }

        public void DeleteRelation(Edge e)
        {
            if(e.Polygon.IsEdgeInRelation(e))
            {
                Relation r = e.Vertice1.RelationNextVertice;
                for(int i = r.Index + 1; i < e.Polygon.RelationCount; i++)
                {
                    e.Polygon.relations[i].ChangeIndex(i - 1);
                }
                r.E1V1.RelationNextVertice = null;
                r.E1V2.RelationPrevVertice = null;
                r.E2V1.RelationNextVertice = null;
                r.E2V2.RelationPrevVertice = null;
                relations.RemoveAt(r.Index);
            }
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
                        return list;
                    }
                    iterator = prevV;
                }
                return null;
            }
            return null;
        }

        public List<Vertice> GetVerticesToMove(Vertice v, Direction direction)
        {
            Vertice iterator = v;
            List<Vertice> list = new List<Vertice>();
            Vertice nextV = iterator.GetNextVertice();
            Vertice prevV = iterator.GetPreviousVertice();
            if (direction == Direction.Forward)
            {
                do
                {
                    list.Add(iterator);
                    nextV = iterator.GetNextVertice();
                    if (!iterator.Polygon.IsEdgeInRelation(iterator, nextV))
                    {
                        return list;
                    }
                    iterator = nextV;
                } while (iterator != v);
                return list;
            }
            do
            {
                list.Add(iterator);
                prevV = iterator.GetPreviousVertice();
                if (!iterator.Polygon.IsEdgeInRelation(prevV, iterator))
                {
                    return list;
                }
                iterator = prevV;
            } while (iterator != v);
            return list;
        }
    }
}
