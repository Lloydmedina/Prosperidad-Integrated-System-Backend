using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using eegs_back_end.Health.health_card.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Dynamic;

namespace eegs_back_end.Health.health_card.Repository
{
    public interface IHealthCardRepository : IGlobalInterface
    {
       
        List<object> GetList();
        bool Insert(NewTransactionModel model);
        bool Insert(NewMedicalTransactionModel model);
        bool Insert(HealthCardPayment model);
        bool Edit(string GUID, HealthCardModel model);
        bool Delete(string GUID);
        List<object> GetProject();
        List<object> GetPersonData(string GUID);
        List<object> GetTransactionsInfo(string guid);
        List<object> CheckTransactionsInfo(string guid);
        List<object> GetMedicalExamData(string code);
        HealthCard GetByID(string id);
        List<object> GetMedical_Trans_ExamData(string gUID, string med_id);
        object Generate_Health_Card_id();
        List<object> GetExamData(string dtl_id);
        bool UpdateDetails(string pk_main_id, List<NewMedicalTransactionModel> listDtl);
        List<object> GetTransactionsData(string gUID);
        bool UpdateDtlById(string id, NewMedicalTransactionModel model);
        bool DeleteTrans(string id);
        bool RevertTrans(string id);
        bool PayedTrans(string id);
        List<object> TransactionsHistory(string gUID);
    }
    public class HealthCardRepository : FormNumberGenerator, IHealthCardRepository
    {

      
        public bool Delete(string GUID)
        {
            string sql = "UPDATE general.domain SET status = 'Deleted' where general.domain.domain_guid = @domain_guid";
            if (InsertUpdate.ExecuteSql(sql, new { domain_guid = GUID }))
                return true;
            return false;
        }

        public bool Edit(string GUID, HealthCardModel model)
        {
            model.domain_guid = GUID;

            string sql = @"UPDATE general.domain SET domain_name = @domain_name, description = @description WHERE general.domain.domain_guid = @domain_guid";

            if (InsertUpdate.ExecuteSql(sql, model))
                return true;

            return false;
        }

        public HealthCard GetByID(string ID)
        {
            string sql = "select * from health.health_card_main where health.health_card_main.pk_id = @id";
            HealthCard domain = (HealthCard)QueryModule.DataObject<HealthCard>(sql, new { id = ID });

            return domain;
        }

