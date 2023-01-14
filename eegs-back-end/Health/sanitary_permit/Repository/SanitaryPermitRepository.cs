using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using eegs_back_end.Health.sanitary_permit.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Dynamic;

namespace eegs_back_end.Health.sanitary_permit.Repository
{
    public interface ISanitaryPermitRepository : IGlobalInterface
    {
        
        List<object> GetBusinessList();
        List<object> GetTransactionList();
        List<object> GetInspectorList();
        List<object> CheckTrans(string ID);
        object GetBusinessData(string ID);
        object GetInspectorData(string ID);
        bool Insert(SPTransactionModel model);
        List<object> GetList();
        object GetTransData(string ID);

        bool UpdateTransaction(string id, SPTransactionModel model);
        bool PayedTrans(string id);
        bool DeleteTrans(string id);
        bool RevertTrans(string id);
        public class SanitaryPermitRepository : FormNumberGenerator, ISanitaryPermitRepository
        {

            public List<object> GetBusinessList()
            {
                string sql = @"SELECT * FROM general.business left join general_address.lgu_brgy_setup_temp on general.business.brgy_id = general_address.lgu_brgy_setup_temp.brgy_id";
                List<BusinessModel> res = (List<BusinessModel>)QueryModule.DataSource<BusinessModel>(sql);
                
                ExpandoObject from = Forms.getForm("sanitary-permit");
                List<object> list = new List<object>();
                list.Add(res);
                list.Add(from);
                return list;
            }
            public object GetBusinessData(string ID)
            {
                string sql = @"SELECT * FROM general.business
                left join general.business_entity on general.business.entity_id = general.business_entity.entity_id  
                left join general_address.lgu_brgy_setup_temp on general.business.brgy_id = general_address.lgu_brgy_setup_temp.brgy_id
                where general.business.business_id = @bID
                ";
                var res = (BusinessModel)QueryModule.DataObject<BusinessModel>(sql, new { bID = ID});

               /* ExpandoObject from = Forms.getForm("sanitary-permit");
                List<object> list = new List<object>();
                list.Add(res);
                list.Add(from);*/
                return res;
            }

            public List<object> CheckTrans(string ID)
            {
                string sql = @"SELECT * FROM health.sanitary_permit_main where health.sanitary_permit_main where sp_business_id = @tID";
                List<SanitaryPermitModel> res = (List<SanitaryPermitModel>)QueryModule.DataSource<SanitaryPermitModel>(sql, new { tID = ID});

                ExpandoObject from = Forms.getForm("sanitary-permit");
                List<object> list = new List<object>();
                list.Add(res);
                list.Add(from);
                return list;
            }


            public List<object> GetTransactionList()
            {
                throw new NotImplementedException();
            }

            public bool Insert(SPTransactionModel model)
            {
                if (QueryModule.connection.State == System.Data.ConnectionState.Open)
                {
                    QueryModule.connection.Close();
                }
                QueryModule.connection.Open();
                using (var mc = QueryModule.connection.BeginTransaction())
                {
                    string sql = "";

                    try
                    {
                        model.form_trans_no = generateFormNumber("sanitary-permit");
                        model.sp_pk_id = Guid.NewGuid().ToString();
                        model.sp_or_pkid = model.sp_pk_id;
                        model.transaction_log = model.sp_pk_id;
                        model.sp_payment_id = model.sp_pk_id;
                        sql = "insert into health.sanitary_permit_main(" + ObjectSqlMapping.MapInsert<SPTransactionModel>() + ")";
                        int res = (int)QueryModule.Execute<int>(sql, model);

                           /* model.mc_requestor.mc_pk_id = model.mc_pk_id;
                            string rsql = "insert into health.med_cert_req(" + ObjectSqlMapping.MapInsert<TransactionRequestor>() + ")";
                            int rres = (int)QueryModule.Execute<int>(rsql, model.mc_requestor);*/
                        
                        mc.Commit();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(sql);
                        mc.Rollback();
                        throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, ex.Message);
                    }
                }
                QueryModule.connection.Close();
                return true;
            }

            public List<object> GetList()
            {
                string sql = @"SELECT * FROM health.sanitary_permit_main
                            LEFT JOIN general.transaction_status ts on health.sanitary_permit_main.sp_transaction_status = general.ts.status_id
                            LEFT JOIN health.health_payment ON health.sanitary_permit_main.sp_payment_id = health.health_payment.main_pk_id
                            LEFT JOIN humanresource.employees ON health.sanitary_permit_main.sp_inspector = humanresource.employees.employee_id
                            ORDER BY health.sanitary_permit_main.sp_transaction_date DESC";
                List<SanitaryPermitModel> res = (List<SanitaryPermitModel>)QueryModule.DataSource<SanitaryPermitModel>(sql);

                if (res == null) return null;
                foreach (SanitaryPermitModel model in res) {
                    sql = @"SELECT * FROM general.business
                            inner join general.business_entity on general.business.entity_id = general.business_entity.entity_id  
                            left join general_address.lgu_brgy_setup_temp on general.business.brgy_id = general_address.lgu_brgy_setup_temp.brgy_id
                            where general.business.business_id = @bID

                        ";
                    model.sp_business_data = (BusinessModel)QueryModule.DataObject<BusinessModel>(sql, new { bID = model.sp_business_id });

                    string paySql = @"
                                     SELECT * FROM health.health_payment where health.health_payment.main_pk_id = @pID
                                    ";
                    model.sp_payment_data = (HealthPayment)QueryModule.DataObject<HealthPayment>(paySql, new { pID = model.sp_pk_id });
                }


                ExpandoObject from = Forms.getForm("sanitary-permit");
                List<object> list = new List<object>();
                list.Add(res);
                list.Add(from);
                return list;

            }

