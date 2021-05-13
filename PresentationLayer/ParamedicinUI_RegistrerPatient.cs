using RaspberryPiCore.ADC;
using RaspberryPiCore.JoySticks;
using RaspberryPiCore.LCD;
using RaspberryPiCore.TWIST;
using System;
using System.Threading;
using LogicLayer;

namespace PresentationLayer
{
    class ParamedicinUI_RegistrerPatient
    {
        static SerLCD Display;
        static TWIST Encoder;
        private RegisterPatientControl RPcontrol;
        private string CPRNumber1;
        private string[] patientData;
        public ParamedicinUI_RegistrerPatient()
        {
            patientData = new string[4];
            Display = new SerLCD();
            Encoder = new TWIST();
            RPcontrol = new RegisterPatientControl();

        }
        public void registrerPatientMenu()
        {
            byte m = 0;
            Display.lcdClear();
            string[] rpMenu = new string[4] { "Registrer Patient:", "Scan sygesikring", "Indtast CPR-nummer", "Default Patient"};
            byte c = 0;

            //string[] rpMenu = new string[5] { "Registrer Patient:", "Scan sygesikring", "Indtast CPR-nummer", "Default Patient","Tilbage"}; //pt en test
            //Display.lcdClear();
            //Display.lcdBlink();            
            //int plads =0;
            //int grænse = 1;
            //// string list. tæller der fortæller om hvad, der skal stå i starten og de næste 4
            //// når der scrolles skal tælles sættes en op
            //// funktion efter 4
            ////bryd while løkke 
            //// Uden om den skal der ligge en ny while løkke, der skriver ud fra en ny start position på liste
            //// ved 5 er start position 1 i stedet for nul 
            //// ved scroll den anden gang (op på listen)
            //// tjek end ved start position. different + start position 
            //// grænseværdier ved den inderste while, så den bliver 
            //// inderste sætte rnår djeg uden for grænser
            //// inderste  scrol uden for ud i den yderste while

            //while(true)
            //{
            //    Display.lcdClear();
            //    for(byte i=0;i<4;i++)
            //    {
            //        byte y = Convert.ToByte(i + plads);
            //        Display.lcdGotoXY(0, i);
            //        Display.lcdPrint(rpMenu[y]);
            //    }




            //    while (true)
            //    {
            //        int a = Encoder.getDiff(true);
            //        if (a < 0)
            //            a = -a;

            //        for (int i = a; i >= 0; i = i - 5) // samme metode som main
            //        {
            //            if (i < 5)//måske tilbage til 4
            //            {
            //                m = Convert.ToByte(i);
            //                grænse = i;
            //                byte d = Convert.ToByte(i);                        
            //                Display.lcdGotoXY(0, d);
            //                Display.lcdBlink();
            //            }
            //        }
            //        if (grænse < 1)
            //        {
            //            plads = 0;
            //            grænse = 1;
            //            break;
            //        }
            //        if (grænse > 3)
            //        {
            //            plads = 1;
            //            grænse = 1;
            //            break;
            //        }
            //        if (Encoder.isPressed() == true)
            //        {
            //            switch (m)
            //            {
            //                case 1:
            //                    patientData = RPcontrol.cardScan("9999990000");
            //                    displayValidatedPatient(patientData);
            //                    break;
            //                case 2:
            //                    //Her skal indtastning af CPR_nummer ske
            //                    CPRNumber1 = "0101010101";
            //                    RPcontrol.registrerPatient(CPRNumber1);
            //                    break;
            //                case 3:
            //                    RPcontrol.defaultPatient("9999990000");
            //                    break;
            //                case 4:
            //                    Program.mainMenu();
            //                    break;
            //            }

            //        }
            //}

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
                //Her skal indtastning af CPR_nummer ske
                //CPRNumber1 = "1234567890";

                //get count 
                int a = Encoder.getDiff(true);
                if (a < 0)
                    a = -a;
                //Sprøg Lars om man kan have lade som der 5 på y-aksen
                for (int i = a; i >= 0; i = i - 4) // samme metode som main
                {
                    if (i < 4)
                    {
                        m = Convert.ToByte(i); // der lægges 1 til, da RegistererPatient ikke er relevant at vælge her 
                        Display.lcdGotoXY(0, m);
                        //if (m < 4)
                        //{
                        //    Display.lcdGotoXY(0, m);
                        //    Display.lcdBlink();
                        //}
                        //if (m == 4)
                        //{
                        //    Display.lcdClear();
                        //    Display.lcdHome();
                        //    Display.lcdPrint(rpMenu[m]);
                        //}

                    }
                }

                if (Encoder.isPressed() == true)
                {
                    switch (m)
                    {
                        case 1:
                            {
                                patientData = RPcontrol.cardScan("9999990000");
                                displayValidatedPatient(patientData);
                                break;
                            }

                        case 2:
                            {
                                //Her skal indtastning af CPR_nummer ske
                                 CPRNumber1 = "0101010101"; // her skal funktion med indstast cpr være
                                 patientData = RPcontrol.registrerPatient(CPRNumber1);
                                 displayValidatedPatient(patientData);

                                 break;
                            }

                        case 3:
                            {
                                patientData = RPcontrol.defaultPatient("9999990000");
                                displayValidatedPatient(patientData);
                                break;
                            }

                        //case 4:
                        //    Program.mainMenu();
                        //    break;
                    }

                }

                    //if (Encoder.isPressed() == true)
                    //{
                    //    switch (m)
                    //    {
                    //        case 1:
                    //            RPcontrol.cardScan();
                    //            break;
                    //        case 2:
                    //            //Her skal indtastning af CPR_nummer ske
                    //            CPRNumber1 = "1234567890";
                    //            RPcontrol.registrerPatient(CPRNumber1);
                    //            break;
                    //        case 3:
                    //            RPcontrol.defaultPatient();
                    //            break;
                    //        case 4:
                    //            Program.mainMenu();
                    //            break;
                    //    }

                    //    //skal der kunne komme tilbage til hovedmenu?
                    //    Program.mainMenu(); //Der behøver ikke oprettes et objekt, når det er static class. 

                    //}
                }





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


