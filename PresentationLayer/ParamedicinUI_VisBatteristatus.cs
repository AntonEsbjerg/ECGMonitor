using RaspberryPiCore.ADC;
using RaspberryPiCore.JoySticks;
using RaspberryPiCore.LCD;
using RaspberryPiCore.TWIST;
using System;
using System.Threading;
using LogicLayer;



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
            Bstatus = BstatusControl.requestbatterystatus();
            if (Bstatus > 20)
                displayNormalBatterystatus();
            else
                displayLowBatterystatus();
        }
        
        public void displayNormalBatterystatus()
        {
            Display.lcdClear();
            Display.lcdNoBlink();
            Display.lcdPrint("Batteristatus:");
            Display.lcdGotoXY(0, 1);
            string a = Convert.ToString(Bstatus);
            Display.lcdPrint(a+"%"); // her skal batteristatus tages med
            Display.lcdGotoXY(0, 2);
            Display.lcdPrint("Tilbage");
            Display.lcdCursor();
            if (Encoder.isPressed())
                Program.mainMenu();
        }
        public void displayLowBatterystatus()
        {
            Display.lcdClear();
            Display.lcdNoBlink();
            Display.lcdPrint("Batteristatus:");
            Display.lcdGotoXY(0, 1);
            string a = Convert.ToString(Bstatus);
            Display.lcdPrint(a+"%"); //her skal batteristatus tages med
            Display.lcdGotoXY(0, 2);
            Display.lcdPrint("Lavt batteriniveau!");
            Display.lcdGotoXY(0, 3);
            Display.lcdPrint("Tilbage");
            Display.lcdCursor();
            if (Encoder.isPressed())
                Program.mainMenu();
        }
    }
}
