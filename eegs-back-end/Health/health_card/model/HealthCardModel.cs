using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Health.health_card.Model

{
    public class HealthCardModel
    {
        public string person_details { get; set; }
        public string domain_name { get; set; }
        public string description { get; set; }
        public string domain_guid { get; set; }
        public string project_title_guid { get; set; }
        public string status { get; set; }

        public DateTime transaction_date
        {
            get { return DateTime.UtcNow; }
            set { }
        }
    }

    public class HealthCard {
        public string form_trans_no { get; set; }
        public string pk_id { get; set; }
        public string transaction_no { get; set; }
        public string transaction_date { get; set; }
        public string transaction_type { get; set; }
        public int transaction_status { get; set; }
        public string business_id { get; set; }
        public string person_id { get; set; }
        public string remarks { get; set; }
        public decimal amount_paid { get; set; } 
        public decimal payment_fee { get; set; }
        public string or_id { get; set; }
        public string or_no { get; set; }
        public string or_date { get; set; }
        public string payment_type { get; set; }    
        public int payment_status { get; set; }
        public string requestor_id { get; set; }
        public string requestor_name { get; set; } 
        public string issued_id { get; set; }
        public string transaction_log { get; set; }
        public string issued_date { get; set; }
        public string printed_date { get; set; }
        public string department_head { get; set; }
        public string mayor { get; set; }
        public List<ViewMedicalTransactionModel> hc_form_trans_data_arr { get; set; }
        public PersonModel personModels { get; set; }
        
        public HealthCardPayment paymentModels { get; set; }
    }

    public class NewTransactionModel {
        public string form_trans_no { get; set; }
        public string pk_id { get; set; }
        public string transaction_no { get; set; }
        public string transaction_date { get; set; }
        public string transaction_type { get; set; }
        public int transaction_status { get; set; }
        public string business_id { get; set; }
        public string person_id { get; set; }
        public int brgy_id { get; set; }
        public string brgy_name { get; set; }
        public string civil_status { get; set; }
        public string remarks { get; set; }
 
        public string requestor_id { get; set; }
        public string issued_id { get; set; }
        public string transaction_log { get; set; }
        public string issued_date { get; set; }
        public string printed_date { get; set; }
        public string department_head { get; set; }
        public string mayor { get; set; }
        public List<NewMedicalTransactionModel> hc_form_trans_data_arr { get; set; }
        public MedicalTransactionRequestor hc_requestor { get; set; }
    }

    public class HealthCardPayment
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

    public class MedicalTransactionRequestor{
        public string hc_pk_id { get; set; }
        public string requestor_id { get; set; }
        public string requestor_name { get; set; }  
        public string remarks { get; set; } 
        public string status { get; set; }  
    }

    public class ViewMedicalTransactionModel
    {
        public string dtl_id { get; set; }
        public string main_id { get; set; }
        public int lab_exam_id { get; set; }
        public DateTime lab_exam_date { get; set; }
        public string lab_exam_type { get; set; }
        public string lab_exam_place { get; set; }
        public string lab_exam_result { get; set; }
        public string lab_exam_cause { get; set; }
        public string examiner_name { get; set; }
        public decimal amount { get; set; }
        public int status { get; set; }


    }
    public class NewMedicalTransactionModel {
        public string dtl_id { get; set; }      
        public string main_id { get; set; }
        public int lab_exam_id { get; set; } 
        public string lab_exam_date { get; set; }
        public string lab_exam_place { get; set; }  
        public string lab_exam_result { get; set; } 
        public string lab_exam_cause { get; set; }  
        public string examiner_name { get; set; }
        public int status { get; set; }

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
