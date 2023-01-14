using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using eegs_back_end.Health.health_card_business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Dynamic;

namespace eegs_back_end.Health.health_card_business.Repository
{
    public interface IHealthCardBusinessRepository : IGlobalInterface
    {
        List<object> GetBusinessData(string ID);
        bool Insert(NewTransactionModel model);
        List<object> GetList();
        List <object> CheckBusinessInfo(string gUID);
    }
    public class HealthCardBusinessRepository : FormNumberGenerator, IHealthCardBusinessRepository
    {
        public List<object> GetBusinessData(string ID)
        {
            string sql = @"SELECT * FROM general.business
                inner join general.business_entity on general.business.entity_id = general.business_entity.entity_id  
                left join general_address.lgu_brgy_setup_temp on general.business.brgy_id = general_address.lgu_brgy_setup_temp.brgy_id
                where general.business.business_id = @bID
                ";
            List<object> res = (List<object>)QueryModule.DataSource<object>(sql, new { bID = ID });

       
            return res;
        }

        public List<object> GetList()
        {
            string sql = @"SELECT * FROM health.health_card_main hm
                            LEFT JOIN general.transaction_status ts on health.hm.transaction_status = general.ts.status_id
                            LEFT JOIN health.health_card_req ON health.hm.pk_id = health.health_card_req.hc_pk_id
                            LEFT JOIN health.health_payment ON health.hm.pk_id = health.health_payment.main_pk_id
                            where transaction_type = 'Healthcard-Business'
                            ORDER BY health.hm.transaction_date DESC
						 ";
            List<HealthCard> res = (List<HealthCard>)QueryModule.DataSource<HealthCard>(sql);
            if (res == null) return null;
            foreach (HealthCard model in res)
            {
                sql = @"SELECT * FROM health.health_card_dtl
                                INNER JOIN health.lab_exam_setup ON health.health_card_dtl.lab_exam_id = health.lab_exam_setup.lab_exam_id
                                WHERE health.health_card_dtl.main_id = @guid";
                model.hc_form_trans_data_arr = (List<NewMedicalTransactionModel>)QueryModule.DataSource<NewMedicalTransactionModel>(sql, new { guid = model.pk_id });

                string bsnsql = @"SELECT * FROM general.business
                            
                            left join general_address.lgu_brgy_setup_temp on general.business.brgy_id = general_address.lgu_brgy_setup_temp.brgy_id
                            where general.business.business_id = @bID
                ";
                // inner join general.business_entity on general.business.entity_id = general.business_entity.entity_id  
                model.businessModels = (BusinessModel)QueryModule.DataObject<BusinessModel>(bsnsql, new { bID = model.business_id });

                string persql = @"SELECT * FROM general.person
	            inner join general.gender on general.person.gender_id = general.gender.gender_id
	            inner join general.civil_status on general.person.civil_status_id = general.civil_status.civil_status_id
	            inner join general_address.lgu_province_setup_temp on general.person.province_id = general_address.lgu_province_setup_temp.province_id
	            inner join general_address.lgu_city_mun_setup_temp on general.person.citmun_id = general_address.lgu_city_mun_setup_temp.city_mun_id
	            inner join general_address.lgu_brgy_setup_temp on general.person.barangay_id = general_address.lgu_brgy_setup_temp.brgy_id where general.person.person_guid = @pid";
                model.personModel = (PersonModel)QueryModule.DataObject<PersonModel>(persql, new { pid = model.person_id });
            }
            ExpandoObject form = Forms.getForm("health-card-individual");
            List<object> list = new List<object>();
            list.Add(res);
            list.Add(form);
            return list;
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


                    if (model.hc_form_trans_data_arr.Count > 0)
                    {
                        foreach (var et in model.hc_form_trans_data_arr)
                        {
                            et.dtl_id = Guid.NewGuid().ToString();
                            et.main_id = model.pk_id;
                            string tsql = "insert into health.health_card_dtl(" + ObjectSqlMapping.MapInsert<NewMedicalTransactionModel>() + ")";
                            int tres = (int)QueryModule.Execute<int>(tsql, et);
                        }
                        model.hc_requestor.hc_pk_id = model.pk_id;
                        string rsql = "insert into health.health_card_req(" + ObjectSqlMapping.MapInsert<MedicalTransactionRequestor>() + ")";
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


        public List<object> CheckBusinessInfo(string gUID)
        {
            string sql = @"SELECT * FROM health.health_card_main hm	
                        inner JOIN general.transaction_status ts on health.hm.transaction_status = general.ts.status_id
						inner JOIN health.health_card_req ON health.hm.pk_id= health.health_card_req.hc_pk_id
                        LEFT JOIN health.health_payment ON health.hm.pk_id = health.health_payment.main_pk_id
                        WHERE hm.business_id = @guid ORDER BY health.hm.pk_id DESC";
            List<HealthCard> res = (List<HealthCard>)QueryModule.DataSource<HealthCard>(sql, new { guid = gUID });
            if (res == null) return null;
            foreach (HealthCard model in res)
            {
                model.hc_form_trans_data_arr = new List<NewMedicalTransactionModel>();
                sql = @"SELECT * FROM health.health_card_dtl
                                INNER JOIN health.lab_exam_setup ON health.health_card_dtl.lab_exam_id = health.lab_exam_setup.lab_exam_id
                                WHERE health.health_card_dtl.main_id = @guid";
                model.hc_form_trans_data_arr = (List<NewMedicalTransactionModel>)QueryModule.DataSource<NewMedicalTransactionModel>(sql, new { guid = model.pk_id });
            }
           
            List<object> list = new List<object>();
            list.Add(res);
    
            return list;
        }




    }
}
