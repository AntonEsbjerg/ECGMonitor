using RaspberryPiCore.ADC;
using RaspberryPiCore.JoySticks;
using RaspberryPiCore.LCD;
using RaspberryPiCore.TWIST;
using System;
using System.Threading;
using LogicLayer;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using Extreme.Mathematics;
using DTO;

namespace PresentationLayer
{

    public class ParamedicinUI_MeasureECG
    {
        MeasureECGControl eCGControl = new MeasureECGControl();
        static SerLCD Display;
        static TWIST Encoder;        
        private int maalingID;
        private MeasureECGControl ECGControl;
        private DateTime tidspunktForMaaling;        
        public double[] ECGMaalinger { get; set; }
        private ADC1015 adc;
        private DTO_Measurement nyMaaling { get; set; }        
        private ParamedicinUI_RegistrerPatient paraRP;

        public ParamedicinUI_MeasureECG()
        {
            adc = new ADC1015();
            ECGControl = new MeasureECGControl();
            Display = new SerLCD();
            Encoder = new TWIST();
            paraRP = new ParamedicinUI_RegistrerPatient();
            maalingID = new int();
        }
        public void startMaaling(string CPRNumber) //Modtager CPRN fra hovedmenu, når UC1 er gennemført
        {
            byte c=0;
            byte d=0;
            byte e = 0;
            bool udbryder = false;
            bool RPiAnalyse;
            int doctorAnalyse=2; // så der ikke er svar før lægen har indrapporteret det
            Display.lcdClear();
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
                if (a < 0)
                    a = -a;
                for (int i = a; i >= 0; i = i - 2)
                {
                    if (i < 2)
                    {
                        c = Convert.ToByte(i+1);
                        Display.lcdGotoXY(0, c);
                        Display.lcdBlink();
                    }                          
                }
                if (Encoder.isPressed())
                {
                    switch (c)
                    {
                        case 1: //starter hele maalingen. Ikke muligt at vende retur efter
                            {
                                informECGStart();
                                startECG();                                
                                informECGEnd();
                                udbryder = true;
                                break;
                            }
                        case 2: // Vender retur ved fortrydelse
                            Program.mainMenu();
                            break;
                    }
                }
                if (udbryder) // sikrer at vi kommer helt ud, så ECG kan analyseres. 
                    break;
            }
            
            RPiAnalyse = ECGControl.analyzeECG(ECGMaalinger); // Systemet analyserer ECG, og giver informationen til DTO-objektet
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
            Display.lcdClear();
            string[] ECGtjekLaegeSvar = new string[2] { "Maaling foretaget", "Tjek Laege svar" };
            foreach (var item in ECGtjekLaegeSvar)
            {
                Display.lcdGotoXY(0, e);
                Display.lcdPrint(ECGtjekLaegeSvar[e]);
                e++;
            }
            while(true)
            {
                if (Encoder.isPressed()) //Ved tryk forsøger systemet at hente svaret fra lægen i den loakle database. 
                {
                    doctorAnalyse = ECGControl.confirmSTEMI(Convert.ToString(maalingID));
                }
                if (doctorAnalyse == 0 || doctorAnalyse == 1)
                    break;                
            }
            
            switch(doctorAnalyse)
            {
                case 1:
                    alarmSTEMI();
                    break;
                case 0:
                    noSTEMI();
                    break;
            }
         //Herefter skal lige tænkes over, hvor den så skal hen.
         System.Threading.Thread.Sleep(5000);
         Program.mainMenu();
        }
        public double[] startECG()
        {
            double sample;
            tidspunktForMaaling = DateTime.Now;
            ECGMaalinger = new double[500];
            eCGControl.GetLokalinfo()._start_tid = tidspunktForMaaling;
            eCGControl.GetLokalinfo()._dato = tidspunktForMaaling.Date;
            eCGControl.GetLokalinfo()._antalmaalinger = 500;
            eCGControl.GetLokalinfo()._samplerate_hz = 50;
            eCGControl.GetLokalinfo()._interval_sec = 10;
            eCGControl.GetLokalinfo()._interval_min = 0;
            eCGControl.GetLokalinfo()._dataformat = "double";
            eCGControl.GetLokalinfo()._sfp_maaltagerefternavn = "Mortensen";
            eCGControl.GetLokalinfo()._sfp_maaltagerfornavn = "Lars";
            eCGControl.GetLokalinfo()._sfp_maaltagermedarbjdnr = "1";
            eCGControl.GetLokalinfo()._sfp_mt_kommentar = "";
            eCGControl.GetLokalinfo()._sfp_mt_org = "Aarhus Universitet";
            eCGControl.GetLokalinfo()._maaleformat_type = "double";
            eCGControl.GetLokalinfo()._bin_eller_tekst = "1";
            eCGControl.GetLokalinfo()._maaleenhed_identifikation = "RPi B3+";
            eCGControl.GetLokalinfo()._borger_cprnr = Program.CPRNumber;
            eCGControl.GetLokalinfo()._borger_fornavn = Program.BorgerFornavn;
            eCGControl.GetLokalinfo()._borger_efternavn = Program.BorgerEfternavn;
            eCGControl.GetLokalinfo()._kommentar = "";
            

            // Array.Clear(ECGMaalinger, 0, ECGMaalinger.Length);
            if (ECGMaalinger.Length > 0)
            {
                for (int i = 0; i < ECGMaalinger.Length; i++)
                {
                    ECGMaalinger[i] = 0;
                }
            }
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
            string[] alarmSTEMI = new string[4] { "ECG-Analyseret", "Tegn paa STEMI", "Naermeste PCI-Center", "Akutvej 1, 8200" };
            foreach (var item in alarmSTEMI) // AlarmsSTEMi bliver indlæst
            {
                Display.lcdGotoXY(0, d);
                Display.lcdPrint(alarmSTEMI[d]);
                d++;
            }
            for (int i = 0; i < 8; i++)
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
            for (int i = 0; i < 8; i++)
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

