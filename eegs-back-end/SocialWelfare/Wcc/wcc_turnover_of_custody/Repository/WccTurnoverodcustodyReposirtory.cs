using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using eegs_back_end.SocialWelfare.Wcc.wcc_turnover_of_custody.Model;


namespace eegs_back_end.SocialWelfare.Wcc.wcc_turnover_of_custody.Repository
{
    public interface IWccTurnoverodcustodyReposirtory : IGlobalInterface
    {
        List<object> GetData(string tid);
        List<object> GetList();
        bool Insert(WccTurnoverodcustodyModel model);
        bool UpdateDetails(string id, WccTurnoverodcustodyModel model);
    }
    public class WccTurnoverodcustodyReposirtory : FormNumberGenerator, IWccTurnoverodcustodyReposirtory
    {
        public List<object> GetData(string tid)
        {
            string sql = @"SELECT * FROM mswd.wcc_to_custody WHERE mswd.wcc_to_custody.main_pk_id = @id";
            List<WccTurnoverodcustodyModel> res = (List<WccTurnoverodcustodyModel>)QueryModule.DataSource<WccTurnoverodcustodyModel>(sql, new { id = tid });
            if (res == null) return null;
            foreach (WccTurnoverodcustodyModel model in res)
            {
                sql = @"SELECT * FROM mswd.wcc_to_custody_witness_dtl WHERE mswd.wcc_to_custody_witness_dtl.wcc_toc_pkid = @wtns";
                model.witness_data = (List<TocWitnessModel>)QueryModule.DataSource<TocWitnessModel>(sql, new { wtns = model.main_pk_id });

            }
            ExpandoObject from = Forms.getForm("wcc-custody-turnover");
            List<object> list = new List<object>();
            list.Add(res);
            list.Add(from);
            return list;
        }

        public List<object> GetList()
        {
            {
                string sql = @"SELECT * FROM mswd.wcc_to_custody 
                                ORDER BY mswd.wcc_to_custody.logs DESC";
                List<WccTurnoverodcustodyModel> res = (List<WccTurnoverodcustodyModel>)QueryModule.DataSource<WccTurnoverodcustodyModel>(sql);
                if (res == null) return null;
                foreach (WccTurnoverodcustodyModel model in res)
                {
                    sql = @"SELECT * FROM mswd.wcc_to_custody_witness_dtl where wcc_toc_pkid = @wtns";
                    model.witness_data = (List<TocWitnessModel>)QueryModule.DataSource<TocWitnessModel>(sql, new { wtns = model.main_pk_id });
                }

                ExpandoObject form = Forms.getForm("wcc-custody-turnover");
                List<object> list = new List<object>();
                list.Add(res);
                list.Add(form);
                return list;
            }
        }

        public bool Insert(WccTurnoverodcustodyModel model)
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
                    model.form_trans_no = generateFormNumber("wcc-custody-turnover");
                    model.main_pk_id = Guid.NewGuid().ToString();
                    //  var dateN = DateTime.Now.ToString("ddd, dd MMM yyy HH’:’mm’:’ss ‘GMT’");
                    //  model.wcc_ar_transdate = dateN.ToString();
                    sql = "insert into mswd.wcc_to_custody(" + ObjectSqlMapping.MapInsert<WccTurnoverodcustodyModel>() + ")";
                    int res = (int)QueryModule.Execute<int>(sql, model);

                    if (model.witness_data.Count > 0)
                    {
                        foreach (var wtns in model.witness_data)
                        {
                            wtns.wcc_toc_pkid = model.main_pk_id;
                            string wsql = "insert into mswd.wcc_to_custody_witness_dtl(" + ObjectSqlMapping.MapInsert<TocWitnessModel>() + ")";
                            int hpires = (int)QueryModule.Execute<int>(wsql, wtns);
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

        public bool UpdateDetails(string id, WccTurnoverodcustodyModel model)
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
                    sql = "delete from mswd.wcc_to_custody where main_pk_id = '" + id + "'";
                    QueryModule.Execute<int>(sql);

                    sql = "delete from mswd.wcc_to_custody_witness_dtl where wcc_toc_pkid = '" + id + "'";
                    QueryModule.Execute<int>(sql);

                    model.form_trans_no = model.form_trans_no;
                    model.main_pk_id = id;
                    //  var dateN = DateTime.Now.ToString("ddd, dd MMM yyy HH’:’mm’:’ss ‘GMT’");
                    //  model.wcc_ar_transdate = dateN.ToString();
                    sql = "insert into mswd.wcc_to_custody(" + ObjectSqlMapping.MapInsert<WccTurnoverodcustodyModel>() + ")";
                    int res = (int)QueryModule.Execute<int>(sql, model);

                    if (model.witness_data.Count > 0)
                    {
                        foreach (var wtns in model.witness_data)
                        {
                            wtns.wcc_toc_pkid = id;
                            string pesql = "insert into mswd.wcc_to_custody_witness_dtl(" + ObjectSqlMapping.MapInsert<TocWitnessModel>() + ")";
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
