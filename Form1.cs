using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShapeGeneratorLibrary;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ShapeGenerator
{
    public partial class Form1 : Form
    {
        public ShapeGeneratorLibrary.IShape shape;
        ShapeGeneratorLibrary.ShapeGeneratorLibrary lib;

        public Form1()
        {
            InitializeComponent();
            lib = new ShapeGeneratorLibrary.ShapeGeneratorLibrary();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = false;
            shape = null;
            shape = lib.DetermineShape(textBox1.Text);
            if (shape != null)
            {
                textBox2.Text = shape.description;
            }

            //fire picturebox_Paint event
            pictureBox1.Visible = (shape != null);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (shape != null)
            {
                if (e.ClipRectangle.Width > 0 && e.ClipRectangle.Height > 0)
                {
                    try
                    {

                        e.Graphics.Clear(Color.White);
                        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                        using (Pen pn = new Pen(Color.Black, 5))
                        {
                            switch (shape.shapeType)
                            {
                                case "Rectangle":
                                    e.Graphics.DrawRectangle(pn, 0, 0, shape.measurements[0], shape.measurements[1]);
                                    break;
                                case "Square":
                                    //have 1 side length only therefore use same measurement.
                                    e.Graphics.DrawRectangle(pn, 0, 0, shape.measurements[0], shape.measurements[0]);
                                    break;
                                case "Oval":
                                    //assume has width and height
                                    e.Graphics.DrawEllipse(pn, 0, 0, shape.measurements[0], shape.measurements[1]);
                                    break;
                                case "Circle":
                                    //have radius so double measurement for size.
                                    e.Graphics.DrawEllipse(pn, 0, 0, shape.measurements[0] * 2, shape.measurements[0] * 2);
                                    break;

                                case "Isosceles Triangle":
                                case "Equilateral Triangle":
                                case "Scalene Triangle":
                                    DrawTriangles(shape, e, pn);
                                    break;

                                case "Parallelogram":
                                    DrawParallelogram(shape, e, pn);
                                    break;

                                case "Pentagon":
                                    DrawRegularPolygon(shape, e, pn, 5);
                                    break;
                                case "Hexagon":
                                    DrawRegularPolygon(shape, e, pn, 6);
                                    break;
                                case "Heptagon":
                                    DrawRegularPolygon(shape, e, pn, 7);
                                    break;
                                case "Octagon":
                                    DrawRegularPolygon(shape, e, pn, 8);
                                    break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!EventLog.SourceExists("Application"))
                            EventLog.CreateEventSource("Application", "ShapeGenerator");
                        EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error, 69);

                    }
                }
            }
        }

        void DrawTriangles(IShape shape, PaintEventArgs e, Pen pn)
        {
            try
            {
                switch (shape.shapeType)
                {
                    case "Isosceles Triangle":
                        e.Graphics.DrawLine(pn, 0, 0, shape.measurements[0], 0);    //horizontal across top
                        e.Graphics.DrawLine(pn, 0, 0, shape.measurements[0] / 2, shape.measurements[1]);    //diagonal right
                        e.Graphics.DrawLine(pn, shape.measurements[0] / 2, shape.measurements[1], shape.measurements[0], 0);    //diagonal left
                        break;

                    case "Equilateral Triangle":
                        float height = (float)(shape.measurements[0] * Math.Sqrt(3) / 2);
                        e.Graphics.DrawLine(pn, 0, 0, shape.measurements[0], 0);    //horizontal across top
                        e.Graphics.DrawLine(pn, 0, 0, shape.measurements[0] / 2, height);    //diagonal right
                        e.Graphics.DrawLine(pn, shape.measurements[0], 0, shape.measurements[0] / 2, height);    //diagonal left
                        break;

                    case "Scalene Triangle":
                        e.Graphics.DrawLine(pn, 0, 0, shape.measurements[0], 0);    //horizontal across top
                        e.Graphics.DrawLine(pn, 0, 0, shape.measurements[0] / 3, shape.measurements[0] / 4);    //diagonal right
                        e.Graphics.DrawLine(pn, shape.measurements[0] / 3, shape.measurements[0] / 4, shape.measurements[0], 0);    //diagonal left
                        break;
                }
            }
            catch (Exception ex)
            {
                if (!EventLog.SourceExists("Application"))
                    EventLog.CreateEventSource("Application", "ShapeGenerator");
                EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error, 69);

            }
        }

        void DrawParallelogram(IShape shape, PaintEventArgs e, Pen pn)
        {
            try
            {
                float pWidth = (float)(shape.measurements[0] * 0.75);
                float pHeight = (float)(shape.measurements[1] * 0.75);
                e.Graphics.DrawLine(pn, 0, 0, pWidth, 0);    //horizontal across top
                e.Graphics.DrawLine(pn, pWidth, 0, shape.measurements[0], pHeight);    //diagonal right
                e.Graphics.DrawLine(pn, shape.measurements[0], pHeight, shape.measurements[0] - pWidth, pHeight);    //horizontal across bottom
                e.Graphics.DrawLine(pn, shape.measurements[0] - pWidth, pHeight, 0, 0);    //diagonal left

            }
            catch (Exception ex)
            {
                if (!EventLog.SourceExists("Application"))
                    EventLog.CreateEventSource("Application", "ShapeGenerator");
                EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error, 69);

            }
        }

        void DrawRegularPolygon(IShape shape, PaintEventArgs e, Pen pn, int count)
        {
            try
            {
                float start1 = shape.measurements[0];
                float finish1 = 0;
                float start2 = start1 + shape.measurements[0];
                float finish2 = 0;
                for (int i = 1; i < count + 1; i++)
                {
                    e.Graphics.DrawLine(pn, start1, finish1, start2, finish2);    //horizontal across top

                    switch (count)
                    {
                        case 5:
                            switch (i)
                            {
                                //right diag down
                                case 1:
                                    start1 = start2;
                                    finish1 = finish2;
                                    start2 += (float)(shape.measurements[0] / 3);
                                    finish2 += (float)(shape.measurements[0] * .75);
                                    break;
                                //left diag down
                                case 2:
                                    start1 = start2;
                                    finish1 = finish2;
                                    start2 = start1 - (float)(shape.measurements[0] * .75);
                                    finish2 += (float)(shape.measurements[0] / 2);
                                    break;
                                //left diag up
                                case 3:
                                    start1 = start2;
                                    finish1 = finish2;
                                    start2 = start1 - (float)(shape.measurements[0] * .75);
                                    finish2 = finish1 - (float)(shape.measurements[0] / 2);
                                    break;
                                //right diag up
                                case 4:
                                    start1 = start2;
                                    finish1 = finish2;
                                    start2 = shape.measurements[0];
                                    finish2 = 0;
                                    break;
                            }
                            break;
                        case 6:
                            switch (i)
                            {
                                //right diag down
                                case 1:
                                    start1 = start2;
                                    finish1 = finish2;
                                    start2 += (float)(shape.measurements[0] / 2);
                                    finish2 += (float)(shape.measurements[0] * .75);
                                    break;
                                //left diag down
                                case 2:
                                    start1 = start2;
                                    finish1 = finish2;
                                    start2 = start1 - (float)(shape.measurements[0] / 2);
                                    finish2 += (float)(shape.measurements[0] * .75);
                                    break;
                                //bottom horizontal
                                case 3:
                                    start1 = start2;
                                    finish1 = finish2;
                                    start2 = start1 - shape.measurements[0];
                                    finish2 = finish1;
                                    break;
                                //left diag up
                                case 4:
                                    start1 = start2;
                                    finish1 = finish2;
                                    start2 = start1 - (float)(shape.measurements[0] / 2);
                                    finish2 -= (float)(shape.measurements[0] * .75);
                                    break;
                                //right diag up
                                case 5:
                                    start1 = start2;
                                    finish1 = finish2;
                                    start2 = shape.measurements[0];
                                    finish2 = 0;
                                    break;
                            }
                            break;
                        case 7:
                            switch (i)
                            {
                                //right diag down
                                case 1:
                                    start1 = start2;
                                    finish1 = finish2;
                                    start2 += (float)(shape.measurements[0] / 2);
                                    finish2 += (float)(shape.measurements[0] * .75);
                                    break;
                                //right diag down in
                                case 2:
                                    start1 = start2;
                                    finish1 = finish2;
                                    start2 -= (float)(shape.measurements[0] / 3);
                                    finish2 += (float)(shape.measurements[0] * .75);
                                    break;
                                //left diag down
                                case 3:
                                    start1 = start2;
                                    finish1 = finish2;
                                    start2 = start1 - (float)(shape.measurements[0] * .66);
                                    finish2 += (float)(shape.measurements[0] / 3);
                                    break;
                                //left diag up
                                case 4:
                                    start1 = start2;
                                    finish1 = finish2;
                                    start2 = start2 - (float)(shape.measurements[0] * .66);
                                    finish2 -= (float)(shape.measurements[0] / 3);
                                    break;
                                //left diag up in
                                case 5:
                                    start1 = start2;
                                    finish1 = finish2;
                                    start2 = start1 - (float)(shape.measurements[0] / 3);
                                    finish2 = finish2 - (float)(shape.measurements[0] * .75);
                                    break;
                                //right diag up
                                case 6:
                                    start1 = start2;
                                    finish1 = finish2;
                                    start2 = shape.measurements[0];
                                    finish2 = 0;
                                    break;
                            }
                            break;
                        case 8:
                            switch (i)
                            {
                                //right diag down
                                case 1:
                                    start1 = start2;
                                    finish1 = finish2;
                                    start2 += (float)(shape.measurements[0] * .75);
                                    finish2 += (float)(shape.measurements[0] * .75);
                                    break;
                                //right vertical
                                case 2:
                                    start1 = start2;
                                    finish1 = finish2;
                                    start2 = start1;
                                    finish2 += shape.measurements[0];
                                    break;
                                //left diag down
                                case 3:
                                    start1 = start2;
                                    finish1 = finish2;
                                    start2 = start1 - (float)(shape.measurements[0] * .75);
                                    finish2 += (float)(shape.measurements[0] * .75);
                                    break;
                                //bottom horizontal
                                case 4:
                                    start1 = start2;
                                    finish1 = finish2;
                                    start2 = start1 - shape.measurements[0];
                                    finish2 = finish1;
                                    break;
                                //left diag up
                                case 5:
                                    start1 = start2;
                                    finish1 = finish2;
                                    start2 = start1 - (float)(shape.measurements[0] * .75);
                                    finish2 -= (float)(shape.measurements[0] * .75);
                                    break;
                                //left vertical
                                case 6:
                                    start1 = start2;
                                    finish1 = finish2;
                                    start2 = start1;
                                    finish2 = finish2 - shape.measurements[0];
                                    break;
                                //right diag up
                                case 7:
                                    start1 = start2;
                                    finish1 = finish2;
                                    start2 = shape.measurements[0];
                                    finish2 = 0;
                                    break;
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                if (!EventLog.SourceExists("Application"))
                    EventLog.CreateEventSource("Application", "ShapeGenerator");
                EventLog.WriteEntry("Application", ex.Message, EventLogEntryType.Error, 69);

            }
        }
    }

    //TESTING...

    [TestClass]
    public class UnitTestShapeGeneratorLibrary
    {

        [TestMethod]
        public void TestMethod_DetermineShape_is_Rectangle()
        {
            // TODO : Write test code 
            IShape shape;
            ShapeGeneratorLibrary.ShapeGeneratorLibrary lib;
            lib = new ShapeGeneratorLibrary.ShapeGeneratorLibrary();
            shape = lib.DetermineShape("rectangle width of 100 and height of 200");

            Assert.AreEqual(shape.shapeType, "Rectangle");
            Assert.AreEqual(shape.measurements[0], 100);
            Assert.AreEqual(shape.measurements[1], 200);
            Assert.AreEqual(shape.description, "rectangle width of 100 and height of 200");
        }

        [TestMethod]
        public void TestMethod_DetermineShape_is_Square()
        {
            // TODO : Write test code 
            IShape shape;
            ShapeGeneratorLibrary.ShapeGeneratorLibrary lib;
            lib = new ShapeGeneratorLibrary.ShapeGeneratorLibrary();
            shape = lib.DetermineShape("square side length of 100");

            Assert.AreEqual(shape.shapeType, "Square");
            Assert.AreEqual(shape.measurements[0], 100);
            Assert.AreEqual(shape.description, "square side length of 100");
        }

        [TestMethod]
        public void TestMethod_DetermineShape_is_Circle()
        {
            // TODO : Write test code 
            IShape shape;
            ShapeGeneratorLibrary.ShapeGeneratorLibrary lib;
            lib = new ShapeGeneratorLibrary.ShapeGeneratorLibrary();
            shape = lib.DetermineShape("circle radius of 100");

            Assert.AreEqual(shape.shapeType, "Circle");
            Assert.AreEqual(shape.measurements[0], 100);
            Assert.AreEqual(shape.description, "circle radius of 100");
        }

        [TestMethod]
        public void TestMethod_DetermineShape_is_Oval()
        {
            // TODO : Write test code 
            IShape shape;
            ShapeGeneratorLibrary.ShapeGeneratorLibrary lib;
            lib = new ShapeGeneratorLibrary.ShapeGeneratorLibrary();
            shape = lib.DetermineShape("oval height of 200 and width of 100");

            Assert.AreEqual(shape.shapeType, "Oval");
            Assert.AreEqual(shape.measurements[0], 100);
            Assert.AreEqual(shape.measurements[1], 200);
            Assert.AreEqual(shape.description, "oval height of 200 and width of 100");
        }

        [TestMethod]
        public void TestMethod_DetermineShape_is_IsoscelesTriangle()
        {
            // TODO : Write test code 
            IShape shape;
            ShapeGeneratorLibrary.ShapeGeneratorLibrary lib;
            lib = new ShapeGeneratorLibrary.ShapeGeneratorLibrary();
            shape = lib.DetermineShape("isosceles triangle width of 200 and height of 400");

            Assert.AreEqual(shape.shapeType, "Isosceles Triangle");
            Assert.AreEqual(shape.measurements[0], 200);
            Assert.AreEqual(shape.measurements[1], 400);
            Assert.AreEqual(shape.description, "isosceles triangle width of 200 and height of 400");
        }

        [TestMethod]
        public void TestMethod_DetermineShape_is_EquilateralTriangle()
        {
            // TODO : Write test code 
            IShape shape;
            ShapeGeneratorLibrary.ShapeGeneratorLibrary lib;
            lib = new ShapeGeneratorLibrary.ShapeGeneratorLibrary();
            shape = lib.DetermineShape("equilateral triangle side length of 200");

            Assert.AreEqual(shape.shapeType, "Equilateral Triangle");
            Assert.AreEqual(shape.measurements[0], 200);
            Assert.AreEqual(shape.description, "equilateral triangle side length of 200");
        }

        [TestMethod]
        public void TestMethod_DetermineShape_is_ScaleneTriangle()
        {
            // TODO : Write test code 
            IShape shape;
            ShapeGeneratorLibrary.ShapeGeneratorLibrary lib;
            lib = new ShapeGeneratorLibrary.ShapeGeneratorLibrary();
            shape = lib.DetermineShape("scalene triangle height of 500 and width of 400");

            Assert.AreEqual(shape.shapeType, "Scalene Triangle");
            Assert.AreEqual(shape.measurements[0], 400);
            Assert.AreEqual(shape.measurements[1], 500);
            Assert.AreEqual(shape.description, "scalene triangle height of 500 and width of 400");
        }

        [TestMethod]
        public void TestMethod_DetermineShape_is_Parallelogram()
        {
            // TODO : Write test code 
            IShape shape;
            ShapeGeneratorLibrary.ShapeGeneratorLibrary lib;
            lib = new ShapeGeneratorLibrary.ShapeGeneratorLibrary();
            shape = lib.DetermineShape("parallelogram width of 400 and height of 500");

            Assert.AreEqual(shape.shapeType, "Parallelogram");
            Assert.AreEqual(shape.measurements[0], 400);
            Assert.AreEqual(shape.measurements[1], 500);
            Assert.AreEqual(shape.description, "parallelogram width of 400 and height of 500");
        }

        [TestMethod]
        public void TestMethod_DetermineShape_is_Pentagon()
        {
            // TODO : Write test code 
            IShape shape;
            ShapeGeneratorLibrary.ShapeGeneratorLibrary lib;
            lib = new ShapeGeneratorLibrary.ShapeGeneratorLibrary();
            shape = lib.DetermineShape("pentagon side length of 100");

            Assert.AreEqual(shape.shapeType, "Pentagon");
            Assert.AreEqual(shape.measurements[0], 100);
            Assert.AreEqual(shape.description, "pentagon side length of 100");
        }

        [TestMethod]
        public void TestMethod_DetermineShape_is_Hexagon()
        {
            // TODO : Write test code 
            IShape shape;
            ShapeGeneratorLibrary.ShapeGeneratorLibrary lib;
            lib = new ShapeGeneratorLibrary.ShapeGeneratorLibrary();
            shape = lib.DetermineShape("hexagon side length of 100");

            Assert.AreEqual(shape.shapeType, "Hexagon");
            Assert.AreEqual(shape.measurements[0], 100);
            Assert.AreEqual(shape.description, "hexagon side length of 100");
        }

        [TestMethod]
        public void TestMethod_DetermineShape_is_Heptagon()
        {
            // TODO : Write test code 
            IShape shape;
            ShapeGeneratorLibrary.ShapeGeneratorLibrary lib;
            lib = new ShapeGeneratorLibrary.ShapeGeneratorLibrary();
            shape = lib.DetermineShape("heptagon side length of 100");

            Assert.AreEqual(shape.shapeType, "Heptagon");
            Assert.AreEqual(shape.measurements[0], 100);
            Assert.AreEqual(shape.description, "heptagon side length of 100");
        }

        [TestMethod]
        public void TestMethod_DetermineShape_is_Octagon()
        {
            // TODO : Write test code 
            IShape shape;
            ShapeGeneratorLibrary.ShapeGeneratorLibrary lib;
            lib = new ShapeGeneratorLibrary.ShapeGeneratorLibrary();
            shape = lib.DetermineShape("octagon side length of 100");

            Assert.AreEqual(shape.shapeType, "Octagon");
            Assert.AreEqual(shape.measurements[0], 100);
            Assert.AreEqual(shape.description, "octagon side length of 100");
        }

        [TestMethod]
        public void TestMethod_DetermineShape_is_Unknown()
        {
            // TODO : Write test code 
            IShape shape;
            ShapeGeneratorLibrary.ShapeGeneratorLibrary lib;
            lib = new ShapeGeneratorLibrary.ShapeGeneratorLibrary();
            shape = lib.DetermineShape("dot is 200");

            Assert.IsNotNull(shape);
            Assert.IsNull(shape.shapeType);
            Assert.IsNotNull(shape.description);
        }

    }
}
