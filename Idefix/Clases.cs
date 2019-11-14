using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idefix
{
    public class Archivo
    {
        public List<double[]> MensajesCAT10 = new List<double[]>();
        public List<double[]> MensajesCAT19 = new List<double[]>();
        public List<double[]> MensajesCAT20 = new List<double[]>();
        public List<double[]> MensajesCAT21 = new List<double[]>();

        public void SetMsgCat10(List<double> msgCat10)
        {
            this.MensajesCAT10.Add(msgCat10.ToArray());
        }
        public void SetMsgCat19(List<double> msgCat19)
        {
            this.MensajesCAT19.Add(msgCat19.ToArray());
        }
        public void SetMsgCat20(List<double> msgCat20)
        {
            this.MensajesCAT20.Add(msgCat20.ToArray());
        }
        public void SetMsgCat21(List<double> msgCat21)
        {
            this.MensajesCAT21.Add(msgCat21.ToArray());
        }

        public List<double[]> GetMsgsCat10()
        {
            return this.MensajesCAT10;
        }
        public List<double[]> GetMsgsCat19()
        {
            return this.MensajesCAT19;
        }
        public List<double[]> GetMsgsCat20()
        {
            return this.MensajesCAT20;
        }
        public List<double[]> GetMsgsCat21()
        {
            return this.MensajesCAT21;
        }
    }
    public class CAT10
    {
        // el SAC y el SIC son el DataSourceIdentifier
        public double SAC;
        public double SIC;
        public string MessageType;
        //public string[] TargetReportDescriptor;
        // public TimeSpan TimeofDay;
        //public double[] PolarPosition;
        //public double[] CartesianPosition; //Per plotejar
        // public double[] PolarTrackVelocity;
        //public double[] CartesianTrackVelocity; //Per plotejar??
        //public string Mode3A;
        //public string FlightLevel;
        public int TrackNumber;
        //public string[] TrackStatus;
        //public string[] TargetAddress;
        //public string TargetIdentification;
        //public double[] TargetSizeAndOrientation;
        //public string[] SystemStatus;
        //      public double[] CalculatedAcceleration;

        public double GetSAC(string Octet)
        {
            this.SAC = Convert.ToInt32(Octet);
            //string SAC = Convert.ToString(Hex, 2);
            return SAC;
        }
        public double GetSIC(string Octet)
        {
            this.SIC = Convert.ToInt32(Octet);
            //string SIC = Convert.ToString(Hex, 2);
            return SIC;
        }


        /* public CAT10(double SIC, double SAC, String MsgType, String[] TargetReportDescriptor, TimeSpan TimeofDay, double[] PolarPosition, double[] CartesianPosition, double[] PolarTrackVelocity,double[] CartesianTrackVelocity, int TrackNumber, string[] TrackStatus, double[] TargetSizeAndOrientation, string [] SystemStatus, double[] CalculatedAcceleration)
         {
             this.SIC = SIC;
             this.SAC = SAC;
             this.MessageType = MsgType;
             this.TargetReportDescriptor = TargetReportDescriptor;
             this.TimeofDay = TimeofDay;
             this.PolarPosition = PolarPosition;
             this.CartesianPosition = CartesianPosition;
             this.PolarTrackVelocity = PolarTrackVelocity;
             this.CartesianTrackVelocity = CartesianTrackVelocity;
             this.TrackNumber = TrackNumber;
             this.TrackStatus = TrackStatus;
             this.TargetSizeAndOrientation = TargetSizeAndOrientation;
             this.SystemStatus = SystemStatus;
             this.CalculatedAcceleration = CalculatedAcceleration;
         }
         */

        public CAT10(double SIC, double SAC, String MsgType, int TrackNumber)
        {
            this.SIC = SIC;
            this.SAC = SAC;
            this.MessageType = MsgType;
            // this.TimeofDay = TimeofDay;
            this.TrackNumber = TrackNumber;

        }



        /*public string[] GetCartesianCoordinates(int X, int Y)
        {
            string[] CartesianCoordinates = new string[2];
            if (X > 32768)
            {
                X = X - 65536;
            }
            if (Y > 32768)
            {
                Y = Y - 65536;
            }
            CartesianCoordinates[0] = X.ToString();
            CartesianCoordinates[1] = Y.ToString();
            this.CartesianCoordinates = CartesianCoordinates;
            return this.CartesianCoordinates;

        }*/
        /*public string[] GetPolarTrackVelocity(int GS, int TA)
        {
            string[] TrackVelocity = new string[2];
            TrackVelocity[0] = ((double)GS * (double)0.22).ToString();
            TrackVelocity[1] = (((double)TA * (double)360) / (double)65536).ToString();
            this.PolarTrackVelocity = TrackVelocity;
            return this.PolarTrackVelocity;

        }

        public string[] GetCartesianTrackVelocity(int Vx, int Vy)
        {
            string[] TrackVelocity = new string[2];
            if (Vx > 32768)
            {
                Vx = Vx - 65536;
            }
            if (Vy > 32768)
            {
                Vy = Vy - 65536;
            }

            TrackVelocity[0] = (Convert.ToString((double)Vx * (double)0.25));
            TrackVelocity[1] = (Convert.ToString((double)Vy * (double)0.25));
            this.CartesianTrackVelocity = TrackVelocity;
            return this.CartesianTrackVelocity;

        }
        public string GetTrackNumber(int TrackNumber)
        {
            this.TrackNumber = TrackNumber.ToString();
            return this.TrackNumber;
        }
        public string GetTrackStatus(string TrackStatus)
        {
            if (TrackStatus.Substring(0, 1) == "0")
            {
                this.TrackStatus = "CNF=Confirmed Track";
            }
            if (TrackStatus.Substring(0, 1) == "1")
            {
                this.TrackStatus = "CNF=Track in initialisation phase";
            }
            if (TrackStatus.Substring(1, 1) == "0")
            {
                this.TrackStatus = this.TrackStatus + "-" + "TRE=Default";
            }
            if (TrackStatus.Substring(1, 1) == "1")
            {
                this.TrackStatus = this.TrackStatus + "-" + "TRE=Last report for a track";
            }
            if (TrackStatus.Substring(2, 2) == "00")
            {
                this.TrackStatus = this.TrackStatus + "-" + "CST=No Extrapolation";
            }
            if (TrackStatus.Substring(2, 2) == "01")
            {
                this.TrackStatus = this.TrackStatus + "-" + "CST=Predictable extrapolation due to sensor refresh period (see NOTE)";
            }
            if (TrackStatus.Substring(2, 2) == "10")
            {
                this.TrackStatus = this.TrackStatus + "-" + "CST=Predictable extrapolation in masked area";
            }
            if (TrackStatus.Substring(2, 2) == "11")
            {
                this.TrackStatus = this.TrackStatus + "-" + "CST=Extrapolation due to unpredictable absence of detection";
            }
            if (TrackStatus.Substring(4, 1) == "0")
            {
                this.TrackStatus = this.TrackStatus + "-" + "MAH=Default";
            }
            if (TrackStatus.Substring(4, 1) == "1")
            {
                this.TrackStatus = this.TrackStatus + "-" + "MAH=Horizontal manoeuvre";
            }
            if (TrackStatus.Substring(5, 1) == "0")
            {
                this.TrackStatus = this.TrackStatus + "-" + "TCC=Tracking performed in 'Sensor Plane', i.e. neither slant range correction nor projection was applied.";
            }
            if (TrackStatus.Substring(5, 1) == "1")
            {
                this.TrackStatus = this.TrackStatus + "-" + "TCC=Slant range correction and a suitable projection technique are used to track in a 2D.reference plane, tangential to the earth model at the Sensor Site co-ordinates.";
            }
            if (TrackStatus.Substring(6, 1) == "0")
            {
                this.TrackStatus = this.TrackStatus + "-" + "STH=Measured position";
            }
            if (TrackStatus.Substring(6, 1) == "1")
            {
                this.TrackStatus = this.TrackStatus + "-" + "STH=Smoothed position";
            }
            if (TrackStatus.Substring(7, 1) == "1")
            {
                if (TrackStatus.Substring(8, 2) == "00")
                {
                    this.TrackStatus = this.TrackStatus + "-" + "TOM=Unknown type of movement";
                }
                if (TrackStatus.Substring(8, 2) == "01")
                {
                    this.TrackStatus = this.TrackStatus + "-" + "TOM=Taking-off";
                }
                if (TrackStatus.Substring(8, 2) == "10")
                {
                    this.TrackStatus = this.TrackStatus + "-" + "TOM=Landing";
                }
                if (TrackStatus.Substring(8, 2) == "11")
                {
                    this.TrackStatus = this.TrackStatus + "-" + "TOM=Other types of movement";
                }
                if (TrackStatus.Substring(10, 3) == "000")
                {
                    this.TrackStatus = this.TrackStatus + "-" + "DOU=No doubt";
                }
                if (TrackStatus.Substring(10, 3) == "001")
                {
                    this.TrackStatus = this.TrackStatus + "-" + "DOU=Doubtful correlation (undetermined reason)";
                }
                if (TrackStatus.Substring(10, 3) == "010")
                {
                    this.TrackStatus = this.TrackStatus + "-" + "DOU=Doubtful correlation in clutter";
                }
                if (TrackStatus.Substring(10, 3) == "011")
                {
                    this.TrackStatus = this.TrackStatus + "-" + "DOU=Loss of accuracy";
                }
                if (TrackStatus.Substring(10, 3) == "100")
                {
                    this.TrackStatus = this.TrackStatus + "-" + "DOU=Loss of accuracy in clutter";
                }
                if (TrackStatus.Substring(10, 3) == "101")
                {
                    this.TrackStatus = this.TrackStatus + "-" + "DOU=Unstable track";
                }
                if (TrackStatus.Substring(10, 3) == "110")
                {
                    this.TrackStatus = this.TrackStatus + "-" + "DOU=Previously coasted";
                }
                if (TrackStatus.Substring(13, 2) == "00")
                {
                    this.TrackStatus = this.TrackStatus + "-" + "MRS=Merge or split indication undetermined";
                }
                if (TrackStatus.Substring(13, 2) == "01")
                {
                    this.TrackStatus = this.TrackStatus + "-" + "MRS=Track merged by association to plot";
                }
                if (TrackStatus.Substring(13, 2) == "10")
                {
                    this.TrackStatus = this.TrackStatus + "-" + "MRS=Track merged by non-association to plot";
                }
                if (TrackStatus.Substring(13, 2) == "11")
                {
                    this.TrackStatus = this.TrackStatus + "-" + "MRS=Split track";
                }
                if (TrackStatus.Substring(15, 1) == "1")
                {
                    if (TrackStatus.Substring(16, 1) == "0")
                    {
                        this.TrackStatus = this.TrackStatus + "-" + "GHO=Default";
                    }
                    if (TrackStatus.Substring(16, 1) == "1")
                    {
                        this.TrackStatus = this.TrackStatus + "-" + "GHO=Ghost track";
                    }
                }
            }
            return this.TrackStatus;
        }
        public string GetMode3A(string OctetsMode3A)
        {
            if (OctetsMode3A.Substring(0, 1) == "0")
            {
                this.Mode3A = "V=Code Validated";
            }
            if (OctetsMode3A.Substring(0, 1) == "1")
            {
                this.Mode3A = "V=Code Validated";
            }
            if (OctetsMode3A.Substring(1, 1) == "0")
            {
                this.Mode3A = this.Mode3A + "-" + "G=Default";
            }
            if (OctetsMode3A.Substring(1, 1) == "1")
            {
                this.Mode3A = this.Mode3A + "-" + "G=Garbled Mode";
            }
            if (OctetsMode3A.Substring(2, 1) == "0")
            {
                this.Mode3A = this.Mode3A + "-" + "L=Mode-3/A code derived from the reply of the transponder";
            }
            if (OctetsMode3A.Substring(2, 1) == "1")
            {
                this.Mode3A = this.Mode3A + "-" + "L=Mode-3/A code not extracted during the last scan";
            }
            int Reply = Convert.ToInt32(OctetsMode3A.Substring(4, 12), 2);
            string octal = Convert.ToString(Reply, 8);
            this.Mode3A = this.Mode3A + "-Mode-3/A Reply=" + octal;
            return this.Mode3A;
        }
        //public string GetTargetAddress(string OctetsTargetAddress)
        {
            int TargetAddress = Convert.ToInt32(OctetsTargetAddress, 2);
            string hexadec = Convert.ToString(TargetAddress, 16);
            this.TargetAddress = hexadec;
            return this.TargetAddress;
        }

        public string GetTargetIdentification(string OctetsTargetIndentification)
        {
            if (OctetsTargetIndentification.Substring(0, 2) == "00")
            {
                this.TargetIdentification = "STI=Callsign or registration downlinked from transponder";
            }
            if (OctetsTargetIndentification.Substring(0, 2) == "01")
            {
                this.TargetIdentification = "STI=Callsign not downlinked from transponder";
            }
            if (OctetsTargetIndentification.Substring(0, 2) == "10")
            {
                this.TargetIdentification = "STI=Registration not downlinked from transponder";
            }
            string callsign = null;
            int i = 1;
            int pos = 8;
            bool encontrado = false;
            while (i < 9)
            {

                if (OctetsTargetIndentification.Substring(pos, 6) == "000001" && encontrado == false)
                {
                    callsign = callsign + "A";
                    encontrado = true;
                }
                if (OctetsTargetIndentification.Substring(pos, 6) == "000010" && encontrado == false)
                {
                    callsign = callsign + "B";
                    encontrado = true;
                }
                if (OctetsTargetIndentification.Substring(pos, 6) == "000011" && encontrado == false)
                {
                    callsign = callsign + "C";
                    encontrado = true;
                }
                if (OctetsTargetIndentification.Substring(pos, 6) == "000100" && encontrado == false)
                {
                    callsign = callsign + "D";
                    encontrado = true;
                }
                if (OctetsTargetIndentification.Substring(pos, 6) == "000101" && encontrado == false)
                {
                    callsign = callsign + "E";
                    encontrado = true;
                }
                if (OctetsTargetIndentification.Substring(pos, 6) == "000110" && encontrado == false)
                {
                    callsign = callsign + "F";
                    encontrado = true;
                }
                if (OctetsTargetIndentification.Substring(pos, 6) == "000111" && encontrado == false)
                {
                    callsign = callsign + "G";
                    encontrado = true;
                }
                if (OctetsTargetIndentification.Substring(pos, 6) == "001000" && encontrado == false)
                {
                    callsign = callsign + "H";
                    encontrado = true;
                }
                if (OctetsTargetIndentification.Substring(pos, 6) == "001001" && encontrado == false)
                {
                    callsign = callsign + "I";
                    encontrado = true;
                }
                if (OctetsTargetIndentification.Substring(pos, 6) == "001010" && encontrado == false)
                {
                    callsign = callsign + "J";
                    encontrado = true;
                }
                if (OctetsTargetIndentification.Substring(pos, 6) == "001011" && encontrado == false)
                {
                    callsign = callsign + "K";
                    encontrado = true;
                }
                if (OctetsTargetIndentification.Substring(pos, 6) == "001100" && encontrado == false)
                {
                    callsign = callsign + "L";
                    encontrado = true;
                }
                if (OctetsTargetIndentification.Substring(pos, 6) == "001101" && encontrado == false)
                {
                    callsign = callsign + "M";
                    encontrado = true;
                }
                if (OctetsTargetIndentification.Substring(pos, 6) == "001110" && encontrado == false)
                {
                    callsign = callsign + "N";
                    encontrado = true;
                }
                if (OctetsTargetIndentification.Substring(pos, 6) == "001111" && encontrado == false)
                {
                    callsign = callsign + "O";
                    encontrado = true;
                }
                if (OctetsTargetIndentification.Substring(pos, 6) == "010000" && encontrado == false)
                {
                    callsign = callsign + "P";
                    encontrado = true;
                }
                if (OctetsTargetIndentification.Substring(pos, 6) == "010001" && encontrado == false)
                {
                    callsign = callsign + "Q";
                    encontrado = true;
                }
                if (OctetsTargetIndentification.Substring(pos, 6) == "010010" && encontrado == false)
                {
                    callsign = callsign + "R";
                    encontrado = true;
                }
                if (OctetsTargetIndentification.Substring(pos, 6) == "010011" && encontrado == false)
                {
                    callsign = callsign + "S";
                    encontrado = true;
                }
                if (OctetsTargetIndentification.Substring(pos, 6) == "010100" && encontrado == false)
                {
                    callsign = callsign + "T";
                    encontrado = true;
                }
                if (OctetsTargetIndentification.Substring(pos, 6) == "010101" && encontrado == false)
                {
                    callsign = callsign + "U";
                    encontrado = true;
                }
                if (OctetsTargetIndentification.Substring(pos, 6) == "010110" && encontrado == false)
                {
                    callsign = callsign + "V";
                    encontrado = true;
                }
                if (OctetsTargetIndentification.Substring(pos, 6) == "010111" && encontrado == false)
                {
                    callsign = callsign + "W";
                    encontrado = true;
                }
                if (OctetsTargetIndentification.Substring(pos, 6) == "011000" && encontrado == false)
                {
                    callsign = callsign + "X";
                    encontrado = true;
                }
                if (OctetsTargetIndentification.Substring(pos, 6) == "011001" && encontrado == false)
                {
                    callsign = callsign + "Y";
                    encontrado = true;
                }
                if (OctetsTargetIndentification.Substring(pos, 6) == "011010" && encontrado == false)
                {
                    callsign = callsign + "Z";
                    encontrado = true;
                }
                if (OctetsTargetIndentification.Substring(pos, 6) == "110000" && encontrado == false)
                {
                    callsign = callsign + "0";
                    encontrado = true;
                }
                if (OctetsTargetIndentification.Substring(pos, 6) == "110001" && encontrado == false)
                {
                    callsign = callsign + "1";
                    encontrado = true;
                }
                if (OctetsTargetIndentification.Substring(pos, 6) == "110010" && encontrado == false)
                {
                    callsign = callsign + "2";
                    encontrado = true;
                }
                if (OctetsTargetIndentification.Substring(pos, 6) == "110011" && encontrado == false)
                {
                    callsign = callsign + "3";
                    encontrado = true;
                }
                if (OctetsTargetIndentification.Substring(pos, 6) == "110100" && encontrado == false)
                {
                    callsign = callsign + "4";
                    encontrado = true;
                }
                if (OctetsTargetIndentification.Substring(pos, 6) == "110101" && encontrado == false)
                {
                    callsign = callsign + "5";
                    encontrado = true;
                }
                if (OctetsTargetIndentification.Substring(pos, 6) == "110110" && encontrado == false)
                {
                    callsign = callsign + "6";
                    encontrado = true;
                }
                if (OctetsTargetIndentification.Substring(pos, 6) == "110111" && encontrado == false)
                {
                    callsign = callsign + "7";
                    encontrado = true;
                }
                if (OctetsTargetIndentification.Substring(pos, 6) == "111000" && encontrado == false)
                {
                    callsign = callsign + "8";
                    encontrado = true;
                }
                if (OctetsTargetIndentification.Substring(pos, 6) == "111001" && encontrado == false)
                {
                    callsign = callsign + "9";
                    encontrado = true;
                }
                pos = pos + 6;
                i++;
                encontrado = false;
            }
            this.TargetIdentification = this.TargetIdentification + "-Callsign=" + callsign;
            return this.TargetIdentification;
        }
        public string GetFlightLevel(string OctetsFlightLevel)
        {
            if (OctetsFlightLevel.Substring(0, 1) == "0")
            {
                this.FlightLevel = "V=Code validated";
            }
            if (OctetsFlightLevel.Substring(0, 1) == "1")
            {
                this.FlightLevel = "V=Code not validated";
            }
            if (OctetsFlightLevel.Substring(1, 1) == "0")
            {
                this.FlightLevel = this.FlightLevel + "-" + "G=Default";
            }
            if (OctetsFlightLevel.Substring(1, 1) == "1")
            {
                this.FlightLevel = this.FlightLevel + "-" + "G=Garbled code";
            }
            int FlightLevel = Convert.ToInt32(OctetsFlightLevel.Substring(2, 14), 2);
            this.FlightLevel = this.FlightLevel + "-Flight Level=" + ((double)0.25 * FlightLevel).ToString() + "FL";
            return this.FlightLevel;
        }
        public string GetTargetSizeAndOrientation(string Octets)
        {
            int Length = Convert.ToInt32(Octets.Substring(0, 7), 2);
            this.TargetSizeAndOrientation = "Length=" + Length.ToString() + "m";
            if (Octets.Substring(7, 1) == "1")
            {
                double Orientation = (double)Convert.ToInt32(Octets.Substring(8, 7), 2) * (double)2.8125;
                this.TargetSizeAndOrientation = this.TargetSizeAndOrientation + "-Orientation=" + Orientation.ToString() + "deg";
                if (Octets.Substring(15, 1) == "1")
                {
                    int Width = Convert.ToInt32(Octets.Substring(16, 7), 2);
                    this.TargetSizeAndOrientation = this.TargetSizeAndOrientation + "-Width=" + Width.ToString() + "m";
                }
            }
            return this.TargetSizeAndOrientation;
        }
        public string[] GetCalculatedAcceleration(string Octets)
        {
            double Ax1 = (double)Convert.ToInt32(Octets.Substring(0, 8), 2);
            double Ax = Ax1 * (double)0.25;
            if (Ax > 31)
            {
                Ax = Ax - 64;
            }
            double Ay1 = (double)Convert.ToInt32(Octets.Substring(8, 8), 2);
            double Ay = Ay1 * (double)0.25;
            if (Ay > 31)
            {
                Ay = Ay - 64;
            }
            string[] Acceleration = new string[2]; ;
            Acceleration[0] = Ax.ToString();
            Acceleration[1] = Ay.ToString();
            this.CalculatedAcceleration = Acceleration;
            return this.CalculatedAcceleration;
        }*/
    }

    public class CAT20
    {
        public int SAC;
        public int SIC;
        public string[] TargetReportDescription;
        public TimeSpan TimeofDay;
        public double[] CartesianPisition;
        public int TrackNumber;
        public string TrackStatus;
        public string Mode3A;
        public double[] CartesianTrackVelocity;
        public string[] FlightLevel;
        public string[] modeC;
        public string TargetAddress;
        public string TargetId;
        public double CartesianHeight;
        public double GeometricHeight;
        public double[] CalculatedAcceleration;
        public string VehicleFleetId;
        public string PreprogrammedMessage;
        public double[] DOPofPosition;
        public double[] StandardDeviationofPosition;
        public double[] StandardDeviationofHeigh;
    }
    public class CAT21
    {
        public int SAC;
        public int SIC;
        public TimeSpan TimeUTC;
        public string TargetReportDescriptor;
        public string TargetAddress;
        public string FigureOfMerit;
        public string VelocityAccurancy;
        public string PositionWGS84; //LAT i LON
        public string FlightLevel;
        public string GeometricalVerticalRate;
        public string AirboneGroundVector; //GroundSpeed&TrackAngle
        public string TargetIdentification;
        public string LinkTechnologyIndicator;
    }

    public class Flight
    {
        public string ID;
        public TimeSpan TimeofDay;
        public double[] CartesianPosition;
    }
}