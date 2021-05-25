using RaspberryPiCore.ADC;
using RaspberryPiCore.JoySticks;
using RaspberryPiCore.LCD;
using RaspberryPiCore.TWIST;
using System;
using System.Threading;
using LogicLayer;

namespace PresentationLayer
{

    class ParamedicinUI_MeasureECG
    {
        static SerLCD Display;
        static TWIST Encoder;
        public ParamedicinUI_MeasureECG()
        {
            Display = new SerLCD();
            Encoder = new TWIST();
        }
        public void startMaaling()
        {
            byte c=0;
            Display.lcdClear();
            Display.lcdNoBlink();
            Display.lcdNoCursor();
            Display.lcdPrint("Maalingen er i");
            Display.lcdGotoXY(0, 1);
            Display.lcdPrint("gang...");
            System.Threading.Thread.Sleep(1000); //skal være 10sek
            Display.lcdHome();
            blinkBlack();
            Display.lcdPrint("Afventer analyse");
            Display.lcdGotoXY(0, 1);
            Display.lcdPrint("Vaelg for at forsætte:");
            Display.lcdGotoXY(0, 2);
            Display.lcdPrint("Ingen STEMI");
            Display.lcdGotoXY(0, 3);
            Display.lcdPrint("Mulig STEMI");
            Display.lcdBlink();
            while (true)
            {
                int a = Encoder.getDiff(true);
                for (int i = a; i >= 0; i = i - 2)
                {
                    if (i < 2)
                    {
                        c = Convert.ToByte(i + 2);
                        Display.lcdGotoXY(0, c);
                    }

                }
                if (Encoder.isPressed())
                {
                    if (c == 2)
                    {
                        Display.lcdClear();
                        Display.lcdPrint("Ingen STEMI fundet,");
                        Display.lcdGotoXY(0, 1);
                        Display.lcdPrint("afvent svar fra");
                        Display.lcdGotoXY(0, 2);
                        Display.lcdPrint("sygehus");
                        if (Encoder.isPressed())
                            break;
                    }
                    if (c == 3)
                    {
                        Display.lcdClear();
                        blinkYellow();
                        if (Encoder.isPressed())
                            break;
                    }
                    else
                        break;
                }
            }
            //Denne metode er lang fra done.
            //Er stoppet efter mulig STEMI
        }
        static void blinkRed()
        {
            Display.lcdClear();
            Display.lcdPrint("EKG-Analyseret");
            Display.lcdGotoXY(0, 1);
            Display.lcdPrint("Tegn På STEMI");
            Display.lcdGotoXY(0, 2);
            Display.lcdPrint("Nærmeste PCI-Center");
            Display.lcdGotoXY(0, 3);
            Display.lcdPrint("Adresse:");
            for (int i = 0; i < 4; i++)
            {
                Display.lcdSetBackLight(255, 0, 0);
                System.Threading.Thread.Sleep(250);
                blinkGreen();
                System.Threading.Thread.Sleep(250);
            }
        }
        static void blinkGreen()
        {
            Display.lcdSetBackLight(0, 255, 0);

        }
        static void blinkYellow()
        {
            Display.lcdClear();
            Display.lcdPrint("Mulig STEMI!");
            for (int i = 0; i < 4; i++)
            {
                Display.lcdSetBackLight(255, 255, 0);
                System.Threading.Thread.Sleep(250);
                blinkGreen();
                System.Threading.Thread.Sleep(250);
            }

        }
        static void blinkBlack()
        {
            for (int i = 0; i < 4; i++)
            {
                Display.lcdSetBackLight(0, 0, 0);
                System.Threading.Thread.Sleep(250);
                blinkGreen();
                System.Threading.Thread.Sleep(250);
            }
        }
    }
}

