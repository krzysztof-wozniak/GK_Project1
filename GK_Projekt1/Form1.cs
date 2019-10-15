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
        private bool myDrawFlag = false;
        private bool midPoint = false;
        private bool equalRelation = false;
        private bool perpenRelation = false;
        private bool deletingRelation = false;
    

        private Font font = new Font("Calibri", 15);
        private Brush fontBrush = new SolidBrush(Color.Green);

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
        private Pen relationPen = new Pen(Color.Red, penWidth);

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
            //oldImage = pictureBox.Image;
            //image = new Bitmap(pictureBox.Width, pictureBox.Height);
            int dx = p.X - chosenVertice.Point.X;
            int dy = p.Y - chosenVertice.Point.Y;
            Vertice nextVertice = chosenVertice.GetNextVertice();
            Vertice prevVertice = chosenVertice.GetPreviousVertice();
            bool nextEdgeRelation = chosenVertice.Polygon.IsEdgeInRelation(chosenVertice, nextVertice);
            bool prevEdgeRelation = chosenVertice.Polygon.IsEdgeInRelation(prevVertice, chosenVertice);
            bool backward = false;
            if (nextEdgeRelation && prevEdgeRelation)
            {
                
                if (chosenVertice.RelationNextVertice == chosenVertice.RelationPrevVertice && chosenVertice.RelationNextVertice.IsEquality())
                {
                    var list = chosenVertice.Polygon.GetVerticesToMove(chosenVertice, Direction.Forward);
                    var list2 = chosenVertice.Polygon.GetVerticesToMove(chosenVertice, Direction.Backward);
                    if (list2.Count < list.Count)
                    {
                        backward = true;
                        list = list2;

                    }
                    if (backward)
                    {
                        Vertice temp;
                        temp = nextVertice;
                        nextVertice = prevVertice;
                        prevVertice = temp;
                    }
                    double chosenNextdx = nextVertice.Point.X - chosenVertice.Point.X;
                    double chosenNextdy = nextVertice.Point.Y - chosenVertice.Point.Y;
                    double chosenNextLen = chosenVertice.Point.DistanceToPoint(nextVertice.Point);
                    Edge prevEdge = new Edge(prevVertice, chosenVertice, chosenVertice.Polygon);
                    Point oldPoint = chosenVertice.Point;
                    chosenVertice.MoveVertice(p.X - oldPoint.X, p.Y - oldPoint.Y);
                    int chosenPrevdx = chosenVertice.Point.X - prevVertice.Point.X;
                    int chosenPrevdy = chosenVertice.Point.Y - prevVertice.Point.Y;
                    double chosenPrevLen = chosenVertice.Point.DistanceToPoint(prevVertice.Point);
                    double ratio = chosenPrevLen / chosenNextLen;
                    int newdx = (int)(chosenNextdx * ratio);
                    int newdy = (int)(chosenNextdy * ratio);
                    int newX = chosenVertice.Point.X + newdx;
                    int newY = chosenVertice.Point.Y + newdy;
                    list.Remove(chosenVertice);
                    dx = newX - nextVertice.Point.X;
                    dy = newY - nextVertice.Point.Y;
                    foreach (Vertice v in list)
                    {
                        v.MoveVertice(dx, dy);
                    }
                    ((EqualityRelation)chosenVertice.RelationNextVertice).UpdateLength();
                } //dwie tej samej rownosci
                else if (chosenVertice.RelationPrevVertice.IsEquality() && chosenVertice.RelationNextVertice.IsEquality()) //rozne ale obie rownosci
                {
                    var list = chosenVertice.Polygon.GetVerticesToMove(chosenVertice, Direction.Forward);
                    var list2 = chosenVertice.Polygon.GetVerticesToMove(chosenVertice, Direction.Backward);
                    list.AddRange(list2);
                    list = list.Distinct().ToList();
                    foreach (Vertice v in list)
                        v.MoveVertice(dx, dy);
                }

            }
            else if(nextEdgeRelation)//poprzedni mozna zmieniac
            {
                if (chosenVertice.RelationNextVertice.IsEquality())
                {
                    var list = chosenVertice.Polygon.GetVerticesToMove(chosenVertice, Direction.Forward);
                    EqualityRelation equalityRelation = (EqualityRelation)chosenVertice.RelationNextVertice;
                    foreach (Vertice v in list)
                        v.MoveVertice(dx, dy);
                    // List<Vertice> list = new List<Vertice>();
                    // list.Add(chosenVertice);
                    // MoveAroundCircle(nextVertice, equalityRelation.Length, p, list);
                }

            }
            else if(prevEdgeRelation)
            {
                if (chosenVertice.RelationPrevVertice.IsEquality())
                {
                    //EqualityRelation equalityRelation = (EqualityRelation)chosenVertice.RelationPrevVertice;
                    //List<Vertice> list = new List<Vertice>();
                    //list.Add(chosenVertice);
                    //MoveAroundCircle(prevVertice, equalityRelation.Length, p, list);
                    var list = chosenVertice.Polygon.GetVerticesToMove(chosenVertice, Direction.Backward);
                    //EqualityRelation equalityRelation = (EqualityRelation)chosenVertice.RelationNextVertice;
                    foreach (Vertice v in list)
                        v.MoveVertice(dx, dy);
                }
            }
            else
                chosenVertice.Point = p;
            UpdatePictureBox();

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
            int dx = p.X - startingPoint.X;
            int dy = p.Y - startingPoint.Y;
            var list = chosenEdge.Polygon.GetVerticesToMove(chosenEdge.Vertice2, Direction.Forward);
            var list2 = chosenEdge.Polygon.GetVerticesToMove(chosenEdge.Vertice1, Direction.Backward);
            list.AddRange(list2);
            list = list.Distinct().ToList();
            foreach (Vertice v in list)
                v.MoveVertice(dx, dy);
            startingPoint = p;
            UpdatePictureBox();
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
                return;
            }
            else
            {
                chosenVertice.Polygon.DeleteRelation(new Edge(chosenVertice, chosenVertice.GetNextVertice(), chosenVertice.Polygon));
                chosenVertice.Polygon.DeleteRelation(new Edge(chosenVertice.GetPreviousVertice(), chosenVertice, chosenVertice.Polygon));

                chosenVertice.Polygon.DeleteVertice(chosenVertice);
                chosenVertice = null;
            }
            //oldImage = pictureBox.Image;
            //image = new Bitmap(pictureBox.Width, pictureBox.Height);
            //DrawPolygons(image);
            //pictureBox.Image = image;
            //oldImage.Dispose();
            UpdatePictureBox();
            //RefreshPolygons();
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
            oldImage = pictureBox.Image;
            image = new Bitmap(pictureBox.Width, pictureBox.Height);
            Point newPoint = new Point((chosenEdge.Vertice1.Point.X + chosenEdge.Vertice2.Point.X) / 2, (chosenEdge.Vertice1.Point.Y + chosenEdge.Vertice2.Point.Y) / 2);
            chosenEdge.Polygon.AddMidPoint(chosenEdge.Vertice1, newPoint);
            chosenEdge = null;
            midPoint = false;
            midPointButton.BackColor = normalButtonColor;
            DrawPolygons(image);
            pictureBox.Image = image;
            oldImage.Dispose();
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

            //using (Graphics g = pictureBox.CreateGraphics())
            //{
            //    g.FillEllipse(normalBrush, mouseEventArgs.X - radius / 2, mouseEventArgs.Y - radius / 2, radius, radius);
            //}
            if (currentPolygon.VerticeCount < 2)
                return;
            //a = v.GetPreviousVertice();
            //b = v;
            //MyDraw(blackPen, a.Point.X, a.Point.Y, b.Point.X, b.Point.Y);
            UpdatePictureBox();
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
                {
                    E1 = null;
                    E2 = null;
                    addEqualRButton.BackColor = normalButtonColor;
                    equalRelation = false;
                    return;
                }

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

                UpdatePictureBox();
                addEqualRButton.BackColor = normalButtonColor;
                equalRelation = false;
                E1 = null;
                E2 = null;

            }
        }

        private void AddPerpendicularRelation()
        {
            if (chosenEdge == null)
                return;
            if (chosenEdge.Polygon.IsEdgeInRelation(chosenEdge))
            {
                E1 = null;
                E2 = null;
                addPerpendicularRButton.BackColor = normalButtonColor;
                perpenRelation = false;
                return;
            }
            if (E1 == null)
            {
                E1 = chosenEdge;
                return;
            }
            if (E2 == null)
            {
                if (chosenEdge == E1 || E1.Polygon != chosenEdge.Polygon)
                {
                    E1 = null;
                    E2 = null;
                    addPerpendicularRButton.BackColor = normalButtonColor;
                    perpenRelation = false;
                    return;
                }

                E2 = chosenEdge;

                double dx = E1.Vertice2.Point.X - E1.Vertice1.Point.X;
                double dy = E1.Vertice2.Point.Y - E1.Vertice1.Point.Y;

                double a = dy / dx;
                a = -1 / a;
                int x = 100;
                int y = (int)(x * a);
                double vectorLen = Math.Sqrt(x * x + y * y);
                double Len = E2.Length();

                Vertice V1 = E2.Vertice1;
                Vertice V2 = E2.Vertice2;


                double ratio = Len / vectorLen;

                x = (int)(x * ratio);
                y = (int)(y * ratio);

                //V2.Point = new Point(V1.Point.X + x, V1.Point.Y + y);
                var list = V2.Polygon.GetVerticesToMove(V2, Direction.Backward);
                foreach(var v in list)
                    v.MoveVertice(x, y);
                UpdatePictureBox();
                addPerpendicularRButton.BackColor = normalButtonColor;
                perpenRelation = false;
                E1 = null;
                E2 = null;
            }
        }
        
        private void MyDraw(Pen pen, int x1, int y1, int x2, int y2)
        {
            
            if (myDrawFlag)
            {
                //Bitmap image = Bitmap
                //image = new Bitmap(pictureBox.Image);
                int dx = Math.Abs(x2 - x1), sx = x1 < x2 ? 1 : -1;
                int dy = Math.Abs(y2 - y1), sy = y1 < y2 ? 1 : -1;
                int err = (dx > dy ? dx : -dy) / 2, e2;
                for (; ; )
                {
                    if (x1 >= 0 && x1 < pictureBox.Image.Width && y1 >= 0 && y1 < pictureBox.Image.Height)
                        ((Bitmap)pictureBox.Image).SetPixel(x1, y1, pen.Color);
                    if (x1 == x2 && y1 == y2) break;
                    e2 = err;
                    if (e2 > -dx) { err -= dy; x1 += sx; }
                    if (e2 < dy) { err += dx; y1 += sy; }
                }
                pictureBox.Refresh();

                //pictureBox.Image = image;
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
                int dx = Math.Abs(x2 - x1), sx = x1 < x2 ? 1 : -1;
                int dy = Math.Abs(y2 - y1), sy = y1 < y2 ? 1 : -1;
                int err = (dx > dy ? dx : - dy) / 2, e2;
                for (;;)
                {
                    if(x1 >= 0 && x1 < pictureBox.Image.Width && y1 >= 0 && y1 < pictureBox.Image.Height)
                        image.SetPixel(x1, y1, pen.Color);
                    if (x1 == x2 && y1 == y2) break;
                    e2 = err;
                    if (e2 > -dx) { err -= dy; x1 += sx; }
                    if (e2 < dy) { err += dx; y1 += sy; }
                }
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



        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (drawingPolygon)
                return;
            Point mousePoint = new Point(e.X, e.Y);
            startingPoint = mousePoint;
            if (controlKeyDown && (chosenVertice != null || chosenEdge != null))
            {
                movingPolygon = true;
                return;
            }
            if(chosenEdge != null && controlKeyDown == false)
            {
                movingEdge = true;
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
                    chosenVertice = null;
                    Cursor = Cursors.Arrow;
                }
            }
            if (chosenEdge != null)
            {
                if (distance2Edge > minDistanceEdge)
                {
                    chosenEdge = null;
                    Cursor = Cursors.Arrow;
                }
            }

            if (vertice != null) //
            {
                if (distance2Vertice <= minDistanceVertice)// && distance2Vertice <= distance2Edge) //jest jakis vertice w zasiegu
                {
                    chosenVertice = vertice;
                    chosenEdge = null;
                    Cursor = Cursors.Hand;
                    UpdatePictureBox();
                    return;
                }
            }
            if (edge != null)
            {
                if (distance2Edge <= minDistanceEdge && distance2Edge < distance2Vertice)
                {
                    chosenVertice = null;
                    chosenEdge = edge;
                    Cursor = Cursors.Hand;
                }
            }
            UpdatePictureBox();

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
                return;
            }

            if(perpenRelation)
            {
                AddPerpendicularRelation();
                return;
            }

            if(deletingRelation)
            {
                deleteRelation();
            }

        }

        private void deleteRelation()
        {
            if (chosenEdge == null)
                return;
            if(chosenEdge.Polygon.IsEdgeInRelation(chosenEdge))
            {
                chosenEdge.Polygon.DeleteRelation(chosenEdge);
                deletingRelation = false;
                deleteRelationButton.BackColor = normalButtonColor;
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
            //UpdatePictureBox();
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

        private void addPerpendicularRButton_Click(object sender, EventArgs e)
        {
            perpenRelation = true;
            addPerpendicularRButton.BackColor = activeButtonColor;
        }



        private void checkButtons()
        {

        }

        private void pictureBox_Layout(object sender, EventArgs e)
        {
            RefreshPolygons();
            //UpdatePictureBox();
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
                        if(polygons[i].FullPolygon || nextVertice.Index != 0)
                            MyDrawImage(blackPen, curVertice.Point.X, curVertice.Point.Y, nextVertice.Point.X, nextVertice.Point.Y, image);
                        if(e.Polygon.IsEdgeInRelation(e))
                        {
                            g.DrawString($"={e.Vertice1.RelationNextVertice.Index.ToString()}", font, normalBrush, (curVertice.Point.X + nextVertice.Point.X) / 2, (curVertice.Point.Y + nextVertice.Point.Y) / 2);
                            //g.DrawImage(EqualIcon, (curVertice.Point.X + nextVertice.Point.X) / 2, (curVertice.Point.Y + nextVertice.Point.Y) / 2, 30, 20);
                        }

                    }
                }
                if(chosenEdge != null)
                {
                    MyDrawImage(chosenPen, chosenEdge.Vertice1.Point.X, chosenEdge.Vertice1.Point.Y, chosenEdge.Vertice2.Point.X, chosenEdge.Vertice2.Point.Y, image);
                }
                else if (chosenVertice != null)
                {
                    g.FillEllipse(chosenBrush, chosenVertice.Point.X - radius / 2, chosenVertice.Point.Y - radius / 2, radius, radius);
                }
                if(E1 != null)
                {
                    MyDrawImage(relationPen, E1.Vertice1.Point.X, E1.Vertice1.Point.Y, E1.Vertice2.Point.X, E1.Vertice2.Point.Y, image);
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
            //Bitmap bitmap = (Bitmap)pictureBox.Image;
            //using (Graphics g = Graphics.FromImage(bitmap))
            //    g.Clear(pictureBox.BackColor);
            //DrawPolygons(bitmap);
            //pictureBox.Image = bitmap;
        }

        private void deleteRelationButton_Click(object sender, EventArgs e)
        {
            deletingRelation = true;
            deleteRelationButton.BackColor = activeButtonColor;
        }
    }
    public enum Direction { Forward, Backward };
}
