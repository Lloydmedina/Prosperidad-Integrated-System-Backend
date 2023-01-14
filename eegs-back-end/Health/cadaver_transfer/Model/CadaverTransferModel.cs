using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace eegs_back_end.Health.cadaver_transfer.Model
{
    public class CadaverTransfer
    {
        public string ctp_pk_id { get; set; }
        public string form_trans_no { get; set; }
        public string ctp_transaction_date { get; set; }
        public string ctp_transaction_type { get; set; }
        public int ctp_transaction_status { get; set; }
        public decimal ctp_transaction_total_fee { get; set; }
        public string ctp_department_head { get; set; }
        public string ctp_payment_id { get; set; }
        public int ctp_payment_status { get; set; }
        public string ctp_exhumation_id { get; set; }
        public string ctp_person_id { get; set; }
        public string ctp_person_fullname { get; set; }
        public string ctp_cadaver_name { get; set; }
        public string ctp_cadaver_curr_loc { get; set; }
        public string ctp_cadaver_new_loc { get; set; }
        public string ctp_inspector_id { get; set; }
        public string ctp_remarks { get; set; }
        public string ctp_or_pkid { get; set; }
        public string ctp_or_id { get; set; }
        public string transaction_log { get; set; }
        public string payment_id { get; set; }
        public string main_pk_id { get; set; }
        public string or_id { get; set; }
        public string or_date { get; set; }
        public decimal amount_paid { get; set; }
        public decimal total_fee { get; set; }
        public string payment_type { get; set; }
        public int payment_status { get; set; }
        public string employee_name {get; set;}
        public PersonModel ctp_person_data { get; set; }
        public HealthPayment ctp_payment_data { get; set; }
    }
    public class NewCadaverTransferModel
    {
        public string ctp_pk_id { get; set; }
        public string form_trans_no { get; set; }
        public string ctp_transaction_date { get; set; }
        public string ctp_transaction_type { get; set; }
        public int ctp_transaction_status { get; set; }
        public decimal ctp_transaction_total_fee { get; set; }
        public string ctp_department_head { get; set; }
        public string ctp_payment_id { get; set; }
        public int ctp_payment_status { get; set; }
        public string ctp_exhumation_id { get; set; }
        public string ctp_person_id { get; set; }
        public string ctp_person_fullname { get; set; }
        public string ctp_cadaver_name { get; set; }
        public string ctp_cadaver_curr_loc { get; set; }
        public string ctp_cadaver_new_loc { get; set; }
        public string ctp_inspector_id { get; set; }
        public string ctp_remarks { get; set; }
        public string ctp_or_pkid { get; set; }
        public string ctp_or_id { get; set; }
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
