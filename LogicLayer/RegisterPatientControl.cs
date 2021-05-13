using System;
using System.Collections.Generic;
using System.Text;
using DataLayer;

namespace LogicLayer
{
    public class RegisterPatientControl
    {
        private string CPRNumber;
        private CPR_Register cprR;

        public RegisterPatientControl()
        {
            cprR = new CPR_Register();
            // ved ikke helt, hvad der skal implementeres
        }
        public void cardScan()
        {
            //Parameter skulle have været "card"
            //Metode der skulle have scannet sygesikring
            //skal kalde validate, hvis true, så kald validated
            cprR.validate(CPRNumber);
            if (cprR.validate(CPRNumber) == true)
                patientValidated();
        }
        public void registrerPatient(string CPRNumber_)
        {
            CPRNumber = CPRNumber_;
            if (cprR.validate(CPRNumber) == true)
                patientValidated();

        }
        public void defaultPatient()
        {
            //Oprettes en default patient med unikt ID
            //Oprettes en ny person
        }
        public string patientValidated()
        {
            string a = "Patient"; //patienten skal hentes her
            return a;
        }
    }

}
