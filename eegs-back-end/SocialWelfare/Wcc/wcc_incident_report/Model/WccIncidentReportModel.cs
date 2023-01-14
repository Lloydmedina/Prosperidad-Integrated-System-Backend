using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.SocialWelfare.Wcc.wcc_incident_report.Model
{
    public class WccIncidentReportModel
    {
        public string main_pk_id { get; set; }
        public string wcc_reg_id { get; set; }
        public string form_trans_no { get; set; }
        public int report_status { get; set; }
        public string report_date { get; set; }
        public string client_pid { get; set; }
        public string client_pname { get; set; }
        public decimal client_page { get; set; }
        public string client_paddress { get; set; }
        public string case_tittle { get; set; }
        public int case_id { get; set; }
        public string case_summary { get; set; }
        public string case_action_taken { get; set; }
        public string case_recommendation { get; set; }

    }
    public class WccCases { 
        public int case_id { get; set; }
        public string case_tittle { get; set; }
        public string case_details { get; set; }
    }

    public class PersonModel
    {
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        public string suffix { get; set; }
        public string prefix { get; set; }
        public string gender_name { get; set; }
        public string civil_status_name { get; set; }
        public string citizenship { get; set; }
        public DateTime birth_date { get; set; }
        public string place_of_birth { get; set; }
        public string province_name { get; set; }
        public string city_mun_name { get; set; }
        public string brgy_name { get; set; }
        public int zipcode_id { get; set; }
        public string person_guid { get; set; }
        public string status { get; set; }
        public string street { get; set; }
        public string profession { get; set; }
        public int age { get; set; }
    }
}
