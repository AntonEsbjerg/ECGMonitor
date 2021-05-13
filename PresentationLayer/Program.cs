using RaspberryPiCore.ADC;
using RaspberryPiCore.JoySticks;
using RaspberryPiCore.LCD;
using RaspberryPiCore.TWIST;
using System;
using System.Threading;
using LogicLayer;

namespace PresentationLayer
{
    class Program
    {
        static SerLCD Display;
        static TWIST Encoder;
        private static ParamedicinUI_RegistrerPatient registrerPatientUI;
        private static ParamedicinUI_MeasureECG measureECGUI;
        private static ParamedicinUI_VisBatteristatus visBatteristatusUI;
        
        static void Main(string[] args)
        {
            Display = new SerLCD();
            Encoder = new TWIST();
            registrerPatientUI = new ParamedicinUI_RegistrerPatient();
            measureECGUI = new ParamedicinUI_MeasureECG();
            visBatteristatusUI = new ParamedicinUI_VisBatteristatus();
            mainMenu();   
        }
        
        public static void mainMenu()
        {
            byte b = 0;
            string[] hovedmenu = new string[4] { "HovedMenu:", "Registrer Patient", "Start Maaling", "Vis batteristatus" };
            Display.lcdClear();
            Display.lcdBlink();
            byte c = 0;
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
                        case 1:
                            registrerPatientUI.registrerPatientMenu();
                            break;
                        case 2:
                            measureECGUI.startMaaling();
                            break;
                        case 3:
                            visBatteristatusUI.visBatteristatus();
                            break;
                    }//Måske det kunne være fedt med en sluk funktion.
                }

            }
        }
    }
    
}
