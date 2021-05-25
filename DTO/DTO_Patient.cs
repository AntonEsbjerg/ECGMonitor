using System;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;

namespace DTO
{
    public class DTO_Patient
    {
        public string CPRnummer_ { get; set; }
        public string Navn_ { get; set; }
        public string Telfon_ { get; set; }
        public string Addresse_ { get; set; }
        public DTO_Patient(string CPRnummer, string Navn, string Telefon, string Addresse)
        {
            CPRnummer_ = CPRnummer;
            Navn_ = Navn;
            Telfon_ = Telefon;
            Addresse_ = Addresse;
        }
    }
}
