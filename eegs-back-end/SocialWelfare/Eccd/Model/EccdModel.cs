using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.EccdSetup.Model
{
    public class EccdModel
    {
        public string child_info_guid { get; set; }
        public string person_guid { get; set; }
        public string form_trans_no { get; set; }
        public DateTime application_date { get; set; }
        public string status { get; set; }
        public int member_count { get; set; }
        public string educational_attainment { get; set; }
        public string occupation { get; set; }
        public string prefix { get; set; }
        public string suffix { get; set; }
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        public DateTime birth_date { get; set; }
        public string place_of_birth { get; set; }
        public int gender_id { get; set; }
        public string gender_name { get; set; }
        public int age { get; set; }
        public int province_id { get; set; }
        public string province_name { get; set; }
        public int citmun_id { get; set; }
        public string city_mun_name { get; set; }
        public string street { get; set; }
        public int barangay_id { get; set; }
        public string brgy_name { get; set; }
        public int philhealth_membership_id { get; set; }
        public Decimal total_family_income { get; set; }
        public int civil_status_id { get; set; }
        public string civil_status_name { get; set; }
        public int type_of_disability_id { get; set; }
        public int blood_type_id { get; set; }
        public string blood_type_name { get; set; }
        public int region_id { get; set; }
        public string reg_code { get; set; }
        public string reg_name { get; set; }
        public string telephone_no { get; set; }
        public string phone_no { get; set; }
        public string email_address { get; set; }
        public string circumstances { get; set; }
        public string need_or_problem { get; set; }
        public string reason_for_applying { get; set; }
        public string family_resources { get; set; }
        public string educational_name { get; set; }
        public string profession { get; set; }
        public List<EccdDetails> family_details { get; set; }
    }

    public class EccdDetails
    {
        public string main_guid { get; set; }
        public string person_guid { get; set; }
        public string relation { get; set; }
        public string educational_attainment { get; set; }
        public string occupational_skills { get; set; }
        public Decimal occupation_income { get; set; }
        public string remarks { get; set; }
        public string prefix { get; set; }
        public string suffix { get; set; }
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        public int age { get; set; }
        public int gender_id { get; set; }
        public string gender_name { get; set; }
        public int civil_status_id { get; set; }
        public string civil_status_name { get; set; }
        public DateTime birth_date { get; set; }
    }
}
