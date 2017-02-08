using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Media;
using System.Windows;

namespace img_dots
{
    public partial class Form1 : Form
    {
        string path = "";


        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown2.Maximum = numericUpDown1.Value - 2;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = ".jpg files (*.jpg)|*.jpg|.jpeg files (*.jpeg)|*.jpeg|.bmp files (*.bmp)|*.bmp|.png files (*.png)|*.png|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                path = openFileDialog1.FileName;

                //TEXTBOX VALUE SET TO PATH
                textBox1.Text = path;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int squaresize = Convert.ToInt32(numericUpDown1.Value);
            int circlesize = Convert.ToInt32(numericUpDown2.Value);

            //GET IMAGE OBJECT
            Image img = Image.FromFile(path);
            Size size = img.Size;

            Bitmap bmp = new Bitmap(img);

            string filename = Path.GetFileNameWithoutExtension(path);

            //CREATE EMPTY BITMAP TO DRAW ON IT
            using (Bitmap save = new Bitmap(size.Width, size.Height))
            {
                using (Graphics g = Graphics.FromImage(save))
                {
                    Squares[,] sqrs = new Squares[size.Height / squaresize, size.Width / squaresize];

                    int x = 0;
                    int y = 0;

                    Color sqrAvColor = new Color();

                    SolidBrush sb = new SolidBrush(Color.Black);

                    g.FillRectangle(sb, 0, 0, size.Width, size.Height);

                    //FOR EACH SQUARE IN PHOTO
                    for (int i = 0; i < size.Height / squaresize; i++)
                    {
                        for (int j = 0; j < size.Width / squaresize; j++)
                        {
                            Squares square = new Squares(squaresize);

                            //FOR EACH PIXEL IN SQUARE
                            for (int k = 0; k < squaresize; k++)
                            {
                                y = i * squaresize + k;

                                for (int l = 0; l < squaresize; l++)
                                {
                                    x = j * squaresize + l;

                                    square.SetColor(k, l, bmp.GetPixel(x, y));
                                }
                            }

                            sqrAvColor = Squares.GetAverageColor(square);
                            sb.Color = sqrAvColor;

                            g.FillEllipse(sb, j * squaresize, i * squaresize, circlesize, circlesize);
                        }
                    }

                    g.Flush();
                    save.Save(filename + "_adampisula.png", System.Drawing.Imaging.ImageFormat.Png);
                    g.Dispose();
                }
            }
        }
    }

    public class Squares
    {
        public Color[,] Pixels { get; set; }

        public Squares(int size)
        {
            Pixels = new Color[size, size];
        }

        public void SetColor(int x, int y, Color c)
        {
            Pixels[x, y] = c;
        }

        public static Color GetAverageColor(Squares sqr)
        {
            Color ret = new Color();

            int rowLength = sqr.Pixels.GetLength(0);
            int colLength = sqr.Pixels.GetLength(1);
            int r = 0, g = 0, b = 0;
            int a = rowLength * colLength;

            for (int i = 0; i < rowLength; i++)
            {
                for (int j = 0; j < colLength; j++)
                {
                    r += sqr.Pixels[i, j].R;
                    g += sqr.Pixels[i, j].G;
                    b += sqr.Pixels[i, j].B;
                }
            }

            ret = ColorTranslator.FromHtml("#" + (r / a).ToString("X2") + (g / a).ToString("X2") + (b / a).ToString("X2"));

            return ret;
        }

        public static string PrintSquare(Squares sqr)
        {
            string ret = "";

            int rowLength = sqr.Pixels.GetLength(0);
            int colLength = sqr.Pixels.GetLength(1);

            for (int i = 0; i < rowLength; i++)
            {
                for (int j = 0; j < colLength; j++)
                {
                    ret += HexConverter(sqr.Pixels[i, j]) + " ";
                }

                ret += "\n";
            }

            return ret;
        }

        public static String HexConverter(System.Drawing.Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }
    }
}
