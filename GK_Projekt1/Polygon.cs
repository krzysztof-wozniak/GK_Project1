using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK_Projekt1
{
    public class Polygon
    {
        private List<Vertice> vertices = new List<Vertice>();
        private int index;

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
        
        public int VerticeCount
        {
            get => vertices.Count;
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
            for(int i = v.Index + 1; i < v.Polygon.VerticeCount; i++)
            {
                v.Polygon.vertices[i].Index--;
            }
            v.Polygon.vertices.RemoveAt(v.Index);
        }

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
    }
}
