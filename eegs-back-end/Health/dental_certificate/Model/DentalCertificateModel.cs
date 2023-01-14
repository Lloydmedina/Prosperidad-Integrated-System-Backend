using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Health.dental_certificate.Model
{
    public class DentalCertificateModel
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

    public class DentalCertificate
    {
        public string dc_pk_id { get; set; }
        public string form_trans_no { get; set; }
        public DateTime dc_transaction_date { get; set; }
        public string dc_transaction_type { get; set; }
        public int dc_transaction_status { get; set; }
        public decimal dc_transaction_total_fee { get; set; }
        public string dc_person_id { get; set; }
        public string dc_person_fullname { get; set; }
        public string dc_payment_id { get; set; }
        public string dc_or_id { get; set; }
        public string dc_or_pkid { get; set; }
        public DateTime dc_or_date { get; set; }
        public string dc_issued_id { get; set; }
        public string dc_issued_date { get; set; }
        public DateTime dc_printed_date { get; set; }
        public string dc_department_head { get; set; }
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
        public List<DentalExam> dc_exams_data { get; set; }
        public PersonModel dc_person_data { get; set; }
        public HealthPayment dc_payment_data {get; set;}

    }
    public class DentalExam
    {
        public string dc_dtl_id { get; set; }
        public string dc_main_id { get; set; }
        public string dc_examiner_name { get; set; }
        public int dc_lab_exam_id { get; set; }
        public decimal lab_fee { get; set; }
        public string dc_lab_exam_name { get; set; }
        public DateTime dc_lab_exam_date { get; set; }
        public string dc_lab_exam_place { get; set; }
        public string dc_lab_exam_result { get; set; }
        public string dc_lab_exam_cause { get; set; }
        public string dc_status { get; set; }

    }

    public class DentalCertificateExam
    {
        public string dc_dtl_id { get; set; }
        public string dc_main_id { get; set; }
        public string dc_examiner_name { get; set; }
        public int dc_lab_exam_id { get; set; }
    
        public string dc_lab_exam_name { get; set; }
        public DateTime dc_lab_exam_date { get; set; }
        public string dc_lab_exam_place { get; set; }
        public string dc_lab_exam_result { get; set; }
        public string dc_lab_exam_cause { get; set; }
        public string dc_status { get; set; }

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

    public class DentalCertificateLogs
    {
        public int id { get; set; }
        public string date { get; set; }
        public string status { get; set; }
    }

    public class NewDentalCertificate
    {
        public string dc_pk_id { get; set; }
        public string form_trans_no { get; set; }
        public DateTime dc_transaction_date { get; set; }
        public string dc_transaction_type { get; set; }
        public int dc_transaction_status { get; set; }
        public decimal dc_transaction_total_fee { get; set; }
        public string dc_person_id { get; set; }
        public string dc_person_fullname { get; set; }
        public int dc_person_age { get; set; }
        public string dc_person_address { get; set; }
        public string dc_payment_id { get; set; }
        public string dc_or_pkid { get; set; }
        public string dc_department_head { get; set; }

        public string transaction_log { get; set; }
        public List<DentalCertificateExam> dc_exams_data { get; set; }
        public TransactionRequestor dc_requestor { get; set; }

    }

    public class TransactionRequestor
    {
        public string dc_pk_id { get; set; }
        public string requestor_id { get; set; }
        public string requestor_name { get; set; }
        public string remarks { get; set; }
        public string status { get; set; }
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
