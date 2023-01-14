using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.PersonSetup.Model
{
    public class PersonModel
    {
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        public string suffix { get; set; }
        public string prefix { get; set; }
        public int gender_id { get; set; }
        public int civil_status_id { get; set; }
        public string citizenship { get; set; }
        public int blood_type_id { get; set; }
        public DateTime birth_date { get; set; }
        public string place_of_birth { get; set; }
        public int region_id { get; set; }
        public int province_id { get; set; }
        public int citmun_id { get; set; }
        public int barangay_id { get; set; }
        public int zipcode_id { get; set; }
        public string person_guid { get; set; }
        public string status { get; set; }
        public string tin { get; set; }
        public string street { get; set; }
        public decimal height { get; set; }
        public decimal weight { get; set; }
        public string profession { get; set; }
        public string religion { get; set; }
        public string person_image { get; set; }
        public string person_file_name { get; set; }
        public string person_base64 { get; set; }
        public int age { get; set; }
        public string default_checked { get; set; }
        public int educational_attainment { get; set; }

        public string phone_no { get; set; }
        public string telephone_no { get; set; }
        public string email_address { get; set; }
        //public int? age_count { get; set; }
        public string full_name { get; set; }
        public int count { get; set; }
    }

    public class ImageModel
    {
        public string name { get; set; }
        public string status { get; set; }
        public string url { get; set; }
        public string thumbUrl { get; set; }
    }
}
