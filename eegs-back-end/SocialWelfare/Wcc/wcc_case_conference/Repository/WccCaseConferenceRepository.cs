using eegs_back_end.SocialWelfare.Wcc.wcc_case_conference.Model;
using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
namespace eegs_back_end.SocialWelfare.Wcc.wcc_case_conference.Repository
{
    public interface IWccCaseConferenceRepository : IGlobalInterface
    {
        List<object> GetList();
        object GetData(string tid);
        List<object> GetCaseDtl(string dtl_id);
        object GetIRData(string tid);
        bool Insert(WccCaseConferenceModel model);
         bool UpdateDetails(string id, WccCaseConferenceModel model);
    }
    public class WccCaseConferenceRepository : FormNumberGenerator, IWccCaseConferenceRepository
    {
        public List<object> GetList()
        {
            string sql = @"SELECT * From mswd.wcc_case_conference ORDER BY mswd.wcc_case_conference.logs DESC";
            List<WccCaseConferenceModel> obj = (List<WccCaseConferenceModel>)QueryModule.DataSource<WccCaseConferenceModel>(sql);
            if (obj == null) return null;
            foreach (WccCaseConferenceModel model in obj)
            {
                sql = @"SELECT * FROM mswd.wcc_case_conference_dtl where wcc_cc_id = @ccId 
                          ORDER BY mswd.wcc_case_conference_dtl.logs DESC 
                        ";
                model.wcc_cc_discussion = (List<WccCaseDetails>)QueryModule.DataSource<WccCaseDetails>(sql, new { ccId = model.main_pk_id });
            }
            ExpandoObject form = Forms.getForm("wcc-case-conference");

            List<object> list = new List<object>();
            list.Add(obj);
            list.Add(form);


            return list;
        }

        public object GetData(string tid)
        {
            string sql = @"SELECT * From mswd.wcc_case_conference where main_pk_id = @id";
            object obj = QueryModule.DataObject<object>(sql, new { id = tid });
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
            List<object> res = (List<object>)QueryModule.DataSource<object>(sql, new { ccId = dtl_id});
            return res;
        }

        public object GetIRData(string tid)
        {
            string sql = @"SELECT * From mswd.wcc_incident_report where wcc_reg_id = @id";
            object obj = QueryModule.DataObject<object>(sql, new { id = tid });
            ExpandoObject form = Forms.getForm("wcc-case-conference");

            List<object> res = new List<object>();
            res.Add(obj);
            res.Add(form);


            return res;
        }

               public bool Insert(WccCaseConferenceModel model)
        {
            if (QueryModule.connection.State == System.Data.ConnectionState.Open)
            {
                QueryModule.connection.Close();
            }
            QueryModule.connection.Open();
            using (var ma = QueryModule.connection.BeginTransaction())
            {
                try
                {
                    model.form_trans_no = generateFormNumber("wcc-case-conference");
                    model.main_pk_id = Guid.NewGuid().ToString();
                    //  var dateN = DateTime.Now.ToString("ddd, dd MMM yyy HH’:’mm’:’ss ‘GMT’");
                    //  model.wcc_ar_transdate = dateN.ToString();
                    string sql = "insert into mswd.wcc_case_conference(" + ObjectSqlMapping.MapInsert<WccCaseConferenceModel>() + ")";
                    int res = (int)QueryModule.Execute<int>(sql, model);
                    if (model.wcc_cc_discussion.Count > 0){
                        foreach(var cc in model.wcc_cc_discussion)
                        {
                            cc.wcc_cc_id = model.main_pk_id;
                            string ccsql = "insert into mswd.wcc_case_conference_dtl(" + ObjectSqlMapping.MapInsert<WccCaseDetails>() + ")";
                            int hpires = (int)QueryModule.Execute<int>(ccsql, cc);
                        }
                    }

                    ma.Commit();
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(sql);
                    ma.Rollback();
                    throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, ex.Message);
                }
            }
            QueryModule.connection.Close();
            return true; 
        }


        public bool UpdateDetails(string id, WccCaseConferenceModel model)
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
                    sql = "delete from mswd.wcc_case_conference where main_pk_id = '" + id + "'";
                    QueryModule.Execute<int>(sql);

                    sql = "delete from mswd.wcc_case_conference_dtl where wcc_cc_id = '" + id + "'";
                    QueryModule.Execute<int>(sql);    

                    model.form_trans_no = model.form_trans_no;
                    model.main_pk_id = id;
                    //  var dateN = DateTime.Now.ToString("ddd, dd MMM yyy HH’:’mm’:’ss ‘GMT’");
                    //  model.wcc_ar_transdate = dateN.ToString();
                    sql = "insert into mswd.wcc_case_conference(" + ObjectSqlMapping.MapInsert<WccCaseConferenceModel>() + ")";
                    int res = (int)QueryModule.Execute<int>(sql, model);

                    if (model.wcc_cc_discussion.Count > 0)
                    {
                        foreach (var wtns in model.wcc_cc_discussion)
                        {
                            wtns.wcc_cc_id = id;
                            string pesql = "insert into mswd.wcc_case_conference_dtl(" + ObjectSqlMapping.MapInsert<WccCaseDetails>() + ")";
                            int peres = (int)QueryModule.Execute<int>(pesql, wtns);
                        }
                    }

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
