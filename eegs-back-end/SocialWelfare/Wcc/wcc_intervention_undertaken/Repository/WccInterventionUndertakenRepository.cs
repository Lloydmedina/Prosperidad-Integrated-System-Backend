using eegs_back_end.SocialWelfare.Wcc.wcc_intervention_undertaken.Model;
using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.SocialWelfare.Wcc.wcc_intervention_undertaken.Repository
{
     public interface IWccInterventionUndertakenRepository : IGlobalInterface
     {
         List<object> GetList();
         List<object> GetIUlist();
         bool Insert(NewWccInterventionUndertaken model);
         List<object> GetData(string tid);

     }
    public class WccInterventionUndertakenRepository : FormNumberGenerator, IWccInterventionUndertakenRepository
    {
          public List<object> GetList()
        {
            {
                string sql = @"SELECT * FROM mswd.wcc_intervention_undertaken 
                                ORDER BY mswd.wcc_intervention_undertaken.logs DESC";
                List<WccInterventionUndertakenModel> res = (List<WccInterventionUndertakenModel>)QueryModule.DataSource<WccInterventionUndertakenModel>(sql);
                 if (res == null) return null;
                foreach (WccInterventionUndertakenModel model in res)
                {
                    sql = @"SELECT * FROM mswd.wcc_intervention_undertaken_dtl where main_iu_pkid = @ius";
                    model.wcc_iu_data = (List<WccInterventionUndertakenSetup>)QueryModule.DataSource<WccInterventionUndertakenSetup>(sql, new { ius = model.main_pk_id });
                }

                ExpandoObject form = Forms.getForm("wcc-custody-turnover");
                List<object> list = new List<object>();
                list.Add(res);
                list.Add(form);
                return list;
            }
        }
        public List<object> GetData(string tid)
        {
            string sql = @"SELECT * FROM mswd.wcc_intervention_undertaken WHERE mswd.wcc_intervention_undertaken.main_pk_id = @id";
            List<WccInterventionUndertakenModel> res = (List<WccInterventionUndertakenModel>)QueryModule.DataSource<WccInterventionUndertakenModel>(sql, new { id = tid });
            if (res == null) return null;
            foreach (WccInterventionUndertakenModel model in res)
            {
                sql = @"SELECT * FROM mswd.wcc_intervention_undertaken_dtl where main_iu_pkid = @wtns";
                model.wcc_iu_data = (List<WccInterventionUndertakenSetup>)QueryModule.DataSource<WccInterventionUndertakenSetup>(sql, new { wtns = model.main_pk_id });

            }
            ExpandoObject from = Forms.getForm("wcc-custody-turnover");
            List<object> list = new List<object>();
            list.Add(res);
            list.Add(from);
            return list;
        }
        public List<object> GetIUlist()
        {
            string sql = @"SELECT * From mswd.wcc_intervention_undertaken_setup";
            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);
            return obj;
        }

        public bool Insert (NewWccInterventionUndertaken model)
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
                    model.form_trans_no = generateFormNumber("wcc-intervention-undertaken");
                    model.main_pk_id = Guid.NewGuid().ToString();
                    //  var dateN = DateTime.Now.ToString("ddd, dd MMM yyy HH’:’mm’:’ss ‘GMT’");
                    //  model.wcc_ar_transdate = dateN.ToString();
                    sql = "insert into mswd.wcc_intervention_undertaken(" + ObjectSqlMapping.MapInsert<NewWccInterventionUndertaken>() + ")";
                    int res = (int)QueryModule.Execute<int>(sql, model);

                    if (model.wcc_iu_data.Count > 0)
                    {
                        foreach (var iu in model.wcc_iu_data)
                        {
                            iu.main_iu_pkid = model.main_pk_id;
                            string iuql = "insert into mswd.wcc_intervention_undertaken_dtl(" + ObjectSqlMapping.MapInsert<WccInterventionUndertakenSetup>() + ")";
                            int iures = (int)QueryModule.Execute<int>(iuql, iu);
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
