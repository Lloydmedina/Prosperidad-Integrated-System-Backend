using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.SocialWelfare.Wcc.wcc_intervention_undertaken.Model
{
    public class WccInterventionUndertakenModel
    {
        public string main_pk_id {get; set;}
        public string form_trans_no {get; set;}
        public string wcc_reg_id {get; set;}
        public int status {get; set;}
        public string client_pid {get; set;}
        public string client_pname {get; set;}
        public int client_page {get; set;}
        public string client_paddress {get; set;}
        public string client_familyid {get; set;}
        public int client_4ps {get; set;}
        public string case_tittle {get; set;}
        public int case_id {get; set;}
        public List<WccInterventionUndertakenSetup> wcc_iu_data {get; set;}
        public string logs {get; set;}
    }
    public class NewWccInterventionUndertaken
    {
        public string main_pk_id {get; set;}
        public string form_trans_no {get; set;}
        public string wcc_reg_id {get; set;}
        public int status {get; set;}
        public string client_pid {get; set;}
        public string client_pname {get; set;}
        public int client_page {get; set;}
        public string client_paddress {get; set;}
        public string client_familyid {get; set;}
        public int client_4ps {get; set;}
        public string case_tittle {get; set;}
        public int case_id {get; set;}
        public List<WccInterventionUndertakenSetup> wcc_iu_data {get; set;}
    }

    public class WccInterventionUndertakenSetup
    {
        public int iu_id {get; set;}
        public string main_iu_pkid {get; set;}
        public string iu_tittle {get; set;}
        public string iu_details {get; set;}
    }

}
