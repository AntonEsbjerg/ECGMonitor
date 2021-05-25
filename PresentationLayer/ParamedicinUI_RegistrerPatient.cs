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
        private uint CPRNumber1;
        public ParamedicinUI_RegistrerPatient()
        {
            Display = new SerLCD();
            Encoder = new TWIST();
            RPcontrol = new RegisterPatientControl();

        }
        public void registrerPatientMenu()
        {
            byte b = 0;
            Display.lcdClear();
            Display.lcdPrint("Register Patient:");
            Display.lcdGotoXY(0, 1);
            Display.lcdPrint("Scan sygesikring");
            Display.lcdGotoXY(0, 2);
            Display.lcdPrint("Indtast CPR-nummer");
            Display.lcdGotoXY(0, 3);
            Display.lcdPrint("Default Patient");
            Display.lcdHome();

            //Her skal indtastning af CPR_nummer ske
            CPRNumber1 = 1234567890;

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
                        RPcontrol.cardScan();
                        break;
                    case 2:
                        //Her skal indtastning af CPR_nummer ske
                        CPRNumber1 = 1234567890;
                        RPcontrol.registrerPatient(CPRNumber1);
                        break;
                    case 3:
                        RPcontrol.defaultPatient();
                        break;
                }

                //skal der kunne komme tilbage til hovedmenu?
                Program.mainMenu(); //Der behøver ikke oprettes et objekt, når det er static class. 

            }



            

        }
        public void displayValidatedPatient()
        {
            //Patienten skal her vises på skærmen
            //Hvordan skal den ritige patient kunne vises på skærmen?
        }
    }
}


