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
        private int maalingID;
        public MeasureECGControl()
        {
            ecgData = new ECGData();
        }
        public void startECG()
        {
            //måske den skal slettes herfra igen
        }
        public DTO_Measurement GetLokalinfo()
        {
            DTO_Measurement info = ecgData.lokalmaaling();
            return info;
        }
        public bool analyzeECG(double[] ECGMaalinger_)
        {
            bool RPianalyseretSTEMI = false;
            //Her skal ECGMaalinger_ doublearray analyseres for stemi
            //Hvis der er muligt STEMI sættes den til true, ellers false

            return RPianalyseretSTEMI;

        }
        public int convertToBlobAndUpload(DTO_Measurement nyMaaling)
        {
            //Konvertering sker direkte i datalag
            nyMaaling._STEMI_suspected=analyzeECG(nyMaaling._lokalECG);
            maalingID= ecgData.uploadECG(nyMaaling);
            return maalingID;
        }
        public int confirmSTEMI(string maalingID) //tjekker hele tiden efter om der er ændringer på den aktuelle plads i databasen
        {
            int result = 0;
            result=ecgData.doctorAnalyses(maalingID);
            //skal returnere true eller false alt efter hvad lægen har uploaded til database af analyse. 
            return result;
        }

   }
}
