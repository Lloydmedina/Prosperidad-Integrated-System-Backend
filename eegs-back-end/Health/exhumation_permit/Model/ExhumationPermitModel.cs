using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Health.exhumation_permit.Model
{
    public class ExPermitModel
    { 
        public string exp_pk_id { get; set; }
        public string form_trans_no { get; set; }
        public DateTime exp_transaction_date { get; set; }
        public string exp_transaction_type { get; set; }
        public int exp_transaction_status { get; set; }
        public decimal exp_transaction_total_fee { get; set; }
        public string exp_department_head { get; set; }
        public string exp_payment_id { get; set; }
        public int exp_payment_status { get; set; }
        public string exp_person_id { get; set; }
        public string exp_person_fullname { get; set; }
        public string exp_person_address { get; set; }
        public string exp_cadaver_name { get; set; }
        public string exp_cadaver_address { get; set; }
        public string exp_cadaver_buriedat { get; set; }
        public string exp_remarks { get; set; }
        public string exp_or_pkid { get; set; }
        public string exp_or_id { get; set; }
        public string time_stamp { get; set; }
        public string transaction_log { get; set; }
        public string payment_id { get; set; }
        public string main_pk_id { get; set; }
        public string or_id { get; set; }
        public string or_date { get; set; }
        public decimal amount_paid { get; set; }
        public decimal total_fee { get; set; }
        public string payment_type { get; set; }
        public int payment_status { get; set; }
        public PersonModel exp_person_data { get; set; }
        public HealthPayment exp_payment_data {get; set;}

    }
    public class NewExPermitModel
    {
        public string exp_pk_id { get; set; }
        public string form_trans_no { get; set; }
        public DateTime exp_transaction_date { get; set; }
        public string exp_transaction_type { get; set; }
        public int exp_transaction_status { get; set; }
        public decimal exp_transaction_total_fee { get; set; }
        public string exp_department_head { get; set; }
        public string exp_payment_id { get; set; }
        public int exp_payment_status { get; set; }
        public string exp_person_id { get; set; }
        public string exp_person_fullname { get; set; }
        public string exp_person_address { get; set; }
        public string exp_cadaver_name { get; set; }
        public string exp_cadaver_address { get; set; }
        public string exp_cadaver_buriedat { get; set; }
        public string exp_remarks { get; set; }
        public string exp_or_pkid { get; set; }
        public string exp_or_id { get; set; }
        public string transaction_log { get; set; }
    }

    public class HealthPayment
    {
        public string payment_id { get; set; }
        public string main_pk_id { get; set; }
        public string or_id { get; set; }
        public string or_date { get; set; }
        public decimal amount_paid { get; set; }
        public decimal total_fee { get; set; }
        public string payment_type { get; set; }
        public int payment_status { get; set; }
    }

    public class PersonModel
    {
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        public string suffix { get; set; }
        public string prefix { get; set; }
        public int gender_id { get; set; }
        public string gender_name { get; set; }
        public int civil_status_id { get; set; }
        public string civil_status_name { get; set; }
        public string citizenship { get; set; }
        public int blood_type_id { get; set; }
        public DateTime birth_date { get; set; }
        public string place_of_birth { get; set; }
        public int region_id { get; set; }
        public string reg_name { get; set; }
        public int province_id { get; set; }
        public string province_name { get; set; }
        public int citmun_id { get; set; }
        public string city_mun_name { get; set; }
        public int barangay_id { get; set; }
        public string brgy_name { get; set; }
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

    }
}
