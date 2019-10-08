using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GK_Projekt1
{

    

    public partial class Form1 : Form
    {
        private static Bitmap image;
        private static bool drawingPolygon = false;
        private const int radius = 8;
        private static bool myDrawFlag = false;

        private Color activeButtonColor = SystemColors.ActiveCaption;
        private Color normalButtonColor = SystemColors.Control;


        //private List<List<Point>> polygons = new List<List<Point>>();
        private List<Polygon> polygons = new List<Polygon>();


        //private List<Point> currentPolygon;
        private Polygon currentPolygon;


        private const int minDistance = 10;


        private Vertice chosenVertice = null;

        private static bool movingVertice = false;
        private static int penWidth = 2;
        private Pen blackPen = new Pen(Color.Black, penWidth);
        private Pen backColorPen = null;
        private bool deletingPoint = false;
        private SolidBrush backColorBrush = null;
        private SolidBrush chosenBrush = new SolidBrush(Color.Orange);
        private SolidBrush normalBrush = new SolidBrush(Color.Black);

        public Form1()
        {
            InitializeComponent();
            
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            backColorPen = new Pen(pictureBox.BackColor, penWidth);
            backColorBrush = new SolidBrush(pictureBox.BackColor);
        }

        protected override void OnShown(EventArgs e)
        {
            image = new Bitmap(pictureBox.Width, pictureBox.Height);
            pictureBox.Image = image;
            this.DoubleBuffered = true;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (!drawingPolygon)
            {
                drawingPolygon = true;
                addPolygonButton.BackColor = activeButtonColor;
                polygons.Add(new Polygon(polygons.Count));
                currentPolygon = polygons[polygons.Count - 1];
                
                return;
            }
            drawingPolygon = false;
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            if(drawingPolygon)
            {
                DrawPolygon(sender, e);
            }

            if(deletingPoint)
            {
                DeletePoint(sender, e);
            }

        }

        private void DeletePoint(object sender, EventArgs e)
        {
            MouseEventArgs mouse = (MouseEventArgs)e;
            if (chosenVertice == null)
                return;
            if(chosenVertice.Polygon.VerticeCount == 3)
            {
                //usun caly
                DeletePolygon(chosenVertice.Polygon);
                RedrawPolygon(chosenVertice.Polygon, backColorBrush, backColorPen);
                chosenVertice = null;
            }
            else
            {
                using (Graphics g = pictureBox.CreateGraphics())
                {
                    ////int prevInd = GetPrevIndex();
                    ////int nextInd = GetNextIndex();
                    ////Point prevPoint = polygons[nearestPointPolygonIndex][prevInd];
                    ////Point nextPoint = polygons[nearestPointPolygonIndex][nextInd];

                    Vertice prevVertice = chosenVertice.GetPreviousVertice();
                    Vertice nextVertice = chosenVertice.GetNextVertice();

                    g.FillEllipse(backColorBrush, chosenVertice.Point.X - radius / 2, chosenVertice.Point.Y - radius / 2, radius, radius);

                    MyDraw(backColorPen, prevVertice.Point.X, prevVertice.Point.Y, 
                        chosenVertice.Point.X, chosenVertice.Point.Y);
                    MyDraw(backColorPen, nextVertice.Point.X, nextVertice.Point.Y, chosenVertice.Point.X, chosenVertice.Point.Y);

                    chosenVertice.Polygon.DeleteVertice(chosenVertice);
                    chosenVertice = null;
                    //polygons[nearestPointPolygonIndex].RemoveAt(nearestPointIndex);
                }
                
                //chosenVertice = null;
                //nearestPointIndex = -1;
                //nearestPointPolygonIndex = -1;
                //FindNearestPoint(new Point(mouse.X, mouse.Y));
                    
            }
            RefreshPolygons();
            deletingPoint = false;
            deleteVerticeButton.BackColor = normalButtonColor;

        }

        private void DrawPolygon(object sender, EventArgs e)
        {
            Vertice a, b;
            MouseEventArgs mouseEventArgs = (MouseEventArgs)e;
            Point clickedPoint = new Point(mouseEventArgs.X, mouseEventArgs.Y);

            if (currentPolygon.VerticeCount >= 3 && chosenVertice == currentPolygon[0]) //koniec
            {
                a = currentPolygon.GetFirstVertice();
                b = currentPolygon.GetLastVertice();
                MyDraw(blackPen, a.Point.X, a.Point.Y, b.Point.X, b.Point.Y);
                currentPolygon = null;
                drawingPolygon = false;
                addPolygonButton.BackColor = normalButtonColor;
                return;
            }


            if ((currentPolygon.VerticeCount == 2 || currentPolygon.VerticeCount == 1) &&
                chosenVertice != null && chosenVertice == currentPolygon.GetFirstVertice()) // co najmniej trzy wierzcholki
                return;
            if (currentPolygon.VerticeCount > 2 && chosenVertice != null && 
                chosenVertice != currentPolygon.GetFirstVertice()) //koniec == poczatek
                return;

            Vertice v = new Vertice(new Point(mouseEventArgs.X, mouseEventArgs.Y), currentPolygon, currentPolygon.VerticeCount);
            currentPolygon.AddVertice(v);

            using (Graphics g = pictureBox.CreateGraphics())
            {
                g.FillEllipse(normalBrush, mouseEventArgs.X - radius / 2, mouseEventArgs.Y - radius / 2, radius, radius);
            }
            if (currentPolygon.VerticeCount < 2)
                return;
            a = v.GetPreviousVertice();
            b = v;
            MyDraw(blackPen, a.Point.X, a.Point.Y, b.Point.X, b.Point.Y);
        }


        private void MyDraw(Pen pen, int x1, int y1, int x2, int y2)
        {
            if (myDrawFlag)
            {
            }
            else
            {
                using (Graphics g = pictureBox.CreateGraphics())
                {
                    g.DrawLine(pen, x1, y1, x2, y2);
                }
            }
        }
        
        

        private (int, Edge)? FindNearestEdge(Point point)
        {
            return null;
        }

        private (double, Vertice) FindNearestVertice(Point point)
        {
            Vertice res = null;
            double distance = 2 * (pictureBox.Width + pictureBox.Height);
            for(int i = 0; i < polygons.Count; i++)
            {
                for(int j = 0; j < polygons[i].VerticeCount; j++)
                {
                    double curDistance = polygons[i][j].DistanceToPoint(point);
                    if (curDistance < distance)
                    {
                        distance = curDistance;
                        res = polygons[i][j];
                    }
                }
            }
            return (distance, res);
        }

        private (double, Point, Point)? FindNearestEdge(Point point)
        {
            return null;
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePoint = new Point(e.X, e.Y);
            if (movingVertice)
                MoveVertice(sender, e);
            
            (double distance, Vertice vertice)  = FindNearestVertice(mousePoint);
            if (vertice != null)
                HighlightChosenPoint(distance, vertice);
            
        }

        private void MoveVertice(object sender, MouseEventArgs e)//moze trzeba zmienic
        {
            Point mousePoint = new Point(e.X, e.Y);
            int dx = e.X - chosenVertice.Point.X;
            int dy = e.Y - chosenVertice.Point.Y;
            using (Graphics g = pictureBox.CreateGraphics())
            {
                g.FillEllipse(backColorBrush, chosenVertice.Point.X - radius / 2, chosenVertice.Point.Y - radius / 2, radius, radius);
                //g.FillEllipse(Brushes.Black, e.X - radius / 2, e.Y - radius / 2, radius, radius);

                //int prevIndex = GetPrevIndex();
                //int nextIndex = GetNextIndex();
                //Point prev = polygons[nearestPointPolygonIndex][prevIndex];
                //Point next = polygons[nearestPointPolygonIndex][nextIndex];
                Vertice prevVertice = chosenVertice.GetPreviousVertice();
                Vertice nextVertice = chosenVertice.GetNextVertice();
                MyDraw(blackPen, prevVertice.Point.X, prevVertice.Point.Y, mousePoint.X, mousePoint.Y); //--------------------------------------czy pomaga?
                MyDraw(blackPen, nextVertice.Point.X, nextVertice.Point.Y, mousePoint.X, mousePoint.Y);
                MyDraw(backColorPen, chosenVertice.Point.X, chosenVertice.Point.Y, prevVertice.Point.X, prevVertice.Point.Y);
                MyDraw(backColorPen, chosenVertice.Point.X, chosenVertice.Point.Y, nextVertice.Point.X, nextVertice.Point.Y);
            }
            //chosenVertice = mousePoint;
            //FindNearestVertice(mousePoint);
            chosenVertice.Point = mousePoint;
            RefreshPolygons();
            
        }

        private void HighlightChosenPoint(double distance, Vertice vertice)
        {
            using (Graphics g = pictureBox.CreateGraphics())
            {
                if (vertice != null && chosenVertice == vertice && distance <= minDistance) //nie zmienia sie
                {
                    this.Cursor = Cursors.Hand;
                    return;
                }

                if (chosenVertice != null && chosenVertice == vertice && distance > minDistance) //odchodzi i nic
                {
                    g.FillEllipse(normalBrush, chosenVertice.Point.X - radius / 2, chosenVertice.Point.Y - radius / 2, radius, radius);
                    chosenVertice = null;
                    Cursor = Cursors.Arrow;
                    return;
                }

                if (chosenVertice != null && chosenVertice != vertice && distance <= minDistance) //zmienia sie chosenVertice
                {
                    g.FillEllipse(normalBrush, chosenVertice.Point.X - radius / 2, chosenVertice.Point.Y - radius / 2, radius, radius);
                    chosenVertice = vertice;
                    g.FillEllipse(chosenBrush, chosenVertice.Point.X - radius / 2, chosenVertice.Point.Y - radius / 2, radius, radius);
                    //Cursor = Cursors.Hand;
                    return;
                }

                if (chosenVertice != null && chosenVertice != vertice && distance > minDistance)//niemozliwe?
                {
                    Cursor = Cursors.Arrow;
                    return;
                }

                if (chosenVertice == null && distance <= minDistance)//nowy chosenVertice
                {
                    chosenVertice = vertice;
                    g.FillEllipse(chosenBrush, chosenVertice.Point.X - radius / 2, chosenVertice.Point.Y - radius / 2, radius, radius);
                    Cursor = Cursors.Hand;
                    return;
                }

                if (chosenVertice == null && distance > minDistance)
                {
                    Cursor = Cursors.Arrow;
                    return;
                }
                Cursor = Cursors.Arrow;
            }
        }


        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (drawingPolygon)
                return;
            Point mousePoint = new Point(e.X, e.Y);
            (double distance, Vertice vertice) = FindNearestVertice(mousePoint);
            if (vertice == null || distance > minDistance)
                return;
            //jest wybrany
            movingVertice = true;

        }

        //private int GetPrevIndex()
        //{
        //    if (nearestPointIndex == 0)
        //        return polygons[nearestPointPolygonIndex].Count - 1;
        //    return nearestPointIndex - 1;
        //}

        //private int GetNextIndex()
        //{
        //    return (nearestPointIndex + 1) % polygons[nearestPointPolygonIndex].Count;
        //}

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            movingVertice = false;
        }

        private void RefreshPolygons()
        {
            //Bitmap bitmap = new Bitmap(pictureBox.Width, pictureBox.Height);
            //pictureBox.Image = bitmap;
            using (Graphics g = pictureBox.CreateGraphics())
            {
                //g.Clear(pictureBox.BackColor);
                this.DoubleBuffered = true;
                for (int i = 0; i < polygons.Count; i++)
                {
                    for (int j = 0; j < polygons[i].VerticeCount; j++)
                    {
                        Vertice curVertice = polygons[i][j];
                        Vertice nextVertice = curVertice.GetNextVertice();
                        if(curVertice == chosenVertice)
                            g.FillEllipse(chosenBrush, curVertice.Point.X - radius / 2, curVertice.Point.Y - radius / 2, radius, radius);
                        else
                            g.FillEllipse(normalBrush, curVertice.Point.X - radius / 2, curVertice.Point.Y - radius / 2, radius, radius);

                        MyDraw(blackPen, curVertice.Point.X, curVertice.Point.Y, nextVertice.Point.X, nextVertice.Point.Y);
                    }
                }
            }

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (drawingPolygon)
            {
                
                return;
            }
            if(deletingPoint)
            {
                deletingPoint = false;
                deleteVerticeButton.BackColor = normalButtonColor;
                return;
            }
            deletingPoint = true;
            deleteVerticeButton.BackColor = activeButtonColor;

        }

        private void pictureBox_Layout(object sender, LayoutEventArgs e)
        {
            RefreshPolygons();
        }

        private void DeletePolygon(int index)
        {
            if (index < 0 || index >= polygons.Count)
                return;
            for(int i = index + 1; i < polygons.Count; i++)
            {
                polygons[i].ChangeIndex(i - 1);
            }
            this.polygons.RemoveAt(index);
            Cursor = Cursors.Arrow;
        }

        private void DeletePolygon(Polygon polygon)
        {
            DeletePolygon(polygon.Index);
        }

        private void RedrawPolygon(Polygon polygon, Brush brush, Pen pen)
        {
            using (Graphics g = pictureBox.CreateGraphics())
            {
                foreach (Vertice vertice in polygon.Vertices)
                {
                    g.FillEllipse(brush, vertice.Point.X - radius / 2, vertice.Point.Y - radius / 2, radius, radius);
                    Vertice nextVertice = vertice.GetNextVertice();
                    MyDraw(pen, vertice.Point.X, vertice.Point.Y, nextVertice.Point.X, nextVertice.Point.Y);
                }
            }
        }

    }
}
