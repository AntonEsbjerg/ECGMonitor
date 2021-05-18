using System;
using System.Collections.Generic;
using System.Text;
using DataLayer;
using DTO;
using System.Threading;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;

namespace LogicLayer
{
   public class MeasureECGControl
   {
        private ECGData ecgData;
        public MeasureECGControl()
        {
            ecgData = new ECGData();
        }
        public void startECG()
        {
            //måske den skal slettes herfra igen
        }
        public bool analyzeECG(double[] ECGMaalinger_)
        {
            bool RPianalyseretSTEMI = false;
            //Her skal ECGMaalinger_ doublearray analyseres for stemi
            //Hvis der er muligt STEMI sættes den til true, ellers false

            return RPianalyseretSTEMI;

        }
        public void convertToBlobAndUpload(double[] ECGMaalinger_)
        {
            //Konvertering sker direkte i datalag
            ecgData.uploadECG(ECGMaalinger_);
        }
        public int confirmSTEMI(string CPRNumber) //tjekker hele tiden efter om der er ændringer på den aktuelle plads i databasen
        {
            int result = 0;
            ecgData.doctorAnalyses(CPRNumber);
            //skal returnere true eller false alt efter hvad lægen har uploaded til database af analyse. 
            return result;
        }

   }
}
