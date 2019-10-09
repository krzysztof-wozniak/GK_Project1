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
        private Bitmap image;
        private Image oldImage;

        private bool drawingPolygon = false;
        private bool deletingPoint = false;
        private bool deletingPolygon = false;
        private bool movingPolygon = false;
        private bool movingVertice = false;
        private bool movingEdge = false;
        private bool controlKeyDown = false;
        private bool myDrawFlag = false;
        private bool midPoint = false;
        

        private List<Polygon> polygons = new List<Polygon>();
        private Polygon currentPolygon;
        private Vertice chosenVertice = null;
        private Edge chosenEdge = null;

        private const int minDistanceVertice = 15;
        private const int minDistanceEdge = 10;
        private const int radius = 8;
        private static int penWidth = 2;

        private Color activeButtonColor = SystemColors.ActiveCaption;
        private Color normalButtonColor = SystemColors.Control;
        private Pen blackPen = new Pen(Color.Black, penWidth);
        private Pen backColorPen = null;
        private Pen chosenPen = new Pen(Color.Orange, penWidth);
        private SolidBrush backColorBrush = null;
        private SolidBrush chosenBrush = new SolidBrush(Color.Orange);
        private SolidBrush normalBrush = new SolidBrush(Color.Black);

        private Point startingPoint;

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
            nPolygonsLabel.Text = "Number of polygons: 0";
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if(drawingPolygon && polygons[polygons.Count - 1].VerticeCount == 0)
            {
                drawingPolygon = false;
                addPolygonButton.BackColor = normalButtonColor;
                currentPolygon = null;
                return;
            }
            if (deletingPoint)
            {
                deletingPoint = false;
                deleteVerticeButton.BackColor = normalButtonColor;
            }
            if (deletingPolygon)
            {
                deletingPolygon = false;
                deletePolygonButton.BackColor = normalButtonColor;
            }
            if (midPoint)
            {
                midPointButton.BackColor = normalButtonColor;
                midPoint = false;
            }
            if (!drawingPolygon)
            {
                drawingPolygon = true;
                addPolygonButton.BackColor = activeButtonColor;
                polygons.Add(new Polygon(polygons.Count));
                currentPolygon = polygons[polygons.Count - 1];
                return;
            }
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            if (drawingPolygon)
            {
                DrawPolygon(sender, e);
                return;
            }

            if (deletingPoint)
            {
                DeletePoint(sender, e);
                return;
            }

            if(deletingPolygon && (chosenVertice != null || chosenEdge != null))
            {
                int index = chosenVertice == null ? chosenEdge.Polygon.Index : chosenVertice.Polygon.Index;
                DeletePolygon(index);
            }

        }


        private void DeletePoint(object sender, EventArgs e)
        {
            MouseEventArgs mouse = (MouseEventArgs)e;
            if (chosenVertice == null)
                return;
            if (chosenVertice.Polygon.VerticeCount == 3)
            {
                //usun caly
                DeletePolygon(chosenVertice.Polygon);
                
            }
            else
            {
                using (Graphics g = pictureBox.CreateGraphics())
                {
                    Vertice prevVertice = chosenVertice.GetPreviousVertice();
                    Vertice nextVertice = chosenVertice.GetNextVertice();

                    g.FillEllipse(backColorBrush, chosenVertice.Point.X - radius / 2, chosenVertice.Point.Y - radius / 2, radius, radius);

                    MyDraw(backColorPen, prevVertice.Point.X, prevVertice.Point.Y,
                        chosenVertice.Point.X, chosenVertice.Point.Y);
                    MyDraw(backColorPen, nextVertice.Point.X, nextVertice.Point.Y, chosenVertice.Point.X, chosenVertice.Point.Y);

                    chosenVertice.Polygon.DeleteVertice(chosenVertice);
                    chosenVertice = null;
                    
                }
            }
            RefreshPolygons();
            Cursor = Cursors.Arrow;
            deletingPoint = false;
            deleteVerticeButton.BackColor = normalButtonColor;

        }

        private void DeletePolygon(int index)
        {
            if (index < 0 || index >= polygons.Count)
                return;
            for (int i = index + 1; i < polygons.Count; i++)
            {
                polygons[i].ChangeIndex(i - 1);
            }
            this.polygons.RemoveAt(index);
            Cursor = Cursors.Arrow;
            deletingPolygon = false;
            if (chosenVertice != null)
            {
                RedrawPolygon(chosenVertice.Polygon, backColorBrush, backColorPen);
                chosenVertice = null;
            }
            else
            {
                RedrawPolygon(chosenEdge.Polygon, backColorBrush, backColorPen);
                chosenEdge = null;
            }
            RefreshPolygons();
            deletePolygonButton.BackColor = normalButtonColor;
            nPolygonsLabel.Text = "Number of polygons: " + polygons.Count.ToString();

        }

        private void DeletePolygon(Polygon polygon)
        {
            DeletePolygon(polygon.Index);
        }

        private void deletePolygonButton_Click(object sender, EventArgs e)
        {
            if (drawingPolygon)
                return;
            if (deletingPoint)
            {
                deletingPoint = false;
                deleteVerticeButton.BackColor = normalButtonColor;
            }
            if (midPoint)
            {
                midPoint = false;
                midPointButton.BackColor = normalButtonColor;
            }
            deletePolygonButton.BackColor = activeButtonColor;
            deletingPolygon = true;
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
                currentPolygon.FinishDrawing();
                currentPolygon = null;
                drawingPolygon = false;
                addPolygonButton.BackColor = normalButtonColor;
                nPolygonsLabel.Text = "Number of polygons: " + polygons.Count.ToString();
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
                oldImage = pictureBox.Image;
                image = new Bitmap(pictureBox.Image);
                int dx = Math.Abs(x2 - x1), sx = x1 < x2 ? 1 : -1;
                int dy = Math.Abs(y2 - y1), sy = y1 < y2 ? 1 : -1;
                int err = (dx > dy ? dx : -dy) / 2, e2;
                for (; ; )
                {
                    image.SetPixel(x1, y1, pen.Color);
                    if (x1 == x2 && y1 == y2) break;
                    e2 = err;
                    if (e2 > -dx) { err -= dy; x1 += sx; }
                    if (e2 < dy) { err += dx; y1 += sy; }
                }
                pictureBox.Image = image;
                oldImage.Dispose();
            }
            else
            {
                using (Graphics g = pictureBox.CreateGraphics())
                {
                    g.DrawLine(pen, x1, y1, x2, y2);
                }
            }
        }

        private void MyDrawImage(Pen pen, int x1, int y1, int x2, int y2, Bitmap image)
        {
            if (myDrawFlag)
            {
                oldImage = pictureBox.Image;
                image = new Bitmap(pictureBox.Image);
                int dx = Math.Abs(x2 - x1), sx = x1 < x2 ? 1 : -1;
                int dy = Math.Abs(y2 - y1), sy = y1 < y2 ? 1 : -1;
                int err = (dx > dy ? dx : - dy) / 2, e2;
                for (; ; )
                {
                    image.SetPixel(x1, y1, pen.Color);
                    if (x1 == x2 && y1 == y2) break;
                    e2 = err;
                    if (e2 > -dx) { err -= dy; x1 += sx; }
                    if (e2 < dy) { err += dx; y1 += sy; }
                }
                pictureBox.Image = image;
                oldImage.Dispose();
            }
            else
            {
                using (Graphics g = Graphics.FromImage(image))
                {
                    g.DrawLine(pen, x1, y1, x2, y2);
                }
            }
        }



        private (double, Edge) FindNearestEdge(Point point)
        {
            double minDistance = 2 * (pictureBox.Width + pictureBox.Height);
            Edge resEdge = null;
            foreach (Polygon polygon in polygons)
            {
                foreach (Edge edge in polygon.GetEdges())
                {
                    double distance = point.DistanceToEdge(edge);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        resEdge = edge;
                    }
                }
            }
            return (minDistance, resEdge);
        }

        private (double, Vertice) FindNearestVertice(Point point)
        {
            Vertice res = null;
            double distance = 2 * (pictureBox.Width + pictureBox.Height);
            for (int i = 0; i < polygons.Count; i++)
            {
                for (int j = 0; j < polygons[i].VerticeCount; j++)
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


        

        private void ColorChosenEdge(Edge edge) // e -> e, v -> e, nic -> v
        {
            Cursor = Cursors.Hand;
            using (Graphics g = pictureBox.CreateGraphics())
            {
                if (chosenVertice != null) 
                {
                    g.FillEllipse(normalBrush, chosenVertice.Point.X - radius / 2, chosenVertice.Point.Y - radius / 2, radius, radius);
                    chosenVertice = null;
                }
                if (chosenEdge != null && chosenEdge != edge)
                {
                    MyDraw(blackPen, chosenEdge.Vertice1.Point.X, chosenEdge.Vertice1.Point.Y, chosenEdge.Vertice2.Point.X, chosenEdge.Vertice2.Point.Y);
                }
                if (chosenEdge != null && chosenEdge == edge)
                    return;
                chosenEdge = edge;
                MyDraw(chosenPen, chosenEdge.Vertice1.Point.X, chosenEdge.Vertice1.Point.Y, chosenEdge.Vertice2.Point.X, chosenEdge.Vertice2.Point.Y);
                g.FillEllipse(normalBrush, edge.Vertice1.Point.X - radius / 2, edge.Vertice1.Point.Y - radius / 2, radius, radius);
                g.FillEllipse(normalBrush, edge.Vertice2.Point.X - radius / 2, edge.Vertice2.Point.Y - radius / 2, radius, radius);
            }

        }

        private void ColorChosenVertice(Vertice vertice) // e -> v, v -> v, nic -> v
        {
            Cursor = Cursors.Hand;
            if(chosenEdge != null) //jesli byla chosenEdge to na czarno
            {
                MyDraw(blackPen, chosenEdge.Vertice1.Point.X, chosenEdge.Vertice1.Point.Y, chosenEdge.Vertice2.Point.X, chosenEdge.Vertice2.Point.Y);
                chosenEdge = null;
            }
            using (Graphics g = pictureBox.CreateGraphics())
            {
                if (chosenVertice != null && chosenVertice != vertice) //jesli byl inny wierzcholek to na czarno
                {
                    g.FillEllipse(normalBrush, chosenVertice.Point.X - radius / 2, chosenVertice.Point.Y - radius / 2, radius, radius);

                }
                if (chosenVertice == vertice) //czyli juz byl ten pokolorowany
                {
                    return;
                }
                chosenVertice = vertice;
                g.FillEllipse(chosenBrush, chosenVertice.Point.X - radius / 2, chosenVertice.Point.Y - radius / 2, radius, radius);
            }

        }

        private void MoveVertice(Point p)//moze trzeba zmienic
        {
            oldImage = pictureBox.Image;
            image = new Bitmap(oldImage.Width, oldImage.Height);
            chosenVertice.Point = p;
            DrawPolygons(image);
            pictureBox.Image = image;
            oldImage.Dispose();
            
        }

        private void MoveEdge(Point p)
        {
            if (chosenEdge == null)
                return;
            oldImage = pictureBox.Image;
            image = new Bitmap(oldImage.Width, oldImage.Height);
            int dx = p.X - startingPoint.X;
            int dy = p.Y - startingPoint.Y;
            Vertice v1 = chosenEdge.Vertice1;
            Vertice v2 = chosenEdge.Vertice2;
            v1.Point = new Point(v1.Point.X + dx, v1.Point.Y + dy);
            v2.Point = new Point(v2.Point.X + dx, v2.Point.Y + dy);
            startingPoint = p;
            DrawPolygons(image);
            pictureBox.Image = image;
            oldImage.Dispose();
        }

        private void MovePolygon(Point p)
        {
            oldImage = pictureBox.Image;
            image = new Bitmap(oldImage.Width, oldImage.Height);
            int dx = p.X - startingPoint.X;
            int dy = p.Y - startingPoint.Y;
            Polygon polygon;
            if (chosenEdge != null)
                polygon = chosenEdge.Polygon;
            else if (chosenVertice != null)
                polygon = chosenVertice.Polygon;
            else return;

            for(int i = 0; i < polygon.VerticeCount; i++)
            {
                Vertice v = polygon[i];
                v.Point = new Point(v.Point.X + dx, v.Point.Y + dy);
            }
            startingPoint = p;
            DrawPolygons(image);
            pictureBox.Image = image;
            oldImage.Dispose();
        }



        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (drawingPolygon)
                return;
            Point mousePoint = new Point(e.X, e.Y);
            if(controlKeyDown && (chosenVertice != null || chosenEdge != null))
            {
                movingPolygon = true;
                startingPoint = mousePoint;
                return;
            }
            if(chosenEdge != null && controlKeyDown == false)
            {
                movingEdge = true;
                startingPoint = mousePoint;
                return;
            }
            if(chosenVertice != null && controlKeyDown == false)
                movingVertice = true;
        }
        
        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            movingVertice = false;
            movingEdge = false;
            movingPolygon = false;
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePoint = new Point(e.X, e.Y);
            if (movingVertice)
            {
                MoveVertice(mousePoint);
                return;
            }
            else if (movingEdge)
            {
                MoveEdge(mousePoint);
                return;
            }
            else if (movingPolygon)
            {
                MovePolygon(mousePoint);
                return;
            }

            (double distance2Vertice, Vertice vertice) = FindNearestVertice(mousePoint);
            (double distance2Edge, Edge edge) = FindNearestEdge(mousePoint);

            if (chosenVertice != null)
            {
                if (distance2Vertice > minDistanceVertice)
                {
                    using (Graphics g = pictureBox.CreateGraphics())
                    {
                        g.FillEllipse(normalBrush, chosenVertice.Point.X - radius / 2, chosenVertice.Point.Y - radius / 2, radius, radius);
                        chosenVertice = null;
                        Cursor = Cursors.Arrow;
                    }
                }
            }
            if (chosenEdge != null)
            {
                if (distance2Edge > minDistanceEdge)
                {
                    MyDraw(blackPen, chosenEdge.Vertice1.Point.X, chosenEdge.Vertice1.Point.Y, chosenEdge.Vertice2.Point.X, chosenEdge.Vertice2.Point.Y);
                    chosenEdge = null;
                    Cursor = Cursors.Arrow;
                }
            }

            if (vertice != null) //
            {
                if (distance2Vertice <= minDistanceVertice)// && distance2Vertice <= distance2Edge) //jest jakis vertice w zasiegu
                {
                    ColorChosenVertice(vertice);
                    return;
                }
            }
            if (edge != null)
            {
                if (distance2Edge <= minDistanceEdge && distance2Edge < distance2Vertice)
                {
                    ColorChosenEdge(edge);
                }
            }

        }

        

        private void RefreshPolygons()
        {
            using (Graphics g = pictureBox.CreateGraphics())
            {
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
            if (deletingPoint)
            {
                deletingPoint = false;
                deleteVerticeButton.BackColor = normalButtonColor;
                return;
            }
            if (deletingPolygon)
            {
                deletingPolygon = false;
                deletePolygonButton.BackColor = normalButtonColor;
            }
            if (midPoint)
            {
                midPointButton.BackColor = normalButtonColor;
                midPoint = false;
            }
            if (deletingPoint)
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

        

        private void midPointButton_Click(object sender, EventArgs e)
        {
            if (drawingPolygon)
                return;
            if(midPoint)
            {
                midPoint = false;
                midPointButton.BackColor = normalButtonColor;
            }
            if (deletingPoint)
            {
                deletingPoint = false;
                deleteVerticeButton.BackColor = normalButtonColor;
            }
            if (deletingPolygon)
            {
                deletingPolygon = false;
                deletePolygonButton.BackColor = normalButtonColor;
            }
            
            midPointButton.BackColor = activeButtonColor;
            midPoint = true;
        }

        private void checkButtons()
        {

        }

        private void pictureBox_Layout(object sender, EventArgs e)
        {
            RefreshPolygons();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
            {
                controlKeyDown = true;
                controlLabel.Text = "Control";
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
            {
                controlKeyDown = false;
                movingPolygon = false;
                controlLabel.Text = "No control";
            }
        }

        private void DrawPolygons(Bitmap image)
        {
            using (Graphics g = Graphics.FromImage(image))
            {
                for (int i = 0; i < polygons.Count; i++)
                {
                    for (int j = 0; j < polygons[i].VerticeCount; j++)
                    {
                        Vertice curVertice = polygons[i][j];
                        Vertice nextVertice = curVertice.GetNextVertice();
                        if (curVertice == chosenVertice)
                            g.FillEllipse(chosenBrush, curVertice.Point.X - radius / 2, curVertice.Point.Y - radius / 2, radius, radius);
                        else
                            g.FillEllipse(normalBrush, curVertice.Point.X - radius / 2, curVertice.Point.Y - radius / 2, radius, radius);
                        if (chosenEdge != null)
                        {
                            if ((chosenEdge.Vertice1 == curVertice && chosenEdge.Vertice2 == nextVertice) || (chosenEdge.Vertice1 == nextVertice && chosenEdge.Vertice2 == curVertice))
                                MyDrawImage(chosenPen, curVertice.Point.X, curVertice.Point.Y, nextVertice.Point.X, nextVertice.Point.Y, image);
                            else
                                MyDrawImage(blackPen, curVertice.Point.X, curVertice.Point.Y, nextVertice.Point.X, nextVertice.Point.Y, image);
                        }
                        else
                            MyDrawImage(blackPen, curVertice.Point.X, curVertice.Point.Y, nextVertice.Point.X, nextVertice.Point.Y, image);

                    }
                }
            }
        }

    }
}
