using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ForestCitizens
{
    class FormForestVisualizer : Form, IForestVisualizer
    {
        private Dictionary<string, Image> images;
        private IForest forest;
        private float scaleW;
        private float scaleH;
        private StringFormat format;
        private Action<Graphics> drawingAction;

        public FormForestVisualizer(IForest forest)
        {
            this.forest = forest;
            drawingAction = DrawMap;
            DoubleBuffered = true;
            InitImages();
            InitStringFormat();
            CalculateScale();
        }

        private void InitImages()
        {
            images = Directory.EnumerateFiles("Images\\", "*png").ToDictionary(Path.GetFileNameWithoutExtension, Image.FromFile);
        }

        private void InitStringFormat()
        {
            format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
        }

        private void CalculateScale()
        {
            var rowsCount = forest.Map[0].Length + 1;
            var columnsCount = forest.Map.Length + 1;
            scaleW = (float) Width / rowsCount;
            scaleH = (float) Height / columnsCount;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            drawingAction(e.Graphics);
        }

        protected override void OnResize(EventArgs e)
        {
            CalculateScale();
            Invalidate();
        }

        private void DrawMap(Graphics graphics)
        {
            for (int x = 0; x < forest.Map[0].Length; x++)
                for (int y = 0; y < forest.Map.Length; y++)
                    graphics.DrawImage(images[forest.Map[y][x].GetImageString()],
                        x * scaleW, y * scaleH, 
                        scaleW, scaleH);
            foreach (var citizen in forest.Citizens)
            {
                graphics.DrawImage(images["Citizen"], 
                    citizen.Location.Y * scaleW, citizen.Location.X * scaleH,
                    scaleW, scaleH);
                graphics.DrawString(citizen.Name[0].ToString(),
                    new Font("Comic Sans", (int) (Math.Min(scaleW, scaleH) / 2), FontStyle.Bold), Brushes.Black,
                    new RectangleF(citizen.Location.Y * scaleW, citizen.Location.X * scaleH,
                        scaleW, scaleH), format);
            }
        }

        private void DrawLose(Graphics graphics)
        {
            graphics.DrawString("ПОТРАЧЕНО",
                new Font("Impact", (int) Math.Min(scaleW, scaleH), FontStyle.Bold), Brushes.Black,
                    new RectangleF(0, 0, Width, Height), format);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            try
            {
                foreach (var citizen in forest.Citizens)
                {
                    try
                    {
                        forest.MoveCitizen(citizen, citizen.KeySet[e.KeyCode.ToString()].FirstOrDefault());
                    }
                    catch (NullReferenceException)
                    {
                        citizen.Ai.Move();
                    }
                }
            }
            catch (DivideByZeroException)
            {
                drawingAction = DrawLose;
            }
            catch (InvalidOperationException)
            {
                // Nothing to do here
            }
            Invalidate();
        }

        public void Display()
        {
            // You don't ever need to use this method
            Invalidate();
        }

        public void RunForestVisualization()
        {
            //var handle = GetConsoleWindow();
            //ShowWindow(handle, 0);
            Application.Run(this);
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr HWND, int nCmdShow);
    }
}