            public List<object> GetInspectorList()
            {
                string sql = @"SELECT * FROM humanresource.position 
                    LEFT JOIN humanresource.person_position on humanresource.position.position_id = humanresource.person_position.position_id
                    LEFT JOIN humanresource.employees on humanresource.person_position.person_id = humanresource.employees.person_guid
                    WHERE humanresource.position.position_name = 'Sanitary Inspector'                          
                ";
                List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);
                return obj;
            }
            public object GetInspectorData(string ID)
            {
                string sql = @"SELECT SysPK_Empl,employee_id,employee_name FROM humanresource.employees WHERE humanresource.employees.employee_id = @id";
                List<object> obj = (List<object>)QueryModule.DataSource<object>(sql, new{ id = ID});
                return obj;
            }
            public object GetTransData(string ID)
            {
                string sql = @"SELECT * FROM health.sanitary_permit_main
                            LEFT JOIN general.transaction_status ts on health.sanitary_permit_main.sp_transaction_status = general.ts.status_id
                            LEFT JOIN health.health_payment ON health.sanitary_permit_main.sp_payment_id = health.health_payment.main_pk_id
                            LEFT JOIN humanresource.employees ON health.sanitary_permit_main.sp_inspector = humanresource.employees.employee_id
                            where health.sanitary_permit_main.sp_pk_id = @id
                            ";
                var res = (SanitaryPermitModel)QueryModule.DataObject<SanitaryPermitModel>(sql, new { id = ID });
                if (res == null) return null;
                
                    sql = @"SELECT * FROM general.business
                           
                            left join general_address.lgu_brgy_setup_temp on general.business.brgy_id = general_address.lgu_brgy_setup_temp.brgy_id
                            where general.business.business_id = @bID
                        ";
                    res.sp_business_data = (BusinessModel)QueryModule.DataObject<BusinessModel>(sql, new { bID = res.sp_business_id });
               

                ExpandoObject from = Forms.getForm("sanitary-permit");
                List<object> list = new List<object>();
                list.Add(res);
                list.Add(from);
                return list;
            }

            public bool UpdateTransaction(string id, SPTransactionModel model)
            {
                if (QueryModule.connection.State == System.Data.ConnectionState.Open)
                {
                    QueryModule.connection.Close();
                }
                QueryModule.connection.Open();
                using (var mc = QueryModule.connection.BeginTransaction())
                {
                    string sql = "";

                    try
                    {
                        sql = "delete from health.sanitary_permit_main where sp_pk_id = '" + id + "'";
                        QueryModule.Execute<int>(sql);

                        model.form_trans_no = model.form_trans_no;
                        model.sp_pk_id = Guid.NewGuid().ToString();
                        model.sp_or_pkid = model.sp_pk_id;
                        model.transaction_log = model.sp_pk_id;
                        model.sp_payment_id = model.sp_pk_id;
                        string tsql = "insert into health.sanitary_permit_main(" + ObjectSqlMapping.MapInsert<SPTransactionModel>() + ")";
                        int res = (int)QueryModule.Execute<int>(tsql, model);

                        mc.Commit();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(sql);
                        mc.Rollback();
                        throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, ex.Message);
                    }
                }
                QueryModule.connection.Close();
                return true;
            }

            public bool PayedTrans(string id)
            {
                string sql = "UPDATE health.sanitary_permit_main SET sp_transaction_status = '1' where health.sanitary_permit_main.sp_pk_id= @transid";
                if (InsertUpdate.ExecuteSql(sql, new { transid = id }))
                    return true;
                return false;
            }

            public bool DeleteTrans(string id)
            {
                string sql = "UPDATE health.sanitary_permit_main SET sp_transaction_status = '3' where health.sanitary_permit_main.sp_pk_id= @transid";
                if (InsertUpdate.ExecuteSql(sql, new { transid = id }))
                    return true;
                return false;
            }

            public bool RevertTrans(string id)
            {
                string sql = "UPDATE health.sanitary_permit_main SET sp_transaction_status = '0' where health.sanitary_permit_main.sp_pk_id= @transid";
                if (InsertUpdate.ExecuteSql(sql, new { transid = id }))
                    return true;
                return false;
            }



            //END
        }
        
    }
}
