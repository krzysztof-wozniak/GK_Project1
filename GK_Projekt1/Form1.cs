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
        private bool midPoint = false;
        private bool equalRelation = false;
        private bool perpenRelation = false;
        private bool deletingRelation = false;
        private bool antialias = true;


        private bool myDrawFlag = true;



        private List<Polygon> polygons = new List<Polygon>();
        private Polygon currentPolygon;
        private Vertice chosenVertice = null;
        private Edge chosenEdge = null;
        private Edge E1 = null;
        private Edge E2 = null;
        private Point startingPoint;


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
        private Font font = new Font("Calibri", 15);
        private Brush fontBrush = new SolidBrush(Color.Green);



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
            Polygon pol = new Polygon(0);
            polygons.Add(pol);
            Point[] points = new Point[]
            {
                new Point(100, 100),
                new Point(200, 200),
                new Point(200, 300),
                new Point(250, 320),
                new Point(370, 370),
                new Point(420, 380),
                new Point(503, 390),
                new Point(600, 300),
                new Point(550, 250),
                new Point(300, 200),
                new Point(200, 150),
            };
            List<Vertice> vertices = new List<Vertice>();
            for(int i = 0; i < points.Count(); i++)
            {
                vertices.Add(new Vertice(points[i], pol, i));
            }
            for(int i = 0; i < vertices.Count; i++)
            {
                pol.AddVertice(vertices[i]);
            }
            pol.FinishDrawing();
            E1 = new Edge(vertices[2], vertices[3], pol);
            chosenEdge = new Edge(vertices[3], vertices[4], pol);
            AddEqualityRelation();
            E1 = new Edge(vertices[4], vertices[5], pol);
            chosenEdge = new Edge(vertices[5], vertices[6], pol);
            AddPerpendicularRelation();
            E1 = new Edge(vertices[1], vertices[2], pol);
            chosenEdge = new Edge(vertices[7], vertices[8], pol);
            AddEqualityRelation();
            E1 = new Edge(vertices[10], vertices[0], pol);
            chosenEdge = new Edge(vertices[8], vertices[9], pol);
            AddPerpendicularRelation();
            UpdatePictureBox();
            
        }

        
        
        private void MoveVertice(Point p)
        {
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
                else if(chosenVertice.RelationNextVertice == chosenVertice.RelationPrevVertice && !chosenVertice.RelationNextVertice.IsEquality())//ta sama prostopadlosc
                {
                    var list = nextVertice.Polygon.GetVerticesToMove(chosenVertice, Direction.Forward);//jesli forward blizej to ruszamy next
                    var list2 = prevVertice.Polygon.GetVerticesToMove(chosenVertice, Direction.Backward);
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
                    }// prev stały

                    double deltaX = chosenVertice.Point.X - prevVertice.Point.X;
                    double deltaY = chosenVertice.Point.Y - prevVertice.Point.Y;

                    if (deltaX < 2 && deltaX > -2)
                        deltaX = 1;
                    if (deltaY < 2 && deltaY > -2)
                        deltaY = 1;
                    double a = deltaY / deltaX;
                    a = -1 / a; //prostopadle
                    int x = 100;
                    int y = (int)Math.Round(x * a);
                    double vectorLen = Math.Sqrt(x * x + y * y);
                    double Len = chosenVertice.Point.DistanceToPoint(nextVertice.Point);

                   


                    double ratio = Len / vectorLen;

                    x = (int)Math.Round(x * ratio);
                    y = (int)Math.Round(y * ratio);

                    x = p.X + x - nextVertice.Point.X;
                    y = p.Y + y - nextVertice.Point.Y;


                    //V2.Point = new Point(V1.Point.X + x, V1.Point.Y + y);
                    //var list = V2.Polygon.GetVerticesToMove(V2, Direction.Forward);
                    foreach (var v in list)
                        v.MoveVertice(x, y);


                    chosenVertice.Point = p;



                }
                else
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
                var list = chosenVertice.Polygon.GetVerticesToMove(chosenVertice, Direction.Forward);
                foreach (Vertice v in list)
                    v.MoveVertice(dx, dy);
            }
            else if(prevEdgeRelation)
            {   
                var list = chosenVertice.Polygon.GetVerticesToMove(chosenVertice, Direction.Backward);
                foreach (Vertice v in list)
                    v.MoveVertice(dx, dy);
            }
            else
                chosenVertice.Point = p;
            UpdatePictureBox();

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
            UpdatePictureBox();
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

        }

        private void DeletePolygon(Polygon polygon)
        {
            DeletePolygon(polygon.Index);
        }

        private void AddMidPoint()
        {
            if (chosenEdge == null)
                return;
            chosenEdge.Polygon.DeleteRelation(chosenEdge);
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
                //nPolygonsLabel.Text = "Number of polygons: " + polygons.Count.ToString();
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
            
            if (currentPolygon.VerticeCount < 2)
                return;
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
                E2.Polygon.AddPerpendicularRelation(E1, E2);

                double dx = E1.Vertice2.Point.X - E1.Vertice1.Point.X;
                double dy = E1.Vertice2.Point.Y - E1.Vertice1.Point.Y;

                double a = dy / dx;
                a = -1 / a; //prostopadle
                int x = 20;
                int y = (int)Math.Round(x * a);
                double vectorLen = Math.Sqrt(x * x + y * y);
                double Len = E2.Length();

                Vertice V1 = E2.Vertice1;
                Vertice V2 = E2.Vertice2;


                double ratio = Len / vectorLen;

                x = (int)Math.Round(x * ratio);
                y = (int)Math.Round(y * ratio);

                x = V1.Point.X + x - V2.Point.X;
                y = V1.Point.Y + y - V2.Point.Y;


                //V2.Point = new Point(V1.Point.X + x, V1.Point.Y + y);
                var list = V2.Polygon.GetVerticesToMove(V2, Direction.Forward);
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
                bool steep = Math.Abs(y2 - y1) > Math.Abs(x2 - x1);
                if (steep)
                {
                    Swap(ref x1, ref y1);
                    Swap(ref x2, ref y2);
                }

                if (x1 > x2)
                {
                    Swap(ref x1, ref x2);
                    Swap(ref y1, ref y2);
                }

                int dx = x2 - x1;
                int dy = Math.Abs(y2 - y1);
                int d = 2 * dy - dx;
                int ystep = (y1 < y2) ? 1 : -1;
                int y = y1;
                int incdy = 2 * dy;
                int incdx = 2 * dx;

                for (int x = x1; x < x2; x++)
                {
                    if (steep)
                    {
                        if (y >= 0 && y < pictureBox.Image.Width && x >= 0 && x < pictureBox.Image.Height)
                            ((Bitmap)pictureBox.Image).SetPixel(y, x, pen.Color);
                    }
                    else
                    {
                        if (x >= 0 && x < pictureBox.Image.Width && y >= 0 && y < pictureBox.Image.Height)
                            ((Bitmap)pictureBox.Image).SetPixel(x, y, pen.Color);
                    }

                    d -= incdy;
                    if (d < 0)
                    {
                        y += ystep;
                        d += incdx;
                    }
                }
                pictureBox.Refresh();
            }
            else
            {
                using (Graphics g = pictureBox.CreateGraphics())
                {
                    if (antialias)
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.DrawLine(pen, x1, y1, x2, y2);
                }
            }
        }

        private void MyDrawImage(Pen pen, int x1, int y1, int x2, int y2, Bitmap image)
        {
            if (myDrawFlag)
            {
                bool steep = Math.Abs(y2 - y1) > Math.Abs(x2 - x1);
                if (steep)
                {
                    Swap(ref x1, ref y1);
                    Swap(ref x2, ref y2);
                }

                if (x1 > x2)
                {
                    Swap(ref x1, ref x2);
                    Swap(ref y1, ref y2);
                }

                int dx = x2 - x1;
                int dy = Math.Abs(y2 - y1);

                int d = 2 * dy - dx;
                int incdy = 2 * dy;
                int incdx = 2 * dx;
                int ystep = (y1 < y2) ? 1 : -1;
                int y = y1;
                for (int x = x1; x < x2; x++)
                {
                    if (steep)
                    {
                        if (y >= 0 && y < pictureBox.Image.Width && x >= 0 && x < pictureBox.Image.Height)
                            image.SetPixel(y, x, pen.Color);
                    }
                    else
                    {
                        if (x >= 0 && x < pictureBox.Image.Width && y >= 0 && y < pictureBox.Image.Height)
                            image.SetPixel(x, y, pen.Color);
                    }

                    d -= incdy;
                    if (d < 0)
                    {
                        y += ystep;
                        d += incdx;
                    }
                }
            }
            else
            {
                using (Graphics g = Graphics.FromImage(image))
                {
                    if (antialias)
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
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





        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (drawingPolygon && currentPolygon.VerticeCount == 0)
            {
                drawingPolygon = false;
                addPolygonButton.BackColor = normalButtonColor;
                polygons.RemoveAt(currentPolygon.Index);
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
            else if (midPoint)
            {
                midPointButton.BackColor = normalButtonColor;
                midPoint = false;
            }
            else if (equalRelation)
            {
                equalRelation = false;
                addEqualRButton.BackColor = normalButtonColor;
            }
            else if (perpenRelation)
            {
                perpenRelation = false;
                addPerpendicularRButton.BackColor = normalButtonColor;
            }
            else if (deletingRelation)
            {
                deletingRelation = false;
                deleteRelationButton.BackColor = normalButtonColor;
            }
            else if (drawingPolygon)
                return;
            drawingPolygon = true;
            addPolygonButton.BackColor = activeButtonColor;
            polygons.Add(new Polygon(polygons.Count));
            currentPolygon = polygons[polygons.Count - 1];
            return;
        }

        private void midPointButton_Click(object sender, EventArgs e)
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
                return;
            }
            else if (deletingPolygon)
            {
                deletingPolygon = false;
                deletePolygonButton.BackColor = normalButtonColor;
            }
            else if (equalRelation)
            {
                equalRelation = false;
                addEqualRButton.BackColor = normalButtonColor;
            }
            else if (perpenRelation)
            {
                perpenRelation = false;
                addPerpendicularRButton.BackColor = normalButtonColor;
            }
            else if (deletingRelation)
            {
                deletingRelation = false;
                deleteRelationButton.BackColor = normalButtonColor;
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
            else if (midPoint)
            {
                midPoint = false;
                midPointButton.BackColor = normalButtonColor;
            }
            else if (deletingPolygon)
            {
                deletingPolygon = false;
                deletePolygonButton.BackColor = normalButtonColor;
            }
            else if (equalRelation)
            {
                equalRelation = false;
                addEqualRButton.BackColor = normalButtonColor;
            }
            else if (perpenRelation)
            {
                perpenRelation = false;
                addPerpendicularRButton.BackColor = normalButtonColor;
            }
            else if (deletingRelation)
            {
                deletingRelation = false;
                deleteRelationButton.BackColor = normalButtonColor;
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
            else if (equalRelation)
            {
                equalRelation = false;
                addEqualRButton.BackColor = normalButtonColor;
            }
            else if (perpenRelation)
            {
                perpenRelation = false;
                addPerpendicularRButton.BackColor = normalButtonColor;
            }
            else if (deletingRelation)
            {
                deletingRelation = false;
                deleteRelationButton.BackColor = normalButtonColor;
            }
            else if (drawingPolygon)
                return;
            deletePolygonButton.BackColor = activeButtonColor;
            deletingPolygon = true;
        }

        private void addEqualRButton_Click(object sender, EventArgs e)
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
            }
            else if(equalRelation)
            {
                equalRelation = false;
                addEqualRButton.BackColor = normalButtonColor;
                return;
            }
            else if(perpenRelation)
            {
                perpenRelation = false;
                addPerpendicularRButton.BackColor = normalButtonColor;
            }
            else if(deletingRelation)
            {
                deletingRelation = false;
                deleteRelationButton.BackColor = normalButtonColor;
            }
            else if (drawingPolygon)
                return;
            addEqualRButton.BackColor = activeButtonColor;
            equalRelation = true;
        }

        private void addPerpendicularRButton_Click(object sender, EventArgs e)
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
            }
            else if (equalRelation)
            {
                equalRelation = false;
                addEqualRButton.BackColor = normalButtonColor;
            }
            else if (perpenRelation)
            {
                perpenRelation = false;
                addPerpendicularRButton.BackColor = normalButtonColor;
                return;
            }
            else if (deletingRelation)
            {
                deletingRelation = false;
                deleteRelationButton.BackColor = normalButtonColor;
            }
            else if (drawingPolygon)
                return;
            perpenRelation = true;
            addPerpendicularRButton.BackColor = activeButtonColor;
        }

        private void deleteRelationButton_Click(object sender, EventArgs e)
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
            }
            else if (equalRelation)
            {
                equalRelation = false;
                addEqualRButton.BackColor = normalButtonColor;
            }
            else if (perpenRelation)
            {
                perpenRelation = false;
                addPerpendicularRButton.BackColor = normalButtonColor;
            }
            else if (deletingRelation)
            {
                deletingRelation = false;
                deleteRelationButton.BackColor = normalButtonColor;
                return;
            }
            else if (drawingPolygon)
                return;
            deletingRelation = true;
            deleteRelationButton.BackColor = activeButtonColor;
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
                            if (e.Vertice1.RelationNextVertice.IsEquality())
                                g.DrawString($"={e.Vertice1.RelationNextVertice.Index.ToString()}", font, normalBrush,
                                    (curVertice.Point.X + nextVertice.Point.X) / 2, (curVertice.Point.Y + nextVertice.Point.Y) / 2);
                            else
                                g.DrawString($"⊥{e.Vertice1.RelationNextVertice.Index.ToString()}", font, normalBrush,
                                    (curVertice.Point.X + nextVertice.Point.X) / 2, (curVertice.Point.Y + nextVertice.Point.Y) / 2);
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

        private void Swap(ref int a, ref int b)
        {
            int temp = a;
            a = b;
            b = temp;
        }
    }
    public enum Direction { Forward, Backward };
}
