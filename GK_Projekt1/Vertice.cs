﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK_Projekt1
{
    public class Vertice
    {
        private Point point;
        private int index; //index of the vertice in the polygon
        private Polygon polygon;
        private Relation relationNextVertice = null;
        private Relation relationPrevVertice = null;

        public Polygon Polygon { get => polygon; set => polygon = value; }

        public Vertice(Point point, Polygon polygon, int index)
        {
            this.point = point;
            this.polygon = polygon;
            this.index = index;
        }

        public int Index { get => index; set => index = value; }

        public Point Point { get => point; set => point = value; }

        public double DistanceToPoint(Point point)
        {
            return this.Point.DistanceToPoint(point);
        }

        public double DistanceToEdge(Edge edge)
        {
            return this.Point.DistanceToEdge(edge.Vertice1.Point, edge.Vertice2.Point);
        }

        public Vertice GetNextVertice()
        {
            return polygon[(index + 1) % polygon.VerticeCount];
        }

        public Vertice GetPreviousVertice()
        {
            if(index == 0)
            {
                return polygon[polygon.VerticeCount - 1];
            }
            return polygon[index - 1];
        }

        public void MoveVertice(int dx, int dy)
        {
            this.point.Offset(dx, dy);
           
        }

        public bool InRelation
        {
            get
            {
                return this.polygon.IsVerticeInRelation(this);
            }
        }

        public Relation RelationNextVertice
        {
            get => relationNextVertice;
            set => relationNextVertice = value;
        }

        public Relation RelationPrevVertice
        {
            get => relationPrevVertice;
            set => relationPrevVertice = value;
        }

    }
}
