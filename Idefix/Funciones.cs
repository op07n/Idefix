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

        public List<CAT10> ReadCat10(List<double[]> msgcat10_T, List<string[]> FSPEC_T) {
            int a = 0;
            double SAC = 0; double SIC = 0;
            String MsgType = String.Empty;
            TimeSpan TimeOfDay= TimeSpan.Zero;
            int TN = 0;
            string[] TRD = new string[0] ; string[] TS = new string[0]; string[] SS = new string[0];
            double[] PP = new double[0]; double[] CP = new double[0]; double[] PTV = new double[0]; double[] CTV = new double[0]; double[] TSO = new double[0]; double[] CA = new double[0];

            List<CAT10> listCAT10 = new List<CAT10>();

            while (a< msgcat10_T.Count)
            {
                string FSPEC_1 = FSPEC_T[a][0];
                double[] msgcat10 = msgcat10_T[a];
                // int n = 0;
                int pos = FSPEC_T[a].Count(); // posició de byte en el missatge rebut de categoria 10 SENSE cat,lenght,Fspec.
                if(FSPEC_1[0] == 1)// FRN = 1: Data Source ID
                {
                    SAC = msgcat10[pos]; // assumim que es un vector de double on cada posició és el valor decimal del byte corresponent
                    SIC = msgcat10[pos+1];
                    pos = pos + 2;
                }// FRN = 1: Data Source ID
                if (FSPEC_1[1] == 1)// FRN = 2: Message Type
                {
                    double val = msgcat10[pos];
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
                }// FRN = 2: Message Type
                if (FSPEC_1[2] == 1)// FRN = 3: Target Report Description
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
                    TRD = new string[5] { TYP, DCR, CHN, GBS, CRT };
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
                        TRD = new string[10] { TYP, DCR, CHN, GBS, CRT, SIM, TST, RAB, LOP, TOT};

                        pos += 1;

                        if(va2[7].Equals("1")) 
                        {
                            string SIP = String.Empty;
                            string va3 = Convert2Binary(msgcat10[pos]);
                            if (va3[0].Equals("0")) { SIP = "Absence of SPI"; }
                            else { SIP = "Special Position Identification"; }
                            
                            TRD = new string[11] { TYP, DCR, CHN, GBS, CRT, SIM, TST, RAB, LOP, TOT, SIP };
                            pos += 1;
                        }

                        
                    }
                }// FRN = 3: Target Report Description
                if (FSPEC_1[3] == 1) //FRN = 4: Time Of Day
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
                    TimeOfDay = TimeSpan.FromSeconds(Hour); // hh:mm:ss
                    pos += 3;
                }//FRN = 4: Time Of Day
                if (FSPEC_1[4] == 1) { pos += 8; } //FRN = 5: we all gon'die
                if (FSPEC_1[5] == 1) // FRN = 6: Polar Position
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
                    PP = new double[2] { rho, theta };
                    pos += 4;
                } // FRN = 6: Polar Position
                if (FSPEC_1[6] == 1) // FRN = 7: Cartesian Position
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
                    CP = new double[2] { x, y};
                    pos += 4;
                }// FRN = 7: Cartesian Position
                string error = FSPEC_1;
                char error_in = FSPEC_1[7];
                if (FSPEC_1[7] == '0') { }
                else
                {
                    string FSPEC_2 = FSPEC_T[a][1];
                    if (FSPEC_2[0] == 1) // FRN = 8: Polar Track Velocity
                    {
                        string ground_speed1 = Convert2Binary(msgcat10[pos]);
                        string ground_speed2 = Convert2Binary(msgcat10[pos + 1]);
                        StringBuilder ground_speed_BIN = new StringBuilder(ground_speed1);
                        ground_speed_BIN.Append(ground_speed2);
                        string ground_speed_BIN_TOTAL = ground_speed_BIN.ToString();
                        double ground_speed = ((int)Convert.ToInt64(ground_speed_BIN_TOTAL, 10)) * 0.22; // in m
                        string track_angle1 = Convert2Binary(msgcat10[pos + 2]);
                        string track_angle2 = Convert2Binary(msgcat10[pos + 3]);
                        StringBuilder track_angle_BIN = new StringBuilder(track_angle1);
                        track_angle_BIN.Append(track_angle2);
                        string track_angle_BIN_TOTAL = track_angle_BIN.ToString();
                        double track_angle = ((int)Convert.ToInt64(track_angle_BIN_TOTAL, 10)) * 360 / 2 ^ 16; // in degrees
                        PTV = new double[2] {ground_speed, track_angle};
                        pos += 4;
                    }// FRN = 8: Polar Track Velocity
                    if (FSPEC_2[1] == 1) // FRN = 9: Cartesian Track Velocity
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
                        CTV = new double[2] { Vx, Vy };
                        pos += 4;
                    }// FRN = 9: Cartesian Track Velocity
                    if (FSPEC_2[2] == 1) // FRN = 10: Track Number
                    {
                        string tn_1 = Convert2Binary(msgcat10[pos]);
                        string tn_2 = Convert2Binary(msgcat10[pos + 1]);
                        string tn_t = tn_1[4] + tn_1[5] + tn_1[6] + tn_1[7] + tn_2;
                        TN = (int)Convert.ToInt32(tn_t, 10);
                        pos += 2;
                    }// FRN = 10: Track Number
                    if (FSPEC_2[3] == 1) // FRN = 11: Track Status
                    {
                        string ts = Convert2Binary(msgcat10[pos]);
                        string CNF = String.Empty;
                        if (ts[0].Equals(1)) { CNF = "Track initialization phase"; }
                        else { CNF = "Confirmed Track"; }

                        string TRE = String.Empty;
                        if (ts[1].Equals(1)) { TRE = "Last report of track"; }
                        else { TRE = "Default"; }

                        StringBuilder cst = new StringBuilder(ts[2]);
                        cst.Append(ts[3]);
                        string CST = string.Empty;
                        if (cst.Equals("00")) { CST = "No Extrapolation"; }
                        else if (cst.Equals("01")) { CST = "Predictable extrapolation due to sensor refresh period"; }
                        else if (cst.Equals("10")) { CST = "Predictable extrapolation in masked area"; }
                        else if (cst.Equals("11")) { CST = "Extrapolation due to unpredictable absence of detection"; }

                        string MAH = String.Empty;
                        if (ts[4].Equals(1)) { MAH = "Horizontal manoeuvre"; }
                        else { MAH = "Default"; }

                        string TCC = String.Empty;
                        if (ts[5].Equals(1)) { TCC = "Slant range correction and a suitable projection technique are used to track in a 2D.reference plane, tangential to the earth model at the Sensor Site co-ordinates."; }
                        else { TCC = "Tracking performed in 'Sensor Plane', i.e. neither slant range correction nor projection was applied"; }

                        string STH = String.Empty;
                        if (ts[6].Equals(1)) { STH = "Smoothed position"; }
                        else { STH = "Measured position"; }

                        TS = new string[6] {CNF, TRE, CST, MAH, TCC, STH};
                        pos += 1;

                        if (ts[7].Equals(0)) { }
                        else
                        {
                            string ts_1 = Convert2Binary(msgcat10[pos]);
                            StringBuilder tom = new StringBuilder(ts_1[0]);
                            cst.Append(ts_1[1]);
                            string TOM = string.Empty;
                            if (tom.Equals("00")) { TOM = "Unknown type of movement "; }
                            else if (tom.Equals("01")) { TOM = "Taking-off "; }
                            else if (tom.Equals("10")) { TOM = "Landing"; }
                            else if (tom.Equals("11")) { TOM = "Other types of movement"; }

                            StringBuilder dou = new StringBuilder(ts_1[2]);
                            dou.Append(ts_1[3]);
                            dou.Append(ts_1[4]);
                            String DOU = String.Empty;
                            if (dou.Equals("000")) { DOU = "No doubt "; }
                            else if (dou.Equals("001")) { DOU = "Doubtful correlation (undetermined reason)"; }
                            else if (dou.Equals("010")) { DOU = "Doubtful correlation in clutter"; }
                            else if (dou.Equals("011")) { DOU = "Loss of accuracy"; }
                            else if (dou.Equals("100")) { DOU = "Loss of accuracy in clutter"; }
                            else if (dou.Equals("101")) { DOU = "HF Multilateration"; }
                            else if (dou.Equals("110")) { DOU = "Unstable track "; }
                            else if (dou.Equals("111")) { DOU = "Previously coasted"; }

                            StringBuilder mrs = new StringBuilder(ts_1[5]);
                            mrs.Append(ts_1[6]);
                            string MRS = string.Empty;
                            if (mrs.Equals("00")) { MRS = "Merge or split indication undetermined"; }
                            else if (mrs.Equals("01")) { MRS = "Track merged by association to plot"; }
                            else if (mrs.Equals("10")) { MRS = "Track merged by non-association to plot"; }
                            else if (mrs.Equals("11")) { MRS = "Split track"; }
                            TS = new string[9] { CNF, TRE, CST, MAH, TCC, STH, TOM, DOU, MRS};
                            pos += 1;

                            if (ts_1[7].Equals(0)) { }
                            else
                            {
                                string ts_2 = Convert2Binary(msgcat10[pos]);
                                string GHO = String.Empty;
                                if (ts_2[4].Equals(1)) { GHO = "Default"; }
                                else { GHO = "Ghost track"; }
                                TS = new string[10] { CNF, TRE, CST, MAH, TCC, STH, TOM, DOU, MRS, GHO};
                                pos += 1;
                            }
                        }
                    }// FRN = 11; Track Status
                    //some more shit shit here
                    if (FSPEC_2[7] == '0') { }
                    else
                    {
                        string FSPEC_3 = FSPEC_T[a][2];
                        if (FSPEC_3[4] == 1) //FRN = 19; Target Size and Orientation --> TSO
                        {
                            double LEN; double ORI = 0; double WID = 0;
                            string tso = Convert2Binary(msgcat10[pos]);
                            string len = tso.Remove(tso.Length - 1);
                            int LEN_0 = (int)Convert.ToInt32(len, 10);
                            LEN = Convert.ToDouble(LEN_0);
                            pos += 1;
                            if (tso[7].Equals("1"))
                            {
                                string tso_1 = Convert2Binary(msgcat10[pos]);
                                string ori = tso_1.Remove(tso.Length - 1);
                                int ORI_0 = (int)Convert.ToInt32(ori, 10);
                                ORI = Convert.ToDouble(ORI_0);
                                ORI *= 360 / 128;
                                pos += 1;
                                if (ori[7].Equals("1"))
                                {
                                    string tso_2 = Convert2Binary(msgcat10[pos]);
                                    string wid = tso_2.Remove(tso.Length - 1);
                                    int WID_0 = (int)Convert.ToInt32(wid, 10);
                                    WID = Convert.ToDouble(WID_0);
                                    pos += 1;
                                }
                            }

                            TSO = new double[3] {LEN, ORI, WID };
                        }//FRN = 19; Target Size and Orientation
                        if (FSPEC_3[5] == 1) //FRN = 20; SYSTEM STATUS
                        {
                            String NOGO = String.Empty;
                            String OVL = String.Empty;
                            String TSV = String.Empty;
                            String DIV = String.Empty;
                            String TTF = String.Empty;
                            string ss = Convert2Binary(msgcat10[pos]);
                            StringBuilder nogo = new StringBuilder(ss[0]);
                            nogo.Append(ss[1]);
                            string nogo_1 = nogo.ToString();
                            switch (nogo_1)
                            {
                                case "00":
                                    NOGO = "Operational";
                                    break;
                                case "01":
                                    NOGO = "Degradated";
                                    break;
                                case "10":
                                    NOGO = "NOGO";
                                    break;
                            }

                            if (ss[2].Equals("1")) { OVL = "Overload"; }
                            else { OVL = "No Overload"; }

                            if (ss[3].Equals("1")) { TSV = "Time Source Invalid"; }
                            else { TSV = "Time Source Valid"; }

                            if (ss[4].Equals("1")) { DIV = "Diversity Degraded"; }
                            else { DIV = "Normal Operation"; }

                            if (ss[5].Equals("1")) { TTF = "Test Target Failure"; }
                            else { TTF = "Test Target Operative"; }

                            SS = new string[5] {NOGO, OVL, TSV, DIV, TTF};
                            pos += 1;
                        }//FRN = 20; SYSTEM STATUS
                        if (FSPEC_3[7] == '0') { }
                        else
                        {
                            string FSPEC_4 = FSPEC_T[a][3];
                            if (FSPEC_4[3] == 1) // FRN = 25; Calculated acceleration
                            {
                                string ax1 = Convert2Binary(msgcat10[pos]);
                                string ax2 = Convert2Binary(msgcat10[pos + 1]);
                                StringBuilder ax_BIN = new StringBuilder(ax1);
                                ax_BIN.Append(ax2);
                                string ax_BIN_TOTAL = ax_BIN.ToString();
                                double ax = (int)Convert.ToInt64(ax_BIN_TOTAL, 10);
                                ax /= 4;//m/s^2
                                string ay1 = Convert2Binary(msgcat10[pos + 2]);
                                string ay2 = Convert2Binary(msgcat10[pos + 3]);
                                StringBuilder ay_BIN = new StringBuilder(ay1);
                                ay_BIN.Append(ay2);
                                string ay_BIN_TOTAL = ay_BIN.ToString();
                                double ay = ((int)Convert.ToInt64(ay_BIN_TOTAL, 10)); 
                                ay /= 4;//m/s^2
                                CA = new double[2] {ax, ay};
                                pos += 4;
                            }// FRN = 25; Calculated acceleration
                        }
                    }
                }
                CAT10 obj = new CAT10();
                obj.CAT10Constructor(obj, SIC, SAC, MsgType, TRD, TimeOfDay, PP, CP, PTV, CTV, TN, TS, TSO, SS, CA);
                listCAT10.Add(obj);
                a += 1;
            }

            return listCAT10;
        }
        
        public string Convert2Binary(double input)
        {
            String input_s = input.ToString();
            String output = Convert.ToInt32(input_s, 2).ToString();
            // Cal crear una funció que posi cada bit a una posició de l'string
            return output;
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