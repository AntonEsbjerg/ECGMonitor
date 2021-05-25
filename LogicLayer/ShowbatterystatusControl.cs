using System;
using System.Collections.Generic;
using System.Text;
using DataLayer;

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
        public int requestbatterystatus()
        {
            batteristatus = batteryData.getbatterystatus();
            return batteristatus;

        }
    }
}
