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
                List<double[]> msgsCat19 = fichero.GetMsgsCat19();
                List<double[]> msgsCat20 = fichero.GetMsgsCat20();
                List<string[]> fspecs19 = funcs.GetFSPEC(msgsCat19);
                List<string[]> fspecs20 = funcs.GetFSPEC(msgsCat20);
                string[] helou = fspecs19[7379];
                label2.Text = "Done";
            }
        }
    }
}
