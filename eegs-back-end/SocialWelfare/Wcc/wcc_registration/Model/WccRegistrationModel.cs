using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace eegs_back_end.SocialWelfare.Wcc.wcc_registration.Model
{
    public class WccRegistrationModel
    {
        public string main_pk_id { get; set; }
        public string form_trans_no { get; set; }
        public string transaction_date { get; set; }
        public int transaction_status { get; set; }
        public string applicant_pid { get; set; }
        public string applicant_name { get; set; }
        public string client_pid { get; set; }
        public string client_name { get; set; }
        public int client_age { get; set; }
        public string client_address { get; set; }
        public string client_parent_pid { get; set; }
        public string client_parent_name { get; set; }
        public string referral_eid { get; set; }
        public string referral_pname { get; set; }
        public string social_worker_id { get; set; }
        public string social_worker_name { get; set; }
        public string rescue_details { get; set; }

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
