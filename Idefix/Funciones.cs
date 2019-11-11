using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Collections;

namespace Idefix
{
    public class Funciones
    {
        public Archivo LeerArchivo(string fileName)
        {
            Archivo fichero = new Archivo();

            using (BinaryReader read = new BinaryReader(File.Open(fileName, FileMode.Open)))
            {
                List<double> msgCAT10 = new List<double>();
                List<double> msgCAT19 = new List<double>();
                List<double> msgCAT20 = new List<double>();
                List<double> msgCAT21 = new List<double>();

                int pos = 0;
                int end = (int)read.BaseStream.Length;
                int lengthMsg = 0;
                bool isStarting = true;

                double category = read.BaseStream.ReadByte();
                pos++;

                while (pos <= end)
                {
                    if (isStarting)
                    {
                        byte byte1 = (byte)read.BaseStream.ReadByte();
                        byte byte2 = (byte)read.BaseStream.ReadByte();
                        lengthMsg = (byte1 << 8 | byte2)-3;
                        isStarting = false;
                        pos += 2;
                    }
                    else
                    {
                        while (lengthMsg != 0)
                        {
                            if (category == 10) msgCAT10.Add(read.BaseStream.ReadByte());
                            else if (category == 19) msgCAT19.Add(read.BaseStream.ReadByte());
                            else if (category == 20) msgCAT20.Add(read.BaseStream.ReadByte());
                            else if (category == 21) msgCAT21.Add(read.BaseStream.ReadByte());
                            lengthMsg--;
                            pos++;
                        }

                        if (category == 10)
                        {
                            fichero.SetMsgCat10(msgCAT10);
                            msgCAT10.Clear();
                        }
                        if (category == 19)
                        {
                            fichero.SetMsgCat19(msgCAT19);
                            msgCAT19.Clear();
                        }
                        else if (category == 20)
                        {
                            fichero.SetMsgCat20(msgCAT20);
                            msgCAT20.Clear();
                        }
                        else if (category == 21)
                        {
                            fichero.SetMsgCat21(msgCAT21);
                            msgCAT21.Clear();
                        }

                        category = read.BaseStream.ReadByte();
                        isStarting = true;
                        pos++;
                    }
                }
            }

            return fichero;
        }

        public List<string[]> GetFSPEC(List<double[]> msgs)
        {
            List<string[]> fspecsList = new List<string[]>();
            foreach(double[] msg in msgs)
            {
                double[] fspecInit = new double[4];
                Array.Copy(msg, fspecInit, 4);
                List<string> fspecFinal = new List<string>();
                bool checkbyte = true;
                int i = 0;

                while (checkbyte)
                {
                    string tempFspec = Convert.ToString(Convert.ToInt32(fspecInit[i].ToString(), 10), 2).PadLeft(8, '0');

                    if (tempFspec[tempFspec.Length - 1] == '0')
                    {
                        fspecFinal.Add(tempFspec);
                        checkbyte = false;
                    }
                    else
                    {
                        fspecFinal.Add(tempFspec);
                    }
                    i++;
                }
                fspecsList.Add(fspecFinal.ToArray());
            }
            return fspecsList;
        }

        public string[] SepararMensajes(double[] ar, string path, string filename)
        {
            string d = path + @"\" + filename + ".txt";
            string a = path + @"\Cat10-" + filename + ".txt";
            string b = path + @"\Cat20-" + filename + ".txt";
            string c = path + @"\Cat21-" + filename + ".txt";
            string[] archivos = new string[4];
            archivos[0] = d;
            archivos[1] = a;
            archivos[2] = b;
            archivos[3] = c;
            if (!File.Exists(path))
            {
                using (StreamWriter sa = File.CreateText(a))
                using (StreamWriter sb = File.CreateText(b))
                using (StreamWriter sc = File.CreateText(c))
                using (StreamWriter sd = File.CreateText(d))
                {
                    int pos = 0;
                    int length = (int)ar.Length;
                    int len = Convert.ToInt32(ar[2]);
                    int cat = Convert.ToInt32(ar[0]);
                    while ((pos + 1) < length)
                    {
                        if (pos == len)
                        {
                            cat = Convert.ToInt32(ar[len]);
                            len = len + Convert.ToInt32(ar[pos + 2]);
                        }
                        string line = null;
                        while (pos < len)
                        {
                            if (pos == len - 1)
                            {
                                line = line + Convert.ToString(ar[pos]);
                            }
                            else
                            {
                                line = line + Convert.ToString(ar[pos] + " ");
                            }
                            pos++;
                        }
                        if (cat == 10)
                        {
                            sa.WriteLine(line);
                        }
                        else if (cat == 20)
                        {
                            sb.WriteLine(line);
                        }
                        else if (cat == 21)
                        {
                            sc.WriteLine(line);
                        }
                        sd.WriteLine(line);
                    }
                }

                FileInfo fa = new FileInfo(a);
                FileInfo fb = new FileInfo(b);
                FileInfo fc = new FileInfo(c);
                if (fa.Length == 0)
                {
                    File.Delete(a);
                }
                if (fb.Length == 0)
                {
                    File.Delete(b);
                }
                if (fc.Length == 0)
                {
                    File.Delete(c);
                }
            }
            return archivos;
        }

        public int ReadCat10(List<double[]> msgcat10_T, List<string[]> FSPEC_T) {
            int a = 0;
            while (a< msgcat10_T.Count)
            {
                string FSPEC_1 = FSPEC_T[a][0];
                double[] msgcat10 = msgcat10_T[a];
                // int n = 0;
                int pos = FSPEC_T[a].Count(); // posició de byte en el missatge rebut de categoria 10 SENSE cat,lenght,Fspec.
                if(FSPEC_1[0] == 1)
                {
                    double SAC = msgcat10[pos]; // assumim que es un vector de double on cada posició és el valor decimal del byte corresponent
                    double SID = msgcat10[pos+1];
                    pos = pos + 2;
                } 
                if (FSPEC_1[1] == 1)
                {
                    double val = msgcat10[pos];
                    String MsgType = String.Empty;
                    switch (val)
                    {
                        case 1:
                            MsgType = "Target Report";
                            break;
                        case 2:
                            MsgType = "Start of Update Cycle";
                            break;
                        case 3:
                            MsgType = "Periodic Status Message";
                            break;
                        case 4:
                            MsgType = "Event-Triggered Status Message";
                            break;
                    }
                    pos += 1;
                }
                if (FSPEC_1[2] == 1)
                {
                    string va = Convert2Binary(msgcat10[pos]);
                    StringBuilder val = new StringBuilder(va[0]);
                    val.Append(va[1]);
                    val.Append(va[2]);
                    String TYP = String.Empty;
                    if (val.Equals("000")){TYP = "SSR Multilateration";}
                    else if (val.Equals("001")){TYP = "Mode S Multilateration";}
                    else if (val.Equals("010")){TYP = "ADS-B";}
                    else if (val.Equals("011")){TYP = "PSR";}
                    else if (val.Equals("100")){TYP = "Magnetic Loop System";}
                    else if (val.Equals("101")){TYP = "HF Multilateration";}
                    else if (val.Equals("110")){TYP = "Not Defined";}
                    else if (val.Equals("111")){TYP = "Other types";}

                    string DCR = String.Empty;
                    if (va[3].Equals("0")) { DCR = "No differential correction"; }
                    else { DCR = "Differential correction"; }

                    string CHN = String.Empty;
                    if (va[4].Equals("0")) { CHN = "Chain 1"; }
                    //else { CHN = "Revisar ELSE"; }

                    string GBS = String.Empty;
                    if (va[5].Equals("0")) { GBS = "Transponder Ground Bit Not Set"; }
                    else { GBS = "Transponder Ground Bit Set"; }

                    string CRT = String.Empty;
                    if (va[6].Equals("0")) { CRT = "No Corrupted Reply in Multilateration"; }
                    else { CRT = "Corrupted Replies in Multilateration"; }
                    
                    pos += 1;

                    if (va[7].Equals("1")) 
                    {
                        string va2 = Convert2Binary(msgcat10[pos]);

                        string SIM = String.Empty;
                        if (va2[0].Equals("0")) { SIM = "Actual Target Report"; }
                        else { SIM = "Simulated Target Report"; }

                        string TST = String.Empty;
                        if (va2[1].Equals("0")) { TST = "Default"; }
                        else { TST = "Test Target"; }

                        string RAB = String.Empty;
                        if (va2[2].Equals("0")) { RAB = "Report from Target Responder"; }
                        else { TST = "Report From Field Monitor (fixed transpoder)"; }

                        StringBuilder val2 = new StringBuilder(va2[3]);
                        val2.Append(va2[4]);
                        string LOP = String.Empty;
                        if (val2.Equals("00")) { LOP = "Undetermined"; }
                        else if (val2.Equals("01")) { LOP = "Loop Start"; }
                        else if (val2.Equals("10")) { LOP = "Loop Finish"; }

                        StringBuilder val3 = new StringBuilder(va2[5]);
                        val3.Append(va2[6]);
                        string TOT = String.Empty;
                        if (val3.Equals("00")) { TOT = "Undetermined"; }
                        else if (val3.Equals("01")) { TOT = "Aircraft"; }
                        else if (val3.Equals("10")) { TOT = "Ground Vehicle"; }
                        else if (val3.Equals("11")) { TOT = "Helicopter"; }

                        pos += 1;

                        if(va2[7].Equals("1")) 
                        {
                            string va3 = Convert2Binary(msgcat10[pos]);

                            string SIP = String.Empty;
                            if (va3[0].Equals("0")) { SIP = "Absence of SPI"; }
                            else { SIP = "Special Position Identification"; }
                            pos += 1;
                        }
                    }


                }
                if (FSPEC_1[3] == 1) //FRN = 4
                {
                    string a1 = Convert2Binary(msgcat10[pos]);
                    string a2 = Convert2Binary(msgcat10[pos + 1]);
                    string a3 = Convert2Binary(msgcat10[pos + 2]);
                    StringBuilder hour = new StringBuilder(a1);
                    hour.Append(a2);
                    hour.Append(a3);
                    string hour_in_seconds = hour.ToString();
                    int Hour = (int)Convert.ToInt64(hour_in_seconds, 2);
                    Hour /= 128;
                    TimeSpan TimeOfDay = TimeSpan.FromSeconds(Hour); // hh:mm:ss
                    pos += 3;
                }
                if (FSPEC_1[4] == 1) { pos += 8; } // we all gon'die // FRN = 5
                if (FSPEC_1[5] == 1) // FRN = 6
                {
                    string rho1 = Convert2Binary(msgcat10[pos]);
                    string rho2 = Convert2Binary(msgcat10[pos + 1]);
                    StringBuilder rho_BIN = new StringBuilder(rho1);
                    rho_BIN.Append(rho2);
                    string rho_BIN_TOTAL = rho_BIN.ToString();
                    double rho = (int)Convert.ToInt64(rho_BIN_TOTAL, 10); // in m
                    string theta1 = Convert2Binary(msgcat10[pos + 2]);
                    string theta2 = Convert2Binary(msgcat10[pos + 3]);
                    StringBuilder theta_BIN = new StringBuilder(theta1);
                    theta_BIN.Append(theta2);
                    string theta_BIN_TOTAL = theta_BIN.ToString();
                    double theta = ((int)Convert.ToInt64(theta_BIN_TOTAL, 10)) * 360 / 2 ^ 16; // in degrees
                    pos += 4;
                }
                if (FSPEC_1[6] == 1) // FRN = 7
                {
                    string x1 = Convert2Binary(msgcat10[pos]);
                    string x2 = Convert2Binary(msgcat10[pos + 1]);
                    StringBuilder x_BIN = new StringBuilder(x1);
                    x_BIN.Append(x2);
                    string x_BIN_TOTAL = x_BIN.ToString();
                    double x = (int)Convert.ToInt64(x_BIN_TOTAL, 10); // in m
                    string y1 = Convert2Binary(msgcat10[pos + 2]);
                    string y2 = Convert2Binary(msgcat10[pos + 3]);
                    StringBuilder y_BIN = new StringBuilder(y1);
                    y_BIN.Append(y2);
                    string y_BIN_TOTAL = y_BIN.ToString();
                    double y = ((int)Convert.ToInt64(y_BIN_TOTAL, 10)); // in degrees
                    pos += 4;
                }
                if (FSPEC_1[7] == 0) { }
                else {
                    string FSPEC_2 = FSPEC_T[a][1];
                    if (FSPEC_2[0] == 1) // FRN = 8
                    {
                        string ground_speed1 = Convert2Binary(msgcat10[pos]);
                        string ground_speed2 = Convert2Binary(msgcat10[pos + 1]);
                        StringBuilder ground_speed_BIN = new StringBuilder(ground_speed1);
                        ground_speed_BIN.Append(ground_speed2);
                        string ground_speed_BIN_TOTAL = ground_speed_BIN.ToString();
                        double ground_speed = ((int)Convert.ToInt64(ground_speed_BIN_TOTAL, 10))*0.22; // in m
                        string track_angle1 = Convert2Binary(msgcat10[pos + 2]);
                        string track_angle2 = Convert2Binary(msgcat10[pos + 3]);
                        StringBuilder track_angle_BIN = new StringBuilder(track_angle1);
                        track_angle_BIN.Append(track_angle2);
                        string track_angle_BIN_TOTAL = track_angle_BIN.ToString();
                        double track_angle = ((int)Convert.ToInt64(track_angle_BIN_TOTAL, 10))*360/2^16; // in degrees
                        pos += 4;
                        }
                    if (FSPEC_2[1] == 1) // FRN = 9
                    {
                        string Vx1 = Convert2Binary(msgcat10[pos]);
                        string Vx2 = Convert2Binary(msgcat10[pos + 1]);
                        StringBuilder Vx_BIN = new StringBuilder(Vx1);
                        Vx_BIN.Append(Vx2);
                        string Vx_BIN_TOTAL = Vx_BIN.ToString();
                        double Vx = (int)Convert.ToInt64(Vx_BIN_TOTAL, 10); // in m
                        string Vy1 = Convert2Binary(msgcat10[pos + 2]);
                        string Vy2 = Convert2Binary(msgcat10[pos + 3]);
                        StringBuilder Vy_BIN = new StringBuilder(Vy1);
                        Vy_BIN.Append(Vy2);
                        string Vy_BIN_TOTAL = Vy_BIN.ToString();
                        double Vy = ((int)Convert.ToInt64(Vy_BIN_TOTAL, 10)); // in degrees
                        pos += 4;
                    }
                    if (FSPEC_2[2] == 1) // FRN = 10; Track Number
                    {
                        string frn_1 = Convert2Binary(msgcat10[pos]);
                        string frn_2 = Convert2Binary(msgcat10[pos+1]);
                        // cal concatenar els 2 B, eliminar els 4 bits primers i agafar la resta de bits en decimal per a track number
                        pos += 2;
                        
                    }

                }
            }

         return 0;
        }
        
