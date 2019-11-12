using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Idefix
{
    public partial class Idefix : Form
    {


        public List<double[]> msgsCat10 = new List<double[]>();
        public List<double[]> msgsCat19 = new List<double[]>();
        public List<double[]> msgsCat20 = new List<double[]>();
        public List<double[]> msgsCat21 = new List<double[]>();
        public List<string[]> fspecsCat10 = new List<string[]>();
        public List<string[]> fspecsCat19 = new List<string[]>();
        public List<string[]> fspecsCat20 = new List<string[]>();
        public List<string[]> fspecsCat21 = new List<string[]>();

        string mode = null;
        string mapmode = null;

        public Idefix()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label2.Text = "To start, please click the 'Read file' button on the left to select an ASTERIX file to be read.\n" +
                "You will then be able to show the file data on a table or on a map by using the controls on the left.";
            button2.Enabled = false;
            button3.Enabled = false;
            button5.Visible = false;
            button6.Visible = false;
            button7.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "ASTERIX files (*.ast)|*.ast";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Funciones funcs = new Funciones();
                Archivo fichero = funcs.LeerArchivo(openFileDialog1.FileName);
                this.msgsCat10 = fichero.GetMsgsCat10();
                this.msgsCat19 = fichero.GetMsgsCat19();
                this.msgsCat20 = fichero.GetMsgsCat20();
                this.msgsCat21 = fichero.GetMsgsCat21();
                this.fspecsCat10 = funcs.GetFSPEC(this.msgsCat10);
                this.fspecsCat19 = funcs.GetFSPEC(this.msgsCat19);
                this.fspecsCat20 = funcs.GetFSPEC(this.msgsCat20);
                this.fspecsCat21 = funcs.GetFSPEC(this.msgsCat21);
                label2.Visible = true;
                tableLayoutPanel1.Visible = false;
                pictureBox2.Visible = false;
                label2.Text = "Successfully read file " + openFileDialog1.FileName.Split('\\').Last() + "! Use buttons on the left to access file data.";
                label2.BackColor = System.Drawing.Color.LightGreen;
                button2.Enabled = true;
                button3.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label2.Visible = false;
            tableLayoutPanel1.Visible = true;
            pictureBox2.Visible = false;
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            tableLayoutPanel1.Controls.Add(new Label() { Text = "Column 1" }, 0, 0);
            tableLayoutPanel1.Controls.Add(new Label() { Text = "Column 2" }, 1, 0);
            tableLayoutPanel1.Controls.Add(new Label() { Text = "Column 3" }, 2, 0);
            tableLayoutPanel1.Controls.Add(new Label() { Text = "Data" }, 0, 1);
            tableLayoutPanel1.Controls.Add(new Label() { Text = "Data" }, 1, 1);
            tableLayoutPanel1.Controls.Add(new Label() { Text = "Data" }, 2, 1);
        }

        public void pintarMapaLEBL()
        {
            pictureBox2.Visible = true;

            Graphics myCanvas = pictureBox2.CreateGraphics();
            Pen pen = new Pen(Color.FromArgb(255, 0, 0, 0), 3);

            string[] mapasAeropuerto = new string[7] { "Aeropuerto_Barcelona.map", "BCN_Aparcamientos.map", "BCN_CarreterasServicio.map", "BCN_Edificios.map", "BCN_Parterres.map", "BCN_Pistas.map", "BCN_ZonasMovimiento.map" };

            int mapa = 0;
            int numMapas = mapasAeropuerto.Length;
            while (mapa < numMapas)
            {
                // Leo los puntos del archivo en coordenadas Llh
                List<double> puntos1N = new List<double>();
                List<double> puntos1E = new List<double>();
                List<double> puntos2N = new List<double>();
                List<double> puntos2E = new List<double>();

                string line;
                System.IO.StreamReader file = new System.IO.StreamReader("../../maps/" + mapasAeropuerto[mapa]);
                while ((line = file.ReadLine()) != null)
                {
                    string[] lineSplit = line.Split(' ');
                    if (lineSplit[0] == "Linea")
                    {
                        puntos1N.Add(Convert.ToDouble(lineSplit[1].Remove(9)) / 10000000.0);
                        puntos1E.Add(Convert.ToDouble(lineSplit[2].Remove(10)) / 10000000.0);
                        puntos2N.Add(Convert.ToDouble(lineSplit[3].Remove(9)) / 10000000.0);
                        puntos2E.Add(Convert.ToDouble(lineSplit[4].Remove(10)) / 10000000.0);
                    }
                }

                //Convierto los puntos de coordenadas Llh a XY, donde X=0,Y=0 es el centro de LEBL
                double leblLatitude = 41.1749426;
                double leblLongitude = 2.0442410;

                int i = 0;
                int end = puntos1N.Count;
                List<double> puntos1x = new List<double>();
                List<double> puntos1y = new List<double>();
                List<double> puntos2x = new List<double>();
                List<double> puntos2y = new List<double>();

                while (i < end)
                {
                    double a = 6378137;
                    double b = 6356752.3142;
                    double e_sq = 1 - Math.Pow(b / a, 2);
                    double tmp = Math.Sqrt(1 - e_sq * Math.Pow(Math.Sin(leblLatitude), 2));

                    double lat1 = puntos1N[i] - leblLatitude;
                    double long1 = puntos1E[i] - leblLongitude;
                    double x1 = (a * long1 / tmp) * (Math.Cos(leblLatitude) - (1 - e_sq / Math.Pow(tmp, 2)) * Math.Sin(leblLatitude) * lat1);
                    double y1 = (a * (1 - e_sq) / Math.Pow(tmp, 3)) * lat1 + a * Math.Cos(leblLatitude) * Math.Sin(leblLatitude) * (1.5 * e_sq * Math.Pow(lat1, 2) + (Math.Pow(long1, 2) / (2 * tmp)));
                    puntos1x.Add(x1);
                    puntos1y.Add(y1);

                    double lat2 = puntos2N[i] - leblLatitude;
                    double long2 = puntos2E[i] - leblLongitude;
                    double x2 = (a * long2 / tmp) * (Math.Cos(leblLatitude) - (1 - e_sq / Math.Pow(tmp, 2)) * Math.Sin(leblLatitude) * lat2);
                    double y2 = (a * (1 - e_sq) / Math.Pow(tmp, 3)) * lat2 + a * Math.Cos(leblLatitude) * Math.Sin(leblLatitude) * (1.5 * e_sq * Math.Pow(lat2, 2) + (Math.Pow(long2, 2) / (2 * tmp)));
                    puntos2x.Add(x2);
                    puntos2y.Add(y2);
                    i++;
                }

                if (puntos1x.Count > 0)
                {
                    //Normalizo las coordenadas X e Y, y grafico sobre el canvas
                    double maxCoordX = Math.Max(puntos1x.Max<double>(), puntos2x.Max<double>());
                    double maxCoordY = Math.Max(puntos1y.Max<double>(), puntos2y.Max<double>());

                    i = 0;
                    end = puntos1N.Count;
                    while (i < end)
                    {
                        puntos1x[i] = pictureBox2.Width / 2 + puntos1x[i] / maxCoordX * (pictureBox2.Width / 2);
                        puntos1y[i] = pictureBox2.Height / 2 + puntos1y[i] / maxCoordY * (pictureBox2.Height / 2);
                        puntos2x[i] = pictureBox2.Width / 2 + puntos2x[i] / maxCoordX * (pictureBox2.Width / 2);
                        puntos2y[i] = pictureBox2.Height / 2 + puntos2y[i] / maxCoordY * (pictureBox2.Height / 2);
                        myCanvas.DrawLine(pen, (float)puntos1x[i], (float)puntos1y[i], (float)puntos2x[i], (float)puntos2y[i]);
                        i++;
                    }
                }
                //Paso a pintar el siguiente mapa
                file.Close();
                mapa++;
            }
        }

        public void pintarMapaPeninsula()
        {
            pictureBox2.Visible = true;
            pictureBox2.Dispose();

            Graphics myCanvas = pictureBox2.CreateGraphics();
            Pen pen = new Pen(Color.FromArgb(255, 0, 0, 0), 3);

            string[] mapasAeropuerto = new string[5] { "Peninsula_Aerodromos.map", "Peninsula_Aerovias.map", "BCN_CarreterasServicio.map", "Peninsula_Costas.map", "Peninsula_Fijos.map" };

            int mapa = 0;
            int numMapas = mapasAeropuerto.Length;
            while (mapa < numMapas)
            {
                // Leo los puntos del archivo en coordenadas Llh
                List<double> puntos1N = new List<double>();
                List<double> puntos1E = new List<double>();
                List<double> puntos2N = new List<double>();
                List<double> puntos2E = new List<double>();

                string line;
                System.IO.StreamReader file = new System.IO.StreamReader("../../maps/" + mapasAeropuerto[mapa]);
                while ((line = file.ReadLine()) != null)
                {
                    string[] lineSplit = line.Split(' ');
                    if (lineSplit[0] == "Linea")
                    {
                        puntos1N.Add(Convert.ToDouble(lineSplit[1].Remove(9)) / 10000000.0);
                        puntos1E.Add(Convert.ToDouble(lineSplit[2].Remove(10)) / 10000000.0);
                        puntos2N.Add(Convert.ToDouble(lineSplit[3].Remove(9)) / 10000000.0);
                        puntos2E.Add(Convert.ToDouble(lineSplit[4].Remove(10)) / 10000000.0);
                    }
                }

                //Convierto los puntos de coordenadas Llh a XY, donde X=0,Y=0 es el centro de LEBL
                double leblLatitude = 41.1749426;
                double leblLongitude = 2.0442410;

                int i = 0;
                int end = puntos1N.Count;
                List<double> puntos1x = new List<double>();
                List<double> puntos1y = new List<double>();
                List<double> puntos2x = new List<double>();
                List<double> puntos2y = new List<double>();

                while (i < end)
                {
                    double a = 6378137;
                    double b = 6356752.3142;
                    double e_sq = 1 - Math.Pow(b / a, 2);
                    double tmp = Math.Sqrt(1 - e_sq * Math.Pow(Math.Sin(leblLatitude), 2));

                    double lat1 = puntos1N[i] - leblLatitude;
                    double long1 = puntos1E[i] - leblLongitude;
                    double x1 = (a * long1 / tmp) * (Math.Cos(leblLatitude) - (1 - e_sq / Math.Pow(tmp, 2)) * Math.Sin(leblLatitude) * lat1);
                    double y1 = (a * (1 - e_sq) / Math.Pow(tmp, 3)) * lat1 + a * Math.Cos(leblLatitude) * Math.Sin(leblLatitude) * (1.5 * e_sq * Math.Pow(lat1, 2) + (Math.Pow(long1, 2) / (2 * tmp)));
                    puntos1x.Add(x1);
                    puntos1y.Add(y1);

                    double lat2 = puntos2N[i] - leblLatitude;
                    double long2 = puntos2E[i] - leblLongitude;
                    double x2 = (a * long2 / tmp) * (Math.Cos(leblLatitude) - (1 - e_sq / Math.Pow(tmp, 2)) * Math.Sin(leblLatitude) * lat2);
                    double y2 = (a * (1 - e_sq) / Math.Pow(tmp, 3)) * lat2 + a * Math.Cos(leblLatitude) * Math.Sin(leblLatitude) * (1.5 * e_sq * Math.Pow(lat2, 2) + (Math.Pow(long2, 2) / (2 * tmp)));
                    puntos2x.Add(x2);
                    puntos2y.Add(y2);
                    i++;
                }

                if (puntos1x.Count > 0)
                {
                    //Normalizo las coordenadas X e Y, y grafico sobre el canvas
                    double maxCoordX = Math.Max(puntos1x.Max<double>(), puntos2x.Max<double>());
                    double maxCoordY = Math.Max(puntos1y.Max<double>(), puntos2y.Max<double>());

                    i = 0;
                    end = puntos1N.Count;
                    while (i < end)
                    {
                        puntos1x[i] = pictureBox2.Width / 2 + puntos1x[i] / maxCoordX * (pictureBox2.Width / 2);
                        puntos1y[i] = pictureBox2.Height / 2 + puntos1y[i] / maxCoordY * (pictureBox2.Height / 2);
                        puntos2x[i] = pictureBox2.Width / 2 + puntos2x[i] / maxCoordX * (pictureBox2.Width / 2);
                        puntos2y[i] = pictureBox2.Height / 2 + puntos2y[i] / maxCoordY * (pictureBox2.Height / 2);
                        myCanvas.DrawLine(pen, (float)puntos1x[i], (float)puntos1y[i], (float)puntos2x[i], (float)puntos2y[i]);
                        i++;
                    }
                }
                //Paso a pintar el siguiente mapa
                file.Close();
                mapa++;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            label2.Visible = true;
            label2.BackColor = default(Color);
            label2.Text = "Choose a map";
            tableLayoutPanel1.Visible = false;
            pictureBox2.Visible = true;
            button5.Visible = true;
            button5.BackColor = default(Color);
            button6.Visible = true;
            button6.BackColor = default(Color);
            button7.Visible = true;
            button7.BackColor = default(Color);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Idefix is an ASTERIX data files reader.\n\nCreated by Aleix Coma, Ramon Garcia, Isabel Montolio, and Marti Prat. Contact ramon@rgalarcia.com for more information.\n\n(C) 2019 Universitat Politecnica de Catalunya - BarcelonaTech.", "About Idefix", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            label2.Visible = false;
            pictureBox2.Visible = true;
            button5.Visible = true;
            button5.BackColor = Color.SteelBlue;
            button6.Visible = true;
            button6.BackColor = default(Color);
            button7.Visible = true;
            button7.BackColor = default(Color);
            pintarMapaLEBL();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            /*
            label2.Visible = false;
            pictureBox2.Visible = true;
            button5.Visible = true;
            button5.BackColor = default(Color);
            button6.Visible = true;
            button6.BackColor = Color.SteelBlue;
            button7.Visible = true;
            button7.BackColor = default(Color);
            this.Invalidate();
            */
        }

        private void button7_Click(object sender, EventArgs e)
        {
            /*
            label2.Visible = false;
            pictureBox2.Visible = true;
            button5.Visible = true;
            button5.BackColor = default(Color);
            button6.Visible = true;
            button6.BackColor = default(Color);
            button7.Visible = true;
            button7.BackColor = Color.SteelBlue;
            pintarMapaPeninsula();
            */
        }
    }
}