        public List<object> GetList()
        {
            string sql = @"SELECT * FROM health.health_card_main hm
                            LEFT JOIN general.transaction_status ts on health.hm.transaction_status = general.ts.status_id
                            LEFT JOIN health.health_card_req ON health.hm.pk_id = health.health_card_req.hc_pk_id
                            LEFT JOIN health.health_payment ON health.hm.pk_id = health.health_payment.main_pk_id
                            where transaction_type = 'Healthcard-Individual'
                            ORDER BY health.hm.time_stamp DESC
						 ";
            List<HealthCard> res = (List<HealthCard>)QueryModule.DataSource<HealthCard>(sql);
            if (res == null) return null;
            foreach (HealthCard model in res){
                sql = @"SELECT * FROM health.health_card_dtl
                                INNER JOIN health.lab_exam_setup ON health.health_card_dtl.lab_exam_id = health.lab_exam_setup.lab_exam_id
                                WHERE health.health_card_dtl.main_id = @guid";
            model.hc_form_trans_data_arr = (List<ViewMedicalTransactionModel>)QueryModule.DataSource<ViewMedicalTransactionModel>(sql, new { guid = model.pk_id });

                string persql = @"SELECT * FROM general.person
	            inner join general.gender on general.person.gender_id = general.gender.gender_id
	            inner join general.civil_status on general.person.civil_status_id = general.civil_status.civil_status_id
	            inner join general_address.lgu_province_setup_temp on general.person.province_id = general_address.lgu_province_setup_temp.province_id
	            inner join general_address.lgu_city_mun_setup_temp on general.person.citmun_id = general_address.lgu_city_mun_setup_temp.city_mun_id
	            inner join general_address.lgu_brgy_setup_temp on general.person.barangay_id = general_address.lgu_brgy_setup_temp.brgy_id where general.person.person_guid = @pid";
                model.personModels = (PersonModel)QueryModule.DataObject<PersonModel>(persql, new { pid = model.person_id });

                string paySql = @"SELECT * FROM health.health_payment where health.health_payment.main_pk_id = @pID";
                model.paymentModels = (HealthCardPayment)QueryModule.DataObject<HealthCardPayment>(paySql, new { pID = model.pk_id });
            }
            ExpandoObject form = Forms.getForm("health-card-individual");
            List<object> list = new List<object>();
            list.Add(res);
            list.Add(form);
            return list;
        }
        public List<object> GetTransactionsInfo(string guid) {
            {
                string sql = @"SELECT * FROM health.health_card_dtl
                                INNER JOIN health.lab_exam_setup ON health.health_card_dtl.lab_exam_id = health.lab_exam_setup.lab_exam_id
                                WHERE health.health_card_dtl.main_id = @guid";
                List<object> transaction_result = (List<object>)QueryModule.DataSource<object>(sql, new { guid = guid });
                return transaction_result;
            }
        }
        public List<object> CheckTransactionsInfo(string guid)
        {

             string sql = @"SELECT * FROM health.health_card_main hm	
                        LEFT JOIN general.transaction_status ts on health.hm.transaction_status = general.ts.status_id
						LEFT JOIN health.health_card_req ON health.hm.person_id = health.health_card_req.requestor_id
                        LEFT JOIN health.health_payment ON health.hm.pk_id = health.health_payment.main_pk_id
                        WHERE hm.person_id = @guid ORDER BY health.hm.pk_id DESC";
            List<HealthCard> res = (List<HealthCard>)QueryModule.DataSource<HealthCard>(sql, new { guid = guid });
            if (res == null) return null;
            foreach (HealthCard model in res){
                model.hc_form_trans_data_arr = new List<ViewMedicalTransactionModel>();
                sql = @"SELECT * FROM health.health_card_dtl
                                INNER JOIN health.lab_exam_setup ON health.health_card_dtl.lab_exam_id = health.lab_exam_setup.lab_exam_id
                                WHERE health.health_card_dtl.main_id = @guid";
                model.hc_form_trans_data_arr = (List<ViewMedicalTransactionModel>)QueryModule.DataSource<ViewMedicalTransactionModel>(sql, new { guid = model.pk_id});
            }
            ExpandoObject form = Forms.getForm("health-card-individual");
            List<object> list = new List<object>();
            list.Add(res);
            list.Add(form);
            return list;
            /*{
                string sql = @"SELECT * FROM health.health_card_main hm	
                                LEFT JOIN general.transaction_status ts on health.hm.transaction_status = general.ts.status_id
						        LEFT JOIN health.health_card_req on health.hm.person_id = health.health_card_req.requestor_id
                                WHERE hm.person_id = @guid";
                List<object> check_result = (List<object>)QueryModule.DataSource<object>(sql, new { guid = guid });
                return check_result;
            }*/
        }