        public string Convert2Binary(double input)
        {
            String input_s = input.ToString();
            String output = Convert.ToInt32(input_s, 2).ToString();
            // Cal crear una funció que posi cada bit a una posició de l'string
            return output;
        }

        public DataTable ReadCAT10_old(DataTable CAT10, string filename)
        {
            DataTable FieldsCAT10 = new DataTable();
            CAT10 Mensajes10 = new CAT10();
            FieldsCAT10.Columns.Add("SAC", typeof(int));
            FieldsCAT10.Columns.Add("SIC", typeof(int));
            FieldsCAT10.Columns.Add("MessageType", typeof(string));
            FieldsCAT10.Columns.Add("TargetReportDescriptor", typeof(string));
            FieldsCAT10.Columns.Add("TimeUTC", typeof(TimeSpan));
            FieldsCAT10.Columns.Add("Rho [m]", typeof(string));
            FieldsCAT10.Columns.Add("Theta[º]", typeof(string));
            FieldsCAT10.Columns.Add("X [m]", typeof(string));
            FieldsCAT10.Columns.Add("Y [m]", typeof(string));
            FieldsCAT10.Columns.Add("Ground Speed [Km/h])", typeof(string));
            FieldsCAT10.Columns.Add("Track Angle [º]", typeof(string));
            FieldsCAT10.Columns.Add("Vx [m/s]", typeof(string));
            FieldsCAT10.Columns.Add("Vy [m/s]", typeof(string));
            FieldsCAT10.Columns.Add("Track Number", typeof(string));
            FieldsCAT10.Columns.Add("Track Status", typeof(string));
            FieldsCAT10.Columns.Add("Mode-3/A Code in Octal Representation", typeof(string));
            FieldsCAT10.Columns.Add("Target Address", typeof(string));
            FieldsCAT10.Columns.Add("Target Identification", typeof(string));
            FieldsCAT10.Columns.Add("Flight Level", typeof(string));
            FieldsCAT10.Columns.Add("Target Size and Orientation", typeof(string));
            FieldsCAT10.Columns.Add("Acceleration (Ax)", typeof(string));
            FieldsCAT10.Columns.Add("Acceleration (Ay)", typeof(string));
            string[] values;
            int Length = CAT10.Rows.Count;
            int i = 0; //para recorrer la tabla
            System.IO.StreamReader file = new System.IO.StreamReader(filename);

            string[] Blanco; //para la linea del fichero una vez separada
            while (i < Length) //RECORREMOS LA TABLA
            {
                string pos = CAT10.Rows[i]["LastPositionFSPEC"].ToString(); //posicion dentro de la linea del fichero
                int posicion = Int32.Parse(pos);
                string line = file.ReadLine();
                Blanco = line.Split(' ');
                string Fields = CAT10.Rows[i]["DataFields"].ToString();
                values = Fields.Split(' '); //els datafields presents en el misaatge
                int f = 0; //para recorrer los DataFields
                while (f < values.Length)
                {
                    if (values[f] == "1")
                    {
                        Mensajes10.SAC = Mensajes10.GetSAC(Blanco[posicion]);
                        posicion++;
                        Mensajes10.SIC = Mensajes10.GetSIC(Blanco[posicion]);
                        posicion++;
                        f = f + 1;
                    }
                    if (f < values.Length && values[f] == "2")
                    {
                        Mensajes10.MessageType = Mensajes10.GetMessageType(Blanco[posicion]);
                        posicion++;
                        f++;
                    }
                    else
                    {
                        Mensajes10.MessageType = null;
                        //posicion++;
                        //f++;
                    }
                    if (f < values.Length && values[f] == "3")
                    {
                        int F;
                        string G;
                        char[] bi;
                        char[] bin;
                        char[] bitsTotals = new char[0];
                        bool terminado = false;
                        while (terminado == false)
                        {
                            F = Convert.ToInt32(Blanco[posicion]);
                            G = Convert.ToString(F, 2);
                            bi = G.ToCharArray(); //el numero binari original/(potser no arriba als 8 bits)
                            char[] zeros = new char[8 - G.Length]; //zeros a l'esquerra
                            bin = bi;
                            int z = 0; //el numeros de zeros que haig d'afegir
                            while (z < zeros.Length)
                            {
                                zeros[z] = '0';
                                z++;
                            }
                            bin = zeros.Concat(bin).ToArray();
                            if (bin[bin.Length - 1] == '0')
                            {
                                terminado = true;
                            }
                            posicion++;
                            bitsTotals = bitsTotals.Concat(bin).ToArray();
                        }
                        Mensajes10.TargetReportDescriptor = Mensajes10.GetTargetReportDescriptor(bitsTotals);
                        f++;
                    }
                    else
                    {
                        Mensajes10.TargetReportDescriptor = " ";
                        //posicion++;
                        //f++;
                    }
                    if (f < values.Length && values[f] == "4")
                    {
                        //int m = 0;
                        int Oct1 = Convert.ToInt32(Blanco[posicion]);
                        string Oct1St = Convert.ToString(Oct1, 2);
                        while (Oct1St.Length < 8)
                        {
                            Oct1St = "0" + Oct1St;

                        }
                        int Oct2 = Convert.ToInt32(Blanco[posicion + 1]);
                        string Oct2St = Convert.ToString(Oct2, 2);
                        while (Oct2St.Length < 8)
                        {
                            Oct2St = "0" + Oct2St;

                        }
                        int Oct3 = Convert.ToInt32(Blanco[posicion + 2]);
                        string Oct3St = Convert.ToString(Oct3, 2);
                        while (Oct3St.Length < 8)
                        {
                            Oct3St = "0" + Oct3St;

                        }
                        string OctetsTime = Oct1St + Oct2St + Oct3St;
                        int b = Convert.ToInt32(OctetsTime, 2);//converteixo el binari a decimal
                        double seconds = ((double)b / (double)128);
                        Mensajes10.TimeUTC = Mensajes10.GetTimeOfDay(seconds);
                        posicion = posicion + 3;
                        f++;
                    }
                    if (f < values.Length && values[f] == "6")
                    {
                        int Oct1 = Convert.ToInt32(Blanco[posicion]);
                        string Oct1St = Convert.ToString(Oct1, 2);
                        while (Oct1St.Length < 8)
                        {
                            Oct1St = "0" + Oct1St;

                        }
                        int Oct2 = Convert.ToInt32(Blanco[posicion + 1]);
                        string Oct2St = Convert.ToString(Oct2, 2);
                        while (Oct2St.Length < 8)
                        {
                            Oct2St = "0" + Oct2St;

                        }
                        string OctetsRho = Oct1St + Oct2St;
                        int Rho = Convert.ToInt32(OctetsRho, 2);
                        int Oct3 = Convert.ToInt32(Blanco[posicion + 2]);
                        string Oct3St = Convert.ToString(Oct3, 2);
                        while (Oct3St.Length < 8)
                        {
                            Oct3St = "0" + Oct3St;

                        }
                        int Oct4 = Convert.ToInt32(Blanco[posicion + 3]);
                        string Oct4St = Convert.ToString(Oct4, 2);
                        while (Oct4St.Length < 8)
                        {
                            Oct4St = "0" + Oct4St;

                        }
                        string OctetsTheta = Oct3St + Oct4St;
                        int Theta = Convert.ToInt32(OctetsTheta, 2);
                        Mensajes10.PolarPosition = Mensajes10.GetPolarCoordinates(Rho, Theta);
                        posicion = posicion + 4;
                        f++;

                    }
                    else
                    {
                        string[] PolarCoordinates = new string[2];
                        PolarCoordinates[0] = " ";
                        PolarCoordinates[1] = " ";
                        Mensajes10.PolarPosition = PolarCoordinates;

                    }
                    if (f < values.Length && values[f] == "7")
                    {
                        int Oct1 = Convert.ToInt32(Blanco[posicion]);
                        string Oct1St = Convert.ToString(Oct1, 2);
                        while (Oct1St.Length < 8)
                        {
                            Oct1St = "0" + Oct1St;

                        }
                        int Oct2 = Convert.ToInt32(Blanco[posicion + 1]);
                        string Oct2St = Convert.ToString(Oct2, 2);
                        while (Oct2St.Length < 8)
                        {
                            Oct2St = "0" + Oct2St;

                        }
                        string OctetsX = Oct1St + Oct2St;
                        int X = Convert.ToInt32(OctetsX, 2);
                        int Oct3 = Convert.ToInt32(Blanco[posicion + 2]);
                        string Oct3St = Convert.ToString(Oct3, 2);
                        while (Oct3St.Length < 8)
                        {
                            Oct3St = "0" + Oct3St;

                        }
                        int Oct4 = Convert.ToInt32(Blanco[posicion + 3]);
                        string Oct4St = Convert.ToString(Oct4, 2);
                        while (Oct4St.Length < 8)
                        {
                            Oct4St = "0" + Oct4St;

                        }
                        string OctetsY = Oct3St + Oct4St;
                        int Y = Convert.ToInt32(OctetsY, 2);
                        Mensajes10.CartesianCoordinates = Mensajes10.GetCartesianCoordinates(X, Y);

                        posicion = posicion + 4;
                        f++;
                    }
                    else
                    {
                        string[] CartCoordinates = new string[2];
                        CartCoordinates[0] = "N/A";
                        CartCoordinates[1] = "N/A";
                        Mensajes10.CartesianCoordinates = CartCoordinates;

                    }
                    if (f < values.Length && values[f] == "8")
                    {
                        int Oct1 = Convert.ToInt32(Blanco[posicion]);
                        string Oct1St = Convert.ToString(Oct1, 2);
                        while (Oct1St.Length < 8)
                        {
                            Oct1St = "0" + Oct1St;

                        }
                        int Oct2 = Convert.ToInt32(Blanco[posicion + 1]);
                        string Oct2St = Convert.ToString(Oct2, 2);
                        while (Oct2St.Length < 8)
                        {
                            Oct2St = "0" + Oct2St;

                        }
                        string OctetsGS = Oct1St + Oct2St;
                        int GS = Convert.ToInt32(OctetsGS, 2);
                        int Oct3 = Convert.ToInt32(Blanco[posicion + 2]);
                        string Oct3St = Convert.ToString(Oct3, 2);
                        while (Oct3St.Length < 8)
                        {
                            Oct3St = "0" + Oct3St;

                        }
                        int Oct4 = Convert.ToInt32(Blanco[posicion + 3]);
                        string Oct4St = Convert.ToString(Oct4, 2);
                        while (Oct4St.Length < 8)
                        {
                            Oct4St = "0" + Oct4St;

                        }
                        string OctetsTA = Oct3St + Oct4St;
                        int TA = Convert.ToInt32(OctetsTA, 2);
                        Mensajes10.PolarTrackVelocity = Mensajes10.GetPolarTrackVelocity(GS, TA);
                        posicion = posicion + 4;
                        f++;
                    }
                    else
                    {
                        string[] PolarTrackVelocity = new string[2];
                        PolarTrackVelocity[0] = " ";
                        PolarTrackVelocity[1] = " ";
                        Mensajes10.PolarTrackVelocity = PolarTrackVelocity;

                    }
                    if (f < values.Length && values[f] == "9")
                    {
                        int Oct1 = Convert.ToInt32(Blanco[posicion]);
                        string Oct1St = Convert.ToString(Oct1, 2);
                        while (Oct1St.Length < 8)
                        {
                            Oct1St = "0" + Oct1St;

                        }
                        int Oct2 = Convert.ToInt32(Blanco[posicion + 1]);
                        string Oct2St = Convert.ToString(Oct2, 2);
                        while (Oct2St.Length < 8)
                        {
                            Oct2St = "0" + Oct2St;

                        }
                        string OctetsVx = Oct1St + Oct2St;
                        int Vx = Convert.ToInt32(OctetsVx, 2);
                        int Oct3 = Convert.ToInt32(Blanco[posicion + 2]);
                        string Oct3St = Convert.ToString(Oct3, 2);
                        while (Oct3St.Length < 8)
                        {
                            Oct3St = "0" + Oct3St;

                        }
                        int Oct4 = Convert.ToInt32(Blanco[posicion + 3]);
                        string Oct4St = Convert.ToString(Oct4, 2);
                        while (Oct4St.Length < 8)
                        {
                            Oct4St = "0" + Oct4St;

                        }
                        string OctetsVy = Oct3St + Oct4St;
                        int Vy = Convert.ToInt32(OctetsVy, 2);
                        posicion = posicion + 4;
                        f++;
                        Mensajes10.CartesianTrackVelocity = Mensajes10.GetCartesianTrackVelocity(Vx, Vy);
                    }
                    else
                    {
                        string[] CartesianVelocity = new string[2];
                        CartesianVelocity[0] = " ";
                        CartesianVelocity[1] = " ";
                        Mensajes10.CartesianTrackVelocity = CartesianVelocity;
                    }

                    if (f < values.Length && values[f] == "10")
                    {
                        int Oct1 = Convert.ToInt32(Blanco[posicion]);
                        string Oct1St = Convert.ToString(Oct1, 2);
                        while (Oct1St.Length < 8)
                        {
                            Oct1St = "0" + Oct1St;

                        }
                        int Oct2 = Convert.ToInt32(Blanco[posicion + 1]);
                        string Oct2St = Convert.ToString(Oct2, 2);
                        while (Oct2St.Length < 8)
                        {
                            Oct2St = "0" + Oct2St;

                        }
                        string OctetsTN = Oct1St.Substring(4, 4) + Oct2St;
                        int TrackNumber = Convert.ToInt32(OctetsTN, 2);
                        Mensajes10.TrackNumber = Mensajes10.GetTrackNumber(TrackNumber);
                        posicion = posicion + 2;
                        f++;
                    }
                    else
                    {
                        Mensajes10.TrackNumber = " ";
                    }
                    if (f < values.Length && values[f] == "11")
                    {

                        int Oct1 = Convert.ToInt32(Blanco[posicion]);
                        string Oct1St = Convert.ToString(Oct1, 2);
                        while (Oct1St.Length < 8)
                        {
                            Oct1St = "0" + Oct1St;

                        }
                        string OctTotal = Oct1St;
                        posicion++;
                        if (Oct1St.Substring(7) == "1")
                        {
                            int Oct2 = Convert.ToInt32(Blanco[posicion]);
                            string Oct2St = Convert.ToString(Oct2, 2);
                            while (Oct2St.Length < 8)
                            {
                                Oct2St = "0" + Oct2St;
                            }
                            OctTotal = Oct1St + Oct2St;
                            posicion++;
                            if (Oct2St.Substring(7) == "1")
                            {
                                int Oct3 = Convert.ToInt32(Blanco[posicion]);
                                string Oct3St = Convert.ToString(Oct3, 2);
                                while (Oct3St.Length < 8)
                                {
                                    Oct3St = "0" + Oct3St;
                                }
                                OctTotal = OctTotal + Oct3St;
                                posicion++;
                            }
                        }
                        f++;
                        Mensajes10.TrackStatus = Mensajes10.GetTrackStatus(OctTotal);
                    }
                    else
                    {
                        Mensajes10.TrackStatus = " ";
                    }
                    if (f < values.Length && values[f] == "12")
                    {
                        int Oct1 = Convert.ToInt32(Blanco[posicion]);
                        string Oct1St = Convert.ToString(Oct1, 2);
                        while (Oct1St.Length < 8)
                        {
                            Oct1St = "0" + Oct1St;

                        }
                        int Oct2 = Convert.ToInt32(Blanco[posicion + 1]);
                        string Oct2St = Convert.ToString(Oct2, 2);
                        while (Oct2St.Length < 8)
                        {
                            Oct2St = "0" + Oct2St;
                        }
                        string OctetsMode3A = Oct1St + Oct2St;
                        Mensajes10.Mode3A = Mensajes10.GetMode3A(OctetsMode3A);
                        posicion = posicion + 2;
                        f++;
                    }
                    else
                    {
                        Mensajes10.Mode3A = " ";
                    }
                    if (f < values.Length && values[f] == "13")
                    {
                        int Oct1 = Convert.ToInt32(Blanco[posicion]);
                        string Oct1St = Convert.ToString(Oct1, 2);
                        while (Oct1St.Length < 8)
                        {
                            Oct1St = "0" + Oct1St;

                        }
                        int Oct2 = Convert.ToInt32(Blanco[posicion + 1]);
                        string Oct2St = Convert.ToString(Oct2, 2);
                        while (Oct2St.Length < 8)
                        {
                            Oct2St = "0" + Oct2St;
                        }
                        int Oct3 = Convert.ToInt32(Blanco[posicion + 2]);
                        string Oct3St = Convert.ToString(Oct3, 2);
                        while (Oct3St.Length < 8)
                        {
                            Oct3St = "0" + Oct3St;
                        }
                        string OctetsTargetAddress = Oct1St + Oct2St + Oct3St;
                        Mensajes10.TargetAddress = Mensajes10.GetTargetAddress(OctetsTargetAddress);
                        posicion = posicion + 3;
                        f++;
                    }
                    else
                    {
                        Mensajes10.TargetAddress = " ";
                    }
                    if (f < values.Length && values[f] == "14")
                    {
                        int Oct1 = Convert.ToInt32(Blanco[posicion]);
                        string Oct1St = Convert.ToString(Oct1, 2);
                        while (Oct1St.Length < 8)
                        {
                            Oct1St = "0" + Oct1St;
                        }
                        int Oct2 = Convert.ToInt32(Blanco[posicion + 1]);
                        string Oct2St = Convert.ToString(Oct2, 2);
                        while (Oct2St.Length < 8)
                        {
                            Oct2St = "0" + Oct2St;
                        }
                        int Oct3 = Convert.ToInt32(Blanco[posicion + 2]);
                        string Oct3St = Convert.ToString(Oct3, 2);
                        while (Oct3St.Length < 8)
                        {
                            Oct3St = "0" + Oct3St;
                        }
                        int Oct4 = Convert.ToInt32(Blanco[posicion + 3]);
                        string Oct4St = Convert.ToString(Oct4, 2);
                        while (Oct4St.Length < 8)
                        {
                            Oct4St = "0" + Oct4St;
                        }
                        int Oct5 = Convert.ToInt32(Blanco[posicion + 4]);
                        string Oct5St = Convert.ToString(Oct5, 2);
                        while (Oct5St.Length < 8)
                        {
                            Oct5St = "0" + Oct5St;
                        }
                        int Oct6 = Convert.ToInt32(Blanco[posicion + 5]);
                        string Oct6St = Convert.ToString(Oct6, 2);
                        while (Oct6St.Length < 8)
                        {
                            Oct6St = "0" + Oct6St;
                        }
                        int Oct7 = Convert.ToInt32(Blanco[posicion + 6]);
                        string Oct7St = Convert.ToString(Oct7, 2);
                        while (Oct7St.Length < 8)
                        {
                            Oct7St = "0" + Oct7St;
                        }
                        string OctetsTotal = Oct1St + Oct2St + Oct3St + Oct4St + Oct5St + Oct6St + Oct7St;
                        Mensajes10.TargetIdentification = Mensajes10.GetTargetIdentification(OctetsTotal);
                        posicion = posicion + 7;
                        f++;
                    }
                    else
                    {
                        Mensajes10.TargetIdentification = " ";
                    }
                    if (f < values.Length && values[f] == "17")
                    {
                        int Oct1 = Convert.ToInt32(Blanco[posicion]);
                        string Oct1St = Convert.ToString(Oct1, 2);
                        while (Oct1St.Length < 8)
                        {
                            Oct1St = "0" + Oct1St;

                        }
                        int Oct2 = Convert.ToInt32(Blanco[posicion + 1]);
                        string Oct2St = Convert.ToString(Oct2, 2);
                        while (Oct2St.Length < 8)
                        {
                            Oct2St = "0" + Oct2St;
                        }
                        string OctetsFlightLevel = Oct1St + Oct2St;
                        Mensajes10.FlightLevel = Mensajes10.GetFlightLevel(OctetsFlightLevel);
                        posicion = posicion + 2;
                        f++;
                    }
                    else
                    {
                        Mensajes10.FlightLevel = " ";
                    }
                    if (f < values.Length && values[f] == "19")
                    {
                        int Oct1 = Convert.ToInt32(Blanco[posicion]);
                        string Oct1St = Convert.ToString(Oct1, 2);
                        while (Oct1St.Length < 8)
                        {
                            Oct1St = "0" + Oct1St;

                        }
                        string OctTotal = Oct1St;
                        posicion++;
                        if (Oct1St.Substring(7) == "1")
                        {
                            int Oct2 = Convert.ToInt32(Blanco[posicion]);
                            string Oct2St = Convert.ToString(Oct2, 2);
                            while (Oct2St.Length < 8)
                            {
                                Oct2St = "0" + Oct2St;
                            }
                            OctTotal = Oct1St + Oct2St;
                            posicion++;
                            if (Oct2St.Substring(7) == "1")
                            {
                                int Oct3 = Convert.ToInt32(Blanco[posicion]);
                                string Oct3St = Convert.ToString(Oct3, 2);
                                while (Oct3St.Length < 8)
                                {
                                    Oct3St = "0" + Oct3St;
                                }
                                OctTotal = OctTotal + Oct3St;
                                posicion++;
                            }
                        }
                        f++;
                        Mensajes10.TargetSizeAndOrientation = Mensajes10.GetTargetSizeAndOrientation(OctTotal);
                    }
                    else
                    {
                        Mensajes10.TargetSizeAndOrientation = " ";
                    }
                    if (f < values.Length && values[f] == "25")
                    {
                        int Oct1 = Convert.ToInt32(Blanco[posicion]);
                        string Oct1St = Convert.ToString(Oct1, 2);
                        while (Oct1St.Length < 8)
                        {
                            Oct1St = "0" + Oct1St;

                        }
                        int Oct2 = Convert.ToInt32(Blanco[posicion + 1]);
                        string Oct2St = Convert.ToString(Oct2, 2);
                        while (Oct2St.Length < 8)
                        {
                            Oct2St = "0" + Oct2St;
                        }
                        string OctetsAcceleration = Oct1St + Oct2St;
                        Mensajes10.CalculatedAcceleration = Mensajes10.GetCalculatedAcceleration(OctetsAcceleration);
                        posicion = posicion + 2;
                        f++;
                    }
                    else
                    {
                        string[] Acceleration = new string[2];
                        Acceleration[0] = " ";
                        Acceleration[1] = " ";
                        Mensajes10.CalculatedAcceleration = Acceleration;
                    }
                    f = values.Length;


                }

                if (Mensajes10.MessageType != "Periodic Status Message" && Mensajes10.MessageType != "State of Update Cycle")
                {
                    FieldsCAT10.Rows.Add(Mensajes10.SAC, Mensajes10.SIC, Mensajes10.MessageType, Mensajes10.TargetReportDescriptor, Mensajes10.TimeUTC,
                    Mensajes10.PolarPosition[0], Mensajes10.PolarPosition[1], Mensajes10.CartesianCoordinates[0], Mensajes10.CartesianCoordinates[1],
                    Mensajes10.PolarTrackVelocity[0], Mensajes10.PolarTrackVelocity[1], Mensajes10.CartesianTrackVelocity[0], Mensajes10.CartesianTrackVelocity[1],
                    Mensajes10.TrackNumber, Mensajes10.TrackStatus, Mensajes10.Mode3A, Mensajes10.TargetAddress, Mensajes10.TargetIdentification,
                    Mensajes10.FlightLevel, Mensajes10.TargetSizeAndOrientation, Mensajes10.CalculatedAcceleration[0], Mensajes10.CalculatedAcceleration[1]);

                }
                i++;
            }
            return FieldsCAT10;

        }

