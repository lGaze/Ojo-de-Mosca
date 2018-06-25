using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading;

namespace Ojo_de_Mosca
{
    public partial class Form1 : Form
    {
        int numPoints;
        List<Point[]> lines = new List<Point[]>();
        List<Color[]> colors = new List<Color[]>();

        public Form1()
        {
            InitializeComponent();
        }

        private Color HsvToRgb(double h, double S, double V)
        {
            double H = h;
            while (H < 0) { H += 360; };
            while (H >= 360) { H -= 360; };
            double R, G, B;
            if (V <= 0)
            { R = G = B = 0; }
            else if (S <= 0)
            {
                R = G = B = V;
            }
            else
            {
                double hf = H / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = V * (1 - S);
                double qv = V * (1 - S * f);
                double tv = V * (1 - S * (1 - f));
                switch (i)
                {

                    // Red is the dominant color

                    case 0:
                        R = V;
                        G = tv;
                        B = pv;
                        break;

                    // Green is the dominant color

                    case 1:
                        R = qv;
                        G = V;
                        B = pv;
                        break;
                    case 2:
                        R = pv;
                        G = V;
                        B = tv;
                        break;

                    // Blue is the dominant color

                    case 3:
                        R = pv;
                        G = qv;
                        B = V;
                        break;
                    case 4:
                        R = tv;
                        G = pv;
                        B = V;
                        break;

                    // Red is the dominant color

                    case 5:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

                    case 6:
                        R = V;
                        G = tv;
                        B = pv;
                        break;
                    case -1:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // The color is not defined, we should throw an error.

                    default:
                        //LFATAL("i Value error in Pixel conversion, Value is %d", i);
                        R = G = B = V; // Just pretend its black/white
                        break;
                }
            }

            return Color.FromArgb(
                Clamp((int)(R * 255.0)), 
                Clamp((int)(G * 255.0)), 
                Clamp((int)(B * 255.0)));
        }

        private int Clamp(int i)
        {
            if (i < 0) return 0;
            if (i > 255) return 255;
            return i;
        }

        private void RecalculateLines()
        {
            PictureBox lienzo = pictureBox1;
            bool esPar = numPoints % 2 == 0;
            double Radio = Math.Min(lienzo.Width, lienzo.Height) / 2.0;
            int x_centro = lienzo.Width / 2;
            int y_centro = lienzo.Height / 2;
            Point[] puntos = new Point[numPoints];
            Color[] colores = new Color[numPoints];

            if (ckbUseThreads.Checked)
            {
                int half = (int)Math.Ceiling(numPoints / 2.0);
                Thread thread1 = new Thread(new ThreadStart(() => ThreadRecalculateLines(0, half, Radio, x_centro, y_centro, ref puntos, ref colores)));
                Thread thread2 = new Thread(new ThreadStart(() => ThreadRecalculateLines(half, numPoints, Radio, x_centro, y_centro, ref puntos, ref colores)));
                thread1.Start();
                thread2.Start();
                thread1.Join();
                thread2.Join();
            }
            else
            {
                for (int i = 0; i < numPoints; i++)
                {
                    double radians = i * (Math.PI * 2.0) / numPoints;
                    double angle = i * 360.0 / numPoints;

                    puntos[i].X = Convert.ToInt32(Math.Sin(radians) * Radio) + x_centro;
                    puntos[i].Y = Convert.ToInt32(Math.Cos(radians) * Radio) + y_centro;

                    colores[i] = HsvToRgb(angle, 1, 1);
                }
            }

            lines.Clear();
            colors.Clear();
            for (int i = 0; i < numPoints - 1; i++)
            {
                for (int j = i + 1; j < numPoints; j++)
                {
                    if (esPar && i + numPoints / 2 == j)
                    {
                        continue;
                    }
                    lines.Add(new Point[] { puntos[i], puntos[j] });
                    colors.Add(new Color[] { colores[i], colores[j] });
                }
            }
        }

        private void ThreadRecalculateLines(int ini, int fin, double radio, int x_centro, int y_centro, ref Point[] puntos, ref Color[] colores)
        {
            for (int i = ini; i < fin; i++)
            {
                double radians = i * (Math.PI * 2.0) / numPoints;
                double angle = i * 360.0 / numPoints;

                puntos[i].X = Convert.ToInt32(Math.Sin(radians) * radio) + x_centro;
                puntos[i].Y = Convert.ToInt32(Math.Cos(radians) * radio) + y_centro;

                colores[i] = HsvToRgb(angle, 1, 1);
            }
        }

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (numPoints <= 0) return;

            PictureBox lienzo = sender as PictureBox;

            var graphics = e.Graphics;

            Random r = new Random();

            var linesAndColors = lines.Zip(colors, (n, w) => new { Line = n, Color = w });
            foreach (var lineColor in linesAndColors)
            {
                using (Brush aGradientBrush = new LinearGradientBrush(lineColor.Line[0], lineColor.Line[1], lineColor.Color[0], lineColor.Color[0]))
                {
                    using (Pen aGradientPen = new Pen(aGradientBrush))
                    {
                        graphics.DrawLine(aGradientPen, lineColor.Line[0], lineColor.Line[1]);
                    }
                }
            }
        }

        private void BtnDibujar_Click(object sender, EventArgs e)
        {
            numPoints = Convert.ToInt32(nudePuntos.Value);
            RecalculateLines();
            pictureBox1.Refresh();
        }

        private void PictureBox1_Resize(object sender, EventArgs e)
        {
            if (numPoints > 0)
            {
                RecalculateLines();
                pictureBox1.Refresh();
            }
        }
    }
}