        public List<object> GetTransactionsData(string gUID)
        {
            string sql = @"SELECT * FROM health.health_card_main hm	
                        LEFT JOIN general.transaction_status ts on health.hm.transaction_status = general.ts.status_id
						LEFT JOIN health.health_card_req ON health.hm.pk_id = health.health_card_req.hc_pk_id
                        LEFT JOIN health.health_payment ON health.hm.pk_id = health.health_payment.main_pk_id
                        WHERE hm.pk_id = @guid ORDER BY health.hm.pk_id DESC";
            HealthCard res = (HealthCard)QueryModule.DataObject<HealthCard>(sql, new { guid = gUID});
            if (res == null) return null;
            
                res.hc_form_trans_data_arr = new List<ViewMedicalTransactionModel>();
                sql = @"SELECT * FROM health.health_card_dtl
                                INNER JOIN health.lab_exam_setup ON health.health_card_dtl.lab_exam_id = health.lab_exam_setup.lab_exam_id
                                WHERE health.health_card_dtl.main_id = @guid";
                res.hc_form_trans_data_arr = (List<ViewMedicalTransactionModel>)QueryModule.DataSource<ViewMedicalTransactionModel>(sql, new { guid = res.pk_id });

                string persql = @"SELECT * FROM general.person
	            inner join general.gender on general.person.gender_id = general.gender.gender_id
	            inner join general.civil_status on general.person.civil_status_id = general.civil_status.civil_status_id
	            inner join general_address.lgu_province_setup_temp on general.person.province_id = general_address.lgu_province_setup_temp.province_id
	            inner join general_address.lgu_city_mun_setup_temp on general.person.citmun_id = general_address.lgu_city_mun_setup_temp.city_mun_id
	            inner join general_address.lgu_brgy_setup_temp on general.person.barangay_id = general_address.lgu_brgy_setup_temp.brgy_id where general.person.person_guid = @pid";
                res.personModels = (PersonModel)QueryModule.DataObject<PersonModel>(persql, new { pid = res.person_id });
            
            ExpandoObject form = Forms.getForm("health-card-individual");
            List<object> list = new List<object>();
            list.Add(res);
            list.Add(form);
            return list;
        }
        public List<object> GetPersonData(string guid)
        {
            string sql = @"SELECT 
general.person.id,
general.person.first_name,
general.person.middle_name,
general.person.last_name,
general.person.suffix,
general.person.gender_id,
general.gender.gender_name,
general.person.civil_status_id,
general.civil_status.civil_status_name,
general.person.citizenship,
general.person.birth_date,
general.person.place_of_birth,
general.person.region_id,
general.person.province_id,
general_address.lgu_province_setup_temp.province_name,
general.person.citmun_id,
general_address.lgu_city_mun_setup_temp.city_mun_name,
general.person.barangay_id,
general_address.lgu_brgy_setup_temp.brgy_name,
general.person.zipcode_id,
general.person.person_guid,
general.person.age,
general.person.prefix,
general.person.street,
general.person.profession,

            from general.person
            INNER JOIN general.gender ON general.person.gender_id = general.gender.gender_id
            INNER JOIN general.civil_status ON general.person.civil_status_id = general.civil_status.civil_status_id
            INNER JOIN general_address.lgu_province_setup_temp ON general.person.province_id = general_address.lgu_province_setup_temp.province_id
            INNER JOIN general_address.lgu_city_mun_setup_temp ON general.person.citmun_id = general_address.lgu_city_mun_setup_temp.city_mun_id
            INNER JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
            INNER JOIN health.health_card_main ON general.person.person_guid = health.health_card_main.person_id

 where general.person.person_guid = @guid LIMIT 1";
            List<object> result = (List<object>)QueryModule.DataSource<object>(sql, new { guid = guid });
            return result;
        }

        public List<object> GetMedicalExamData(string code)
        {
            string sql = "SELECT * FROM health.lab_exam_setup where health.lab_exam_setup.lab_form_code = @code ";
            List<object> result = (List<object>)QueryModule.DataSource<object>(sql, new { code = code });
            return result;
        }

        public List<object> GetExamData(string dtl_id)
        {
            string sql = "SELECT * FROM health.health_card_dtl where dtl_id = @dtl_id";
            List<object> result = (List<object>)QueryModule.DataSource<object>(sql, new { dtl_id = dtl_id });
            return result;
        }
        public List<object> GetProject()
        {
            string sql = "select general.project_title.title_name, general.project_title.project_title_guid from general.project_title";
            List<object> list = (List<object>)QueryModule.DataSource<object>(sql);
            return list;
        }

        public List<object> GetMedical_Trans_ExamData(string gUID, string med_id)
        {
            string sql = @"SELECT * FROM health.health_card_dtl WHERE health.health_card_dtl.main_id = @gUID
                            AND health.health_card_dtl.lab_exam_id = @med_id ";
            List<object> result = (List<object>)QueryModule.DataSource<object>(sql, new { gUID = gUID, med_id = med_id });
            return result;
        }

