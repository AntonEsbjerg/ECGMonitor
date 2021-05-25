using System;
using System.Collections.Generic;
using System.Text;

namespace DTO
{
    public class DTO_Measurement
    {
        public DateTime _dato { get; set; }
        public int _ekgmaaleid { get; set; }
        public int _antalmaalinger { get; set; }
        public String _sfp_maaltagerfornavn { get; set; }
        public String _sfp_maaltagerefternavn { get; set; }
        public String _sfp_maaltagermedarbjdnr { get; set; }
        public String _sfp_mt_kommentar { get; set; }
        public String _sfp_mt_org { get; set; }
        public String _borger_fornavn { get; set; }
        public String _borger_efternavn { get; set; }
        public String _borger_cprnr { get; set; }
        public bool _STEMI_suspected { get; set; }
        public int _ekgdataid { get; set; }
        public double[] _lokalECG { get; set; }
        public int _samplerate_hz { get; set; }
        public int _interval_sec { get; set; }
        public int _interval_min { get; set; }
        public string _dataformat { get; set; }
        public string _bin_eller_tekst { get; set; }
        public string _maaleformat_type { get; set; }
        public DateTime _start_tid { get; set; }
        public string _kommentar { get; set; }
        public string _maaleenhed_identifikation { get; set; }
        public bool _doctor_att { get; set; }
        public DTO_Measurement() { }

        public DTO_Measurement(bool stemi_suspected, DateTime dato, int ekgmaaleid, int antalmaalinger, string sfp_maaltagerfornavn,
           string sfp_maaltagerefternavn, string sfp_maaltagermedarbjdnr, string sfp_mt_kommentar, string sfp_mt_org, string borger_fornavn,
           string borger_efternavn, string borger_cprnr, int ekgdataid, double[] lokalECG, int samplerate_hz, int interval_sec, int interval_min,
           string dataformat, string bin_eller_tekst, string maaleformat_type, DateTime starttid, string kommentar, string maaleenhed_identifikation, bool doctor_att)
        {
            _interval_min = interval_min;
            _samplerate_hz = samplerate_hz;
            _interval_sec = interval_sec;
            _dataformat = dataformat;
            _bin_eller_tekst = bin_eller_tekst;
            _maaleformat_type = maaleformat_type;
            _start_tid = starttid;
            _kommentar = kommentar;
            _maaleenhed_identifikation = maaleenhed_identifikation;
            _ekgdataid = ekgdataid;
            _lokalECG = lokalECG;
            _STEMI_suspected = stemi_suspected;
            _dato = dato;
            _ekgmaaleid = ekgmaaleid;
            _antalmaalinger = antalmaalinger;
            _sfp_maaltagerfornavn = sfp_maaltagerfornavn;
            _sfp_maaltagerefternavn = sfp_maaltagerefternavn;
            _sfp_maaltagermedarbjdnr = sfp_maaltagermedarbjdnr;
            _sfp_mt_kommentar = sfp_mt_kommentar;
            _sfp_mt_org = sfp_mt_org;
            _borger_fornavn = borger_fornavn;
            _borger_efternavn = borger_efternavn;
            _borger_cprnr = borger_cprnr;
            _doctor_att = doctor_att;
        }
    }
}
