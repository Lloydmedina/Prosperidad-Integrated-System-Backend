using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.GeneralIntakeSetup.Model
{
    public class GeneralIntakeModel
    {
        public string general_intake_guid { get; set; }
        public DateTime application_date { get; set; }
        public string family_composition_guid { get; set; }
        public string person_guid { get; set; }
        public decimal monthly_income { get; set; }
        public string fourps_member { get; set; }
        public string ips { get; set; }
        public string house_occupancy { get; set; }
        public decimal property_cost { get; set; }
        public decimal total_family_income { get; set; }
        public string status { get; set; }
        public string contact_no { get; set; }
        public string secondary_contact_no { get; set; }
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
        public string owner { get; set; }
        public string renter { get; set; }
        public decimal estimated_damaged { get; set; }
        public string if_distressed { get; set; }
        public string physical_disability { get; set; }
        public int type_of_disability_id { get; set; }
        public string disability_name { get; set; }
        public string sources_of_income { get; set; }
        public decimal no_of_hectares { get; set; }
        public string crops_planted { get; set; }
        public string area_of_location { get; set; }
        public string other_sources_of_income { get; set; }
        public string educational_name { get; set; }
        public string full_name { get; set; }
        public int count { get; set; }
        public List<GeneralIntakeDetails> details { get; set; }
    }

    public class GeneralIntakeDetails
    {
        public string main_guid { get; set; }
        public string person_guid { get; set; }
        public string status { get; set; }
        public string relation { get; set; }
        public string educational_attainment { get; set; }
        public string occupation { get; set; }
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
