using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using DTO;
//using Microsoft.Data.SqlClient;

namespace DataLayer
{
   public class Person
   {
        public Person()
        {

        }
      private SqlConnection OpenConnectionST
      {
         get
         {
            var con = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            con.Open();
            return con;
         }
      }
      public int findCPR(string number)
      {
         int result = 0;

         SqlDataReader rdr;
         string selectString = "select * from CPR_register where CPRnr= '" + number + "'";

         using (SqlCommand cmd = new SqlCommand(selectString, OpenConnectionST))
         {
            rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    result = 1;

                }
                else
                    result = 2;
            }
         
         return result;

      }
      public List<DTO_Patient> findData(string number)
      {
         List<DTO_Patient> patientObjekter = new List<DTO_Patient>();
            SqlDataReader rdr;
         string selectString = "select * from CPR_register where CPRnr= '" + number + "'";

         using (SqlCommand cmd = new SqlCommand(selectString, OpenConnectionST))
         {
            rdr = cmd.ExecuteReader();
         }
         if (rdr.Read())
         {
                patientObjekter.Add(new DTO_Patient(number, Convert.ToString(rdr["Fornavn"]), Convert.ToString(rdr["Efternavn"]),Convert.ToString(rdr["Tlf"]), Convert.ToString(rdr["Adresse"])));
         }
         return patientObjekter;
      }
   }
}
