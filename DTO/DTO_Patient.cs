using System;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;

namespace DTO
{
    public class DTO_Patient
    {
        public string CPRnummer_ { get; set; }
        public string Fornavn_ { get; set; }
        public string Efternavn_ { get; set; }
        public string Telfon_ { get; set; }
        public string Addresse_ { get; set; }
        public DTO_Patient(string CPRnummer, string Fornavn, string Efternavn, string Telefon, string Addresse)
        {
            CPRnummer_ = CPRnummer;
            Fornavn_ = Fornavn;
            Efternavn_ = Efternavn;
            Telfon_ = Telefon;
            Addresse_ = Addresse;
        }
    }
}
