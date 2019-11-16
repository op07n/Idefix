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
        public int SAC;
        public int SIC;
        public string MessageType;
        public string[] TargetReportDescriptor;
        public TimeSpan TimeofDay;
        public double[] PolarPosition;
        public double[] CartesianPosition; //Per plotejar
        public double[] PolarTrackVelocity;
        public double[] CartesianTrackVelocity; //Per plotejar??
        public int TrackNumber;
        public string[] TrackStatus;
        public string[] Mode3A;
        public string TargetAddress;
        public string[] TargetIdentification;
        public string[] FlightLevel;
        public double[] TargetSizeAndOrientation;
        public string[] SystemStatus;
        public double[] CalculatedAcceleration;



        public CAT10(int SIC, int SAC, String MsgType, String[] TargetReportDescriptor, TimeSpan TimeofDay, double[] PolarPosition, double[] CartesianPosition, double[] PolarTrackVelocity,double[] CartesianTrackVelocity, int TrackNumber, string[] TrackStatus, string[] Mode3A, string ICAO_Address, string[] TargetID, string[] FL, double[] TargetSizeAndOrientation, string [] SystemStatus, double[] CalculatedAcceleration)
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
            this.Mode3A = Mode3A;
            this.TargetAddress = ICAO_Address;
            this.TargetIdentification = TargetID;
            this.FlightLevel = FL;
            this.TargetSizeAndOrientation = TargetSizeAndOrientation;
            this.SystemStatus = SystemStatus;
            this.CalculatedAcceleration = CalculatedAcceleration;
         }
         
    }

    public class CAT20
    {
        public int SAC;
        public int SIC;
        public string[] TargetReportDescriptor;
        public TimeSpan TimeofDay;
        public double[] CartesianPosition;
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


        public CAT20(int SIC, int SAC, String[] TargetReportDescriptor, TimeSpan TimeofDay, double[] CartesianPosition, int TrackNumber, string TrackStatus, double[] TargetSizeAndOrientation, string[] SystemStatus, double[] CalculatedAcceleration)
        {
            this.SIC = SIC;
            this.SAC = SAC;
            this.TargetReportDescriptor = TargetReportDescriptor;
            this.TimeofDay = TimeofDay;
            this.CartesianPosition = CartesianPosition;
            this.CartesianTrackVelocity = CartesianTrackVelocity;
            this.TrackNumber = TrackNumber;
            this.TrackStatus = TrackStatus;
            this.TargetSizeAndOrientation = TargetSizeAndOrientation;
            this.SystemStatus = SystemStatus;
            this.CalculatedAcceleration = CalculatedAcceleration;
        }
    }


 
    public class CAT21
    {
        public int SAC;
        public int SIC;
        public TimeSpan TimeofDay;
        public string TargetReportDescriptor;
        public string TargetAddress;
        public string FigureOfMerit;
        public string VelocityAccurancy;
        public double[] PositionWGS84; //LAT i LON
        public string FlightLevel;
        public string GeometricalVerticalRate;
        public string AirboneGroundVector; //GroundSpeed&TrackAngle
        public string TargetId;
        public string LinkTechnologyIndicator;
    }

    public class Flight
    {
        public string ID;
        public TimeSpan TimeofDay;
        public double[] CartesianPosition;
    }
}