using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.DafacSetup.Model
{
    public class DafacModel
    {
        public string dafac_guid { get; set; }
        public DateTime application_date { get; set; }
        public int region_id { get; set; }
        public string reg_name { get; set; }
        public int province_id { get; set; }
        public string province_name { get; set; }
        public string district { get; set; }
        public int barangay_id { get; set; }
        public string brgy_name { get; set; }
        public int citmun_id { get; set; }
        public string city_mun_name { get; set; }
        public string evacuation_center_guid { get; set; }
        public string venue { get; set; }
        public string person_guid { get; set; }
        public string prefix { get; set; }
        public string suffix { get; set; }
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        public string name_extension { get; set; }
        public string fourps_beneficiary { get; set; }
        public string ips { get; set; }
        public string type_of_ethnicity { get; set; }
        public DateTime birth_date { get; set; }
        public string place_of_birth { get; set; }
        public int gender_id { get; set; }
        public string gender_name { get; set; }
        public string mothers_maiden_name { get; set; }
        public string occupation { get; set; }
        public decimal monthly_family_income { get; set; }
        public string id_card_presented { get; set; }
        public string id_card_number { get; set; }
        public string primary_contact { get; set; }
        public string alternate_contact { get; set; }
        public string house_ownership { get; set; }
        public string housing_conditioning { get; set; }
        public int family_head_barangay_id { get; set; }
        public string family_head_barangay_name { get; set; }
        public string family_head_street { get; set; }
        public string status { get; set; }
        public string form_trans_no { get; set; }
        public int age { get; set; }
        public int no_of_older { get; set; }
        public int no_of_pregnant_or_lactating { get; set; }
        public int no_of_pwds_and_conditions { get; set; }
        public int member_count { get; set; }
        public string educational_attainment { get; set; }
        public string educational_name { get; set; }
        public Decimal total_family_income { get; set; }
        public int count { get; set; }
        public List<DafacDetails> family_details { get; set; }
        public List<AssistanceDetails> assistance_details { get; set; }
    }

    public class DafacDetails
    {
        public string main_guid { get; set; }
        public string person_guid { get; set; }
        public string relation { get; set; }
        public string educational_attainment { get; set; }
        public string educational_name { get; set; }
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
    }

    public class AssistanceDetails
    {
        public string main_guid { get; set; }
        public string person_guid { get; set; }
        public DateTime date { get; set; }
        public string kind_type { get; set; }
        public int qty { get; set; }
        public decimal cost { get; set; }
        public string provider { get; set; }
        public string prefix { get; set; }
        public string suffix { get; set; }
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
    }
}
