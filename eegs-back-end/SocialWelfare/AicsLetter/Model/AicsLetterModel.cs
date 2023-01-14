using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.AicsLetterSetup.Model
{
    public class AicsLetterModel
    {
        public string aics_letter_guid { get; set; }

        public string aics_intake_guid { get; set; }
        public string person_guid { get; set; }

        public DateTime aics_intake_date { get; set; }
        public string form_trans_no { get; set; }
        public DateTime application_date { get; set; }
        public string status { get; set; }
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
        public string full_name { get; set; }
        public int count { get; set; }
        public DateTime date_occurence { get; set; }
    }
    
}
