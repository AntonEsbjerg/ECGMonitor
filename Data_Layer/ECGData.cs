using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using DTO;

namespace DataLayer
{
    public class ECGData
    {
        public ECGData()
        {

        }
        public void uploadECG(double[] ECGMaalinger_)
        {

            /*
            private void GenererB_Click(object sender, RoutedEventArgs e)
        {
            double[] tal = new double[100];
            int id = 0;
            for (int i = 0; i < 100; i++)
            {
                tal[i] = random.NextDouble() * 10 + basis;
            }
            string insertStringParam = @"INSERT INTO Data (Værdier) OUTPUT INSERTED.Id VALUES(@data)";
            using (SqlCommand cmd = new SqlCommand(insertStringParam, OpenConnectionST))
            {
                cmd.Parameters.AddWithValue("@data", tal.SelectMany(value => BitConverter.GetBytes(value)).ToArray());
                id = (int)cmd.ExecuteScalar(); //Returns the identity of the new tuple/record
            }
            Console.WriteLine("ID brugt: " + id);
            DataSetCB.Items.Add(id);
            basis += 10;


        }
            */
        }
        public int doctorAnalyses(string CPRNumber_)
        {
            int doctorAnalyseVaerdi = 0;
            //Her skal oprettes forbindelse til databasen, hvor der skal tjekkes om der er uplaoded et nyt svar fra lægen.

            return doctorAnalyseVaerdi;

        }
    }
}
