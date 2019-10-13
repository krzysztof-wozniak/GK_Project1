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
using System.IO;
using System.Reflection;

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
        private bool myDrawFlag = true;
        private bool midPoint = false;
        private bool equalRelation = false;
        private bool perpenRelation = false;


        private Bitmap EqualIcon;
        

        private List<Polygon> polygons = new List<Polygon>();
        private Polygon currentPolygon;
        private Vertice chosenVertice = null;
        private Edge chosenEdge = null;
        private Edge E1 = null;
        private Edge E2 = null;

        List<(Vertice, Vertice)> relationlist = new List<(Vertice, Vertice)>();


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

            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream myStream = assembly.GetManifestResourceStream("GK_Projekt1.equal-symbol2.jpg");
            EqualIcon = new Bitmap(myStream);
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



        private void MoveVertice(Point p)//nie dziala dla trojkata z relacja, trzeba zrobic inna GetVerticesToMove
        {
            oldImage = pictureBox.Image;
            image = new Bitmap(pictureBox.Width, pictureBox.Height);
            int dx = p.X - chosenVertice.Point.X;
            int dy = p.Y - chosenVertice.Point.Y;
            Vertice nextVertice = chosenVertice.GetNextVertice();
            Vertice prevVertice = chosenVertice.GetPreviousVertice();
            bool nextEdgeRelation = chosenVertice.Polygon.IsEdgeInRelation(chosenVertice, nextVertice);
            bool prevEdgeRelation = chosenVertice.Polygon.IsEdgeInRelation(prevVertice, chosenVertice);
            if (nextEdgeRelation && prevEdgeRelation)
            {
                var list = chosenVertice.Polygon.GetVerticesToMove(chosenVertice, Direction.Forward);
                if (chosenVertice.RelationPrevVertice.IsEquality())
                {
                    EqualityRelation equalityRelation = (EqualityRelation)chosenVertice.RelationPrevVertice;
                    MoveAroundCircle(prevVertice, equalityRelation.Length, p, list);
                }

            }
            else if(nextEdgeRelation)//poprzedni mozna zmieniac
            {
                if (chosenVertice.RelationNextVertice.IsEquality())
                {
                    EqualityRelation equalityRelation = (EqualityRelation)chosenVertice.RelationNextVertice;
                    List<Vertice> list = new List<Vertice>();
                    list.Add(chosenVertice);
                    MoveAroundCircle(nextVertice, equalityRelation.Length, p, list);
                }

            }
            else if(prevEdgeRelation)
            {
                if (chosenVertice.RelationPrevVertice.IsEquality())
                {
                    EqualityRelation equalityRelation = (EqualityRelation)chosenVertice.RelationPrevVertice;
                    List<Vertice> list = new List<Vertice>();
                    list.Add(chosenVertice);
                    MoveAroundCircle(prevVertice, equalityRelation.Length, p, list);
                }
            }
            else
                chosenVertice.Point = p;
            DrawPolygons(image);
            pictureBox.Image = image;
            oldImage.Dispose();

        }

        private void MoveAroundCircle(Vertice vCenter, double Length, Point point, List<Vertice> list)
        {
            double dx = point.X - vCenter.Point.X;
            double dy = point.Y - vCenter.Point.Y;
            double distanceToCenter = Math.Sqrt(Math.Pow(vCenter.Point.X - point.X, 2) + Math.Pow(vCenter.Point.Y - point.Y, 2));
            double ratio = Length / distanceToCenter;
            //poszlo do gory
            
            dx *= ratio;
            dy *= ratio;
            //double ratio = / Length;
            int x = vCenter.Point.X + (int)dx;
            int y = vCenter.Point.Y + (int)dy;

            int dx1 = x - chosenVertice.Point.X;
            int dy1 = y - chosenVertice.Point.Y;

            foreach(Vertice v in list)
            {
                v.MoveVertice(dx1, dy1);
            }

        }

        private void MoveEdge(Point p)
        {
            if (chosenEdge == null)
                return;
            oldImage = pictureBox.Image;
            image = new Bitmap(pictureBox.Width, pictureBox.Height);
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
            image = new Bitmap(pictureBox.Width, pictureBox.Height);
            int dx = p.X - startingPoint.X;
            int dy = p.Y - startingPoint.Y;
            Polygon polygon;
            if (chosenEdge != null)
                polygon = chosenEdge.Polygon;
            else if (chosenVertice != null)
                polygon = chosenVertice.Polygon;
            else return;

            for (int i = 0; i < polygon.VerticeCount; i++)
            {
                Vertice v = polygon[i];
                v.Point = new Point(v.Point.X + dx, v.Point.Y + dy);
            }
            startingPoint = p;
            DrawPolygons(image);
            pictureBox.Image = image;
            oldImage.Dispose();
        }

        private void DeletePoint()
        {
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
            UpdatePictureBox();
            chosenVertice = null;
            chosenEdge = null;
            deletePolygonButton.BackColor = normalButtonColor;
            nPolygonsLabel.Text = "Number of polygons: " + polygons.Count.ToString();

        }

        private void DeletePolygon(Polygon polygon)
        {
            DeletePolygon(polygon.Index);
        }

        private void AddMidPoint()
        {
            if (chosenEdge == null)
                return;
            Point newPoint = new Point((chosenEdge.Vertice1.Point.X + chosenEdge.Vertice2.Point.X) / 2, (chosenEdge.Vertice1.Point.Y + chosenEdge.Vertice2.Point.Y) / 2);
                chosenEdge.Polygon.AddMidPoint(chosenEdge.Vertice1, newPoint);
            using (Graphics g = pictureBox.CreateGraphics())
            {
                g.FillEllipse(normalBrush, newPoint.X - radius / 2, newPoint.Y - radius / 2, radius, radius);
            }
            MyDraw(blackPen, chosenEdge.Vertice1.Point.X, chosenEdge.Vertice1.Point.Y, chosenEdge.Vertice2.Point.X, chosenEdge.Vertice2.Point.Y);

            chosenEdge = null;
            midPoint = false;
            midPointButton.BackColor = normalButtonColor;

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

        private void AddEqualityRelation()
        {
            if (chosenEdge == null)
                return;
            if (chosenEdge.Polygon.IsEdgeInRelation(chosenEdge))
            {
                E1 = null;
                E2 = null;
                addEqualRButton.BackColor = normalButtonColor;
                equalRelation = false;
                return;
            }
            if(E1 == null)
            {
                E1 = chosenEdge;
                return;
            }
            if(E2 == null)
            {
                if (chosenEdge == E1 || E1.Polygon != chosenEdge.Polygon)
                    return;

                E2 = chosenEdge;
                chosenEdge = null;

                E2.Polygon.AddEqualityRelation(E1, E2);

                Vertice V1 = E2.Vertice1;
                Vertice V2 = E2.Vertice2;
                Direction direction = Direction.Forward;
                List<Vertice> VerticesToMove = V1.Polygon.GetVerticesToMove(V2, E1.Vertice1, direction);
                if(VerticesToMove == null)
                {
                    direction = Direction.Backward;
                    VerticesToMove = V1.Polygon.GetVerticesToMove(V1, E1.Vertice2, direction);
                }

                if(VerticesToMove == null)
                {
                    E2.Polygon.DeleteLastRelation();
                    E1 = null;
                    E2 = null;
                    addEqualRButton.BackColor = normalButtonColor;
                    equalRelation = false;
                    return;
                }

                //mam liste
                int dx, dy, newdxE2, newdyE2;
                double dxE2, dyE2;
                double ratio = E1.Length() / E2.Length();
                if (direction == Direction.Forward) //do przodu, ruszamy V2, V1 stoi
                {
                    dxE2 = V2.Point.X - V1.Point.X; //Wektor V1 -> v2
                    dyE2 = V2.Point.Y - V1.Point.Y;
                    newdxE2 = (int)(dxE2 * ratio);
                    newdyE2 = (int)(dyE2 * ratio);
                    dx = V1.Point.X + newdxE2 - V2.Point.X; //nowy punkt minus stary
                    dy = V1.Point.Y + newdyE2 - V2.Point.Y;
                }
                else
                {
                    dxE2 = V1.Point.X - V2.Point.X; //Wektor V2 -> V1
                    dyE2 = V1.Point.Y - V2.Point.Y;
                    newdxE2 = (int)(dxE2 * ratio);
                    newdyE2 = (int)(dyE2 * ratio);
                    dx = V2.Point.X + newdxE2 - V1.Point.X;
                    dy = V2.Point.Y + newdyE2 - V1.Point.Y;
                }

                foreach(Vertice v in VerticesToMove)
                {
                    v.MoveVertice(dx, dy);
                }
                debugLabel.Text = $"L(E1) - {E1.Length()}, L(E2) - {E2.Length()}, E1 - P{E1.Polygon.Index},{E1.Vertice1.Index}-{E1.Vertice2.Index}, E2 - P{E2.Polygon.Index},{E2.Vertice1.Index.ToString()}-{E2.Vertice2.Index.ToString()}";

                UpdatePictureBox();
                addEqualRButton.BackColor = normalButtonColor;
                equalRelation = false;
                E1 = null;
                E2 = null;

            }
        }





        private void MyDraw(Pen pen, int x1, int y1, int x2, int y2)
        {
            
            if (myDrawFlag)
            {
                Bitmap image = (Bitmap)pictureBox.Image;
                //image = new Bitmap(pictureBox.Image);
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
                //oldImage.Dispose();
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

        private void pictureBox_Click(object sender, EventArgs e)
        {
            if (drawingPolygon)
            {
                DrawPolygon(sender, e);
                return;
            }

            if (deletingPoint)
            {
                DeletePoint();
                return;
            }

            if (deletingPolygon && (chosenVertice != null || chosenEdge != null))
            {
                int index = chosenVertice == null ? chosenEdge.Polygon.Index : chosenVertice.Polygon.Index;
                DeletePolygon(index);
                return;
            }

            if(midPoint && chosenEdge != null)
            {
                AddMidPoint();
                return;
            }

            if(equalRelation)
            {
                AddEqualityRelation();
                
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
            if (drawingPolygon && currentPolygon.VerticeCount == 0)
            {
                drawingPolygon = false;
                addPolygonButton.BackColor = normalButtonColor;
                polygons.RemoveAt(currentPolygon.Index);
            }
            else if(midPoint)
            {
                midPoint = false;
                midPointButton.BackColor = normalButtonColor;
                return;
            }
            else if (deletingPoint)
            {
                deletingPoint = false;
                deleteVerticeButton.BackColor = normalButtonColor;
            }
            else if (deletingPolygon)
            {
                deletingPolygon = false;
                deletePolygonButton.BackColor = normalButtonColor;
            }
            else if (drawingPolygon)
                return;

            midPointButton.BackColor = activeButtonColor;
            midPoint = true;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (drawingPolygon && currentPolygon.VerticeCount == 0)
            {
                drawingPolygon = false;
                addPolygonButton.BackColor = normalButtonColor;
                polygons.RemoveAt(currentPolygon.Index);
            }
            else if (deletingPoint)
            {
                deletingPoint = false;
                deleteVerticeButton.BackColor = normalButtonColor;
                return;
            }
            else if (deletingPolygon)
            {
                deletingPolygon = false;
                deletePolygonButton.BackColor = normalButtonColor;
            }
            else if (midPoint)
            {
                midPointButton.BackColor = normalButtonColor;
                midPoint = false;
            }
            else if (drawingPolygon)
                return;
            deletingPoint = true;
            deleteVerticeButton.BackColor = activeButtonColor;

        }

        private void deletePolygonButton_Click(object sender, EventArgs e)
        {
            if (drawingPolygon && currentPolygon.VerticeCount == 0)
            {
                drawingPolygon = false;
                addPolygonButton.BackColor = normalButtonColor;
                polygons.RemoveAt(currentPolygon.Index);
            }
            else if (deletingPoint)
            {
                deletingPoint = false;
                deleteVerticeButton.BackColor = normalButtonColor;
            }
            else if (midPoint)
            {
                midPoint = false;
                midPointButton.BackColor = normalButtonColor;
            }
            else if (deletingPolygon)
            {
                deletingPolygon = false;
                deletePolygonButton.BackColor = normalButtonColor;
                return;
            }
            else if (drawingPolygon)
                return;
            deletePolygonButton.BackColor = activeButtonColor;
            deletingPolygon = true;
        }

        private void addEqualRButton_Click(object sender, EventArgs e)
        {
            addEqualRButton.BackColor = activeButtonColor;
            equalRelation = true;
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
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
            {
                controlKeyDown = false;
                movingPolygon = false;
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
                        Edge e = new Edge(curVertice, nextVertice, curVertice.Polygon);
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

                        if(e.Polygon.IsEdgeInRelation(e))
                        {

                            g.DrawImage(EqualIcon, (curVertice.Point.X + nextVertice.Point.X) / 2, (curVertice.Point.Y + nextVertice.Point.Y) / 2, 30, 20);
                        }

                    }
                }
            }
        }

        private void UpdatePictureBox()
        {
            oldImage = pictureBox.Image;
            image = new Bitmap(pictureBox.Width, pictureBox.Height);
            DrawPolygons(image);
            pictureBox.Image = image;
            oldImage.Dispose();
        }

        


       
    }
    public enum Direction { Forward, Backward };
}
