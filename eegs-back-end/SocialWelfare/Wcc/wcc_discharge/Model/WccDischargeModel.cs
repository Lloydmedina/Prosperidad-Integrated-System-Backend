using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.SocialWelfare.Wcc.wcc_discharge.Model
{
    public class WccDischargeModel
    {
        public string main_pk_id { get; set; }
        public string form_trans_no { get; set; }
        public string wcc_reg_id { get; set; }
        public string wcc_summ_intake_date { get; set; }
        public int status { get; set; }
        public string client_pid { get; set; }
        public string client_pname { get; set; }
        public string client_paddress { get; set; }
        public decimal client_page { get; set; }
        public string case_tittle { get; set; }
        public int case_id { get; set; }
        public string guardian_pid { get; set; }
        public string guardian_pname { get; set; }
        public string guardian_paddress { get; set; }
        public string officer_eid { get; set; }
        public string officer_eoffice { get; set; }
        public string officer_eposition { get; set; }
        public string officer_pid { get; set; }
        public string officer_pname { get; set; }
        public string wcc_df_transdate { get; set; }
        public List<DFWitnessModel> witness_data { get; set; }
    }

    public class DFWitnessModel
    {
        public string wcc_df_pkid { get; set; }
        public string wcc_df_witness_pid { get; set; }
        public string wcc_df_witness_pname { get; set; }
        public string wcc_df_witness_paddress { get; set; }
        public string wcc_df_witness_family_id { get; set; }
        public string wcc_df_witness_relation { get; set; }
    }
}
