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

    class ParamedicinUI_MeasureECG
    {
        MeasureECGControl eCGControl = new MeasureECGControl();
        static SerLCD Display;
        static TWIST Encoder;
        private string CPRNumber;
        private int maalingID;
        private MeasureECGControl ECGControl;
        private DateTime tidspunktForMaaling;
        private DateTime dato;
        public double[] ECGMaalinger { get; set; }
        private ADC1015 adc;
        private DTO_Measurement nyMaaling { get; set; }

        public ParamedicinUI_MeasureECG()
        {
            adc = new ADC1015();
            ECGControl = new MeasureECGControl();
            Display = new SerLCD();
            Encoder = new TWIST();

        }
        public void startMaaling(string CPRNumber)
        {
            byte c=0;
            byte d=0;
            bool udbryder = false;
            bool RPiAnalyse;
            int doctorAnalyse=0;
            Display.lcdClear();
            Display.lcdNoBlink();
            string[] ECGMenu = new string[3] { "Meassure ECG", "Start Maaling", "Tilbage"};
            foreach (var item in ECGMenu) // ECGMenu bliver indlæst
            {
                Display.lcdGotoXY(0, d);
                Display.lcdPrint(ECGMenu[d]);
                d++;                
            }

            while (true)
            {
                int a = Encoder.getDiff(true);
                for (int i = a; i >= 0; i = i - 2)
                {
                    if (i < 2)
                    {
                        c = Convert.ToByte(i+1);
                        Display.lcdGotoXY(0, c);
                    }                          
                }
                if (Encoder.isPressed())
                {
                    switch (c)
                    {
                        case 1:
                            {
                                informECGStart();
                                startECG();
                                //ECGControl.startECG(); kan ikke oprettes i control, da den ikke kan bruge rpi osv
                                informECGEnd();
                                udbryder = true;
                                break;
                            }
                        case 2:
                            Program.mainMenu();
                            break;
                    }
                }
                if (udbryder) // sikrer at vi kommer helt ud, så ECG kan analyseres. 
                    break;
            }
            
            RPiAnalyse = ECGControl.analyzeECG(ECGMaalinger);
            if(RPiAnalyse==true)
            {
                informPossibleSTEMI();
                eCGControl.GetLokalinfo()._STEMI_suspected = true;
            }
            else
            {
                informPossibleNoSTEMI();
                eCGControl.GetLokalinfo()._STEMI_suspected = false;
            }
            //Nu skal ECG oploades i databasen
            maalingID=ECGControl.convertToBlobAndUpload(eCGControl.GetLokalinfo()); // metoden går igennem logiklaget, så reglerne overholdes.
            //Nu er målingen uploaded og vi afventer nu svar fra hospitalet om hvad diagnosen er
            while(doctorAnalyse != 0 || doctorAnalyse != 1) //ved værdi 0 er der ikke svar endnu. Ellers er der svar
            {
                doctorAnalyse = ECGControl.confirmSTEMI(Convert.ToString(maalingID));
                System.Threading.Thread.Sleep(5000);
            }
            switch(doctorAnalyse)
            {
                case 1:
                    alarmSTEMI();
                    break;
                case 2:
                    noSTEMI();
                    break;
            }
         //Herefter skal lige tænkes over, hvor den så skal hen.
         System.Threading.Thread.Sleep(20000);
         Program.mainMenu();
        }
        public double[] startECG()
        {
            double sample;
            tidspunktForMaaling = DateTime.Now;
            eCGControl.GetLokalinfo()._start_tid = tidspunktForMaaling;
            eCGControl.GetLokalinfo()._dato = tidspunktForMaaling.Date;
            Array.Clear(ECGMaalinger, 0, ECGMaalinger.Length);
            for (int i = 0; i < 10 * 50; i++)
            {
                //opsamler datapunkter og konvertere fra heltal i 11 bit format til volt:
                sample = (adc.readADC_SingleEnded(0) / 2048.0) * 6.144;
                //System.Diagnostics.Debug.WriteLine("input fra adc:    :  " + sample);

                //tilføjer målepunkterne til listen af grafpunker:
                ECGMaalinger[i] = sample;
                //EKGLine.Values.Add(sample);

                //mellem hver måling skal man pause en lille smule for at holde styr på hvor mange
                //gange vi sampler pr. sek                
                System.Threading.Thread.Sleep((1000 / 50) - 4);
            }
            eCGControl.GetLokalinfo()._lokalECG = ECGMaalinger;
            return ECGMaalinger;
        }
        public void alarmSTEMI()
        {
            Display.lcdClear();
            byte d = 0;
            string[] alarmSTEMI = new string[4] { "ECG-Analyseret", "Tegn på STEMI", "Nærmeste PCI-Center", "Vej 1, 8200" };
            foreach (var item in alarmSTEMI) // AlarmsSTEMi bliver indlæst
            {
                Display.lcdGotoXY(0, d);
                Display.lcdPrint(alarmSTEMI[d]);
                d++;
            }
            for (int i = 0; i < 4; i++)
            {
                Display.lcdSetBackLight(255, 0, 0);
                System.Threading.Thread.Sleep(250);
                blinkGreen();
                System.Threading.Thread.Sleep(250);
            }
            while(true)
            {
                if (Encoder.isPressed())
                    break;
            }
        }

        public void noSTEMI()
        {
            Display.lcdClear();
            byte d = 0;
            string[] noSTEMI = new string[3] { "ECG Analyseret", "Ingen STEMI fundet", "Tryk for accept"};
            foreach (var item in noSTEMI) // noSTEMi bliver indlæst
            {
                Display.lcdGotoXY(0, d);
                Display.lcdPrint(noSTEMI[d]);
                d++;
            }
            while (true)
            {
                if (Encoder.isPressed())
                    break;
            }
        }
        public void blinkGreen()
        {
            Display.lcdSetBackLight(0, 255, 0);
        }
        public void informPossibleNoSTEMI()
        {
            Display.lcdClear();
            byte d = 0;
            string[] noSTEMI = new string[3] { "Ingen STEMI fundet", "afvent svar fra", "sygehus" };
            foreach (var item in noSTEMI) // noSTEMi bliver indlæst
            {
                Display.lcdGotoXY(0, d);
                Display.lcdPrint(noSTEMI[d]);
                d++;
            }
            while (true)
            {
                if (Encoder.isPressed())
                    break;
            }
        }
        public void informPossibleSTEMI()
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
            while (true)
            {
                if (Encoder.isPressed())
                    break;
            }
        }
        public void informECGStart()
        {
            Display.lcdClear();
            Display.lcdPrint("Maaling igang");
            blinkBlack();
        }
        public void informECGEnd()
        {
            Display.lcdClear();
            Display.lcdPrint("Maaling foretaget");
            blinkBlack();
        }
        public void blinkBlack()
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

