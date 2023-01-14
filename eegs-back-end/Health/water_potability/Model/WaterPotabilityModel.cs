using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace eegs_back_end.Health.water_potability.Model
{
    public class WaterPotabilityModel
    {
        public string wp_pk_id { get; set; }
        public string form_trans_no { get; set; }
        public string wp_transaction_date { get; set; }
        public string wp_transaction_type { get; set; }
        public int wp_transaction_status { get; set; }
        public decimal wp_transaction_total_fee { get; set; }
        public string wp_department_head { get; set; }
        public string wp_payment_id { get; set; }
        public int wp_payment_status { get; set; }
        public string wp_business_id { get; set; }
        public string wp_person_id { get; set; }
        public string wp_person_fullname { get; set; }
        public string wp_source_loc { get; set; }
        public string wp_source_type { get; set; }
        public string wp_inspector_id { get; set; }
        public string employee_name { get; set; }
        public string wp_remarks { get; set; }
        public string wp_or_pkid { get; set; }
        public string wp_or_id { get; set; }
        public string transaction_log { get; set; }
        public string payment_id { get; set; }
        public string main_pk_id { get; set; }
        public string or_id { get; set; }
        public string or_date { get; set; }
        public decimal amount_paid { get; set; }
        public decimal total_fee { get; set; }
        public string payment_type { get; set; }
        public int payment_status { get; set; }
        public BusinessModel wp_business_data { get; set; }
        public PersonModel wp_person_data { get; set; }
        public HealthPayment wp_payment_data {get; set;}
    }
    
    public class NewTransactionModel
    {
        public string wp_pk_id { get; set; }
        public string form_trans_no { get; set; }
        public string wp_transaction_date { get; set; }
        public string wp_transaction_type { get; set; }
        public int wp_transaction_status { get; set; }
        public decimal wp_transaction_total_fee { get; set; }
        public string wp_department_head { get; set; }
        public string wp_payment_id { get; set; }
        public int wp_payment_status { get; set; }
        public string wp_business_id { get; set; }
        public string wp_person_id { get; set; }
        public string wp_person_fullname { get; set; }
        public string wp_source_loc { get; set; }
        public string wp_source_type { get; set; }
        public string wp_inspector_id { get; set; }
        
        public string wp_remarks { get; set; }
        public string wp_or_pkid { get; set; }
        public string wp_or_id { get; set; }
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

    public class BusinessModel
    {
        public string form_trans_no { get; set; }
        public string business_id { get; set; }
        public string business_name { get; set; }
        public string trade_name { get; set; }
        public string owner_id { get; set; }
        public string owner_name { get; set; }
        public string entity_id { get; set; }
        public string entity { get; set; }
        public int reg_status { get; set; }
        public string mobile_no { get; set; }
        public string tel_no { get; set; }
        public string email { get; set; }
        public string street { get; set; }
        public string building { get; set; }
        public int brgy_id { get; set; }
        public string brgy_name { get; set; }
        public BusinessStatus status { get; set; }
    }

    public class BusinessStatus
    {
        public string business_id { get; set; }
        public int status_id { get; set; }
        public string prev_record { get; set; }
        public string activity { get; set; }
        public string user_id { get; set; }
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
