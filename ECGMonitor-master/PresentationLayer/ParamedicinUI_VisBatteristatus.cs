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
    class ParamedicinUI_VisBatteristatus
    {
        static SerLCD Display;
        static TWIST Encoder;
        private int Bstatus;
        private ShowbatterystatusControl BstatusControl;
        public ParamedicinUI_VisBatteristatus()
        {
            Display = new SerLCD();
            Encoder = new TWIST();
            BstatusControl = new ShowbatterystatusControl();

        }
        public void visBatteristatus()
        {
            ADC1015 AD = new ADC1015();
            int ADC = AD.readADC_SingleEnded(1);

            Bstatus = BstatusControl.requestbatterystatus(ADC); //Modtager værdi og viser enten normal eller lav skræm. Udregningen sker i logicLayer
            if (Bstatus > 20) // Vi har valgt grænsen ved 20%
                displayNormalBatterystatus();
            else
                displayLowBatterystatus();
        }

        public void displayNormalBatterystatus()
        {
            Display.lcdClear();
            Display.lcdNoBlink();
            string a = Convert.ToString(Bstatus);
            string[] NormalB = new string[3] { "Batteristatus:", a + "%", "Tilbage" };
            byte c = 0;
            foreach (var item in NormalB) // Hovedmenu bliver indlæst
            {
                Display.lcdGotoXY(0, c);
                Display.lcdPrint(NormalB[c]);
                c++;
            }
            Display.lcdBlink();

            while (true)
            {
                if (Encoder.isPressed())
                {
                    Program.mainMenu();
                }
            }
        }
        public void displayLowBatterystatus()
        {
            Display.lcdClear();
            Display.lcdNoBlink();
            string a = Convert.ToString(Bstatus);
            string[] NormalB = new string[4] { "Batteristatus:", a + "%", "Lavt batteriniveau!", "Tilbage" };
            byte c = 0;
            foreach (var item in NormalB) // Hovedmenu bliver indlæst
            {
                Display.lcdGotoXY(0, c);
                Display.lcdPrint(NormalB[c]);
                c++;
            }
            Display.lcdBlink();

            while (true)
            {
                if (Encoder.isPressed())
                    Program.mainMenu();
            }
        }
    }
}