        public string ConvertToBite(string c)
        {
            int n;
            n = Convert.ToInt32(c);
            int[] a = new int[8];
            string b = null;
            for (int i = 0; i <= 7; i++)
            {
                a[7 - i] = n % 2;
                n = n / 2;
            }
            for (int i = 0; i <= 7; i++)
            {
                b = b + Convert.ToString(a[i]);
            }
            return b;
        }

        public DataTable ReadCAT20(DataTable CAT20, string filename)
        {
            DataTable FieldsCAT20 = new DataTable();
            FieldsCAT20.Columns.Add("SAC", typeof(int));
            FieldsCAT20.Columns.Add("SIC", typeof(int));
            FieldsCAT20.Columns.Add("Target Report Descriptor", typeof(string));
            FieldsCAT20.Columns.Add("Time of Day", typeof(TimeSpan));
            FieldsCAT20.Columns.Add("Position X [m]", typeof(double));
            FieldsCAT20.Columns.Add("Position Y [m]", typeof(double));
            FieldsCAT20.Columns.Add("Track Number", typeof(int));
            FieldsCAT20.Columns.Add("Track Status", typeof(string));
            FieldsCAT20.Columns.Add("Mode-3/A", typeof(int));
            FieldsCAT20.Columns.Add("Mode-3/A Info.", typeof(string));
            FieldsCAT20.Columns.Add("Vx [m/s]", typeof(double));
            FieldsCAT20.Columns.Add("Vy [m/s]", typeof(double));
            FieldsCAT20.Columns.Add("Flight Level", typeof(double));
            FieldsCAT20.Columns.Add("Flight Level Info.", typeof(string));
            FieldsCAT20.Columns.Add("Target Adress", typeof(string));
            FieldsCAT20.Columns.Add("Target Identification", typeof(string));
            FieldsCAT20.Columns.Add("Target Identification Info.", typeof(string));
            FieldsCAT20.Columns.Add("DOP-x [m]", typeof(double));
            FieldsCAT20.Columns.Add("DOP-y [m]", typeof(double));
            FieldsCAT20.Columns.Add("DOP-xy [m]", typeof(double));
            FieldsCAT20.Columns.Add("Standard Deviation of X", typeof(int));
            FieldsCAT20.Columns.Add("Standard Deviation of Y", typeof(int));
            FieldsCAT20.Columns.Add("Correlation coefficient", typeof(int));
            FieldsCAT20.Columns.Add("Standard Deviation of Geometric Height", typeof(int));
            FieldsCAT20.Columns.Add("Contributing Devices", typeof(string));


            StreamReader file = new StreamReader(filename);

            for (int i = 0; i < CAT20.Rows.Count; i++)
            {
                string a = CAT20.Rows[i]["DataFields"].ToString();
                string[] DataItems = a.Split(' ');
                int n = Convert.ToInt32(CAT20.Rows[i]["LastPositionFSPEC"].ToString());
                string line = file.ReadLine();
                string[] Values = line.Split(' ');
                DataRow rowFieldsCAT20 = FieldsCAT20.NewRow();
                int c = 0;
                if (Convert.ToInt32(DataItems[c]) == 1)
                {
                    rowFieldsCAT20["SAC"] = Convert.ToInt32(Values[n]);
                    rowFieldsCAT20["SIC"] = Convert.ToInt32(Values[n + 1]);
                    n = n + 2; //Posición del octeto dentro el txt
                    c = c + 1; //Posición de los dataitems dentro del datagreed "Dataitems"
                }
                if (Convert.ToInt32(DataItems[c]) == 2)
                {
                    string TDRString;
                    string TRDBiteString = ConvertToBite(Values[n]);
                    char[] TRDbite = TRDBiteString.ToCharArray();
                    n = n + 1;

                    if (Char.GetNumericValue(TRDbite[0]) == 1)
                    {
                        TDRString = "Non-Mode S 1090MHz multilateration - ";
                    }
                    else
                    {
                        TDRString = "No Non-Mode S 1090MHz multilat - ";
                    }
                    if (Char.GetNumericValue(TRDbite[1]) == 1)
                    {
                        TDRString = TDRString + "Mode-S 1090 MHz multilateration - ";
                    }
                    else
                    {
                        TDRString = TDRString + "No Mode-S 1090 MHz multilateration - ";
                    }
                    if (Char.GetNumericValue(TRDbite[2]) == 1)
                    {
                        TDRString = TDRString + "HF multilateration - ";
                    }
                    else
                    {
                        TDRString = TDRString + "No HF multilateration - ";
                    }
                    if (Char.GetNumericValue(TRDbite[3]) == 1)
                    {
                        TDRString = TDRString + "VDL Mode 4 multilateration - ";
                    }
                    else
                    {
                        TDRString = TDRString + "No VDL Mode 4 multilateration - ";
                    }
                    if (Char.GetNumericValue(TRDbite[4]) == 1)
                    {
                        TDRString = TDRString + "UAT multilateration - ";
                    }
                    else
                    {
                        TDRString = TDRString + "No UAT multilateration - ";
                    }
                    if (Char.GetNumericValue(TRDbite[5]) == 1)
                    {
                        TDRString = TDRString + "DME/TACAN multilateration - ";
                    }
                    else
                    {
                        TDRString = TDRString + "No DME/TACAN multilateration - ";
                    }
                    if (Char.GetNumericValue(TRDbite[6]) == 1)
                    {
                        TDRString = TDRString + "Other Technology Multilateration. ";
                    }
                    else
                    {
                        TDRString = TDRString + "No Other Technology Multilateration. ";
                    }
                    if (Char.GetNumericValue(TRDbite[7]) == 1)
                    {
                        string TRDBiteString2 = ConvertToBite(Values[n]);
                        char[] TRDbite2 = TRDBiteString2.ToCharArray();
                        n = n + 1;
                        if (Char.GetNumericValue(TRDbite2[0]) == 1)
                        {
                            TDRString = TDRString + "Report from field monitor - ";
                        }
                        else
                        {
                            TDRString = TDRString + "Report from target transponder - ";
                        }
                        if (Char.GetNumericValue(TRDbite2[1]) == 1)
                        {
                            TDRString = TDRString + "Special Position Identification - ";
                        }
                        else
                        {
                            TDRString = TDRString + "Absence of SPI - ";
                        }
                        if (Char.GetNumericValue(TRDbite2[2]) == 1)
                        {
                            TDRString = TDRString + "Chain 2 - ";
                        }
                        else
                        {
                            TDRString = TDRString + "Chain - ";
                        }
                        if (Char.GetNumericValue(TRDbite2[3]) == 1)
                        {
                            TDRString = TDRString + "Transponder Ground bit set - ";
                        }
                        else
                        {
                            TDRString = TDRString + "Transponder Ground bit not set - ";
                        }
                        if (Char.GetNumericValue(TRDbite2[4]) == 1)
                        {
                            TDRString = TDRString + "Corrupted replies in multilateration - ";
                        }
                        else
                        {
                            TDRString = TDRString + "No Corrupted reply in multilateration - ";
                        }
                        if (Char.GetNumericValue(TRDbite2[5]) == 1)
                        {
                            TDRString = TDRString + "Simulated target report - ";
                        }
                        else
                        {
                            TDRString = TDRString + "Actual target report - ";
                        }
                        if (Char.GetNumericValue(TRDbite2[6]) == 1)
                        {
                            TDRString = TDRString + "Test Target.";
                        }
                        else
                        {
                            TDRString = TDRString + "Default.";
                        }

                    }
                    rowFieldsCAT20["Target Report Descriptor"] = TDRString;
                    c = c + 1;
                }
                if (Convert.ToInt32(DataItems[c]) == 3)
                {
                    string TimeString = ConvertToBite(Values[n]);
                    TimeString = TimeString + ConvertToBite(Values[n + 1]);
                    TimeString = TimeString + ConvertToBite(Values[n + 2]);
                    int Time = Convert.ToInt32(TimeString, 2);
                    double Time2 = Time / (double)128;
                    TimeSpan time = TimeSpan.FromSeconds(Time2);
                    rowFieldsCAT20["Time of Day"] = time;
                    c = c + 1;
                    n = n + 3;
                }
                if (Convert.ToInt32(DataItems[c]) == 5)
                {
                    string XString = ConvertToBite(Values[n]);
                    XString = XString + ConvertToBite(Values[n + 1]);
                    XString = XString + ConvertToBite(Values[n + 2]);

                    string YString = ConvertToBite(Values[n + 3]);
                    YString = YString + ConvertToBite(Values[n + 4]);
                    YString = YString + ConvertToBite(Values[n + 5]);
                    int X = Convert.ToInt32(XString, 2);
                    int Y = Convert.ToInt32(YString, 2);
                    if (X > 8388600)
                    {
                        X = X - 16777216;
                    }
                    if (Y > 8388600)
                    {
                        Y = Y - 16777216;
                    }
                    rowFieldsCAT20["Position X [m]"] = X * (double)0.5;
                    rowFieldsCAT20["Position Y [m]"] = Y * (double)0.5;
                    c = c + 1;
                    n = n + 6;
                }
                if (Convert.ToInt32(DataItems[c]) == 6)
                {
                    string TNString = ConvertToBite(Values[n]);
                    string TNString1 = TNString + ConvertToBite(Values[n + 1]);
                    char[] TNSbite = TNString1.ToCharArray();
                    string TNString2 = null;
                    for (int t = 4; t < 16; t++)
                    {
                        TNString2 = TNString2 + Convert.ToString(TNSbite[t]);
                    }
                    rowFieldsCAT20["Track Number"] = Convert.ToInt32(TNString2, 2);
                    c = c + 1;
                    n = n + 2;
                }
                if (Convert.ToInt32(DataItems[c]) == 7)
                {
                    string TSString;
                    string TSBiteString = ConvertToBite(Values[n]);
                    char[] TSbite = TSBiteString.ToCharArray();
                    n = n + 1;
                    c = c + 1;
                    if (Char.GetNumericValue(TSbite[0]) == 1)
                    {
                        TSString = "Track in initation phase - ";
                    }
                    else
                    {
                        TSString = "Confirmed track - ";
                    }
                    if (Char.GetNumericValue(TSbite[1]) == 1)
                    {
                        TSString = TSString + "Last report for a track - ";
                    }
                    else
                    {
                        TSString = TSString + "TRE default - ";
                    }
                    if (Char.GetNumericValue(TSbite[2]) == 1)
                    {
                        TSString = TSString + "CST Extrapolated - ";
                    }
                    else
                    {
                        TSString = TSString + "CTS No extrapolated - ";
                    }
                    if ((Convert.ToString(TSbite[3]) + Convert.ToString(TSbite[4])).Equals("11") == true)
                    {
                        TSString = TSString + "CDM Invalid - ";
                    }
                    if ((Convert.ToString(TSbite[3]) + Convert.ToString(TSbite[4])).Equals("10") == true)
                    {
                        TSString = TSString + "CDM Descending - ";
                    }
                    if ((Convert.ToString(TSbite[3]) + Convert.ToString(TSbite[4])).Equals("01") == true)
                    {
                        TSString = TSString + "CDM Climbing - ";
                    }
                    if ((Convert.ToString(TSbite[3]) + Convert.ToString(TSbite[4])).Equals("00") == true)
                    {
                        TSString = TSString + "CDM Maintaining - ";
                    }
                    if (Char.GetNumericValue(TSbite[5]) == 1)
                    {
                        TSString = TSString + "MAH Horizontal manoeuvre - ";
                    }
                    else
                    {
                        TSString = TSString + "MAH Default - ";
                    }
                    if (Char.GetNumericValue(TSbite[6]) == 1)
                    {
                        TSString = TSString + "STH Smoothed position.";
                    }
                    else
                    {
                        TSString = TSString + "STH Mesured position.";
                    }
                    if (Char.GetNumericValue(TSbite[7]) == 1)
                    {
                        string TSBiteString2 = ConvertToBite(Values[n]);
                        char[] TSbite2 = TSBiteString2.ToCharArray();
                        n = n + 1;
                        if (Char.GetNumericValue(TSbite2[0]) == 1)
                        {
                            TSString = TSString + "GHO Ghost track";
                        }
                        else
                        {
                            TSString = TSString + "GHO Default";
                        }
                    }
                    rowFieldsCAT20["Track Status"] = TSString;
                }
                if (Convert.ToInt32(DataItems[c]) == 8)
                {
                    string AString = ConvertToBite(Values[n]);
                    string AString1 = AString + ConvertToBite(Values[n + 1]);
                    char[] Abite = AString1.ToCharArray();
                    string AString2;
                    string AString3 = null;

                    for (int t = 4; t < 16; t++)
                    {
                        AString3 = AString3 + Convert.ToString(Abite[t]);
                    }
                    if (Char.GetNumericValue(Abite[0]) == 1)
                    {
                        AString2 = "Code not validated - ";
                    }
                    else
                    {
                        AString2 = "Code validated - ";
                    }
                    if (Char.GetNumericValue(Abite[1]) == 1)
                    {
                        AString2 = AString2 + "Garbled code - ";
                    }
                    else
                    {
                        AString2 = AString2 + "Default - ";
                    }
                    if (Char.GetNumericValue(Abite[2]) == 1)
                    {
                        AString2 = AString2 + "Mode-3A code not extracted during the last update period.";
                    }
                    else
                    {
                        AString2 = AString2 + "Mode-3/A code derived from the reply of the transponder.";
                    }
                    int h = Convert.ToInt32(AString3, 2);
                    string g = Convert.ToString(h, 8);
                    rowFieldsCAT20["Mode-3/A"] = Convert.ToInt32(g);
                    rowFieldsCAT20["Mode-3/A Info."] = AString2;
                    n = n + 2;
                    c = c + 1;
                }
                if (Convert.ToInt32(DataItems[c]) == 9)
                {
                    string VxString = ConvertToBite(Values[n]);
                    VxString = VxString + ConvertToBite(Values[n + 1]);
                    string VyString = ConvertToBite(Values[n + 2]);
                    VyString = VyString + ConvertToBite(Values[n + 3]);
                    int Vx = Convert.ToInt32(VxString, 2);
                    int Vy = Convert.ToInt32(VyString, 2);
                    if (Vx > 32768)
                    {
                        Vx = Vx - 65536;
                    }
                    if (Vy > 32768)
                    {
                        Vy = Vy - 65536;
                    }
                    rowFieldsCAT20["Vx [m/s]"] = Vx * (double)0.25;
                    rowFieldsCAT20["Vy [m/s]"] = Vy * (double)0.25;
                    n = n + 4;
                    c = c + 1;
                }
                if (Convert.ToInt32(DataItems[c]) == 10)
                {
                    string FLString = ConvertToBite(Values[n]);
                    FLString = FLString + ConvertToBite(Values[n + 1]);
                    char[] FLbite = FLString.ToCharArray();
                    string FLString2;
                    string FLString3 = null;

                    for (int t = 2; t < 16; t++)
                    {
                        FLString3 = FLString3 + Convert.ToString(FLbite[t]);
                    }
                    if (Char.GetNumericValue(FLbite[0]) == 1)
                    {
                        FLString2 = "Code not validated - ";
                    }
                    else
                    {
                        FLString2 = "Code Validated - ";
                    }
                    if (Char.GetNumericValue(FLbite[1]) == 1)
                    {
                        FLString2 = FLString2 + "Garbled code";
                    }
                    else
                    {
                        FLString2 = FLString2 + "Default";
                    }
                    rowFieldsCAT20["Flight Level Info."] = FLString2;
                    rowFieldsCAT20["Flight Level"] = Convert.ToInt32(FLString3, 2) * (double)0.25;
                    n = n + 2;
                    c = c + 1;
                }
                if (Convert.ToInt32(DataItems[c]) == 12)
                {
                    string TAString = ConvertToBite(Values[n]);
                    TAString = TAString + ConvertToBite(Values[n + 1]);
                    TAString = TAString + ConvertToBite(Values[n + 2]);
                    string TAHex = Convert.ToString(Convert.ToInt32(TAString, 2), 16);

                    rowFieldsCAT20["Target Adress"] = TAHex;
                    n = n + 3;
                    c = c + 1;
                }
                if (Convert.ToInt32(DataItems[c]) == 13)
                {
                    string TIString = null;
                    string TIBiteString = ConvertToBite(Values[n]);
                    char[] TIbite = TIBiteString.ToCharArray();
                    if ((Convert.ToString(TIbite[0]) + Convert.ToString(TIbite[1])).Equals("11") == true)
                    {
                        TIString = "Not defined - ";
                    }
                    if ((Convert.ToString(TIbite[0]) + Convert.ToString(TIbite[1])).Equals("10") == true)
                    {
                        TIString = "Callsign downlinked from transponder";
                    }
                    if ((Convert.ToString(TIbite[0]) + Convert.ToString(TIbite[1])).Equals("01") == true)
                    {
                        TIString = "Registration downlinked from transponder";
                    }
                    if ((Convert.ToString(TIbite[0]) + Convert.ToString(TIbite[1])).Equals("00") == true)
                    {
                        TIString = "Callsign or registration not downlinked from transponder.";
                    }
                    string TIBiteString2 = ConvertToBite(Values[n + 1]);
                    TIBiteString2 = TIBiteString2 + ConvertToBite(Values[n + 2]);
                    TIBiteString2 = TIBiteString2 + ConvertToBite(Values[n + 3]);
                    TIBiteString2 = TIBiteString2 + ConvertToBite(Values[n + 4]);
                    TIBiteString2 = TIBiteString2 + ConvertToBite(Values[n + 5]);
                    TIBiteString2 = TIBiteString2 + ConvertToBite(Values[n + 6]);
                    char[] TIbite2 = TIBiteString2.ToCharArray();
                    string TI = null;
                    int j = 0;
                    for (int p = 0; p < 8; p++)
                    {
                        string g = Convert.ToString(TIbite2[j + 2]) + Convert.ToString(TIbite2[j + 3]) + Convert.ToString(TIbite2[j + 4]) + Convert.ToString(TIbite2[j + 5]);
                        if ((Char.GetNumericValue(TIbite2[j + 0])) == 0 && (Char.GetNumericValue(TIbite2[j + 1])) == 0)
                        {
                            if (g.Equals("0000"))
                            {
                                TI = TI + " ";
                            }
                            if (g.Equals("0001"))
                            {
                                TI = TI + "A";
                            }
                            if (g.Equals("0010"))
                            {
                                TI = TI + "B";
                            }
                            if (g.Equals("0011"))
                            {
                                TI = TI + "C";
                            }
                            if (g.Equals("0100"))
                            {
                                TI = TI + "D";
                            }
                            if (g.Equals("0101"))
                            {
                                TI = TI + "E";
                            }
                            if (g.Equals("0110"))
                            {
                                TI = TI + "F";
                            }
                            if (g.Equals("0111"))
                            {
                                TI = TI + "G";
                            }
                            if (g.Equals("1000"))
                            {
                                TI = TI + "H";
                            }
                            if (g.Equals("1001"))
                            {
                                TI = TI + "I";
                            }
                            if (g.Equals("1010"))
                            {
                                TI = TI + "J";
                            }
                            if (g.Equals("1011"))
                            {
                                TI = TI + "K";
                            }
                            if (g.Equals("1100"))
                            {
                                TI = TI + "L";
                            }
                            if (g.Equals("1101"))
                            {
                                TI = TI + "M";
                            }
                            if (g.Equals("1110"))
                            {
                                TI = TI + "N";
                            }
                            if (g.Equals("1111"))
                            {
                                TI = TI + "O";
                            }
                        }
                        if ((Char.GetNumericValue(TIbite2[j + 0])) == 0 && (Char.GetNumericValue(TIbite2[j + 1])) == 1)
                        {
                            if (g.Equals("0000"))
                            {
                                TI = TI + "P";
                            }
                            else if (g.Equals("0001"))
                            {
                                TI = TI + "Q";
                            }
                            else if (g.Equals("0010"))
                            {
                                TI = TI + "R";
                            }
                            else if (g.Equals("0011"))
                            {
                                TI = TI + "S";
                            }
                            else if (g.Equals("0100"))
                            {
                                TI = TI + "T";
                            }
                            else if (g.Equals("0101"))
                            {
                                TI = TI + "U";
                            }
                            else if (g.Equals("0110"))
                            {
                                TI = TI + "V";
                            }
                            else if (g.Equals("0111"))
                            {
                                TI = TI + "W";
                            }
                            else if (g.Equals("1000"))
                            {
                                TI = TI + "X";
                            }
                            else if (g.Equals("1001"))
                            {
                                TI = TI + "Y";
                            }
                            else if (g.Equals("1010"))
                            {
                                TI = TI + "Z";
                            }
                            else
                            {
                                TI = TI + " ";
                            }
                        }
                        if ((Char.GetNumericValue(TIbite2[j + 0])) == 1 && (Char.GetNumericValue(TIbite2[j + 1])) == 0)
                        {
                            TI = TI + " ";
                        }
                        if ((Char.GetNumericValue(TIbite2[j + 0])) == 1 && (Char.GetNumericValue(TIbite2[j + 1])) == 1)
                        {
                            if (g.Equals("0000"))
                            {
                                TI = TI + "0";
                            }
                            else if (g.Equals("0001"))
                            {
                                TI = TI + "1";
                            }
                            else if (g.Equals("0010"))
                            {
                                TI = TI + "2";
                            }
                            else if (g.Equals("0011"))
                            {
                                TI = TI + "3";
                            }
                            else if (g.Equals("0100"))
                            {
                                TI = TI + "4";
                            }
                            else if (g.Equals("0101"))
                            {
                                TI = TI + "5";
                            }
                            else if (g.Equals("0110"))
                            {
                                TI = TI + "6";
                            }
                            else if (g.Equals("0111"))
                            {
                                TI = TI + "7";
                            }
                            else if (g.Equals("1000"))
                            {
                                TI = TI + "8";
                            }
                            else if (g.Equals("1001"))
                            {
                                TI = TI + "9";
                            }
                            else
                            {
                                TI = TI + " ";
                            }
                        }
                        j = j + 6;
                    }
                    rowFieldsCAT20["Target Identification Info."] = TIString;
                    rowFieldsCAT20["Target Identification"] = TI;
                    n = n + 7;
                    c = c + 1;
                }
                if (Convert.ToInt32(DataItems[c]) == 19)
                {
                    string PABiteString = ConvertToBite(Values[n]);
                    char[] PAbite = PABiteString.ToCharArray();
                    c = c + 1;
                    n = n + 1;
                    if (Char.GetNumericValue(PAbite[0]) == 1)
                    {
                        string DOPxString = ConvertToBite(Values[n]);
                        DOPxString = DOPxString + ConvertToBite(Values[n + 1]);
                        string DOPyString = ConvertToBite(Values[n + 2]);
                        DOPyString = DOPyString + ConvertToBite(Values[n + 3]);
                        string DOPxyString = ConvertToBite(Values[n + 4]);
                        DOPxyString = DOPxyString + ConvertToBite(Values[n + 5]);
                        rowFieldsCAT20["DOP-x [m]"] = Convert.ToInt32(DOPxString, 2) * (double)0.25;
                        rowFieldsCAT20["DOP-y [m]"] = Convert.ToInt32(DOPyString, 2) * (double)0.25;
                        rowFieldsCAT20["DOP-xy [m]"] = Convert.ToInt32(DOPxyString, 2) * (double)0.25;
                        n = n + 6;
                    }
                    if (Char.GetNumericValue(PAbite[1]) == 1)
                    {
                        string SDxString = ConvertToBite(Values[n]);
                        SDxString = SDxString + ConvertToBite(Values[n + 1]);
                        string SDyString = ConvertToBite(Values[n + 2]);
                        SDyString = SDyString + ConvertToBite(Values[n + 3]);
                        string CCString = ConvertToBite(Values[n + 4]);
                        CCString = CCString + ConvertToBite(Values[n + 5]);
                        rowFieldsCAT20["Standard Deviation of X"] = Convert.ToInt32(SDxString, 2);
                        rowFieldsCAT20["Standard Deviation of Y"] = Convert.ToInt32(SDyString, 2);
                        rowFieldsCAT20["Correlation coefficient"] = Convert.ToInt32(CCString, 2);
                        n = n + 6;
                    }
                    if (Char.GetNumericValue(PAbite[2]) == 1)
                    {
                        string SDHString = ConvertToBite(Values[n]);
                        SDHString = SDHString + ConvertToBite(Values[n + 1]);
                        rowFieldsCAT20["Standard Deviation of Geometric Height"] = Convert.ToInt32(SDHString, 2);
                        n = n + 2;
                    }
                }
                if (Convert.ToInt32(DataItems[c]) == 20)
                {
                    rowFieldsCAT20["Contributing Devices"] = "Por Hacer";
                }
                FieldsCAT20.Rows.Add(rowFieldsCAT20);
            }
            return FieldsCAT20;
        }

