using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Health.medical_certificate.Model
{
    public class MedicalCertificateModel
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

    public class MedicalCertificate { 
        public string mc_pk_id { get; set; }   
        public string form_trans_no { get; set; }
        public DateTime mc_transaction_date { get; set; }
        public string mc_transaction_type { get; set; }
        public int mc_transaction_status { get; set; }
        public decimal mc_transaction_total_fee { get; set; }
        public string mc_person_id { get; set; }
        public string mc_person_fullname { get; set; }
        public string mc_payment_id { get; set; }
        public string mc_or_id { get; set; }
        public string mc_or_pkid { get; set; }
        public DateTime mc_or_date { get; set; }
        public string mc_issued_id { get; set; }
        public string mc_issued_date { get; set; }
        public DateTime mc_printed_date { get; set; }
        public string mc_department_head { get; set; }
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
        public string doctorid_in_charge { get; set; }
        public string doctorfname_in_charge { get; set; }
        public string doctorposition_in_charge { get; set; }
        public List<MedicalCertificateExam> mc_exams_data { get; set; }
        public PersonModel mc_person_data { get; set; }
        public HealthPayment mc_payment_data {get; set;}

    }

    public class MedicalCertificateExam {
        public string mc_dtl_id { get; set; }
        public string mc_main_id {get; set;}
        public string mc_examiner_name { get; set; }
        public int mc_lab_exam_id { get; set;}
        public string mc_lab_exam_name { get; set; }
        public DateTime mc_lab_exam_date { get; set; }
        public string mc_lab_exam_place { get; set; }
        public string mc_lab_exam_result { get; set; }
        public string mc_lab_exam_cause { get; set; }  
        public string mc_status { get; set; }
    

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

    public class MedicalCertificateLogs { 
        public int id { get; set; }
        public string date { get; set; }
        public string status { get; set; }
    }

    public class NewMedicalCertificate
    {
        public string mc_pk_id { get; set; }
        public string form_trans_no { get; set; }
        public DateTime mc_transaction_date { get; set; }
        public string mc_transaction_type { get; set; }
        public int mc_transaction_status { get; set; }
        public decimal mc_transaction_total_fee { get; set; }
        public string mc_person_id { get; set; }
        public string mc_person_fullname { get; set; }
        public int mc_person_age { get; set; }
        public string mc_person_address { get; set; }
        public string mc_payment_id { get; set; }
        public string mc_or_pkid { get; set; }
        public string mc_department_head { get; set; }
        public string doctorid_in_charge { get; set; }
        public string doctorfname_in_charge { get; set; }
        public string doctorposition_in_charge { get; set; }

        public string transaction_log { get; set; }
        public List<MedicalCertificateExam> mc_exams_data { get; set; }
        public TransactionRequestor mc_requestor { get; set; }

    }

    public class TransactionRequestor
    {
        public string mc_pk_id { get; set; }
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
