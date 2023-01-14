using eegs_back_end.DbModule;
using eegs_back_end.Shell.Form.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace eegs_back_end.Helpers
{
    public class FormNumberGenerator
    {
        static int getCurrentSeries(string path, FormModel model)
        {
            string sql = "";
            
            switch (path)
            {
                case "domain":
                    sql = @"SELECT form_trans_no FROM general.domain
                            order by id desc limit 1";
                    break;
                case "health-card-individual":
                    sql = @"SELECT form_trans_no FROM health.health_card_main
                            order by id desc limit 1";
                    break;
                case "fc-registration":
                    sql = @"SELECT form_trans_no FROM mswd.family_composition_head
                            order by id desc limit 1";
                    break;
                case "fc-intake":
                    sql = @"SELECT form_trans_no FROM mswd.general_intake
                            order by id desc limit 1";
                    break;
                case "business":
                    sql = @"SELECT form_trans_no FROM general.business
                            order by id desc limit 1";
                    break;
                case "evacuation-center":
                    sql = @"SELECT form_trans_no FROM general.evacuation_center
                            order by id desc limit 1";
                    break;
                case "dafac-registration":
                    sql = @"SELECT form_trans_no FROM mswd.dafac
                            order by id desc limit 1";
                    break;
                case "dafac-intake":
                    sql = @"SELECT form_trans_no FROM mswd.dafac_intake
                            order by id desc limit 1";
                    break;
                case "aics-registration":
                    sql = @"SELECT form_trans_no FROM mswd.aics
                            order by id desc limit 1";
                    break;
                case "aics-intake":
                    sql = @"SELECT form_trans_no FROM mswd.aics_intake
                            order by id desc limit 1";
                    break;
                case "dental-certificate":
                    sql = @"SELECT form_trans_no FROM health.dental_cert_main
                            order by id desc limit 1";
                    break;
                case "medical-certificate":
                    sql = @"SELECT form_trans_no FROM health.med_cert_main
                            order by id desc limit 1";
                    break;
                case "sanitary-permit":
                    sql = @"SELECT form_trans_no FROM health.sanitary_permit_main
                            order by id desc limit 1";
                    break;
                case "medical-abstract":
                    sql = @"SELECT form_trans_no FROM health.med_abstract_main
                            order by id desc limit 1";
                    break;
                case "osca-registration":
                    sql = @"SELECT form_trans_no FROM mswd.osca_registration
                            order by id desc limit 1";
                    break;
                case "osca-id":
                    sql = @"SELECT form_trans_no FROM mswd.osca
                            order by id desc limit 1";
                    break;
                case "osca-intake":
                    sql = @"SELECT form_trans_no FROM mswd.osca_intake
                            order by id desc limit 1";
                    break;
                case "tenant-profile":
                    sql = @"SELECT form_trans_no FROM market.tenant_profile
                            order by id desc limit 1";
                    break;
                case "exhumation-permit":
                    sql = @"SELECT form_trans_no FROM health.exhumation_permit_main
                            order by id desc limit 1";
                    break;
                case "cadaver-transfer":
                    sql = @"SELECT form_trans_no FROM health.cadaver_transfer_main
                            order by id desc limit 1";
                    break;
                case "water-potability":
                    sql = @"SELECT form_trans_no FROM health.water_potability_main
                     order by id desc limit 1";
                    break;
                case "rental-application":
                    sql = @"SELECT form_trans_no FROM market.rental_application
                            order by id desc limit 1";
                    break;
                case "wcc-registration":
                    sql = @"SELECT form_trans_no FROM mswd.wcc_registration
                            order by id desc limit 1";
                    break;
                case "wcc-incident-report":
                    sql = @"SELECT form_trans_no FROM mswd.wcc_incident_report
                            order by id desc limit 1";
                    break;
                case "wcc-case-conference":
                    sql = @"SELECT form_trans_no FROM mswd.wcc_case_conference
                            order by id desc limit 1";
                    break;
                case "wcc-intervention-undertaken":
                    sql = @"SELECT form_trans_no FROM mswd.wcc_intervention_undertaken
                            order by id desc limit 1";
                    break;
                case "wcc-summary-intake":
                    sql = @"SELECT form_trans_no FROM mswd.wcc_summary_intake
                            order by id desc limit 1";
                    break;
                case "wcc-acknowledgement":
                    sql = @"SELECT form_trans_no FROM mswd.wcc_ar_reciept
                            order by id desc limit 1";
                    break;
                case "wcc-custody-turnover":
                    sql = @"SELECT form_trans_no FROM mswd.wcc_to_custody
                            order by id desc limit 1";
                    break;
                case "wcc-discharge":
                    sql = @"SELECT form_trans_no FROM mswd.wcc_discharge
                            order by id desc limit 1";
                    break;

                case "pwd-registration":
                    sql = @"SELECT form_trans_no FROM mswd.pwd
                            order by id desc limit 1";
                    break;
                case "pwd-intake":
                    sql = @"SELECT form_trans_no FROM mswd.pwd_intake
                            order by id desc limit 1";
                    break;
                case "solo-parent-registration":
                    sql = @"SELECT form_trans_no FROM mswd.solo_parent
                            order by id desc limit 1";
                    break;
                case "solo-parent-intake":
                    sql = @"SELECT form_trans_no FROM mswd.solo_parent_intake
                            order by id desc limit 1";
                    break;
                case "child-info-registration":
                    sql = @"SELECT form_trans_no FROM mswd.child_info
                            order by id desc limit 1";
                    break;
                case "child-info-intake":
                    sql = @"SELECT form_trans_no FROM mswd.child_info_intake
                            order by id desc limit 1";
                    break;
                case "market-billing":
                    sql = @"SELECT form_trans_no FROM market.billing_main
                            order by id desc limit 1";
                    break;
                case "aics-letter":
                    sql = @"SELECT form_trans_no FROM mswd.aics_letter
                            order by id desc limit 1";
                    break;
                case "aics-voucher":
                    sql = @"SELECT form_trans_no FROM mswd.aics_voucher
                            order by id desc limit 1";
                    break;
                case "waitlisted-report":
                    sql = @"SELECT form_trans_no FROM mswd.waitlisted_report
                            order by id desc limit 1";
                    break;
            }


            string trans_no = (string)QueryModule.DataObject<string>(sql);

            if (trans_no == null || trans_no == "")
                return 0;


            string[] trans_arr = trans_no.Split(model.series_separator);

            int result = Convert.ToInt32(trans_arr.Where(x => x.Length == model.series_length).Select(c => c).FirstOrDefault());
          

            return result;
        }


        static string getFormID(string path)
        {
            string sql = @"SELECT form_guid FROM general.activity 
                            where activity.executable_path LIKE '%" + path + @"%'
                            LIMIT 1";
            string form_guid = (string)QueryModule.DataObject<string>(sql);

            return form_guid;
        }

        public static string generateFormNumber(string path)
        {

            string form_id = getFormID(path);
            FormModel seriesModel = getSeriesModel(form_id);
            int series = getCurrentSeries(path, seriesModel);
            List<FormSeries> formSeries = getFormSeries(form_id);

            if (series == 0)
            {
                series = seriesModel.series_start;
            }
            else
            {
                series += 1;
            }


            string p = "";
            for(int i =0; i < seriesModel.series_length; i++)
            {
                p += "0";
            }

            string new_ref_no = "";
            foreach (FormSeries obj in formSeries)
            {
                if (obj.series_include == "True")
                {
                    switch (obj.series_type)
                    {
                        case "Prefix":
                            if(new_ref_no == "")
                            {
                                new_ref_no = obj.series_format;
                            }
                            else
                            {
                                new_ref_no += seriesModel.series_separator + obj.series_format;
                            }
                            break;
                        case "Month":
                            if (new_ref_no == "")
                            {
                                new_ref_no = DateTime.Now.ToString(obj.series_format);
                            }
                            else
                            {
                                new_ref_no += seriesModel.series_separator + DateTime.Now.ToString(obj.series_format);
                            }
                            break;
                        case "Series":
                            if (new_ref_no == "")
                            {
                                new_ref_no = series.ToString(p);
                            }
                            else
                            {
                                new_ref_no += seriesModel.series_separator + series.ToString(p);
                            }
                            break;
                        case "Year":
                            if (new_ref_no == "")
                            {
                                new_ref_no = DateTime.Now.ToString(obj.series_format.ToLower());
                            }
                            else
                            {
                                new_ref_no += seriesModel.series_separator + DateTime.Now.ToString(obj.series_format.ToLower());
                            }
                            break;
                    }
                }
            }

            //new_ref_no = ref_no[0] + seriesModel.series_separator + DateTime.Now.ToString(ref_no[1]) + seriesModel.series_separator + series.ToString(p) + seriesModel.series_separator + DateTime.Now.ToString(ref_no[3].ToLower());

            return new_ref_no;
        }

        static FormModel getSeriesModel(string form_id)
        {
            string sql = @"select series_start, series_reset, series_separator, series_ref_no, series_length 
                           from general.tbl_user_form where form_guid = @form_id";
            FormModel seriesStart = (FormModel)QueryModule.DataObject<FormModel>(sql, new { form_id = form_id });

            return seriesStart;
        }

        static List<FormSeries> getFormSeries(string form_id)
        {
            string sql = @"select * from general.tbl_form_setup_series where form_guid = @form_id
                           order by series_order asc";
            List<FormSeries> formSeries = (List<FormSeries>)QueryModule.DataSource<FormSeries>(sql, new { form_id = form_id });

            return formSeries;
        }
    }
}
 