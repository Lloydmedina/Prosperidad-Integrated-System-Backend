using eegs_back_end.SocialWelfare.Wcc.wcc_incident_report.Model;
using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.SocialWelfare.Wcc.wcc_incident_report.Repository
{
    public interface IWccIncidentReportRepository : IGlobalInterface
    {
        List<object> GetList();
        List<object> GetCaseList();
        object GetCaseByid(string case_id);
        object GetData(string tid);
        object GetDataRID(string rid);
        bool Insert(WccIncidentReportModel model);
        bool UpdateDetails(string id, WccIncidentReportModel model);
    }
    public class WccIncidentReportRepository : FormNumberGenerator, IWccIncidentReportRepository
    {

        public List<object> GetList()
        {
            string sql = @"SELECT * From mswd.wcc_incident_report ORDER BY mswd.wcc_incident_report.logs DESC";
            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("wcc-incident-report");

            List<object> list = new List<object>();
            list.Add(obj);
            list.Add(form);


            return list;
        }

        public List<object> GetCaseList()
        {
            string sql = @"SELECT * From mswd.wcc_cases";
            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);
            return obj;
        }
        public object GetCaseByid(string case_id)
        {
            string sql = @"SELECT * From mswd.wcc_cases where case_id =@cid";
            object obj = QueryModule.DataObject<object>(sql, new { cid = case_id});
            return obj;
        }

        public object GetData(string tid)
        {
            string sql = @"SELECT * From mswd.wcc_incident_report where main_pk_id = @id";
            object obj = QueryModule.DataObject<object>(sql, new { id = tid});
            ExpandoObject form = Forms.getForm("wcc-incident-report");

            List<object> res = new List<object>();
            res.Add(obj);
            res.Add(form);


            return res;
        }
        public object GetDataRID(string rid)
        {
            string sql = @"SELECT * From mswd.wcc_incident_report where wcc_reg_id = @id";
            object obj = QueryModule.DataObject<object>(sql, new { id = rid });
            ExpandoObject form = Forms.getForm("wcc-incident-report");

            List<object> res = new List<object>();
            res.Add(obj);
            res.Add(form);


            return res;
        }

        public bool Insert(WccIncidentReportModel model)
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
                    model.form_trans_no = generateFormNumber("wcc-incident-report");
                    model.main_pk_id = Guid.NewGuid().ToString();
                    //  var dateN = DateTime.Now.ToString("ddd, dd MMM yyy HH’:’mm’:’ss ‘GMT’");
                    //  model.wcc_ar_transdate = dateN.ToString();
                    sql = "insert into mswd.wcc_incident_report(" + ObjectSqlMapping.MapInsert<WccIncidentReportModel>() + ")";
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


        public bool UpdateDetails(string id, WccIncidentReportModel model)
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
                    sql = "delete from mswd.wcc_incident_report where main_pk_id = '" + id + "'";
                    QueryModule.Execute<int>(sql);     

                    model.form_trans_no = model.form_trans_no;
                    model.main_pk_id = id;
                    //  var dateN = DateTime.Now.ToString("ddd, dd MMM yyy HH’:’mm’:’ss ‘GMT’");
                    //  model.wcc_ar_transdate = dateN.ToString();
                    sql = "insert into mswd.wcc_incident_report(" + ObjectSqlMapping.MapInsert<WccIncidentReportModel>() + ")";
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
