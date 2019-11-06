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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Text = "Idefix - an ASTERIX data analyzer";
            label2.Text = "Press Start and choose an ASTERIX file to analyze:";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.ShowDialog();

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Funciones funcs = new Funciones();
                Archivo fichero = funcs.LeerArchivo(openFileDialog1.FileName);
                List<List<double>> msgsCat20 = fichero.GetMsgsCat20();
                label2.Text = "Done";
            }
        }
    }
}
