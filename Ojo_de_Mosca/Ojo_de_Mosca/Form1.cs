using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ojo_de_Mosca
{
    public partial class Form1 : Form
    {
       
        
        public Form1()
        {
            InitializeComponent();
        }

        
      

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
          
        
            PictureBox lienzo = sender as PictureBox;
            
            Pen pluma = new Pen(Color.Black, 2f);
            var graphics = e.Graphics;
            lienzo.Image = null;
            int numPuntos = Convert.ToInt32(nudePuntos.Value);
            bool esPar = numPuntos % 2 == 0;
            int Radio = Math.Min(lienzo.Width, lienzo.Height) * 4 / 10;
            int x_centro = lienzo.Width / 2;
            int y_centro = lienzo.Height / 2;
            Point[] puntos = new Point[numPuntos];
            
            
            for (int i = 0; i < numPuntos; i++)
            {
                puntos[i].X = Convert.ToInt32(Math.Sin(i * (Math.PI * 2) / numPuntos) * Radio) + x_centro;
                puntos[i].Y = Convert.ToInt32(Math.Cos(i * (Math.PI * 2) / numPuntos) * Radio) + y_centro;
            }
            for (int i = 0; i < numPuntos - 1; i++)
            {
                for (int j = i + 1; j < numPuntos; j++)
                {
                    if (esPar && i + numPuntos / 2 == j)
                    {
                        continue;
                    }
                    graphics.DrawLine(pluma, puntos[i], puntos[j]);
                }

            }
                
        }

        private void btnDibujar_Click(object sender, EventArgs e)
        {
            
            
        }

        private void pictureBox1_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            PictureBox lienzo = sender as PictureBox;
            lienzo.InitialImage = null;
        }
    }
}