        public DataTable ReadCAT21(DataTable CAT21, string filename)
        {
            DataTable FieldsCAT21 = new DataTable();
            FieldsCAT21.Columns.Add("SAC", typeof(int));
            FieldsCAT21.Columns.Add("SIC", typeof(int));
            FieldsCAT21.Columns.Add("Target Report Descriptor", typeof(string));
            FieldsCAT21.Columns.Add("Time of Day", typeof(TimeSpan));
            FieldsCAT21.Columns.Add("Longitude", typeof(double));
            FieldsCAT21.Columns.Add("Latitude", typeof(double));
            FieldsCAT21.Columns.Add("Target Adress", typeof(string));
            FieldsCAT21.Columns.Add("Figure of Merit", typeof(string));
            FieldsCAT21.Columns.Add("Link Technology Indicator", typeof(string));
            FieldsCAT21.Columns.Add("Flight Level", typeof(double));
            FieldsCAT21.Columns.Add("Geometric Vertical Rate [Ft/M]", typeof(double));
            FieldsCAT21.Columns.Add("Ground Speed[Km/h]", typeof(double));
            FieldsCAT21.Columns.Add("Track Angle[º]", typeof(int));
            FieldsCAT21.Columns.Add("Target Identification", typeof(string));
            FieldsCAT21.Columns.Add("Velocity Accuracy", typeof(int));

            StreamReader file = new StreamReader(filename);

            for (int i = 0; i < CAT21.Rows.Count; i++)
            {
                string a = CAT21.Rows[i]["DataFields"].ToString();
                string[] DataItems = a.Split(' ');
                int n = Convert.ToInt32(CAT21.Rows[i]["LastPositionFSPEC"].ToString());
                string line = file.ReadLine();
                string[] Values = line.Split(' ');
                DataRow rowFieldsCAT21 = FieldsCAT21.NewRow();
                int c = 0;
                if (Convert.ToInt32(DataItems[c]) == 1)
                {
                    rowFieldsCAT21["SAC"] = Convert.ToInt32(Values[n]);
                    rowFieldsCAT21["SIC"] = Convert.ToInt32(Values[n + 1]);
                    n = n + 2; //Posición del octeto dentro el txt
                    c = c + 1; //Posición de los dataitems dentro del datagreed "Dataitems"
                }
                if (c < DataItems.Length && Convert.ToInt32(DataItems[c]) == 2)
                {
                    string TRDString;
                    string TRDBiteString = ConvertToBite(Values[n]);
                    TRDBiteString = TRDBiteString + ConvertToBite(Values[n + 1]);
                    char[] TRDbite = TRDBiteString.ToCharArray();
                    n = n + 2;
                    c = c + 1;
                    if (Char.GetNumericValue(TRDbite[0]) == 1)
                    {
                        TRDString = "Differential correction (ADS-B) - ";
                    }
                    else
                    {
                        TRDString = "No differential correction (ADS-B) - ";
                    }
                    if (Char.GetNumericValue(TRDbite[1]) == 1)
                    {
                        TRDString = TRDString + "Ground Bit set - ";
                    }
                    else
                    {
                        TRDString = TRDString + "Ground Bit not set - ";
                    }
                    if (Char.GetNumericValue(TRDbite[2]) == 1)
                    {
                        TRDString = TRDString + "Simulated target report - ";
                    }
                    else
                    {
                        TRDString = TRDString + "Actual target report - ";
                    }
                    if (Char.GetNumericValue(TRDbite[3]) == 1)
                    {
                        TRDString = TRDString + "Test Target - ";
                    }
                    else
                    {
                        TRDString = TRDString + "Default - ";
                    }
                    if (Char.GetNumericValue(TRDbite[4]) == 1)
                    {
                        TRDString = TRDString + "Report from field monitor (fixed transponder) - ";
                    }
                    else
                    {
                        TRDString = TRDString + "Report from target transponder - ";
                    }
                    if (Char.GetNumericValue(TRDbite[5]) == 1)
                    {
                        TRDString = TRDString + "Equipement capable to provide Selected Altitude - ";
                    }
                    else
                    {
                        TRDString = TRDString + "Equipement not capable to provide Selected Altitude - ";
                    }
                    if (Char.GetNumericValue(TRDbite[6]) == 1)
                    {
                        TRDString = TRDString + "Special Position Identification - ";
                    }
                    else
                    {
                        TRDString = TRDString + "Absence of SPI - ";
                    }
                    bool b = false;
                    if ((Convert.ToString(TRDbite[8]) + Convert.ToString(TRDbite[9]) + Convert.ToString(TRDbite[10])).Equals("000") == true)
                    {
                        TRDString = TRDString + "Non unique address - ";
                        b = true;
                    }
                    if ((Convert.ToString(TRDbite[8]) + Convert.ToString(TRDbite[9]) + Convert.ToString(TRDbite[10])).Equals("001") == true)
                    {
                        TRDString = TRDString + "24 - Bit ICAO address - ";
                        b = true;
                    }
                    if ((Convert.ToString(TRDbite[8]) + Convert.ToString(TRDbite[9]) + Convert.ToString(TRDbite[10])).Equals("010") == true)
                    {
                        TRDString = TRDString + "Surface vehicle address - ";
                        b = true;
                    }
                    if ((Convert.ToString(TRDbite[8]) + Convert.ToString(TRDbite[9]) + Convert.ToString(TRDbite[10])).Equals("011") == true)
                    {
                        TRDString = TRDString + "Anonymous address - ";
                        b = true;
                    }
                    if (b == false)
                    {
                        TRDString = TRDString + "Reserved for future use - ";
                    }
                    if ((Convert.ToString(TRDbite[11]) + Convert.ToString(TRDbite[12])).Equals("00") == true)
                    {
                        TRDString = TRDString + "ARD Unknown";
                    }
                    if ((Convert.ToString(TRDbite[11]) + Convert.ToString(TRDbite[12])).Equals("01") == true)
                    {
                        TRDString = TRDString + "ARD: 25ft";
                    }
                    if ((Convert.ToString(TRDbite[11]) + Convert.ToString(TRDbite[12])).Equals("10") == true)
                    {
                        TRDString = TRDString + "ARD: 100ft";
                    }
                    rowFieldsCAT21["Target Report Descriptor"] = TRDString;

                }
                if (c < DataItems.Length && Convert.ToInt32(DataItems[c]) == 3)
                {
                    string TimeString = ConvertToBite(Values[n]);
                    TimeString = TimeString + ConvertToBite(Values[n + 1]);
                    TimeString = TimeString + ConvertToBite(Values[n + 2]);
                    int Time = Convert.ToInt32(TimeString, 2);
                    double Time2 = Time / (double)128;
                    TimeSpan time = TimeSpan.FromSeconds(Time2);
                    rowFieldsCAT21["Time of Day"] = time;
                    n = n + 3;
                    c = c + 1;
                }
                if (c < DataItems.Length && Convert.ToInt32(DataItems[c]) == 4)
                {
                    string LatitudeString = ConvertToBite(Values[n]);
                    LatitudeString = LatitudeString + ConvertToBite(Values[n + 1]);
                    LatitudeString = LatitudeString + ConvertToBite(Values[n + 2]);
                    string LongitudeString = ConvertToBite(Values[n + 3]);
                    LongitudeString = LongitudeString + ConvertToBite(Values[n + 4]);
                    LongitudeString = LongitudeString + ConvertToBite(Values[n + 6]);
                    double Latitude = (double)Convert.ToInt32(LatitudeString, 2);
                    double Longitude = (double)Convert.ToInt32(LongitudeString, 2);
                    if (Longitude > (double)4194304)
                    {
                        Longitude = Longitude - (double)8388608;
                    }
                    if (Latitude > (double)8388608)
                    {
                        Latitude = Latitude - (double)16777216;
                    }
                    double Latitude2 = Latitude * (double)180 / (double)8388608;
                    double Longitude2 = Longitude * (double)180 / (double)8388608;
                    rowFieldsCAT21["Latitude"] = Latitude2;
                    rowFieldsCAT21["Longitude"] = Longitude2;
                    n = n + 6;
                    c = c + 1;
                }
                if (c < DataItems.Length && Convert.ToInt32(DataItems[c]) == 5)
                {
                    string TAString = ConvertToBite(Values[n]);
                    TAString = TAString + ConvertToBite(Values[n + 1]);
                    TAString = TAString + ConvertToBite(Values[n + 2]);
                    string TAHex = Convert.ToString(Convert.ToInt32(TAString, 2), 16);

                    rowFieldsCAT21["Target Adress"] = TAHex;
                    n = n + 3;
                    c = c + 1;
                }
                if (c < DataItems.Length && Convert.ToInt32(DataItems[c]) == 7)
                {
                    string FMString = null;
                    string FMBiteString = ConvertToBite(Values[n]);
                    FMBiteString = FMBiteString + ConvertToBite(Values[n + 1]);
                    char[] FMBite = FMBiteString.ToCharArray();

                    if ((Convert.ToString(FMBite[0]) + Convert.ToString(FMBite[1])).Equals("00") == true)
                    {
                        FMString = "AC Unknown - ";
                    }
                    if ((Convert.ToString(FMBite[0]) + Convert.ToString(FMBite[1])).Equals("01") == true)
                    {
                        FMString = "ACAS not operational - ";
                    }
                    if ((Convert.ToString(FMBite[0]) + Convert.ToString(FMBite[1])).Equals("10") == true)
                    {
                        FMString = "ACAS operational - ";
                    }
                    if ((Convert.ToString(FMBite[0]) + Convert.ToString(FMBite[1])).Equals("11") == true)
                    {
                        FMString = "AC invalid - ";
                    }
                    if ((Convert.ToString(FMBite[2]) + Convert.ToString(FMBite[3])).Equals("00") == true)
                    {
                        FMString = FMString + "MN Unknown - ";
                    }
                    if ((Convert.ToString(FMBite[2]) + Convert.ToString(FMBite[3])).Equals("01") == true)
                    {
                        FMString = FMString + "Multiple navigational aids not operating - ";
                    }
                    if ((Convert.ToString(FMBite[2]) + Convert.ToString(FMBite[3])).Equals("10") == true)
                    {
                        FMString = FMString + "Multiple navigational aids operating - ";
                    }
                    if ((Convert.ToString(FMBite[2]) + Convert.ToString(FMBite[3])).Equals("11") == true)
                    {
                        FMString = FMString + "MN invalid - ";
                    }
                    if ((Convert.ToString(FMBite[4]) + Convert.ToString(FMBite[5])).Equals("00") == true)
                    {
                        FMString = FMString + "DC Unknown - ";
                    }
                    if ((Convert.ToString(FMBite[4]) + Convert.ToString(FMBite[5])).Equals("01") == true)
                    {
                        FMString = FMString + "Differential correction - ";
                    }
                    if ((Convert.ToString(FMBite[4]) + Convert.ToString(FMBite[5])).Equals("10") == true)
                    {
                        FMString = FMString + "No Differential correction - ";
                    }
                    if ((Convert.ToString(FMBite[4]) + Convert.ToString(FMBite[5])).Equals("11") == true)
                    {
                        FMString = FMString + "DC invalid - ";
                    }
                    string pabits = (Convert.ToString(FMBite[12]) + Convert.ToString(FMBite[13]) + Convert.ToString(FMBite[14]) + Convert.ToString(FMBite[15]));
                    string PA = Convert.ToString(Convert.ToInt32(pabits, 2));
                    rowFieldsCAT21["Figure of Merit"] = FMString + PA;
                    n = n + 2;
                    c = c + 1;
                }
                if (c < DataItems.Length && Convert.ToInt32(DataItems[c]) == 8)
                {
                    string LTIString;
                    string LTIBiteString = ConvertToBite(Values[n]);
                    char[] LTIBite = LTIBiteString.ToCharArray();
                    if (char.GetNumericValue(LTIBite[3]) == 1)
                    {
                        LTIString = "Aircraft equiped with CDTI - ";
                    }
                    else
                    {
                        LTIString = "Unknown - ";
                    }
                    if (char.GetNumericValue(LTIBite[4]) == 1)
                    {
                        LTIString = LTIString + "Used - ";
                    }
                    else
                    {
                        LTIString = LTIString + "Not used - ";
                    }
                    if (char.GetNumericValue(LTIBite[5]) == 1)
                    {
                        LTIString = LTIString + "Used - ";
                    }
                    else
                    {
                        LTIString = LTIString + "Not used - ";
                    }
                    if (char.GetNumericValue(LTIBite[6]) == 1)
                    {
                        LTIString = LTIString + "Used - ";
                    }
                    else
                    {
                        LTIString = LTIString + "Not used - ";
                    }
                    if (char.GetNumericValue(LTIBite[7]) == 1)
                    {
                        LTIString = LTIString + "Used";
                    }
                    else
                    {
                        LTIString = LTIString + "Not used";
                    }
                    rowFieldsCAT21["Link Technology Indicator"] = LTIString;
                    n = n + 1;
                    c = c + 1;
                }
                if (c < DataItems.Length && Convert.ToInt32(DataItems[c]) == 10)
                {
                    string FLString = ConvertToBite(Values[n]);
                    FLString = FLString + ConvertToBite(Values[n + 1]);
                    int FL = Convert.ToInt32(FLString, 2);
                    rowFieldsCAT21["Flight Level"] = FL / (double)4;
                    n = n + 2;
                    c = c + 1;
                }
                if (c < DataItems.Length && Convert.ToInt32(DataItems[c]) == 15)
                {
                    string GVRString = ConvertToBite(Values[n]);
                    GVRString = GVRString + ConvertToBite(Values[n + 1]);
                    int GVR = Convert.ToInt32(GVRString, 2);
                    rowFieldsCAT21["Geometric Vertical Rate [Ft/M]"] = GVR * (double)6.25;
                    n = n + 2;
                    c = c + 1;
                }
                if (c < DataItems.Length && Convert.ToInt32(DataItems[c]) == 16)
                {
                    string GVString = ConvertToBite(Values[n]);
                    GVString = GVString + ConvertToBite(Values[n + 1]);
                    string TAString = ConvertToBite(Values[n + 2]);
                    TAString = TAString + ConvertToBite(Values[n + 3]);
                    double GV = (double)(Convert.ToInt32(GVString, 2));
                    int TA = Convert.ToInt32(TAString, 2);
                    if (GV > 32768)
                    {
                        GV = GV - (double)655536;
                    }
                    rowFieldsCAT21["Ground Speed[Km/h]"] = GV / (double)2.457403408;
                    rowFieldsCAT21["Track Angle[º]"] = TA * (double)0.0055;
                    n = n + 4;
                    c = c + 1;
                }
                if (c < DataItems.Length && Convert.ToInt32(DataItems[c]) == 18)
                {
                    string TIBiteString = ConvertToBite(Values[n]);
                    TIBiteString = TIBiteString + ConvertToBite(Values[n + 1]);
                    TIBiteString = TIBiteString + ConvertToBite(Values[n + 2]);
                    TIBiteString = TIBiteString + ConvertToBite(Values[n + 3]);
                    TIBiteString = TIBiteString + ConvertToBite(Values[n + 4]);
                    TIBiteString = TIBiteString + ConvertToBite(Values[n + 5]);
                    char[] TIbite2 = TIBiteString.ToCharArray();
                    string TI = String.Empty;
                    int j = 0;
                    for (int p = 0; p < 8; p++)
                    {
                        string g = Convert.ToString(TIbite2[j + 2]) + Convert.ToString(TIbite2[j + 3]) + Convert.ToString(TIbite2[j + 4]) + Convert.ToString(TIbite2[j + 5]);
                        if ((Char.GetNumericValue(TIbite2[j + 0])) == 0 && (Char.GetNumericValue(TIbite2[j + 1])) == 0)
                        {
                            if (g.Equals("0000"))
                            {
                                TI += " ";
                            }
                            if (g.Equals("0001"))
                            {
                                TI = TI + "A";
                            }
                            if (g.Equals("0010"))
                            {
                                TI = TI + "B";
                            }
                            if (g.Equals("0011"))
                            {
                                TI = TI + "C";
                            }
                            if (g.Equals("0100"))
                            {
                                TI = TI + "D";
                            }
                            if (g.Equals("0101"))
                            {
                                TI = TI + "E";
                            }
                            if (g.Equals("0110"))
                            {
                                TI = TI + "F";
                            }
                            if (g.Equals("0111"))
                            {
                                TI = TI + "G";
                            }
                            if (g.Equals("1000"))
                            {
                                TI = TI + "H";
                            }
                            if (g.Equals("1001"))
                            {
                                TI = TI + "I";
                            }
                            if (g.Equals("1010"))
                            {
                                TI = TI + "J";
                            }
                            if (g.Equals("1011"))
                            {
                                TI = TI + "K";
                            }
                            if (g.Equals("1100"))
                            {
                                TI = TI + "L";
                            }
                            if (g.Equals("1101"))
                            {
                                TI = TI + "M";
                            }
                            if (g.Equals("1110"))
                            {
                                TI = TI + "N";
                            }
                            if (g.Equals("1111"))
                            {
                                TI = TI + "O";
                            }
                        }
                        if ((Char.GetNumericValue(TIbite2[j + 0])) == 0 && (Char.GetNumericValue(TIbite2[j + 1])) == 1)
                        {
                            if (g.Equals("0000"))
                            {
                                TI = TI + "P";
                            }
                            else if (g.Equals("0001"))
                            {
                                TI = TI + "Q";
                            }
                            else if (g.Equals("0010"))
                            {
                                TI = TI + "R";
                            }
                            else if (g.Equals("0011"))
                            {
                                TI = TI + "S";
                            }
                            else if (g.Equals("0100"))
                            {
                                TI = TI + "T";
                            }
                            else if (g.Equals("0101"))
                            {
                                TI = TI + "U";
                            }
                            else if (g.Equals("0110"))
                            {
                                TI = TI + "V";
                            }
                            else if (g.Equals("0111"))
                            {
                                TI = TI + "W";
                            }
                            else if (g.Equals("1000"))
                            {
                                TI = TI + "X";
                            }
                            else if (g.Equals("1001"))
                            {
                                TI = TI + "Y";
                            }
                            else if (g.Equals("1010"))
                            {
                                TI = TI + "Z";
                            }
                            else
                            {
                                TI += " ";
                            }
                        }
                        if ((Char.GetNumericValue(TIbite2[j + 0])) == 1 && (Char.GetNumericValue(TIbite2[j + 1])) == 0)
                        {
                            TI += " ";
                        }
                        if ((Char.GetNumericValue(TIbite2[j + 0])) == 1 && (Char.GetNumericValue(TIbite2[j + 1])) == 1)
                        {
                            if (g.Equals("0000"))
                            {
                                TI = TI + "0";
                            }
                            else if (g.Equals("0001"))
                            {
                                TI = TI + "1";
                            }
                            else if (g.Equals("0010"))
                            {
                                TI = TI + "2";
                            }
                            else if (g.Equals("0011"))
                            {
                                TI = TI + "3";
                            }
                            else if (g.Equals("0100"))
                            {
                                TI = TI + "4";
                            }
                            else if (g.Equals("0101"))
                            {
                                TI = TI + "5";
                            }
                            else if (g.Equals("0110"))
                            {
                                TI = TI + "6";
                            }
                            else if (g.Equals("0111"))
                            {
                                TI = TI + "7";
                            }
                            else if (g.Equals("1000"))
                            {
                                TI = TI + "8";
                            }
                            else if (g.Equals("1001"))
                            {
                                TI = TI + "9";
                            }
                            else
                            {
                                TI = TI + " ";
                            }
                        }
                        j = j + 6;
                        rowFieldsCAT21["Target Identification"] = TI;
                    }
                    n = n + 6;
                    c = c + 1;
                }
                if (c < DataItems.Length && Convert.ToInt32(DataItems[c]) == 19)
                {
                    string VAString = ConvertToBite(Values[n]);
                    int VA = Convert.ToInt32(VAString, 2);
                    rowFieldsCAT21["Velocity Accuracy"] = VA;
                    n = n + 1;
                    c = c + 1;
                }
                FieldsCAT21.Rows.Add(rowFieldsCAT21);
            }
            return FieldsCAT21;
        }
    }
}