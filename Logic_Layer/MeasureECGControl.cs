using System;
using System.Collections.Generic;
using System.Text;
using DataLayer;
using DTO;
using System.Threading;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using Extreme.Statistics;
using Extreme.DataAnalysis;
using Extreme.Mathematics;

namespace LogicLayer
{
   public class MeasureECGControl
   {
        private ECGData ecgData;
        private int maalingID;
        public MeasureECGControl()
        {
            ecgData = new ECGData();
            maalingID = new int();
        }        
        public DTO_Measurement GetLokalinfo()
        {
            DTO_Measurement info = ecgData.lokalmaaling();
            return info;
        }
        public bool analyzeECG(double[] ECGMaalinger_)
        {
             int Rtak = 0;
             double threshold_Rtak = 0.5;
             bool belowRtak_Threshold = true;
             double threshold_STsegment = 0.1;
             double baseline = 0.0;
             List<double> RRList = new List<double>();
             double tidefterRtak = 0.0;
             double[] ekgarray = new double[500];
             bool RPianalyseretSTEMI = false;
             //Her skal ECGMaalinger_ doublearray analyseres for stemi
             //Hvis der er muligt STEMI sættes den til true, ellers false

            //opretter histogram til at finde baseline opretter 76 bins svarende til intervaller af 0.1 mV
            var histogram1 = Histogram.CreateEmpty(-1.8, 5.8, 76);

            //histogrammet fyldes:
            for (int i = 0; i < ECGMaalinger_.Length; i++)
            {
              histogram1.Increment(ECGMaalinger_[i]);
            }
        
            //vi finder baseline ved at finde max antal observationer
             var max = histogram1.MaxIndex();
             Interval<double> bin = histogram1.Bins[max];
             baseline = bin.LowerBound + bin.Width / 2;

             for (int i = 0; i < ECGMaalinger_.Length; i++)
             {
                if (ECGMaalinger_[i] > baseline + threshold_Rtak && belowRtak_Threshold == true)
                {
                   Rtak = i;
                }
                if (Rtak != 0)
                {
                   tidefterRtak += 0.02;
                }
                if (ECGMaalinger_[i] < threshold_Rtak + baseline)
                {
                   belowRtak_Threshold = true;
                }
                else
                {
                   belowRtak_Threshold = false;
                }
                if (Rtak != 0 && ECGMaalinger_[i] > baseline + threshold_STsegment && 0.07 < tidefterRtak && tidefterRtak < 0.17)
                {
                   RPianalyseretSTEMI = true;
                   break;
                }
                else
                {
                   RPianalyseretSTEMI = false;
                }
             }
            return RPianalyseretSTEMI;
        }
        public int convertToBlobAndUpload(DTO_Measurement nyMaaling)
        {
            //Konvertering sker direkte i datalag
            
            maalingID = ecgData.uploadECG(nyMaaling);
            return maalingID;
        }
        public int confirmSTEMI(string maalingID) //tjekker hele tiden efter om der er ændringer på den aktuelle plads i databasen
        {
            int result = 2;
            result=ecgData.doctorAnalyses(maalingID);
            //skal returnere true eller false alt efter hvad lægen har uploaded til database af analyse. 
            return result;
        }
   }
}