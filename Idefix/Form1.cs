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
            ;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            label2.Visible = false;
            tableLayoutPanel1.Visible = false;
            pictureBox2.Visible = true;
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Idefix is an ASTERIX data files reader.\n\nCreated by Aleix Coma, Ramon Garcia, Isabel Montolio, and Marti Prat. Contact ramon@rgalarcia.com for more information.\n\n(C) 2019 Universitat Politecnica de Catalunya - BarcelonaTech.", "About Idefix", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