        public bool Insert(HealthCardModel model)
        {
            model.domain_guid = Guid.NewGuid().ToString();
            model.status = "Active";
            string sql = @"INSERT INTO general.domain (`domain_name`, `description`, `domain_guid`, `project_title_guid`, `transaction_date`, `status`)" +
                "VALUES (@domain_name, @description, @domain_guid, @project_title_guid, @transaction_date, @status)";

            if (InsertUpdate.ExecuteSql(sql, model))
                return true;

            return false;

        }
       

        public bool Insert(NewTransactionModel model)
        {
            if (QueryModule.connection.State == System.Data.ConnectionState.Open)
            {
                QueryModule.connection.Close();
            }
            QueryModule.connection.Open();
            using (var hc = QueryModule.connection.BeginTransaction())
            {
                string sql = "";
                try
                {
                    model.form_trans_no = generateFormNumber("health-card-individual");
                    model.pk_id = Guid.NewGuid().ToString();
                    sql = "insert into health.health_card_main(" + ObjectSqlMapping.MapInsert<NewTransactionModel>() + ")";
                    int res = (int)QueryModule.Execute<int>(sql, model);
                    

                    if (model.hc_form_trans_data_arr.Count > 0) {
                        foreach (var et in model.hc_form_trans_data_arr) {
                            et.dtl_id = Guid.NewGuid().ToString();
                            et.main_id = model.pk_id;
                            string tsql = "insert into health.health_card_dtl(" + ObjectSqlMapping.MapInsert<NewMedicalTransactionModel>() + ")";
                            int tres = (int)QueryModule.Execute<int>(tsql, et);
                        }
                        model.hc_requestor.hc_pk_id = model.pk_id;
                        string rsql = "insert into health.health_card_req("+ ObjectSqlMapping.MapInsert<MedicalTransactionRequestor>() + ")";
                            int rres = (int)QueryModule.Execute<int>(rsql, model.hc_requestor);
                    }
                    hc.Commit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(sql);
                    hc.Rollback();
                    throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, ex.Message);
                }
            }
            QueryModule.connection.Close();
            return true;
        }

