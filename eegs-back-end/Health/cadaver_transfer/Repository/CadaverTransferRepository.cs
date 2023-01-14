using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using eegs_back_end.Health.cadaver_transfer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Dynamic;
namespace eegs_back_end.Health.cadaver_transfer.Repository
{
    public interface ICadaverTransferRepository : IGlobalInterface
    {
        List<object> Getlist();
        bool Insert(NewCadaverTransferModel model);
        List<object> GetPersonData(string iD);
        List<object> GetListById(string iD);
        bool UpdateTransaction(string id, NewCadaverTransferModel model);
        bool PayedTrans(string id);
        bool DeleteTrans(string id);
        bool RevertTrans(string id);
    }
        public class CadaverTransferRepository : FormNumberGenerator, ICadaverTransferRepository
        {
            public List<object> Getlist()
            {
            string sql = @"SELECT * FROM health.cadaver_transfer_main
                            LEFT JOIN general.transaction_status ts on health.cadaver_transfer_main.ctp_transaction_status = general.ts.status_id
                            LEFT JOIN health.health_payment ON health.cadaver_transfer_main.ctp_payment_id = health.health_payment.main_pk_id
                            LEFT JOIN humanresource.employees ON health.cadaver_transfer_main.ctp_inspector_id = humanresource.employees.employee_id
                            ORDER BY health.cadaver_transfer_main.ctp_transaction_date DESC
                             ";
            List<CadaverTransfer> res = (List<CadaverTransfer>)QueryModule.DataSource<CadaverTransfer>(sql);
            if (res == null) return null;
            foreach (CadaverTransfer model in res)
            {
                string persql = @"SELECT * FROM general.person
	            inner join general.gender on general.person.gender_id = general.gender.gender_id
	            inner join general.civil_status on general.person.civil_status_id = general.civil_status.civil_status_id
	            inner join general_address.lgu_province_setup_temp on general.person.province_id = general_address.lgu_province_setup_temp.province_id
	            inner join general_address.lgu_city_mun_setup_temp on general.person.citmun_id = general_address.lgu_city_mun_setup_temp.city_mun_id
	            inner join general_address.lgu_brgy_setup_temp on general.person.barangay_id = general_address.lgu_brgy_setup_temp.brgy_id where general.person.person_guid = @pid";
                model.ctp_person_data = (PersonModel)QueryModule.DataObject<PersonModel>(persql, new { pid = model.ctp_person_id });
                
                 string paysql = @"SELECT * FROM health.health_payment where main_pk_id = @req";
                model.ctp_payment_data = (HealthPayment)QueryModule.DataObject<HealthPayment>(paysql, new { req = model.main_pk_id });

            }
            ExpandoObject from = Forms.getForm("cadaver-transfer");
            List<object> list = new List<object>();
            list.Add(res);
            list.Add(from);
            return list;
        }

        public List<object> GetListById(string iD)
        {
            string sql = @"SELECT * FROM health.cadaver_transfer_main
                            LEFT JOIN general.transaction_status ts on health.cadaver_transfer_main.ctp_transaction_status = general.ts.status_id
                            LEFT JOIN health.health_payment ON health.cadaver_transfer_main.ctp_payment_id = health.health_payment.main_pk_id
                                          LEFT JOIN humanresource.employees ON health.cadaver_transfer_main.ctp_inspector_id = humanresource.employees.employee_id
                            where health.cadaver_transfer_main.ctp_pk_id = @exid
                            ";
            CadaverTransfer res = (CadaverTransfer)QueryModule.DataObject<CadaverTransfer>(sql, new { exid = iD });
            ExpandoObject from = Forms.getForm("cadaver-transfer");
            List<object> list = new List<object>();
            list.Add(res);
            list.Add(from);
            return list;
        }

        public List<object> GetPersonData(string iD)
        {
            string sql = @"select * from general.person
inner join general.gender on general.person.gender_id = general.gender.gender_id
	            inner join general.civil_status on general.person.civil_status_id = general.civil_status.civil_status_id
	            inner join general_address.lgu_province_setup_temp on general.person.province_id = general_address.lgu_province_setup_temp.province_id
	            inner join general_address.lgu_city_mun_setup_temp on general.person.citmun_id = general_address.lgu_city_mun_setup_temp.city_mun_id
	            inner join general_address.lgu_brgy_setup_temp on general.person.barangay_id = general_address.lgu_brgy_setup_temp.brgy_id
                where general.person.person_guid = @person_guid";
            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql, new { person_guid = iD });

            return obj;
        }

        public bool Insert(NewCadaverTransferModel model)
        {
            if (QueryModule.connection.State == System.Data.ConnectionState.Open)
            {
                QueryModule.connection.Close();
            }
            QueryModule.connection.Open();
            using (var ctp = QueryModule.connection.BeginTransaction())
            {
                string sql = "";

                try
                {
                    model.form_trans_no = generateFormNumber("cadaver-transfer");
                    model.ctp_pk_id = Guid.NewGuid().ToString();
                    model.ctp_or_pkid = model.ctp_pk_id;
                    model.transaction_log = model.ctp_pk_id;
                    model.ctp_payment_id = model.ctp_pk_id;
                    sql = "insert into health.cadaver_transfer_main(" + ObjectSqlMapping.MapInsert<NewCadaverTransferModel>() + ")";
                    int res = (int)QueryModule.Execute<int>(sql, model);

                    ctp.Commit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(sql);
                    ctp.Rollback();
                    throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, ex.Message);
                }
            }
            QueryModule.connection.Close();
            return true;
        }

        public bool UpdateTransaction(string id, NewCadaverTransferModel model)
        {
            if (QueryModule.connection.State == System.Data.ConnectionState.Open)
            {
                QueryModule.connection.Close();
            }
            QueryModule.connection.Open();
            using (var ctp = QueryModule.connection.BeginTransaction())
            {
                string sql = "";

                try
                {
                    sql = "delete from health.cadaver_transfer_main where ctp_pk_id = '" + id + "'";
                    QueryModule.Execute<int>(sql);

                    model.form_trans_no = model.form_trans_no;
                    model.ctp_pk_id = id;
                    model.ctp_or_pkid = id;
                    model.transaction_log = id;
                    model.ctp_payment_id = id;
                    sql = "insert into health.cadaver_transfer_main(" + ObjectSqlMapping.MapInsert<NewCadaverTransferModel>() + ")";
                    int res = (int)QueryModule.Execute<int>(sql, model);

                    ctp.Commit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(sql);
                    ctp.Rollback();
                    throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, ex.Message);
                }
            }
            QueryModule.connection.Close();
            return true;
        }

        public bool PayedTrans(string id)
        {
            string sql = "UPDATE health.cadaver_transfer_main SET ctp_transaction_status = '1' where health.cadaver_transfer_main.ctp_pk_id= @transid";
            if (InsertUpdate.ExecuteSql(sql, new { transid = id }))
                return true;
            return false;
        }

        public bool DeleteTrans(string id)
        {
            string sql = "UPDATE health.cadaver_transfer_main SET ctp_transaction_status = '3' where health.cadaver_transfer_main.ctp_pk_id= @transid";
            if (InsertUpdate.ExecuteSql(sql, new { transid = id }))
                return true;
            return false;
        }

        public bool RevertTrans(string id)
        {
            string sql = "UPDATE health.cadaver_transfer_main SET ctp_transaction_status = '0' where health.cadaver_transfer_main.ctp_pk_id= @transid";
            if (InsertUpdate.ExecuteSql(sql, new { transid = id }))
                return true;
            return false;
        }




        // END 
    }
    

}
