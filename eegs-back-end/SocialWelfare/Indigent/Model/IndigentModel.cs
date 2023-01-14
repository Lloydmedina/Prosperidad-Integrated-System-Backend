using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.IndigentSetup.Model
{
    public class IndigentModel
    {
        public string indigent_guid { get; set; }
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
        public DateTime date_of_membership { get; set; }
        public DateTime date_elected { get; set; }
        public string sss_pensioner { get; set; }
        public Decimal sss_monthly_pension { get; set; }
        public string gsis_pensioner { get; set; }
        public Decimal gsis_monthly_pension { get; set; }
        public string pvao_pensioner { get; set; }
        public Decimal pvao_monthly_pension { get; set; }
        public int philhealth_membership_id { get; set; }
        public int household_id_no { get; set; }
        public string pantawid_beneficiary_type { get; set; }
        public Decimal total_family_income { get; set; }
        public int civil_status_id { get; set; }
        public string civil_status_name { get; set; }
        public string house_ownership { get; set; }
        public Decimal monthly_income { get; set; }
        public string person_image { get; set; }
        public string full_name { get; set; }
        public int count { get; set; }
        public List<IndigentDetails> family_details { get; set; }
    }

    public class IndigentDetails
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
