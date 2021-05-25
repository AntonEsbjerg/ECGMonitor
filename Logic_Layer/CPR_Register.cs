using System;
using System.Collections.Generic;
using System.Text;
using DataLayer;
using DTO;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;

namespace LogicLayer
{
   public class CPR_Register
   {
      private Person p;

      public CPR_Register()
      {
         p = new Person();
      }
      public bool validate(string CPRNumber)
      {
         bool result = false;
         if (p.findCPR(CPRNumber) == 1)
         {
            result = true;
         }
         else if (p.findCPR(CPRNumber) == 2)
         {

            result = false;
         }
         return result;
         //skal validere om en patient eksistere i databasen
      }

   }
}
