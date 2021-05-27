using RaspberryPiCore.ADC;
using RaspberryPiCore.JoySticks;
using RaspberryPiCore.LCD;
using RaspberryPiCore.TWIST;
using System;
using System.Threading;
using LogicLayer;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using DTO;

namespace PresentationLayer
{
    public class ParamedicinUI_RegistrerPatient
    {
        static SerLCD Display;
        static TWIST Encoder;
        private RegistrerPatientControl RPcontrol;
        private string[] patientData;

        public ParamedicinUI_RegistrerPatient()
        {
            patientData = new string[4]; // vi ønsker at få vist oplysninger på display
            Display = new SerLCD();
            Encoder = new TWIST();
            RPcontrol = new RegistrerPatientControl(); //opretter forbindelse til RP logicLayer
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
                                Program.mainMenu(); // det er muligt at vende tilbage ved at trykke på øverste
                                break;
                            }
                        case 1:
                            {
                                patientData = RPcontrol.cardScan("0101010101"); // Det er kun patient, der har sygesikrin med;) 
                                // ovenstående sender et CPR nummer med helt ned til datalag, hvorefter persondata sende retur. 
                                Program.CPRNumber = "0101010101"; // Ellers skulle en scanner få CPR fra sygesikring.                                
                                displayValidatedPatient(patientData); //metoder, der printer patientens data
                                break;
                            }
                        case 2:
                            {
                                Program.CPRNumber = indtatstCPR(); // Metode kaldes først, og her indtastes CPR
                                patientData = RPcontrol.registrerPatient(Program.CPRNumber); // det indtastede CPR-nummer sendes med
                                displayValidatedPatient(patientData);
                                break;
                            }
                        case 3:
                            {
                                patientData = RPcontrol.defaultPatient("9999990000"); //Dette er vores default CPR-Nr
                                Program.CPRNumber = "9999990000";                               
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
                Display.lcdPrint("        "); // sikrer at den gamle tekst hele tiden ryddes
                int a = Encoder.getDiff(true);
                if (a < 0)
                    a = -a; //modvirker crash ved negative værdier

                for (int i = a; i >= 0; i = i - 13) // Der er 13 valmuligheder. 0-9, clear, ryd og bekræft
                {
                    if (i < 10) //sætter hvert tal 0-9 - afhængig af cursor
                    {
                        l = Convert.ToByte(i);
                        Display.lcdGotoXY(0, 1); // Det sker på linje 2
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
                    if(i==12)
                    {
                        Display.lcdGotoXY(0, 1);
                        Display.lcdPrint("Bekraeft");
                        l = Convert.ToByte(i);
                    }
                }
                if (Encoder.isPressed() == true && l < 10)
                {
                    cprN += tal; // Det nye ciffer lægges oveni cprN
                    Display.lcdGotoXY(0, 2);
                    Display.lcdPrint(cprN); // Og skrives ud nedenunder, så det kan ses. 
                }
                if (Encoder.isPressed() == true && l == 10) //Rydder hvad der er gemt i cprN og sletter skærm
                {
                    cprN = "";
                    Display.lcdGotoXY(0, 2);
                    Display.lcdPrint("               ");
                }
                if (Encoder.isPressed() == true && l == 11) // rydder cprN og går tilbage til mainMenu uden at give nummer med
                {
                    cprN = "";
                    Program.mainMenu();
                    break;
                }
                Display.lcdGotoXY(0, 2);
                Display.lcdPrint(cprN); // skriver det aktuelle cprN på linje 3
                if (cprN.Length == 10 && l ==12 && Encoder.isPressed()) // Hvis cprN er 10 cifre og der trykkes bekræft, så retuneres cprN
                {
                    break;
                }

            }            
            return cprN; // sender cprN retur. Kun ved bekræft sendes et gyldigt cprN retur.
        }

        public void displayValidatedPatient(string[] displayP)
        {
            Display.lcdClear(); //metode motager string array og udskriver det
            for (byte i = 0; i < displayP.Length; i++)
            {
                Display.lcdGotoXY(0, i);
                Display.lcdPrint(displayP[i]);
            }
            string[] forOgEfternavn = patientData[1].Split(' ');
            Program.BorgerFornavn = forOgEfternavn[0];
            Program.BorgerEfternavn = forOgEfternavn[1];
            Program.CPRNumber = patientData[0];

            //BorgerFornavn = displayP[1];
            //BorgerEfternavn = displayP[2];

            //eCGControl.GetLokalinfo()._borger_cprnr = CPRNumber1;
            //eCGControl.GetLokalinfo()._borger_fornavn = displayP[1];
            //eCGControl.GetLokalinfo()._borger_efternavn = displayP[2];

            while (true)
            {
                if(Encoder.isPressed())
                {
                    break;
                }              
                
            }
            Program.mainMenu();
            //Patienten skal her vises på skærmen
            //Hvordan skal den ritige patient kunne vises på skærmen?
        }
    }
}