        public bool Insert(NewMedicalTransactionModel model)
        {
            if (QueryModule.connection.State == System.Data.ConnectionState.Open)
                    {
                QueryModule.connection.Close();
                    }
            QueryModule.connection.Open();
            using (var medexam = QueryModule.connection.BeginTransaction() ) {
                try
                {
                    model.dtl_id = Guid.NewGuid().ToString();
                    string sql = "insert into health.health_card_dtl(" + ObjectSqlMapping.MapInsert<NewMedicalTransactionModel>() + ")";
                    int res = (int)QueryModule.Execute<int>(sql, model);
                    medexam.Commit();
                }
                catch (Exception ex) {
                    medexam.Rollback();
                    throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, ex.Message);
                }
            }
            QueryModule.connection.Close();
            return true;
        }

        public bool Insert(HealthCardPayment model)
        {

            if (QueryModule.connection.State == System.Data.ConnectionState.Open)
            {
                QueryModule.connection.Close();
            }
            QueryModule.connection.Open();
            using (var medexam = QueryModule.connection.BeginTransaction())
            {
                try
                {
                    model.payment_id = Guid.NewGuid().ToString();
                    string sql = "insert into health.health_payment(" + ObjectSqlMapping.MapInsert<HealthCardPayment>() + ")";
                    int res = (int)QueryModule.Execute<int>(sql, model);
                    medexam.Commit();
                }
                catch (Exception ex)
                {
                    medexam.Rollback();
                    throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, ex.Message);
                }
            }
            QueryModule.connection.Close();
            return true;
        }

        public object Generate_Health_Card_id()
        {
            string sql = "SELECT COUNT(DISTINCT(dtl_id)) AS NumberOfTrans FROM health.health_card_dtl";
            object res = (object)QueryModule.DataSource<object>(sql);

            //object hc_card_id = "hc-"+res;
            return res;
        }

        public bool UpdateDetails(string pk_main_id, List<NewMedicalTransactionModel> listDtl)
        {
            if (QueryModule.connection.State == System.Data.ConnectionState.Open)
            {
                QueryModule.connection.Close();
            }
            QueryModule.connection.Open();
            using (var hc = QueryModule.connection.BeginTransaction())
            {
                string sql = "";
                try
                {
                    sql = "delete from health.health_card_dtl where main_id = '" + pk_main_id + "'";
                    QueryModule.Execute<int>(sql);


                    if(listDtl.Count > 0)
                    {
                        foreach (var et in listDtl)
                        {
                            et.dtl_id = Guid.NewGuid().ToString();
                            et.main_id = pk_main_id;
                            string tsql = "insert into health.health_card_dtl(" + ObjectSqlMapping.MapInsert<NewMedicalTransactionModel>() + ")";
                            int tres = (int)QueryModule.Execute<int>(tsql, et);
                        }
                    }



                    hc.Commit();
                }
                catch (Exception)
                {
                    hc.Rollback();
                    return false;
                }
            }

            return true;
        }

        public bool UpdateDtlById(string id, NewMedicalTransactionModel model)
        {
            if (QueryModule.connection.State == System.Data.ConnectionState.Open)
            {
                QueryModule.connection.Close();
            }
            QueryModule.connection.Open();
            using (var hc = QueryModule.connection.BeginTransaction())
            {
                string sql = "";
                try
                {
                    sql = "delete from health.health_card_dtl where dtl_id = '" + id + "'";
                    QueryModule.Execute<int>(sql);


                            model.dtl_id = Guid.NewGuid().ToString();
                            model.main_id = model.main_id;
                            string tsql = "insert into health.health_card_dtl(" + ObjectSqlMapping.MapInsert<NewMedicalTransactionModel>() + ")";
                            int tres = (int)QueryModule.Execute<int>(tsql, model);

                    hc.Commit();
                }
                catch (Exception)
                {
                    hc.Rollback();
                    return false;
                }
            }

            return true;
        }

        public bool DeleteTrans(string id)
        {
            string sql = "UPDATE health.health_card_main SET transaction_status = '3' where health.health_card_main.pk_id= @transid";
            if (InsertUpdate.ExecuteSql(sql, new { transid = id }))
                return true;
            return false;
        }

        public bool RevertTrans(string id)
        {
            string sql = "UPDATE health.health_card_main SET transaction_status = '0' where health.health_card_main.pk_id= @transid";
            if (InsertUpdate.ExecuteSql(sql, new { transid = id }))
                return true;
            return false;
        }

        public bool PayedTrans(string id)
        {
            string sql = "UPDATE health.health_card_main SET transaction_status = '1' where health.health_card_main.pk_id= @transid";
            if (InsertUpdate.ExecuteSql(sql, new { transid = id }))
                return true;
            return false;
        }

           public List<object> TransactionsHistory(string gUID)
        {
            string sql = @"SELECT * FROM health.health_card_main hm	
                        LEFT JOIN general.transaction_status ts on health.hm.transaction_status = general.ts.status_id
						LEFT JOIN health.health_card_req ON health.hm.person_id = health.health_card_req.requestor_id
                        LEFT JOIN health.health_payment ON health.hm.pk_id = health.health_payment.main_pk_id
                        WHERE hm.person_id = @guid ORDER BY health.hm.pk_id DESC";
            List<HealthCard> res = (List<HealthCard>)QueryModule.DataSource<HealthCard>(sql, new { guid = gUID });
            if (res == null) return null;
            foreach (HealthCard model in res){
                model.hc_form_trans_data_arr = new List<ViewMedicalTransactionModel>();
                sql = @"SELECT * FROM health.health_card_dtl
                                INNER JOIN health.lab_exam_setup ON health.health_card_dtl.lab_exam_id = health.lab_exam_setup.lab_exam_id
                                WHERE health.health_card_dtl.main_id = @guid";
                model.hc_form_trans_data_arr = (List<ViewMedicalTransactionModel>)QueryModule.DataSource<ViewMedicalTransactionModel>(sql, new { guid = model.pk_id});
            }
            ExpandoObject form = Forms.getForm("health-card-individual");
            List<object> list = new List<object>();
            list.Add(res);
            list.Add(form);
            return list;
        }
    }
}
