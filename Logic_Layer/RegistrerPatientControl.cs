using System;
using System.Collections.Generic;
using System.Text;
using DataLayer;
using DTO;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;

namespace LogicLayer
{
   public class RegistrerPatientControl
   { 
      private CPR_Register cprR;
      private List<DTO_Patient> patientData;
      string[] ikkeRegistreretP = new string[2] { "Personnummer ikke", "genkendt" };
      private Person p;
      public DTO_Patient patient;

      public RegistrerPatientControl()
      {         
         cprR = new CPR_Register();
         p = new Person();
         patientData = new List<DTO_Patient>();

      }
      public string[] cardScan(string CPRNumber_)
      {
         //Parameter skulle have været "card"
         //Metode der skulle have scannet sygesikring
         //skal kalde validate, hvis true, så kald validated
         if (cprR.validate(CPRNumber_) == true)
            return patientValidated(CPRNumber_);
         else
         {
            return ikkeRegistreretP;
         }
      }
      public string[] registrerPatient(string CPRNumber_)
      {
         if (cprR.validate(CPRNumber_) == true)
            return patientValidated(CPRNumber_);
         else
         {
            return ikkeRegistreretP;
         }

      }
      public string[] defaultPatient(string defaultpatient)
      {
         if (cprR.validate(defaultpatient) == true)
         {
            return patientValidated(defaultpatient);
         }
         else
         {
            return ikkeRegistreretP;
         }
      }
      public string[] patientValidated(string CPRNumber_)
      {         
         patientData = p.findData(CPRNumber_);
         string[] patienArr = new string[4];
         foreach (var data in patientData)
         {
            patienArr[0] = data.CPRnummer_;
            patienArr[1] = data.Fornavn_+" "+ data.Efternavn_;
            patienArr[2] = data.Telfon_;
            patienArr[3] = data.Addresse_;
         }         
         return patienArr;
      }
   }

}
