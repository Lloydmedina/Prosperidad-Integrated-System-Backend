using eegs_back_end.SocialWelfare.Wcc.wcc_acknowledgement_rcp.Model;
using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using eegs_back_end.Health.medical_abstract.Model;
using eegs_back_end.Health.medical_certificate.Model;

namespace eegs_back_end.SocialWelfare.Wcc.wcc_acknowledgement_rcp.Repository
{
    public interface IWccAcknowledgementRcpRepository : IGlobalInterface
    {
        List<object> GetData(string tid);
        List<object> GetList();
        bool Insert(WccAcknowledgementRcpModel model);
        bool UpdateDetails(string id, WccAcknowledgementRcpModel model);
    }
    public class WccAcknowledgementRcpRepository : FormNumberGenerator, IWccAcknowledgementRcpRepository
    {
        public List<object> GetData(string tid)
        {
            string sql = @"SELECT * FROM mswd.wcc_ar_reciept WHERE mswd.wcc_ar_reciept.main_pk_id = @id";
            List<WccAcknowledgementRcpModel> res = (List<WccAcknowledgementRcpModel>)QueryModule.DataSource<WccAcknowledgementRcpModel>(sql, new { id = tid });
            if (res == null) return null;
            foreach (WccAcknowledgementRcpModel model in res)
            {
                sql = @"SELECT * FROM mswd.wcc_ar_reciept_dtl WHERE mswd.wcc_ar_reciept_dtl.wcc_ar_pkid = @rcpt";
                model.recipient_data = (List<RecipientModel>)QueryModule.DataSource<RecipientModel>(sql, new { rcpt = model.main_pk_id });
                sql = @"SELECT * FROM mswd.wcc_ar_witness_dtl WHERE mswd.wcc_ar_witness_dtl.wcc_ar_pkid = @wtns";
                model.witness_data = (List<WitnessModel>)QueryModule.DataSource<WitnessModel>(sql, new { wtns = model.main_pk_id });

            }
            ExpandoObject from = Forms.getForm("wcc-acknowledgement");
            List<object> list = new List<object>();
            list.Add(res);
            list.Add(from);
            return list;
        }

        public List<object> GetList()
        {
            string sql = @"SELECT * FROM mswd.wcc_ar_reciept ORDER BY mswd.wcc_ar_reciept.logs DESC";
            List<WccAcknowledgementRcpModel> res = (List<WccAcknowledgementRcpModel>)QueryModule.DataSource<WccAcknowledgementRcpModel>(sql);
            if (res == null) return null;
            foreach (WccAcknowledgementRcpModel model in res)
            {
                sql = @"SELECT * FROM mswd.wcc_ar_reciept_dtl where wcc_ar_pkid = @rcpt";
                model.recipient_data = (List<RecipientModel>)QueryModule.DataSource<RecipientModel>(sql, new { rcpt = model.main_pk_id });
            }
            if (res == null) return null;
            foreach (WccAcknowledgementRcpModel model in res)
            {
                sql = @"SELECT * FROM mswd.wcc_ar_witness_dtl where wcc_ar_pkid = @wtns";
                model.witness_data = (List<WitnessModel>)QueryModule.DataSource<WitnessModel>(sql, new { wtns = model.main_pk_id });
            }

            ExpandoObject form = Forms.getForm("wcc-acknowledgement");
            List<object> list = new List<object>();
            list.Add(res);
            list.Add(form);
            return list;
        }

        public bool Insert(WccAcknowledgementRcpModel model)
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
                    model.form_trans_no = generateFormNumber("wcc-acknowledgement");
                    model.main_pk_id = Guid.NewGuid().ToString();
                  //  var dateN = DateTime.Now.ToString("ddd, dd MMM yyy HH’:’mm’:’ss ‘GMT’");
                  //  model.wcc_ar_transdate = dateN.ToString();
                    sql = "insert into mswd.wcc_ar_reciept(" + ObjectSqlMapping.MapInsert<WccAcknowledgementRcpModel>() + ")";
                    int res = (int)QueryModule.Execute<int>(sql, model);

                    if (model.recipient_data.Count > 0)
                    {
                        foreach (var rcpt in model.recipient_data)
                        {
                           
                            rcpt.wcc_ar_pkid = model.main_pk_id;
                            string rsql = "insert into mswd.wcc_ar_reciept_dtl(" + ObjectSqlMapping.MapInsert<RecipientModel>() + ")";
                            int hpires = (int)QueryModule.Execute<int>(rsql, rcpt);
                        }
                    }

                    if (model.witness_data.Count > 0)
                    {
                        foreach (var wtns in model.witness_data)
                        {
                            wtns.wcc_ar_pkid = model.main_pk_id;
                            string wsql = "insert into mswd.wcc_ar_witness_dtl(" + ObjectSqlMapping.MapInsert<WitnessModel>() + ")";
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

        public bool UpdateDetails(string id, WccAcknowledgementRcpModel model)
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
                    sql = "delete from mswd.wcc_ar_reciept_dtl where wcc_ar_pkid = '" + id + "'";
                    QueryModule.Execute<int>(sql);

                    sql = "delete from mswd.wcc_ar_witness_dtl where wcc_ar_pkid = '" + id + "'";
                    QueryModule.Execute<int>(sql);

                    if (model.recipient_data.Count > 0)
                    {
                        foreach (var rcpt in model.recipient_data)
                        {
                            rcpt.wcc_ar_pkid = id;
                            string hpisql = "insert into mswd.wcc_ar_reciept_dtl(" + ObjectSqlMapping.MapInsert<RecipientModel>() + ")";
                            int hpires = (int)QueryModule.Execute<int>(hpisql, rcpt);
                        }
                    }
                    if (model.witness_data.Count > 0)
                    {
                        foreach (var wtns in model.witness_data)
                        {
                            wtns.wcc_ar_pkid = id;
                            string pesql = "insert into mswd.wcc_ar_witness_dtl(" + ObjectSqlMapping.MapInsert<WitnessModel>() + ")";
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
