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
            Display.lcdClear();
            Display.lcdPrint("HovedMenu:");
            Display.lcdBlink();
            Display.lcdGotoXY(0, 1);
            Display.lcdPrint("Registrer Patient");
            Display.lcdGotoXY(0, 2);
            Display.lcdPrint("Start Maaling");
            Display.lcdGotoXY(0, 3);
            Display.lcdPrint("Vis batteristatus");
            Display.lcdGotoXY(0, 0);
            while (true)
            {
                
                //måske clear b og lave alle om til d igen, så det fungere ens. test
                int a = Encoder.getDiff(true);
                if (a < 0)
                    a = -a;

                for (int i = a; i >= 0; i = i - 4)
                {
                    if (i < 4)
                    {
                        b = Convert.ToByte(i);
                        Display.lcdGotoXY(0, b);
                        Display.lcdBlink();
                    }
                }

                if (Encoder.isPressed() == true)
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
                    }
                }

            }
        }
    }
    
}
