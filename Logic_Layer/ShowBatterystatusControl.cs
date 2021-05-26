using System;
using System.Collections.Generic;
using System.Text;
using DataLayer;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;

namespace LogicLayer
{
    public class ShowbatterystatusControl
    {
        private int batteristatus;
        private Battery batteryData;

        public ShowbatterystatusControl()
        {
            batteryData = new Battery();
        }
        public int requestbatterystatus(int ADCValue)
        {
            double sample = ((Convert.ToDouble(ADCValue) / 2048.0) * 6.144);

            if (sample >= 2.9)
            {
                batteristatus = 100;
            }
            else if (sample >= 2.765 && sample < 2.9)
            {
                batteristatus = 80;
            }
            else if (sample >= 2.701 && sample < 2.765)
            {
                batteristatus = 60;
            }
            else if (sample >= 2.657 && sample < 2.701)
            {
                batteristatus = 40;
            }
            else if (sample >= 2.593 && sample < 2.657)
            {
                batteristatus = 20;
            }
            else if (sample >= 2.506 && sample < 2.593)
            {
                batteristatus = 10;
            }
            else if (sample < 2.506)
            {
                batteristatus = 1;
            }

            //batteristatus = batteryData.getbatterystatus();

            return batteristatus;

        }
    }
}
