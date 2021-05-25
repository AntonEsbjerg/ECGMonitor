using System;
using System.Collections.Generic;
using System.Text;
using DataLayer;

namespace LogicLayer
{
    public class CPR_Register
    {
        private Person p;
        private List<Person> borger;
        public CPR_Register()
        {
            p = new Person();
            borger = new List<Person>();
        }
        public bool validate(uint CPRNumber)
        {
            return true;
            //skal validere om en patient eksistere i databasen
        }

    }
}
