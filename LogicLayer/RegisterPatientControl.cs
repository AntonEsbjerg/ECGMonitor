using System;
using System.Collections.Generic;
using System.Text;
using DataLayer;
using DTO;

namespace LogicLayer
{
    public class RegisterPatientControl
    {
        private string CPRNumber;
        private CPR_Register cprR;
        private List<DTO_Patient> patientData;
        string[] ikkeRegistreretP = new string[2] { "Personnummer ikke", "genkendt" };
        Person p;

        public RegisterPatientControl()
        {
            cprR = new CPR_Register();
            p = new Person();
            patientData = new List<DTO_Patient>();
            // ved ikke helt, hvad der skal implementeres
        }
        public string[] cardScan(string CPRNumber_)
        {
            CPRNumber = CPRNumber_;
            //Parameter skulle have været "card"
            //Metode der skulle have scannet sygesikring
            //skal kalde validate, hvis true, så kald validated
            
            if (cprR.validate(CPRNumber) == true)
                return patientValidated();
            else
            {
                return ikkeRegistreretP;
            }
        }
        public string[] registrerPatient(string CPRNumber_)
        {
            CPRNumber = CPRNumber_;

            if (cprR.validate(CPRNumber) == true)
                return patientValidated();
            else
            {
                return ikkeRegistreretP;
            }

        }
        public string[] defaultPatient(string defaultpatient)
        {
            CPRNumber = defaultpatient;
            if (cprR.validate(CPRNumber) == true)
                return patientValidated();
            else
            {
                return ikkeRegistreretP;
            }
            //Oprettes en default patient med unikt ID
            //Oprettes en ny person
        }
        public string[] patientValidated()
        {
            patientData = p.findData(CPRNumber);
            string[] patienArr = new string[4];
            foreach(var data in patientData)
            {
                patienArr[0] = data.CPRnummer_;
                patienArr[1] = data.Navn_;
                patienArr[2] = data.Telfon_;
                patienArr[3] = data.Addresse_;
            }

            return patienArr;
        }
    }

}
