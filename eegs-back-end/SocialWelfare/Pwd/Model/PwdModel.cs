using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.PwdSetup.Model
{
    public class PwdModel
    {
        public string pwd_guid { get; set; }
        public string person_guid { get; set; }
        public string form_trans_no { get; set; }
        public DateTime application_date { get; set; }
        public string status { get; set; }
        public string fourps_beneficiary { get; set; }
        public string ips { get; set; }
        public string ip_name { get; set; }
        public int member_count { get; set; }
        public string application_type { get; set; }
        public string educational_attainment { get; set; }
        public Decimal annual_income { get; set; }
        public string occupation { get; set; }
        public string other_source_of_income { get; set; }
        public string name_of_association { get; set; }
        public string association_address { get; set; }
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
        public string pantawid_beneficiary_type { get; set; }
        public Decimal total_family_income { get; set; }
        public int civil_status_id { get; set; }
        public string civil_status_name { get; set; }
        public int type_of_disability_id { get; set; }
        public string disability_name { get; set; }
        public int cause_of_disability_id { get; set; }
        public string cause_of_disability_name { get; set; }
        public int employment_status_id { get; set; }
        public string employment_status_name { get; set; }
        public int employment_type_id { get; set; }
        public string employment_type_name { get; set; }
        public int employer_type_id { get; set; }
        public string employer_type_name { get; set; }
        public int occupation_id { get; set; }
        public string occupation_name { get; set; }
        public int blood_type_id { get; set; }
        public string blood_type_name { get; set; }
        public string sss_no { get; set; }
        public string gsis_no { get; set; }
        public string pagibig_no { get; set; }
        public string philhealth_no { get; set; }
        public string philhealth_membership { get; set; }
        public string organization { get; set; }
        public string contact_person { get; set; }
        public string office_address { get; set; }
        public string tel_no { get; set; }
        public string reporting_unit { get; set; }
        public string mobile_no { get; set; }
        public string specify_disability { get; set; }
        public string occupation_others { get; set; }
        public int region_id { get; set; }
        public string reg_code { get; set; }
        public string reg_name { get; set; }
        public string telephone_no { get; set; }
        public string phone_no { get; set; }
        public string email_address { get; set; }
        public int count { get; set; }
        public List<PwdDetails> family_details { get; set; }
    }

    public class PwdDetails
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
        public string full_name { get; set; }
        public string civil_status_name { get; set; }
    }
}
