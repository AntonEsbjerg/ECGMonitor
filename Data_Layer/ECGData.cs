﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data.SqlClient;
using DTO;

namespace DataLayer
{
    public class ECGData
    {
        public ECGData()
        {

        }
        private SqlConnection connect
        {
            get
            {
                var con = new SqlConnection(@"Data Source=st-i4dab.uni.au.dk;Initial Catalog=ST2PRJ2OffEKGDatabase;Integrated Security=False;User ID=ST2PRJ2OffEKGDatabase;Password=ST2PRJ2OffEKGDatabase;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False");
                con.Open();
                return con;
            }
        }
        public DTO_Measurement lokalmaaling()
        {
            DTO_Measurement lokalinfo;
            lokalinfo = new DTO_Measurement();
            return lokalinfo;
        }
        public int uploadECG(DTO_Measurement nyMaaling)
        {
            int maalingID;
            double[] tal;
            double[] ecgVoltage = new double[500];
            string insertStringDLEDBData = "INSERT INTO EKGDATA (raa_data,samplerate_hz,interval_sec,interval_min,data_format," +
                "bin_eller_tekst,maaleformat_type,start_tid,kommentar,ekgmaaleid,maalenehed_identifikation) " +
                "VALUES (@raa_data, @samplerate_hz, @interval_sec, @interval_min, @data_format, @bin_eller_tekst, " +
                "@maaleformat_type,@start_tid,@kommentar,@ekgmaaleid,@maalenehed_identifikation)";
            string insertStringDLEDBMaeling = "INSERT INTO EKGMAELING (dato,antalmaalinger,sfp_maaltagerfornavn,sfp_maltagerefternavn," +
                "sfp_maaltagermedarbjnr,sfp_mt_org,sfp_mt_kommentar,borger_fornavn,borger_efternavn,borger_cprnr,stemi_mistaenkt) " +
                "VALUES (@dato,@antalmaalinger,@sfp_maaltagerfornavn, @sfp_maltagerefternavn,@sfp_maaltagermedarbjnr," +
                "@sfp_mt_org,@sfp_mt_kommentar,@borger_fornavn,@borger_efternavn,@borger_cprnr,@stemi_mistaenkt)";
            using (SqlCommand command = new SqlCommand(insertStringDLEDBData, connect))
            {
                tal = nyMaaling._lokalECG;
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
                //command.ExecuteNonQuery();
            }
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
                maalingID = (int)command.ExecuteScalar();
                connect.Close();
                return maalingID;
                //command.ExecuteNonQuery();

            }
        }
        public int doctorAnalyses(string MaelingID)
        {
            SqlDataReader rdr;
            int doctorAnalyseVaerdi = 2;
            //Her skal oprettes forbindelse til databasen, hvor der skal tjekkes om der er uplaoded et nyt svar fra lægen.
            string insertStringParam1 = ("Select * from EKGMaeling where stemi_paavist IS NOT NULL and ekgmaaleid= " + MaelingID);
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