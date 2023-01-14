using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
namespace eegs_back_end.SocialWelfare.Wcc.wcc_acknowledgement_rcp.Model
{
    public class WccAcknowledgementRcpModel
    {
        public string main_pk_id { get; set; }
        public string form_trans_no { get; set; }
        public string wcc_reg_id { get; set; }
        public string wcc_summ_intake_date { get; set; }
        public int status { get; set; }
        public string client_pid { get; set; }
        public string client_pname { get; set; }
        public string client_address { get; set; }
        public string client_family_id { get; set; }
        public string client_4ps { get; set; }
        public string case_tittle { get; set; }
        public int case_id { get; set; }
        public List<RecipientModel> recipient_data { get; set; }
        public List<WitnessModel> witness_data { get; set; }
    }

    public class RecipientModel { 
        public string wcc_ar_pkid { get; set; }
        public string wcc_ar_recipient_pid { get; set; }
        public string wcc_ar_recipient_name { get; set; }
        public string wcc_ar_recipient_address { get; set; }
        public string wcc_ar_recipient_familyid { get; set; }
        public string wcc_ar_recipient_relation { get; set; }

    }
    public class WitnessModel {
        public string wcc_ar_pkid { get; set; }
        public string wcc_ar_witness_pid { get; set; }
        public string wcc_ar_witness_name { get; set; }
        public string wcc_ar_witness_address { get; set; }
        public string wcc_ar_witness_familyid { get; set; }
        public string wcc_ar_witness_relation { get; set; }
    }
}
