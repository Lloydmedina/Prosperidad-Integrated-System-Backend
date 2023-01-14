using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using eegs_back_end.Health.water_potability.Model;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Dynamic;
namespace eegs_back_end.Health.water_potability.Repository
{
    public interface IWaterPotabilityRepository : IGlobalInterface
    {
        object GetBusinessData(string iD);
        bool Insert(NewTransactionModel model);
        List<object> Getlist();
        List<object> GetListById(string iD);
        bool UpdateTransaction(string id, NewTransactionModel model);
        bool PayedTrans(string id);
        bool DeleteTrans(string id);
        bool RevertTrans(string id);
    }
    public class WaterPotabilityRepository : FormNumberGenerator, IWaterPotabilityRepository
    {
        public object GetBusinessData(string iD)
        {
            string sql = @"SELECT * FROM general.business
                inner join general.business_entity on general.business.entity_id = general.business_entity.entity_id  
                left join general_address.lgu_brgy_setup_temp on general.business.brgy_id = general_address.lgu_brgy_setup_temp.brgy_id
                where general.business.business_id = @bID
                ";
            object res = (object)QueryModule.DataSource<object>(sql, new { bID = iD });


            return res;
        }

        public List<object> Getlist()
        {
            string sql = @"SELECT * FROM health.water_potability_main
                            LEFT JOIN general.transaction_status ts on health.water_potability_main.wp_transaction_status = general.ts.status_id
                            LEFT JOIN health.health_payment ON health.water_potability_main.wp_pk_id = health.health_payment.main_pk_id
                            LEFT JOIN humanresource.employees ON health.water_potability_main.wp_inspector_id = humanresource.employees.employee_id
                            ORDER BY health.water_potability_main.wp_transaction_date DESC
						 ";
            List<WaterPotabilityModel> res = (List<WaterPotabilityModel>)QueryModule.DataSource<WaterPotabilityModel>(sql);
            if (res == null) return null;
            foreach (WaterPotabilityModel model in res)
            {
                string bsnsql = @"SELECT * FROM general.business
                inner join general.business_entity on general.business.entity_id = general.business_entity.entity_id  
                left join general_address.lgu_brgy_setup_temp on general.business.brgy_id = general_address.lgu_brgy_setup_temp.brgy_id
                where general.business.business_id = @bID";
                model.wp_business_data = (BusinessModel)QueryModule.DataObject<BusinessModel>(bsnsql, new { bID = model.wp_business_id });

                string persql = @"SELECT * FROM general.person
	            inner join general.gender on general.person.gender_id = general.gender.gender_id
	            inner join general.civil_status on general.person.civil_status_id = general.civil_status.civil_status_id
	            inner join general_address.lgu_province_setup_temp on general.person.province_id = general_address.lgu_province_setup_temp.province_id
	            inner join general_address.lgu_city_mun_setup_temp on general.person.citmun_id = general_address.lgu_city_mun_setup_temp.city_mun_id
	            inner join general_address.lgu_brgy_setup_temp on general.person.barangay_id = general_address.lgu_brgy_setup_temp.brgy_id where general.person.person_guid = @pid";
                model.wp_person_data = (PersonModel)QueryModule.DataObject<PersonModel>(persql, new { pid = model.wp_person_id });

                 string paysql = @"SELECT * FROM health.health_payment where main_pk_id = @req";
                model.wp_payment_data = (HealthPayment)QueryModule.DataObject<HealthPayment>(paysql, new { req = model.main_pk_id });
            }
            ExpandoObject form = Forms.getForm("water-potability");
            List<object> list = new List<object>();
            list.Add(res);
            list.Add(form);
            return list;
        }

        public List<object> GetListById(string iD)
        {
            string sql = @"SELECT * FROM health.water_potability_main
                            LEFT JOIN general.transaction_status ts on health.water_potability_main.wp_transaction_status = general.ts.status_id
                            LEFT JOIN health.health_payment ON health.water_potability_main.wp_payment_id = health.health_payment.main_pk_id
                            LEFT JOIN humanresource.employees ON health.water_potability_main.wp_inspector_id = humanresource.employees.employee_id
                            where health.water_potability_main.wp_pk_id = @exid
                            ";
            WaterPotabilityModel res = (WaterPotabilityModel)QueryModule.DataObject<WaterPotabilityModel>(sql, new { exid = iD });
           
                string bsnsql = @"SELECT * FROM general.business
                inner join general.business_entity on general.business.entity_id = general.business_entity.entity_id  
                left join general_address.lgu_brgy_setup_temp on general.business.brgy_id = general_address.lgu_brgy_setup_temp.brgy_id
                where general.business.business_id = @bID";
                res.wp_business_data = (BusinessModel)QueryModule.DataObject<BusinessModel>(bsnsql, new { bID = res.wp_business_id });

                string persql = @"SELECT * FROM general.person
	            inner join general.gender on general.person.gender_id = general.gender.gender_id
	            inner join general.civil_status on general.person.civil_status_id = general.civil_status.civil_status_id
	            inner join general_address.lgu_province_setup_temp on general.person.province_id = general_address.lgu_province_setup_temp.province_id
	            inner join general_address.lgu_city_mun_setup_temp on general.person.citmun_id = general_address.lgu_city_mun_setup_temp.city_mun_id
	            inner join general_address.lgu_brgy_setup_temp on general.person.barangay_id = general_address.lgu_brgy_setup_temp.brgy_id where general.person.person_guid = @pid";
                res.wp_person_data = (PersonModel)QueryModule.DataObject<PersonModel>(persql, new { pid = res.wp_person_id });
            
           
            ExpandoObject from = Forms.getForm("water-potability");
            List<object> list = new List<object>();
            list.Add(res);
            list.Add(from);
            return list;
        }

        public bool Insert(NewTransactionModel model)
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
                    model.form_trans_no = generateFormNumber("water-potability");
                    model.wp_pk_id = Guid.NewGuid().ToString();
                    model.wp_or_pkid = model.wp_pk_id;
                    model.transaction_log = model.wp_pk_id;
                    model.wp_payment_id = model.wp_pk_id;
                    sql = "insert into health.water_potability_main(" + ObjectSqlMapping.MapInsert<NewTransactionModel>() + ")";
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

        public bool UpdateTransaction(string id, NewTransactionModel model)
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
                    sql = "delete from health.water_potability_main where wp_pk_id = '" + id + "'";
                    QueryModule.Execute<int>(sql);

                    model.form_trans_no = model.form_trans_no;
                    model.wp_pk_id = id;
                    model.wp_or_pkid = id;
                    model.transaction_log = id;
                    model.wp_payment_id = id;
                    sql = "insert into health.water_potability_main(" + ObjectSqlMapping.MapInsert<NewTransactionModel>() + ")";
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
            string sql = "UPDATE health.water_potability_main SET wp_transaction_status = '1' where health.water_potability_main.wp_pk_id= @transid";
            if (InsertUpdate.ExecuteSql(sql, new { transid = id }))
                return true;
            return false;
        }

        public bool DeleteTrans(string id)
        {
            string sql = "UPDATE health.water_potability_main SET wp_transaction_status = '3' where health.water_potability_main.wp_pk_id= @transid";
            if (InsertUpdate.ExecuteSql(sql, new { transid = id }))
                return true;
            return false;
        }

        public bool RevertTrans(string id)
        {
            string sql = "UPDATE health.water_potability_main SET wp_transaction_status = '0' where health.water_potability_main.wp_pk_id= @transid";
            if (InsertUpdate.ExecuteSql(sql, new { transid = id }))
                return true;
            return false;
        }

        //END
    }
}
