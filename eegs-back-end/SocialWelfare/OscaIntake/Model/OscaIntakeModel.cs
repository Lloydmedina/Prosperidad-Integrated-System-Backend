using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.OscaIntakeSetup.Model
{
    public class OscaIntakeModel
    {
        public string osca_intake_guid { get; set; }
        public DateTime application_date { get; set; }
        public string osca_guid { get; set; }
        public string osca_registration_guid { get; set; }
        public string fourps_beneficiary { get; set; }
        public decimal total_family_income { get; set; }
        public string status { get; set; }
        public string contact_no { get; set; }
        public string educational_attainment { get; set; }
        public string occupation { get; set; }
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        public string suffix { get; set; }
        public string prefix { get; set; }
        public int gender_id { get; set; }
        public string gender_name { get; set; }
        public int civil_status_id { get; set; }
        public string civil_status_name { get; set; }
        public DateTime birth_date { get; set; }
        public string place_of_birth { get; set; }
        public int province_id { get; set; }
        public string province_name { get; set; }
        public int citmun_id { get; set; }
        public string city_mun_name { get; set; }
        public int barangay_id { get; set; }
        public string brgy_name { get; set; }
        public string street { get; set; }
        public string religion { get; set; }
        public int age { get; set; }
        public string person_image { get; set; }
        public string form_trans_no { get; set; }
        public int member_count { get; set; }
        public string fathers_name { get; set; }
        public string mothers_name { get; set; }
        public string listahanan { get; set; }
        public string senior_citizen_organization { get; set; }
        public string ip { get; set; }
        public string other { get; set; }
        public string osca_no { get; set; }
        public string tin_no { get; set; }
        public string gsis_no { get; set; }
        public string sss_no { get; set; }
        public string philhealth_no { get; set; }
        public string others_no { get; set; }
        public int living_arrangement_id { get; set; }
        public string specify_listahanan { get; set; }
        public string specify_ip { get; set; }
        public string specify_other { get; set; }
        public string pensioner { get; set; }
        public Decimal pensioner_amount { get; set; }
        public string source_gsis { get; set; }
        public string source_sss { get; set; }
        public string source_afpslai { get; set; }
        public string source_others { get; set; }
        public string specify_other_source { get; set; }
        public string permanent_source { get; set; }
        public string what_source { get; set; }
        public string family_support { get; set; }
        public string support_type { get; set; }
        public Decimal how_much { get; set; }
        public string how_often { get; set; }
        public string in_kind { get; set; }
        public string condition { get; set; }
        public string with_maintenance { get; set; }
        public string specify_maintenance { get; set; }
        public string assessment_description { get; set; }
        public string profession { get; set; }
        public string citizenship { get; set; }
        public string educational_name { get; set; }
        public string specify_living_others { get; set; }
        public string person_guid { get; set; }
        public int count { get; set; }
        public List<OscaIntakeDetails> details { get; set; }
    }

    public class OscaIntakeDetails
    {
        public string main_guid { get; set; }
        public string person_guid { get; set; }
        public string status { get; set; }
        public string relation { get; set; }
        public string educational_attainment { get; set; }
        public string occupation { get; set; }
        public string occupational_skills { get; set; }
        public decimal occupation_income { get; set; }
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        public string suffix { get; set; }
        public string prefix { get; set; }
        public int age { get; set; }
        public int civil_status_id { get; set; }
        public string civil_status_name { get; set; }
    }
}
