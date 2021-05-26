using RaspberryPiCore.ADC;
using RaspberryPiCore.JoySticks;
using RaspberryPiCore.LCD;
using RaspberryPiCore.TWIST;
using System;
using System.Threading;
using LogicLayer;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;

namespace PresentationLayer
{
    class ParamedicinUI_RegistrerPatient
    {
        static SerLCD Display;
        static TWIST Encoder;
        private RegistrerPatientControl RPcontrol;
        private string CPRNumber1;
        private string[] patientData;
        MeasureECGControl eCGControl = new MeasureECGControl();


        public ParamedicinUI_RegistrerPatient()
        {
            patientData = new string[4];
            Display = new SerLCD();
            Encoder = new TWIST();
            RPcontrol = new RegistrerPatientControl();
        }
        public void registrerPatientMenu()
        {
            byte m = 0;
            Display.lcdClear();
            string[] rpMenu = new string[4] { "Registrer Patient:", "Scan sygesikring", "Indtast CPR-nummer", "Default Patient"};
            byte c = 0;

            foreach (var item in rpMenu) // rpMenu bliver indlæst
            {
                if (c < 4)
                {
                    Display.lcdGotoXY(0, c);
                    Display.lcdPrint(rpMenu[c]);
                    c++;
                }
            }

            Display.lcdHome(); // curserblink sættes til 0,0

            while (true)
            { 
                int a = Encoder.getDiff(true);
                if (a < 0)
                    a = -a;
                
                for (int i = a; i >= 0; i = i - 4) // samme metode som main
                {
                    if (i < 4)
                    {
                        m = Convert.ToByte(i); 
                        Display.lcdGotoXY(0, m);
                    }
                }

                if (Encoder.isPressed() == true)
                {
                    switch (m)
                    {
                        case 0:
                            {
                                Program.mainMenu();
                                break;
                            }
                        case 1:
                            {
                                patientData = RPcontrol.cardScan("9999990000");
                                Program.CPRNumber = "9999990000";
                                displayValidatedPatient(patientData);
                                break;
                            }

                        case 2:
                            {
                                //Her skal indtastning af CPR_nummer ske
                                Program.CPRNumber = indtatstCPR();
                                CPRNumber1 = Program.CPRNumber;
                                //CPRNumber1 = indtatstCPR();
                                patientData = RPcontrol.registrerPatient(CPRNumber1);
                                displayValidatedPatient(patientData);
                                break;
                            }

                        case 3:
                            {
                                patientData = RPcontrol.defaultPatient("0101010101");
                                Program.CPRNumber = "0101010101";
                                displayValidatedPatient(patientData);
                                break;
                            }
                    }
                }                   
            }
        }

        public string indtatstCPR()
        {
            string cprN = "";
            byte l = 0;
            string tal = "";
            Display.lcdClear();
            Display.lcdHome();
            Display.lcdPrint("Indtast CPR-Numner");
            Display.lcdGotoXY(0, 1);
            Display.lcdBlink();

            while (true)
            {
                Display.lcdGotoXY(0, 1);
                Display.lcdPrint("       ");
                int a = Encoder.getDiff(true);
                if (a < 0)
                    a = -a;

                for (int i = a; i >= 0; i = i - 12)
                {

                    if (i < 10)
                    {
                        l = Convert.ToByte(i);
                        Display.lcdGotoXY(0, 1);
                        tal = Convert.ToString(l);
                        Display.lcdPrint(tal);
                    }
                    if (i == 10)
                    {
                        Display.lcdGotoXY(0, 1);
                        Display.lcdPrint("Clear");
                        l = Convert.ToByte(i);
                    }
                    if (i == 11)
                    {
                        Display.lcdGotoXY(0, 1);
                        Display.lcdPrint("Tilbage");
                        l = Convert.ToByte(i);
                    }
                }
                if (Encoder.isPressed() == true && l < 10)
                {
                    cprN += tal;
                    Display.lcdGotoXY(0, 2);
                    Display.lcdPrint(cprN);
                }
                if (Encoder.isPressed() == true && l == 10)
                {
                    cprN = "";
                    Display.lcdGotoXY(0, 2);
                    Display.lcdPrint("              ");
                }
                if (Encoder.isPressed() == true && l == 11)
                {
                    cprN = "";
                    break;
                }
                Display.lcdGotoXY(0, 2);
                Display.lcdPrint(cprN);
                if (cprN.Length == 10 && Encoder.isPressed())
                {
                    break;
                }

            }
            eCGControl.GetLokalinfo()._borger_cprnr = cprN;
            eCGControl.GetLokalinfo()._borger_fornavn = patientData[1];
            eCGControl.GetLokalinfo()._borger_efternavn = patientData[2];
            return cprN;
        }

        public void displayValidatedPatient(string[] displayP)
        {
            Display.lcdClear();
            for (byte i = 0; i < displayP.Length; i++)
            {
                Display.lcdGotoXY(0, i);
                Display.lcdPrint(displayP[i]);
            }

            while(true)
            {

            }
            //Patienten skal her vises på skærmen
            //Hvordan skal den ritige patient kunne vises på skærmen?
        }
    }
}


