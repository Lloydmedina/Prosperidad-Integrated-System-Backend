using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace eegs_back_end.SocialWelfare.Wcc.wcc_case_conference.Model
{
    public class WccCaseConferenceModel
    {
        public string main_pk_id { get; set; }
        public string wcc_reg_id { get; set; }
        public string form_trans_no { get; set; }
        public string report_date { get; set; }
        public int case_id { get; set; }
        public string case_tittle { get; set; }
        public string client_pid { get; set; }
        public string client_pname { get; set; }
        public int client_page { get; set; }
        public string client_paddress { get; set; }
        public string client_guardian_id { get; set; }
        public string client_guardian_name { get; set; }
        public int case_conference_status {get; set;}
        public List<WccCaseDetails> wcc_cc_discussion { get; set; }
    }

    public class WccCaseDetails
    { 

        public string wcc_cc_id { get; set; }
        public string case_pod { get; set; }
        public string case_fic { get; set; }
        public int case_time_frame { get; set; }
        public string case_time_type { get; set; }
        public string case_ofc_employee_id { get; set; }
        public string case_ofc_responsible { get; set; }
        public string case_per_responsible_id { get; set; }
        public string case_per_responsible_name { get; set; }
        public string case_per_responsible_position { get; set; }
        public string case_agreement { get; set; }
    }

 
}
