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
    class Program
    {
        static SerLCD Display;
        static TWIST Encoder;
        private static ParamedicinUI_RegistrerPatient registrerPatientUI;
        private static ParamedicinUI_MeasureECG measureECGUI;
        private static ParamedicinUI_VisBatteristatus visBatteristatusUI; 
        public static string CPRNumber { get; set; }
        public static string CPRNumber1 { get; set; }
        public static string BorgerFornavn { get; set; }
        public static string BorgerEfternavn { get; set; }

        static void Main(string[] args)
        {
            MeasureECGControl eCGControl = new MeasureECGControl();
            Display = new SerLCD();
            Encoder = new TWIST();
            CPRNumber = "";
            registrerPatientUI = new ParamedicinUI_RegistrerPatient();
            measureECGUI = new ParamedicinUI_MeasureECG();
            visBatteristatusUI = new ParamedicinUI_VisBatteristatus();
            eCGControl.GetLokalinfo()._maaleenhed_identifikation = "1007";
            eCGControl.GetLokalinfo()._maaleformat_type = "double";
            eCGControl.GetLokalinfo()._samplerate_hz = 50;
            eCGControl.GetLokalinfo()._antalmaalinger = 500;
            eCGControl.GetLokalinfo()._interval_sec = 10;
            eCGControl.GetLokalinfo()._interval_min = 0;
            mainMenu();   
        }
        
        public static void mainMenu()
        {
            byte b = 0;
            string[] hovedmenu = new string[4] { "HovedMenu:", "Registrer Patient", "Start Maaling", "Vis batteristatus" };
            Display.lcdClear();
            Display.lcdBlink();
            byte c = 0;
            Display.lcdSetBackLight(0, 255, 0);

            foreach(var item in hovedmenu) // Hovedmenu bliver indlæst
            {
                Display.lcdGotoXY(0, c);
                Display.lcdPrint(hovedmenu[c]);
                c++;
            }
            Display.lcdHome(); // curserblink sættes til 0,0
            System.Threading.Thread.Sleep(500); //Forebygger antiprell ved tryk fra andre menuer

            while (true) //Kører indtil en menu vælges
            {
                int a = Encoder.getDiff(true);
                if (a < 0)
                    a = -a; // Hvis den er rullet negativt spejles tallet

                for (int i = a; i >= 0; i = i - 4) //sikrer hele tiden, at man kun ruller mellem de mulige menuer
                {
                    if (i < 4)
                    {
                        b = Convert.ToByte(i);
                        Display.lcdGotoXY(0, b);
                        Display.lcdBlink();
                    }
                }

                if (Encoder.isPressed() == true) // Sender brugeren til den valgte menu
                {
                    switch (b)
                    {
                        case 0:
                           Display.lcdClear();
                           Display.lcdSetBackLight(0, 0, 0);
                           Display.lcdNoBlink();
                           //Display.lcdNoDisplay();
                           Environment.Exit(0);
                           break;
                        case 1:
                            registrerPatientUI.registrerPatientMenu();
                            break;
                        case 2:
                           if (CPRNumber.Length == 10)
                           {
                              measureECGUI.startMaaling(CPRNumber); //Der skal være givet en værdi til CPRNumber fra UC1
                           }
                           else
                           {
                              Display.lcdClear();
                              Display.lcdHome();
                              Display.lcdPrint("Ugyldigt CPR");
                              System.Threading.Thread.Sleep(3000);
                              mainMenu();
                           }
                           break;
                        case 3:
                            visBatteristatusUI.visBatteristatus();
                            break;
                    }
                }

            }
        }
    }
    
}
