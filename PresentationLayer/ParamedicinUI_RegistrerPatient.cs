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
            patientData = new string[4]; // Patientens data hentet fra CPR-register
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
                    a = -a; // modvirker crash ved negativ værdi fra getDiff
                
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
                    switch (m) // De fire valgmuligheder fra Register patient
                    {
                        case 0:
                            {
                                Program.mainMenu();
                                break;
                            }
                        case 1:
                            {
                                patientData = RPcontrol.cardScan("0101010101"); // Det er kun patient, der har sygesikrin med;) 
                                Program.CPRNumber = "0101010101"; // Ellers skulle en scanner få CPR fra sygesikring.                               
                                displayValidatedPatient(patientData);
                                break;
                            }
                        case 2:
                            {
                                //Her skal indtastning af CPR_nummer ske
                                Program.CPRNumber = indtatstCPR();
                                patientData = RPcontrol.registrerPatient(Program.CPRNumber);
                                displayValidatedPatient(patientData);
                                break;
                            }
                        case 3:
                            {
                                patientData = RPcontrol.defaultPatient("9999990000");
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
                Display.lcdPrint("        ");
                int a = Encoder.getDiff(true);
                if (a < 0)
                    a = -a;

                for (int i = a; i >= 0; i = i - 13) // Der er 13 valgmuligheder ved indtast af CPR, 0-9, Clear, Tilbage og Bekraeft
                {
                    if (i < 10) // ciffer indtastning
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
                    if(i==12)
                    {
                        Display.lcdGotoXY(0, 1);
                        Display.lcdPrint("Bekraeft");
                        l = Convert.ToByte(i);
                    }
                }
                if (Encoder.isPressed() == true && l < 10) // tilføjer valgte ciffer til CPR-string
                {
                    cprN += tal;
                    Display.lcdGotoXY(0, 2);
                    Display.lcdPrint(cprN);
                }
                if (Encoder.isPressed() == true && l == 10) // CPR-ryddes
                {
                    cprN = "";
                    Display.lcdGotoXY(0, 2);
                    Display.lcdPrint("               ");
                }
                if (Encoder.isPressed() == true && l == 11) // CPR-ryddes og der vendes retur til hovedmenu
                {
                    cprN = "";
                    Program.mainMenu();
                    break;
                }
                Display.lcdGotoXY(0, 2);
                Display.lcdPrint(cprN); // Det foreløbelige cprN vises på display
                if (cprN.Length == 10 && l ==12 && Encoder.isPressed()) // Ved tryk på bekraeft og cprN.lenght =10 gemmes cprN, og der brydes ud.
                {
                    break;
                }

            }           
            return cprN; // Metoden sendes retur til hovedmenu
        }

        public void displayValidatedPatient(string[] displayP)
        {
            //Patient data udskrives, og gemmes så det kan komme i databasen.
            Display.lcdClear();
            for (byte i = 0; i < displayP.Length; i++)
            {
                Display.lcdGotoXY(0, i);
                Display.lcdPrint(displayP[i]);
            }
            string[] forOgEfternavn = patientData[1].Split(' ');
            Program.BorgerFornavn = forOgEfternavn[0];
            Program.BorgerEfternavn = forOgEfternavn[1];
            Program.CPRNumber = patientData[0];

            while (true)
            {
                if(Encoder.isPressed()) // afventer tryk før returnering til hovedmenu
                {
                    break;
                }              
                
            }
            Program.mainMenu();            
        }
    }
}


