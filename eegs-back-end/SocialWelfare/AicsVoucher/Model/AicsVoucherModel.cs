using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.AicsVoucherSetup.Model
{
    public class AicsVoucherModel
    {
        public string aics_voucher_guid { get; set; }
        public string person_guid { get; set; }

        public string aics_intake_guid { get; set; }
        public string form_trans_no { get; set; }
        public DateTime application_date { get; set; }
        public string status { get; set; }
        public string fourps_beneficiary { get; set; }
        public string ips { get; set; }
        public int member_count { get; set; }
        public int application_id { get; set; }
        public string application_name { get; set; }
        public int application_type_id { get; set; }
        public string application_type { get; set; }
        public string educational_attainment { get; set; }
        public string house_ownership { get; set; }
        public Decimal monthly_family_income { get; set; }
        public string occupation { get; set; }
        public int recommendation_id { get; set; }
        public string recommendation_name { get; set; }
        public DateTime date_recommended { get; set; }
        public Decimal total_family_income { get; set; }
        public string type_of_ethnicity { get; set; }
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
        public string full_name { get; set; }
        public int count { get; set; }
    }

}
