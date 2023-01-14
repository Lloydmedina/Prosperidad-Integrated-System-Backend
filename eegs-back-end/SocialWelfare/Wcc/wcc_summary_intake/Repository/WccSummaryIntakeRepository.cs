using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using eegs_back_end.SocialWelfare.Wcc.wcc_summary_intake.Model;

namespace eegs_back_end.SocialWelfare.Wcc.wcc_summary_intake.Repository
{
    public interface IWccSummaryIntakeRepository : IGlobalInterface
    {
        List<object> GetList();
        List<object> CCList(string id);
        object GetData(string tid);
        object GetCCData(string regid);
        List<object> GetCaseDtl(string dtl_id);
        bool Insert(WccSummaryIntakeModel model);
    }
    public class WccSummaryIntakeRepository : FormNumberGenerator, IWccSummaryIntakeRepository
    {
     

        public List<object> GetList()
        {
            string sql = @"SELECT * FROM mswd.wcc_summary_intake";
            List<WccSummaryIntakeModel> res = (List<WccSummaryIntakeModel>)QueryModule.DataSource<WccSummaryIntakeModel>(sql);
            if (res == null) return null;
            foreach (WccSummaryIntakeModel model in res)
            {
                sql = @"SELECT * FROM mswd.wcc_registration WHERE main_pk_id =@regId";
                model.wcc_reg_dtl = (WccRegistrationModel)QueryModule.DataObject<WccRegistrationModel>(sql, new { regid = model.wcc_reg_id});
                sql = @"SELECT * FROM mswd.wcc_incident_report WHERE wcc_reg_id =@irId";
                model.wcc_ir_dtl = (WccIncidentReportModel)QueryModule.DataObject<WccIncidentReportModel>(sql, new { irId = model.wcc_reg_id });
                sql = @"SELECT * FROM mswd.wcc_case_conference WHERE wcc_reg_id =@casecon_id ORDER BY mswd.wcc_case_conference.logs DESC";
                model.wcc_caseCon_dtt = (WccCaseConferenceModel)QueryModule.DataObject<WccCaseConferenceModel>(sql, new { casecon_id = model.wcc_reg_id });
                var caseCon = model.wcc_caseCon_dtt;
                var ccid = caseCon.main_pk_id;
              
                sql = @"SELECT * FROM mswd.wcc_summary_intake_dtl WHERE wcc_si_id =@dtl_id ORDER BY mswd.wcc_summary_intake_dtl.logs DESC";
                model.wcc_si_dtl = (List<WccSummaryIntakeDtlModel>)QueryModule.DataSource<WccSummaryIntakeDtlModel>(sql, new{ dtl_id = model.main_pk_id });

                sql = @"SELECT mswd.family_composition_details.person_guid,
                        general.person.first_name,
                        general.person.middle_name,
                        general.person.last_name,
                        general.person.suffix,
                        general.person.birth_date,
                        general.person.phone_no,
                        general.person.telephone_no, 
                        general.person.civil_status_id,
                        general.civil_status.civil_status_name, 
                        general.person.profession,
                        general.person.age, 
                        mswd.family_composition_details.main_guid,
                        mswd.family_composition_details.status,
                        mswd.family_composition_details.relation,
                        mswd.family_composition_details.educational_attainment,
                        mswd.family_composition_details.occupation 
                        FROM mswd.family_composition_details 
                        INNER JOIN general.person ON general.person.person_guid = mswd.family_composition_details.person_guid  
                        INNER JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id 
                        WHERE mswd.family_composition_details.main_guid =@familyId";
                model.family_list = (List<FamilyModel>)QueryModule.DataSource<FamilyModel>(sql, new { familyId = model.client_family_id });


            }
            ExpandoObject from = Forms.getForm("wcc-summary-intake");
            List<object> list = new List<object>();
            list.Add(res);
            list.Add(from);
            return list;
        }

        public List<object> CCList(string id)
        {
            throw new NotImplementedException();
        }

        public object GetData(string tid)
        {
            string sql = @"SELECT * FROM mswd.wcc_summary_intake WHERE main_pk_id =@id";
            object res = QueryModule.DataObject<object>(sql, new { id = tid});

            ExpandoObject from = Forms.getForm("wcc-summary-intake");
            List<object> list = new List<object>();
            list.Add(res);
            list.Add(from);
            return list;
        }
        public object GetCCData(string regid)
        {
            string sql = @"SELECT * From mswd.wcc_case_conference where wcc_reg_id = @id";
            object obj = QueryModule.DataObject<object>(sql, new { id = regid });
            ExpandoObject form = Forms.getForm("wcc-case-conference");

            List<object> res = new List<object>();
            res.Add(obj);
            res.Add(form);


            return res;
        }
        public List<object> GetCaseDtl(string dtl_id)
        {
            string sql = @"SELECT * FROM mswd.wcc_case_conference_dtl where wcc_cc_id = @ccId 
                          ORDER BY mswd.wcc_case_conference_dtl.logs DESC 
                        ";
            List<object> res = (List<object>)QueryModule.DataSource<object>(sql, new { ccId = dtl_id });
            return res;
        }

        public bool Insert(WccSummaryIntakeModel model)
        {
            if (QueryModule.connection.State == System.Data.ConnectionState.Open)
            {
                QueryModule.connection.Close();
            }
            QueryModule.connection.Open();
            using (var ma = QueryModule.connection.BeginTransaction())
            {
                string sql = "";

                try
                {
                    model.form_trans_no = generateFormNumber("wcc-summary-intake");
                    model.main_pk_id = Guid.NewGuid().ToString();
                    //  var dateN = DateTime.Now.ToString("ddd, dd MMM yyy HH’:’mm’:’ss ‘GMT’");
                    //  model.wcc_ar_transdate = dateN.ToString();
                    sql = "insert into mswd.wcc_summary_intake(" + ObjectSqlMapping.MapInsert<WccSummaryIntakeModel>() + ")";
                    int res = (int)QueryModule.Execute<int>(sql, model);


                    ma.Commit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(sql);
                    ma.Rollback();
                    throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, ex.Message);
                }
            }
            QueryModule.connection.Close();
            return true; 
        }

    }
}
