using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
//using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using DTO;

namespace DataLayer
{
    public class ECGData
    {
      private DTO_Measurement lokalinfo;
        public ECGData()
        {
            
            lokalinfo = new DTO_Measurement();
        }
        private SqlConnection connect
        {
            get
            {
            var con = new SqlConnection(@"Data Source=DESKTOP-PDTN5JP\SQLEXPRESS;Initial Catalog=LokalDatabase;User ID=LokalDatabase;Password=LokalDatabase;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

            con.Open();
                return con;
            }
        }
        public DTO_Measurement lokalmaaling()
        {
            return lokalinfo;
        }
        public int uploadECG(DTO_Measurement nyMaaling)
        {
            double[] tal;
            double[] ecgVoltage = new double[500];
            string insertStringDLEDBData = "INSERT INTO db_owner.EKGDATA (raa_data,samplerate_hz,interval_sec,interval_min,data_format," +
                "bin_eller_tekst,maaleformat_type,start_tid,kommentar,ekgmaaleid,maalenehed_identifikation) " +
                "VALUES (@raa_data, @samplerate_hz, @interval_sec, @interval_min, @data_format, @bin_eller_tekst, " +
                "@maaleformat_type,@start_tid,@kommentar,@ekgmaaleid,@maalenehed_identifikation)";
            string insertStringDLEDBMaeling = "INSERT INTO db_owner.EKGMAELING (dato,antalmaalinger,sfp_maaltagerfornavn,sfp_maltagerefternavn," +
                "sfp_maaltagermedarbjnr,sfp_mt_org,sfp_mt_kommentar,borger_fornavn,borger_efternavn,borger_cprnr,stemi_mistaenkt) " + "OUTPUT INSERTED.ekgmaaleid "+
                "VALUES (@dato,@antalmaalinger,@sfp_maaltagerfornavn, @sfp_maltagerefternavn,@sfp_maaltagermedarbjnr," +
                "@sfp_mt_org,@sfp_mt_kommentar,@borger_fornavn,@borger_efternavn,@borger_cprnr,@stemi_mistaenkt)";
            
            using (SqlCommand command = new SqlCommand(insertStringDLEDBMaeling, connect))
            {
                command.Parameters.AddWithValue("@dato", nyMaaling._dato);
                command.Parameters.AddWithValue("@antalmaalinger", nyMaaling._antalmaalinger);
                command.Parameters.AddWithValue("@sfp_maaltagerfornavn", nyMaaling._sfp_maaltagerfornavn);
                command.Parameters.AddWithValue("@sfp_maltagerefternavn", nyMaaling._sfp_maaltagerefternavn);
                command.Parameters.AddWithValue("@sfp_maaltagermedarbjnr", nyMaaling._sfp_maaltagermedarbjdnr);
                command.Parameters.AddWithValue("@sfp_mt_org", nyMaaling._sfp_mt_org);
                command.Parameters.AddWithValue("@sfp_mt_kommentar", nyMaaling._sfp_mt_kommentar);
                command.Parameters.AddWithValue("@borger_fornavn", nyMaaling._borger_fornavn);
                command.Parameters.AddWithValue("@borger_efternavn", nyMaaling._borger_efternavn);
                command.Parameters.AddWithValue("@borger_cprnr", nyMaaling._borger_cprnr);
                command.Parameters.AddWithValue("@stemi_mistaenkt", nyMaaling._STEMI_suspected);

                nyMaaling._ekgmaaleid = Convert.ToInt32(command.ExecuteScalar());
            }
            using (SqlCommand command = new SqlCommand(insertStringDLEDBData, connect))
            {
                tal = nyMaaling._lokalECG;
                for (int i = 0; i < nyMaaling._lokalECG.Length; i++)
                {
                    tal[i] = Math.Round(nyMaaling._lokalECG[i], 4);
                }

                command.Parameters.AddWithValue("@raa_data", tal.SelectMany(value => BitConverter.GetBytes(value)).ToArray());
                command.Parameters.AddWithValue("@samplerate_hz", nyMaaling._samplerate_hz);
                command.Parameters.AddWithValue("@interval_sec", nyMaaling._interval_sec);
                command.Parameters.AddWithValue("@interval_min", nyMaaling._interval_min);
                command.Parameters.AddWithValue("@data_format", nyMaaling._dataformat);
                command.Parameters.AddWithValue("@bin_eller_tekst", nyMaaling._bin_eller_tekst);
                command.Parameters.AddWithValue("@maaleformat_type", nyMaaling._maaleformat_type);
                command.Parameters.AddWithValue("@start_tid", nyMaaling._start_tid);
                command.Parameters.AddWithValue("@kommentar", nyMaaling._kommentar);
                command.Parameters.AddWithValue("@ekgmaaleid", nyMaaling._ekgmaaleid);
                command.Parameters.AddWithValue("@maalenehed_identifikation", nyMaaling._maaleenhed_identifikation);
                command.ExecuteScalar();

                connect.Close();
                return nyMaaling._ekgmaaleid;
            }
        }
        public int doctorAnalyses(string MaelingID)
        {
            SqlDataReader rdr;
            int doctorAnalyseVaerdi = 2;
            //Her skal oprettes forbindelse til databasen, hvor der skal tjekkes om der er uplaoded et nyt svar fra lægen.
            string insertStringParam1 = ("Select * from db_owner.EKGMaeling where stemi_paavist IS NOT NULL and ekgmaaleid= " + MaelingID);
            using (SqlCommand command = new SqlCommand(insertStringParam1, connect))
            {
                rdr = command.ExecuteReader();
                if (rdr.Read())
                {
                    doctorAnalyseVaerdi=Convert.ToInt32(rdr["stemi_paavist"]);
                }
            }
            connect.Close();
            return doctorAnalyseVaerdi;
        }
    }
}
