using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using eegs_back_end.Health.exhumation_permit.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Dynamic;


namespace eegs_back_end.Health.exhumation_permit.Repository
{
    public interface IExhumationPermitRepository : IGlobalInterface
    {
        List<object> GetList();
        List<object> GetPersonData(string ID);
        bool Insert(NewExPermitModel model);
        List<object> GetListById(string ID);
        bool UpdateTransaction(string id, NewExPermitModel model);
        bool PayedTrans(string id);
        bool DeleteTrans(string id);
        bool RevertTrans(string id);
    }
    public class ExhumationPermitRepository : FormNumberGenerator, IExhumationPermitRepository
    {
        public List<object> GetList()
        {
            string sql = @"SELECT * FROM health.exhumation_permit_main
                            LEFT JOIN general.transaction_status ts on health.exhumation_permit_main.exp_transaction_status = general.ts.status_id
                            LEFT JOIN health.health_payment ON health.exhumation_permit_main.exp_payment_id = health.health_payment.main_pk_id
                            ORDER BY health.exhumation_permit_main.exp_transaction_date DESC
                            ";
            List<ExPermitModel> res = (List<ExPermitModel>)QueryModule.DataSource<ExPermitModel>(sql);
            if (res == null) return null;
            foreach (ExPermitModel model in res)
            {
                string persql = @"SELECT * FROM general.person
	            inner join general.gender on general.person.gender_id = general.gender.gender_id
	            inner join general.civil_status on general.person.civil_status_id = general.civil_status.civil_status_id
	            inner join general_address.lgu_province_setup_temp on general.person.province_id = general_address.lgu_province_setup_temp.province_id
	            inner join general_address.lgu_city_mun_setup_temp on general.person.citmun_id = general_address.lgu_city_mun_setup_temp.city_mun_id
	            inner join general_address.lgu_brgy_setup_temp on general.person.barangay_id = general_address.lgu_brgy_setup_temp.brgy_id where general.person.person_guid = @pid";
                model.exp_person_data = (PersonModel)QueryModule.DataObject<PersonModel>(persql, new { pid = model.exp_person_id });
            
                 string paysql = @"SELECT * FROM health.health_payment where main_pk_id = @req";
                model.exp_payment_data = (HealthPayment)QueryModule.DataObject<HealthPayment>(paysql, new { req = model.main_pk_id });
            }
            ExpandoObject from = Forms.getForm("exhumation-permit");
            List<object> list = new List<object>();
            list.Add(res);
            list.Add(from);
            return list;
        }

        public List<object> GetListById(string ID)
        {
            string sql = @"SELECT * FROM health.exhumation_permit_main
                            LEFT JOIN general.transaction_status ts on health.exhumation_permit_main.exp_transaction_status = general.ts.status_id
                            LEFT JOIN health.health_payment ON health.exhumation_permit_main.exp_payment_id = health.health_payment.main_pk_id
                            where health.exhumation_permit_main.exp_pk_id = @exid
                            ";
            List<ExPermitModel> res = (List<ExPermitModel>)QueryModule.DataSource<ExPermitModel>(sql, new { exid = ID});
            ExpandoObject from = Forms.getForm("exhumation-permit");
            List<object> list = new List<object>();
            list.Add(res);
            list.Add(from);
            return list;
        }

        public List<object> GetPersonData(string ID)
        {
            
            string sql = @"select * from general.person
inner join general.gender on general.person.gender_id = general.gender.gender_id
	            inner join general.civil_status on general.person.civil_status_id = general.civil_status.civil_status_id
	            inner join general_address.lgu_province_setup_temp on general.person.province_id = general_address.lgu_province_setup_temp.province_id
	            inner join general_address.lgu_city_mun_setup_temp on general.person.citmun_id = general_address.lgu_city_mun_setup_temp.city_mun_id
	            inner join general_address.lgu_brgy_setup_temp on general.person.barangay_id = general_address.lgu_brgy_setup_temp.brgy_id
                where general.person.person_guid = @person_guid";
            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql, new { person_guid = ID });

            return obj;
        }

        public bool Insert(NewExPermitModel model)
        {
            if (QueryModule.connection.State == System.Data.ConnectionState.Open)
            {
                QueryModule.connection.Clone();
            }
            QueryModule.connection.Open();
            using (var exp = QueryModule.connection.BeginTransaction())
            {
                string sql = "";
                try
                {
                    model.form_trans_no = generateFormNumber("exhumation-permit");
                    model.exp_pk_id = Guid.NewGuid().ToString();
                    model.exp_or_pkid = model.exp_pk_id;
                    model.transaction_log = model.exp_pk_id;
                    model.exp_payment_id = model.exp_pk_id;
                    sql = "insert into health.exhumation_permit_main (" + ObjectSqlMapping.MapInsert<NewExPermitModel>() + ")";
                    int res = (int)QueryModule.Execute<int>(sql, model);

                    exp.Commit();
                }
                catch (Exception ex) 
                {
                    Console.WriteLine(sql);
                    exp.Rollback();
                    throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, ex.Message);
                }
            }
           QueryModule.connection.Close();
            return true;
        }

        public bool UpdateTransaction(string id, NewExPermitModel model)
        {
            if (QueryModule.connection.State == System.Data.ConnectionState.Open)
            {
                QueryModule.connection.Clone();
            }
            QueryModule.connection.Open();
            using (var exp = QueryModule.connection.BeginTransaction())
            {
                string sql = "";
                try
                {
                    sql = "delete from health.exhumation_permit_main where exp_pk_id = '" + id + "'";
                    QueryModule.Execute<int>(sql);

                    model.form_trans_no = model.form_trans_no;
                    model.exp_pk_id = id;
                    model.exp_or_pkid = id;
                    model.transaction_log = id;
                    model.exp_payment_id = id;
                    sql = "insert into health.exhumation_permit_main (" + ObjectSqlMapping.MapInsert<NewExPermitModel>() + ")";
                    int res = (int)QueryModule.Execute<int>(sql, model);

                    exp.Commit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(sql);
                    exp.Rollback();
                    throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, ex.Message);
                }
            }
            QueryModule.connection.Close();
            return true;
        }


        public bool PayedTrans(string id)
        {
            string sql = "UPDATE health.exhumation_permit_main SET exp_transaction_status = '1' where health.exhumation_permit_main.exp_pk_id= @transid";
            if (InsertUpdate.ExecuteSql(sql, new { transid = id }))
                return true;
            return false;
        }

        public bool DeleteTrans(string id)
        {
            string sql = "UPDATE health.exhumation_permit_main SET exp_transaction_status = '3' where health.exhumation_permit_main.exp_pk_id= @transid";
            if (InsertUpdate.ExecuteSql(sql, new { transid = id }))
                return true;
            return false;
        }

        public bool RevertTrans(string id)
        {
            string sql = "UPDATE health.exhumation_permit_main SET exp_transaction_status = '0' where health.exhumation_permit_main.exp_pk_id= @transid";
            if (InsertUpdate.ExecuteSql(sql, new { transid = id }))
                return true;
            return false;
        }
    }
}
