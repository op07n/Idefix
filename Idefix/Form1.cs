using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Timers;
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
        public List<CAT10> objCat10 = new List<CAT10>();
        //public List<CAT19> objCat19 = new List<CAT19>();
        public List<CAT20> objCat20 = new List<CAT20>();
        public List<CAT21> objCat21 = new List<CAT21>();
        public List<Flight> flightList = new List<Flight>();

        public Funciones funcs = new Funciones();

        public Graphics myCanvas;
        public bool[] opcionesMapa = new bool[7] { true, false, false, false, false, false, false };
        public int mapSelected = 0;
        public double maxCoordX;
        public double maxCoordY;
        public double flightsXmax = 0;
        public double flightsYmax = 0;

        public int firsttimebutton2 = 0;
        public int simSpeed = 1;
        public System.Timers.Timer myTimer = new System.Timers.Timer();
        public int simTime = 0;

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
            radioButton1.Visible = false;
            radioButton2.Visible = false;
            radioButton3.Visible = false;
            firsttimebutton2 = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "ASTERIX files (*.ast)|*.ast";
            label2.Text = "Please wait while we process your request.";
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
                this.objCat10 = funcs.ReadCat10(msgsCat10, fspecsCat10);
                this.objCat20 = funcs.ReadCat20(msgsCat20, fspecsCat20);
                this.flightList = funcs.DistributeFlights(objCat10, objCat20, objCat21);
                this.simTime = (int)this.flightList[0].TimeofDay.TotalSeconds;
                label4.Text = this.flightList[0].TimeofDay.ToString();

                int cnt = 0;
                while (cnt < this.flightList.Count)
                {
                    if (Math.Abs(this.flightList[cnt].CartesianPosition[0]) > flightsXmax)
                    {
                        flightsXmax = Math.Abs(this.flightList[cnt].CartesianPosition[0]);
                    }
                    if (Math.Abs(this.flightList[cnt].CartesianPosition[1]) > flightsYmax)
                    {
                        flightsYmax = Math.Abs(this.flightList[cnt].CartesianPosition[1]);
                    }
                    cnt++;
                }

                label2.Visible = true;
                dataGridView1.Visible = false;
                pictureBox2.Visible = false;
                label2.Text = "Successfully read file " + openFileDialog1.FileName.Split('\\').Last() + "! Use the buttons on the left to access file data.";
                label2.BackColor = System.Drawing.Color.LightGreen;
                button2.Enabled = true;
                button3.Enabled = true;
                button5.Visible = false;
                button6.Visible = false;
                button7.Visible = false;
                label4.Visible = false;
                pictureBox3.Visible = false;
                pictureBox4.Visible = false;
                pictureBox5.Visible = false;
                label5.Visible = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label2.Visible = false;
            dataGridView1.Visible = true;
            pictureBox2.Visible = false;
            button5.Visible = false;
            button6.Visible = false;
            button7.Visible = false;
            label4.Visible = false;
            pictureBox3.Visible = false;
            pictureBox4.Visible = false;
            pictureBox5.Visible = false;
            label5.Visible = false;
            radioButton1.Visible = true;
            radioButton2.Visible = true;
            radioButton3.Visible = true;
            radioButton1.Text = "CAT10";
            radioButton2.Text = "CAT20";
            radioButton3.Text = "CAT10 & CAT 20";
            radioButton3.Enabled = false;//under construction
            if (firsttimebutton2 == 0) { radioButton1.Checked = true; }
            dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ColumnHeadersVisible = false;
            dataGridView1.ReadOnly = true;

            if (radioButton1.Checked == true)
            {
                dataGridView1.ColumnCount = 44;//numero de paràmetres que vull mostrar
                string[] tits = new string[44] { "SIC", "SAC", "Message Type", "Time of Day", "TYP", "DCR", "CHN", "GBS", "CRT", "SIM", "TST", "RAB", "LOP", "TOT", "SIP", "rho", "theta", "x", "y", "ground speed", "track angle", "vx", "vy", "Track Number", "CNF", "TRT", "CST", "MAH", "TCC", "STH", "TOM", "DOU", "MRS", "GHO", "Length", "Orientation", "Width", "NOGO", "OVL", "TSV", "DIV", "TTF", "ax", "ay" };
                dataGridView1.Rows.Add(tits);
                DataGridViewCellStyle style = new DataGridViewCellStyle();
                style.BackColor = Color.CadetBlue;
                style.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                dataGridView1.Rows[0].DefaultCellStyle = style;
                foreach (CAT10 a in objCat10.Take(50))
                {
                    string[] vs = new string[44] { a.SIC.ToString(), a.SAC.ToString(), a.MessageType, a.TimeofDay.ToString(), a.TargetReportDescriptor[0], a.TargetReportDescriptor[1], a.TargetReportDescriptor[2], a.TargetReportDescriptor[3], a.TargetReportDescriptor[4], a.TargetReportDescriptor[5], a.TargetReportDescriptor[6], a.TargetReportDescriptor[7], a.TargetReportDescriptor[8], a.TargetReportDescriptor[9], a.TargetReportDescriptor[10], a.PolarPosition[0].ToString(), a.PolarPosition[1].ToString(), a.CartesianPosition[0].ToString(), a.CartesianPosition[1].ToString(), a.PolarTrackVelocity[0].ToString(), a.PolarTrackVelocity[1].ToString(), a.CartesianTrackVelocity[0].ToString(), a.CartesianTrackVelocity[1].ToString(), a.TrackNumber.ToString(), a.TrackStatus[0], a.TrackStatus[1], a.TrackStatus[2], a.TrackStatus[3], a.TrackStatus[4], a.TrackStatus[5], a.TrackStatus[6], a.TrackStatus[7], a.TrackStatus[8], a.TrackStatus[9], a.TargetSizeAndOrientation[0].ToString(), a.TargetSizeAndOrientation[1].ToString(), a.TargetSizeAndOrientation[2].ToString(), a.SystemStatus[0], a.SystemStatus[1], a.SystemStatus[2], a.SystemStatus[3], a.SystemStatus[4], a.CalculatedAcceleration[0].ToString(), a.CalculatedAcceleration[1].ToString() };
                    dataGridView1.Rows.Add(vs);
                }
                dataGridView1.Name = "CAT 10 info";
                dataGridView1.AutoResizeColumnHeadersHeight();
                dataGridView1.CurrentRow.Selected = false;
            }

            if (radioButton2.Checked)
            {

            }
            /*
            var bindingList = new BindingList<CAT10>(objCat10);
            dataGridView1.DataSource = typeof(List<>);
            dataGridView1.DataSource = bindingList;
            dataGridView1.AutoResizeColumns();
            dataGridView1.Refresh();*/
        }

        public void pintarMapaLEBL()
        {
            pictureBox2.Visible = true;

            myCanvas.Clear(Control.DefaultBackColor);
            Pen pen = new Pen(Color.FromArgb(255, 0, 0, 0), 3);

            string[] mapasAeropuerto = new string[7] { "Aeropuerto_Barcelona.map", "BCN_Aparcamientos.map", "BCN_CarreterasServicio.map", "BCN_Edificios.map", "BCN_Parterres.map", "BCN_Pistas.map", "BCN_ZonasMovimiento.map" };

            int mapa = 0;
            int numMapas = mapasAeropuerto.Length;
            while (mapa < numMapas)
            {
                // Si el mapa en cuestion no esta activado, no lo pintamos
                if (opcionesMapa[mapa])
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
                            int deg1N = Convert.ToInt16(lineSplit[1].Substring(0, 2));
                            int min1N = Convert.ToInt16(lineSplit[1].Substring(2, 2));
                            double sec1N = Convert.ToDouble(lineSplit[1].Substring(4, 5)) / 1000.0;
                            double decimal1N = deg1N + (double)min1N / 60.0 + sec1N / 3600.0;

                            int deg1E = Convert.ToInt16(lineSplit[2].Substring(0, 3));
                            int min1E = Convert.ToInt16(lineSplit[2].Substring(3, 2));
                            double sec1E = Convert.ToDouble(lineSplit[2].Substring(5, 5)) / 1000.0;
                            double decimal1E = deg1E + (double)min1E / 60.0 + sec1E / 3600.0;

                            int deg2N = Convert.ToInt16(lineSplit[3].Substring(0, 2));
                            int min2N = Convert.ToInt16(lineSplit[3].Substring(2, 2));
                            double sec2N = Convert.ToDouble(lineSplit[3].Substring(4, 5)) / 1000.0;
                            double decimal2N = deg2N + (double)min2N / 60.0 + sec2N / 3600.0;

                            int deg2E = Convert.ToInt16(lineSplit[4].Substring(0, 3));
                            int min2E = Convert.ToInt16(lineSplit[4].Substring(3, 2));
                            double sec2E = Convert.ToDouble(lineSplit[4].Substring(5, 5)) / 1000.0;
                            double decimal2E = deg2E + (double)min2E / 60.0 + sec2E / 3600.0;

                            puntos1N.Add(decimal1N);
                            puntos1E.Add(decimal1E);
                            puntos2N.Add(decimal2N);
                            puntos2E.Add(decimal2E);
                        }
                        else if (lineSplit[0] == "Polilinea")
                        {
                            int read = 0;
                            int numPoints = Convert.ToInt32(lineSplit[1]);
                            double lastN = 0.0;
                            double lastE = 0.0;
                            while (read < numPoints - 1)
                            {
                                if (read == 0)
                                {
                                    line = file.ReadLine();
                                    int deg1N = Convert.ToInt16(line.Split(' ')[0].Substring(0, 2));
                                    int min1N = Convert.ToInt16(line.Split(' ')[0].Substring(2, 2));
                                    double sec1N = Convert.ToDouble(line.Split(' ')[0].Substring(4, 5)) / 1000.0;
                                    double decimal1N = deg1N + (double)min1N / 60.0 + sec1N / 3600.0;

                                    int deg1E = Convert.ToInt16(line.Split(' ')[1].Substring(0, 3));
                                    int min1E = Convert.ToInt16(line.Split(' ')[1].Substring(3, 2));
                                    double sec1E = Convert.ToDouble(line.Split(' ')[1].Substring(5, 5)) / 1000.0;
                                    double decimal1E = deg1E + (double)min1E / 60.0 + sec1E / 3600.0;

                                    puntos1N.Add(decimal1N);
                                    puntos1E.Add(decimal1E);

                                    line = file.ReadLine();

                                    int deg2N = Convert.ToInt16(line.Split(' ')[0].Substring(0, 2));
                                    int min2N = Convert.ToInt16(line.Split(' ')[0].Substring(2, 2));
                                    double sec2N = Convert.ToDouble(line.Split(' ')[0].Substring(4, 5)) / 1000.0;
                                    double decimal2N = deg2N + (double)min2N / 60.0 + sec2N / 3600.0;

                                    int deg2E = Convert.ToInt16(line.Split(' ')[1].Substring(0, 3));
                                    int min2E = Convert.ToInt16(line.Split(' ')[1].Substring(3, 2));
                                    double sec2E = Convert.ToDouble(line.Split(' ')[1].Substring(5, 5)) / 1000.0;
                                    double decimal2E = deg2E + (double)min2E / 60.0 + sec2E / 3600.0;

                                    lastN = decimal2N;
                                    lastE = decimal2E;
                                    puntos2N.Add(lastN);
                                    puntos2E.Add(lastE);
                                }
                                else
                                {
                                    puntos1N.Add(lastN);
                                    puntos1E.Add(lastE);
                                    line = file.ReadLine();

                                    int deg2N = Convert.ToInt16(line.Split(' ')[0].Substring(0, 2));
                                    int min2N = Convert.ToInt16(line.Split(' ')[0].Substring(2, 2));
                                    double sec2N = Convert.ToDouble(line.Split(' ')[0].Substring(4, 5)) / 1000.0;
                                    double decimal2N = deg2N + (double)min2N / 60.0 + sec2N / 3600.0;

                                    int deg2E = Convert.ToInt16(line.Split(' ')[1].Substring(0, 3));
                                    int min2E = Convert.ToInt16(line.Split(' ')[1].Substring(3, 2));
                                    double sec2E = Convert.ToDouble(line.Split(' ')[1].Substring(5, 5)) / 1000.0;
                                    double decimal2E = deg2E + (double)min2E / 60.0 + sec2E / 3600.0;

                                    lastN = decimal2N;
                                    lastE = decimal2E;
                                    puntos2N.Add(lastN);
                                    puntos2E.Add(lastE);
                                }
                                read++;
                            }
                        }
                    }

                    //Convierto los puntos de coordenadas Llh a XY, donde X=0,Y=0 es el centro de LEBL
                    double leblLatitude = 41.0 + 17.0 / 60.0 + 49.426 / 3600.0;  //41.1749426
                    double leblLongitude = 2.0 + 4.0 / 60.0 + 42.410 / 3600.0; //2.0442410;

                    int i = 0;
                    int end = puntos1N.Count;
                    List<double> puntos1x = new List<double>();
                    List<double> puntos1y = new List<double>();
                    List<double> puntos2x = new List<double>();
                    List<double> puntos2y = new List<double>();

                    double constan = Math.PI / 2;

                    while (i < end)
                    {
                        double a = 6378137;
                        double b = 6356752.3142;
                        double e_sq = 1 - Math.Pow(b / a, 2);
                        double tmp = Math.Sqrt(1 - e_sq * Math.Pow(Math.Sin(leblLatitude * (Math.PI / 180.0) + constan), 2));

                        double lat1 = puntos1N[i] - leblLatitude;
                        double long1 = puntos1E[i] - leblLongitude;
                        double x1 = ((a * long1) / tmp) * (Math.Cos(leblLatitude * (Math.PI / 180.0) + constan) - ((1 - e_sq) / Math.Pow(tmp, 2)) * Math.Sin(leblLatitude * (Math.PI / 180.0) + constan) * lat1);
                        double y1 = ((a * (1 - e_sq)) / Math.Pow(tmp, 3)) * lat1 + a * Math.Cos(leblLatitude * (Math.PI / 180.0) + constan) * Math.Sin(leblLatitude * (Math.PI / 180.0) + constan) * (1.5 * e_sq * Math.Pow(lat1, 2) + (Math.Pow(long1, 2) / (2 * tmp)));
                        puntos1x.Add(x1);
                        puntos1y.Add(y1);

                        double lat2 = puntos2N[i] - leblLatitude;
                        double long2 = puntos2E[i] - leblLongitude;
                        double x2 = ((a * long2) / tmp) * (Math.Cos(leblLatitude * (Math.PI / 180.0) + constan) - ((1 - e_sq) / Math.Pow(tmp, 2)) * Math.Sin(leblLatitude * (Math.PI / 180.0) + constan) * lat2);
                        double y2 = ((a * (1 - e_sq)) / Math.Pow(tmp, 3)) * lat2 + a * Math.Cos(leblLatitude * (Math.PI / 180.0) + constan) * Math.Sin(leblLatitude * (Math.PI / 180.0) + constan) * (1.5 * e_sq * Math.Pow(lat2, 2) + (Math.Pow(long2, 2) / (2 * tmp)));
                        puntos2x.Add(x2);
                        puntos2y.Add(y2);
                        i++;
                    }

                    if (puntos1x.Count > 0)
                    {
                        //Normalizo las coordenadas X e Y, y grafico sobre el canvas
                        double maxCoord1X = puntos1x.Select(Math.Abs).Max<double>();
                        double maxCoord1Y = puntos1y.Select(Math.Abs).Max<double>();
                        double maxCoord2X = puntos2x.Select(Math.Abs).Max<double>();
                        double maxCoord2Y = puntos2y.Select(Math.Abs).Max<double>();
                        maxCoordX = Math.Max(puntos1x.Select(Math.Abs).Max<double>(), puntos2x.Select(Math.Abs).Max<double>());
                        maxCoordY = Math.Max(puntos1y.Select(Math.Abs).Max<double>(), puntos2y.Select(Math.Abs).Max<double>());

                        i = 0;
                        end = puntos1N.Count;
                        while (i < end)
                        {
                            puntos1x[i] = (pictureBox2.Width / 2) - (puntos1x[i] / maxCoordX) * (pictureBox2.Width / 2);
                            puntos1y[i] = (pictureBox2.Height / 2) - (puntos1y[i] / maxCoordY) * (pictureBox2.Height / 2);
                            puntos2x[i] = (pictureBox2.Width / 2) - (puntos2x[i] / maxCoordX) * (pictureBox2.Width / 2);
                            puntos2y[i] = (pictureBox2.Height / 2) - (puntos2y[i] / maxCoordY) * (pictureBox2.Height / 2);
                            myCanvas.DrawLine(pen, (float)puntos1x[i], (float)puntos1y[i], (float)puntos2x[i], (float)puntos2y[i]);
                            i++;
                        }
                    }
                    //Paso a pintar el siguiente mapa
                    file.Close();
                }
                mapa++;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            label2.Visible = true;
            label2.BackColor = default(Color);
            label2.Text = "Choose a map";
            dataGridView1.Visible = false;
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
            string[] mapasAeropuerto = new string[1] { "Aeropuerto_Barcelonanue.map" };
            myCanvas = pictureBox2.CreateGraphics();
            pintarMapaLEBL();
            label4.Visible = true;
            pictureBox3.Visible = true;
            pictureBox4.Visible = true;
            pictureBox5.Visible = true;
            label5.Visible = true;
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
            pintarMapaBCN();
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

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            if (simSpeed == 6)
                simSpeed = 10;
            else if (simSpeed == 10)
                simSpeed = 15;
            else if (simSpeed == 15)
                simSpeed = 30;
            else if (simSpeed == 30)
                simSpeed = 1;
            else
                simSpeed = (simSpeed + 1) % 7;

            if (simSpeed == 0)
                simSpeed = 1;
            label5.Text = "x" + simSpeed.ToString();
            if(myTimer.Enabled)
                myTimer.Elapsed -= new ElapsedEventHandler(simulationStep);
                myTimer.Stop();
                pictureBox3_Click(null, null);
        }

        public void simulationStep(object source, ElapsedEventArgs e)
        {
            Funciones funcs = new Funciones();
            string horaSim = funcs.ConvertTime(simTime);

            this.Invoke((MethodInvoker)delegate {
                label4.Text = horaSim; // runs on UI thread
                int x = 0;
                bool ended = false;
                while(x < flightList.Count || !ended)
                {
                    //myCanvas.FillEllipse(Brushes.Blue, (pictureBox2.Width / 2), (pictureBox2.Height / 2), 5, 5);
                    if ((int)flightList[x].TimeofDay.TotalSeconds == Convert.ToInt32(simTime))
                    { 
                        myCanvas.FillEllipse(Brushes.Red, 
                            (float)((pictureBox2.Width / 2) + (flightList[x].CartesianPosition[0] / flightsXmax) * (pictureBox2.Width / 2)),
                            (float)((pictureBox2.Height / 2) - (flightList[x].CartesianPosition[1] / flightsYmax) * (pictureBox2.Height / 2)), 
                            5, 
                            5);
                    }
                    else if ((int)flightList[x].TimeofDay.TotalSeconds > Convert.ToInt32(simTime))
                    {
                        ended = true;
                    }
                       
                    x++;
                }
            });
            simTime++;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if (!myTimer.Enabled)
            {
                myTimer.Elapsed += new ElapsedEventHandler(simulationStep);
                myTimer.Interval = 1000.0 / (double)simSpeed; // 1000 ms is one second
                myTimer.Start();
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            if (myTimer.Enabled)
            {
                myTimer.Elapsed -= new ElapsedEventHandler(simulationStep);
                myTimer.Stop();
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            firsttimebutton2 = 1;
            button2_Click(sender,e);
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            firsttimebutton2 = 1;
            button2_Click(sender, e);
        }
    }
}
