using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer
{
   public class Battery
   {
      private int Bstatus;

      public Battery()
      {

      }
      public int getbatterystatus()
      {
         Random rd = new Random();
         int rand_num = rd.Next(1, 100);
         Bstatus = rand_num;

         return Bstatus;
      }
   }
}
