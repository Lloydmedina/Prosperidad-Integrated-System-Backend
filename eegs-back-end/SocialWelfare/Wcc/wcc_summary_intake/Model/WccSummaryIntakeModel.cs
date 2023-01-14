using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.SocialWelfare.Wcc.wcc_summary_intake.Model
{
    public class WccSummaryIntakeModel
    {
        public string main_pk_id { get; set; }
        public string form_trans_no { get; set; }
        public string wcc_reg_id { get; set; }
        public string wcc_summ_intake_date { get; set; }
       public int status { get; set; } 
        public string client_pid { get; set; }
        public string client_pname { get; set; }
        public string client_paddress { get; set; }
        public string client_family_id { get; set; }
        public int client_4ps { get; set; }
        public string case_tittle { get; set; }
        public int case_id { get; set; }
        public string wcc_si_assesment {get; set;}
        public string wcc_si_recommendation {get; set;}
        public string wcc_si_actiontaken {get;set;}
        public WccRegistrationModel wcc_reg_dtl { get; set; }
        public WccIncidentReportModel wcc_ir_dtl { get; set; }
        public WccCaseConferenceModel wcc_caseCon_dtt { get; set; }
        public List<WccSummaryIntakeDtlModel> wcc_si_dtl { get; set; }
        public List<FamilyModel> family_list { get; set; }
        public List<WccCaseDetails> case_dlts { get; set; }

    }
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
        public string family_id { get; set; }
        public string referral_eid { get; set; }
        public string referral_pname { get; set; }
        public string social_worker_id { get; set; }
        public string social_worker_name { get; set; }
        public string rescue_details { get; set; }

    }
    public class WccIncidentReportModel
    {
        public string main_pk_id { get; set; }
        public string wcc_reg_id { get; set; }
        public string form_trans_no { get; set; }
        public string report_date { get; set; }
        public string client_pid { get; set; }
        public string client_pname { get; set; }
        public int client_age { get; set; }
        public string client_address { get; set; }
        public string case_tittle { get; set; }
        public string case_type { get; set; }
        public string case_summary { get; set; }
        public string case_action_taken { get; set; }
        public string case_recommendation { get; set; }
    }


    public class WccCaseConferenceModel
    {
        public string main_pk_id { get; set; }
        public string wcc_ir_id { get; set; }
        public string form_trans_no { get; set; }
        public string report_date { get; set; }
        public int case_id { get; set; }
        public string case_tittle { get; set; }
        public string client_pid { get; set; }
        public string client_pname { get; set; }
        public decimal client_age { get; set; }
        public string client_address { get; set; }
        public string client_guardian_id { get; set; }
        public string client_guardian { get; set; }
        public string case_pod { get; set; }
        public string case_fic { get; set; }
        public string case_timeframe { get; set; }
        public string case_officeresponsible { get; set; }
        public string case_personresponsible { get; set; }
        public string case_agreement { get; set; }
        public string case_dtl_id { get; set; }
        //public List<WccCaseDetails> case_dtl { get; set; }
    }

    public class WccCaseDetails
    {
        public int id { get; set; }
        public string wcc_cc_id { get; set; }
        public string case_pod { get; set; }
        public string case_fic { get; set; }
        public string case_time_frame { get; set; }
        public string case_ofc_responsible_id { get; set; }
        public string case_ofc_responsible { get; set; }
        public string case_per_responsible_id { get; set; }
        public string case_per_responsible_name { get; set; }
        public string case_agreement { get; set; }

    }
    public class WccSummaryIntakeDtlModel
    {
        public string wcc_si_dtl_id { get; set; }
        public string wcc_si_id { get; set; }
        public string wcc_si_assesment { get; set; }
        public string wcc_si_recommendation { get; set; }
        public string wcc_si_actiontaken { get; set; }

    }
    public class FamilyModel { 
        public string person_guid { get; set; }
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        public string suffix { get; set; }
        public string birth_date { get; set; }
        public string phone_no { get; set; }
        public string telephone_no { get; set; }
        public int civil_status_id { get; set; }
        public string civil_status_name { get; set; }
        public string profession { get; set; }
        public int age { get; set; }
        public string main_guid { get; set; }
        public string status { get; set; }
        public string relation { get; set; }
        public string educational_attainment { get; set; }
        public string occupation { get; set; }

    }

}
