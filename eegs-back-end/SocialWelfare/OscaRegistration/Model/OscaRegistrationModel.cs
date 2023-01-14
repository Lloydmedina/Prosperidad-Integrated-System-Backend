using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.OscaRegistrationSetup.Model
{
    public class OscaRegistrationModel
    {
        public string osca_registration_guid { get; set; }
        public string person_guid { get; set; }
        public string form_trans_no { get; set; }
        public DateTime application_date { get; set; }
        public string status { get; set; }
        public string phone_no { get; set; }
        public string telephone_no { get; set; }
        public string email_address { get; set; }
        public int blood_type_id { get; set; }
        public string blood_type_name { get; set; }
        public int educational_attainment { get; set; }
        public string religion { get; set; }
        public string occupation { get; set; }
        public string other_source_of_income { get; set; }
        public string name_of_association { get; set; }
        public string address_of_association { get; set; }
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
        public string date_of_membership { get; set; }
        public string date_elected { get; set; }
        public string sss_no { get; set; }
        public Decimal sss_monthly_pension { get; set; }
        public string gsis_no { get; set; }
        public Decimal gsis_monthly_pension { get; set; }
        public string tin_no { get; set; }
        public string phil_health_no { get; set; }
        public string incase_of_emergency { get; set; }
        public string contact { get; set; }
        public int civil_status_id { get; set; }
        public string civil_status_name { get; set; }
        public int employment_status_id { get; set; }
        public int classification_id { get; set; }
        public int region_id { get; set; }
        public string profession { get; set; }
        public string educational_name { get; set; }
        public string reg_name { get; set; }
        public string reg_code { get; set; }
        public string employment_status_name { get; set; }
        public string classification_name { get; set; }
        public Decimal annual_income { get; set; }
        public string household_id_no { get; set; }
        public int philhealth_membership_id { get; set; }
        public string fourps_beneficiary { get; set; }
        public string fourps_beneficiary_type { get; set; }
        public string gsis_pensioner { get; set; }
        public string sss_pensioner { get; set; }
        public int living_arrangement_id { get; set; }
        public int member_count { get; set; }
        public string specify_others { get; set; }
        public string citizenship { get; set; }
        public string philhealth_no { get; set; }
        public string person_image { get; set; }
        public int count { get; set; }
        public List<OscaRegistrationDetails> family_details { get; set; }
    }

    public class OscaRegistrationDetails
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
    }
}
