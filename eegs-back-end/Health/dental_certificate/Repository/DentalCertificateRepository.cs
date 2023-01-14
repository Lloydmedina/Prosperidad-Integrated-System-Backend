using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using eegs_back_end.Health.dental_certificate.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Dynamic;

namespace eegs_back_end.Health.dental_certificate.Repository
{
    public interface IDentalCertificateRepository : IGlobalInterface
    {
        List<object> GetList();
        List<object> GetByID(string id);
        bool Insert(NewDentalCertificate model);
        List<object> GetExamDatas(string dtl_id);
        List<object> GetDatas(string ID);
        bool UpdateTransaction(string id, List<DentalCertificateExam> model);
        List<object> GetDataById(string dtl_id);
        bool UpdateDtlById(string id, DentalCertificateExam model);

        bool PayedTrans(string id);
        bool DeleteTrans(string id);
        bool RevertTrans(string id);
    }
    public class DentalCertificateRepository : FormNumberGenerator, IDentalCertificateRepository
    {
        public List<object> GetByID(string ID)
        {
            string sql = @"SELECT * FROM health.dental_cert_main
            LEFT JOIN general.transaction_status ts on health.dental_cert_main.dc_transaction_status = general.ts.status_id
            LEFT JOIN health.health_payment ON health.dental_cert_main.dc_payment_id = health.health_payment.main_pk_id
            WHERE health.dental_cert_main.dc_person_id = @id
            ORDER BY health.dental_cert_main.dc_transaction_date DESC";
            List<DentalCertificate> res = (List<DentalCertificate>)QueryModule.DataSource<DentalCertificate>(sql, new { id = ID });
            if (res == null) return null;
            foreach (DentalCertificate model in res)
            {
                sql = @"SELECT * FROM health.dental_cert_dtl
                        INNER JOIN health.lab_exam_setup ON health.dental_cert_dtl.dc_lab_exam_id = health.lab_exam_setup.lab_exam_id
                        WHERE health.dental_cert_dtl.dc_main_id = @dtl_id
                        ";
                model.dc_exams_data = (List<DentalExam>)QueryModule.DataSource<DentalExam>(sql, new { dtl_id = model.dc_pk_id });
                /* string paysql = @"SELECT * FROM health.health_payment WHERE main_pk_id = @payid";
                 model.mc_payment_data = (HealthPayment)QueryModule.DataObject<HealthPayment>(paysql, new { payid = model.mc_pk_id});*/
                string persql = @"SELECT * FROM general.person
	            inner join general.gender on general.person.gender_id = general.gender.gender_id
	            inner join general.civil_status on general.person.civil_status_id = general.civil_status.civil_status_id
	            inner join general_address.lgu_province_setup_temp on general.person.province_id = general_address.lgu_province_setup_temp.province_id
	            inner join general_address.lgu_city_mun_setup_temp on general.person.citmun_id = general_address.lgu_city_mun_setup_temp.city_mun_id
	            inner join general_address.lgu_brgy_setup_temp on general.person.barangay_id = general_address.lgu_brgy_setup_temp.brgy_id where general.person.person_guid = @pid";
                model.dc_person_data = (PersonModel)QueryModule.DataObject<PersonModel>(persql, new { pid = model.dc_person_id });
            }
            ExpandoObject from = Forms.getForm("dental-certificate");
            List<object> list = new List<object>();
            list.Add(res);
            list.Add(from);
            return list;
        }

        public List<object> GetList()
        {
            string sql = @"SELECT * FROM health.dental_cert_main
            LEFT JOIN general.transaction_status ts on health.dental_cert_main.dc_transaction_status = general.ts.status_id
            LEFT JOIN health.health_payment ON health.dental_cert_main.dc_payment_id = health.health_payment.main_pk_id
            ORDER BY health.dental_cert_main.dc_transaction_date DESC";
            List<DentalCertificate> res = (List<DentalCertificate>)QueryModule.DataSource<DentalCertificate>(sql);
            if (res == null) return null;
            foreach (DentalCertificate model in res)
            {
                sql = @"SELECT * FROM health.dental_cert_dtl
                        INNER JOIN health.lab_exam_setup ON health.dental_cert_dtl.dc_lab_exam_id = health.lab_exam_setup.lab_exam_id
                        WHERE health.dental_cert_dtl.dc_main_id = @dtl_id
                        ";
                model.dc_exams_data = (List<DentalExam>)QueryModule.DataSource<DentalExam>(sql, new { dtl_id = model.dc_pk_id });
                 string paysql = @"SELECT * FROM health.health_payment WHERE main_pk_id = @payid";
                 model.dc_payment_data = (HealthPayment)QueryModule.DataObject<HealthPayment>(paysql, new { payid = model.dc_pk_id});
                string persql = @"SELECT * FROM general.person
	            inner join general.gender on general.person.gender_id = general.gender.gender_id
	            inner join general.civil_status on general.person.civil_status_id = general.civil_status.civil_status_id
	            inner join general_address.lgu_province_setup_temp on general.person.province_id = general_address.lgu_province_setup_temp.province_id
	            inner join general_address.lgu_city_mun_setup_temp on general.person.citmun_id = general_address.lgu_city_mun_setup_temp.city_mun_id
	            inner join general_address.lgu_brgy_setup_temp on general.person.barangay_id = general_address.lgu_brgy_setup_temp.brgy_id where general.person.person_guid = @pid";
                model.dc_person_data = (PersonModel)QueryModule.DataObject<PersonModel>(persql, new { pid = model.dc_person_id });

                
            }
            ExpandoObject from = Forms.getForm("dental-certificate");
            List<object> list = new List<object>();
            list.Add(res);
            list.Add(from);
            return list;
        }

        public List<object> GetExamDatas(string dtl_id)
        {
            string sql = @"SELECT * FROM health.dental_cert_dtl 
                            INNER JOIN health.lab_exam_setup ON health.dental_cert_dtl.dc_lab_exam_id = health.lab_exam_setup.lab_exam_id
                            where dc_main_id = @dtl_id";
            List<object> result = (List<object>)QueryModule.DataSource<object>(sql, new { dtl_id = dtl_id });
            return result;
        }

        public bool Insert(NewDentalCertificate model)
        {
            if (QueryModule.connection.State == System.Data.ConnectionState.Open)
            {
                QueryModule.connection.Close();
            }
            QueryModule.connection.Open();
            using (var dc = QueryModule.connection.BeginTransaction())
            {
                string sql = "";

                try
                {
                    model.form_trans_no = generateFormNumber("dental-certificate");
                    model.dc_pk_id = Guid.NewGuid().ToString();
                    model.dc_or_pkid = model.dc_pk_id;
                    model.transaction_log = model.dc_pk_id;
                    model.dc_payment_id = model.dc_pk_id;
                    sql = "insert into health.dental_cert_main(" + ObjectSqlMapping.MapInsert<NewDentalCertificate>() + ")";
                    int res = (int)QueryModule.Execute<int>(sql, model);

                    if (model.dc_exams_data.Count > 0)
                    {
                        foreach (var ed in model.dc_exams_data)
                        {
                            ed.dc_dtl_id = Guid.NewGuid().ToString();
                            ed.dc_main_id = model.dc_pk_id;
                            string esql = "insert into health.dental_cert_dtl(" + ObjectSqlMapping.MapInsert<DentalCertificateExam>() + ")";
                            int eres = (int)QueryModule.Execute<int>(esql, ed);
                        }
                        model.dc_requestor.dc_pk_id = model.dc_pk_id;
                        string rsql = "insert into health.dental_cert_req(" + ObjectSqlMapping.MapInsert<TransactionRequestor>() + ")";
                        int rres = (int)QueryModule.Execute<int>(rsql, model.dc_requestor);
                    }
                    dc.Commit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(sql);
                    dc.Rollback();
                    throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, ex.Message);
                }
            }
            QueryModule.connection.Close();
            return true;
        }

        public List<object> GetDatas(string ID)
        {
            string sql = @"SELECT * FROM health.dental_cert_main
            WHERE health.dental_cert_main.dc_pk_id = @id";
            List<DentalCertificate> res = (List<DentalCertificate>)QueryModule.DataSource<DentalCertificate>(sql, new { id = ID });
            if (res == null) return null;
            foreach (DentalCertificate model in res)
            {
                sql = @"SELECT * FROM health.dental_cert_dtl
                        left JOIN health.lab_exam_setup ON health.dental_cert_dtl.dc_lab_exam_id = health.lab_exam_setup.lab_exam_id 
                        WHERE health.dental_cert_dtl.dc_main_id = @dtl_id
                        ";
                model.dc_exams_data = (List<DentalExam>)QueryModule.DataSource<DentalExam>(sql, new { dtl_id = model.dc_pk_id });
                /* string paysql = @"SELECT * FROM health.health_payment WHERE main_pk_id = @payid";
                 model.mc_payment_data = (HealthPayment)QueryModule.DataObject<HealthPayment>(paysql, new { payid = model.mc_pk_id});*/
                string persql = @"SELECT * FROM general.person
	            inner join general.gender on general.person.gender_id = general.gender.gender_id
	            inner join general.civil_status on general.person.civil_status_id = general.civil_status.civil_status_id
	            inner join general_address.lgu_province_setup_temp on general.person.province_id = general_address.lgu_province_setup_temp.province_id
	            inner join general_address.lgu_city_mun_setup_temp on general.person.citmun_id = general_address.lgu_city_mun_setup_temp.city_mun_id
	            inner join general_address.lgu_brgy_setup_temp on general.person.barangay_id = general_address.lgu_brgy_setup_temp.brgy_id where general.person.person_guid = @pid";
                model.dc_person_data = (PersonModel)QueryModule.DataObject<PersonModel>(persql, new { pid = model.dc_person_id });
            }
            ExpandoObject from = Forms.getForm("dental-certificate");
            List<object> list = new List<object>();
            list.Add(res);
            list.Add(from);
            return list;
        }

        public bool UpdateTransaction(string id, List<DentalCertificateExam> model)
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
                    sql = "delete from health.dental_cert_dtl where dc_main_id = '" + id + "'";
                    QueryModule.Execute<int>(sql);


                    if (model.Count > 0)
                    {
                        foreach (var et in model)
                        {
                            et.dc_dtl_id = Guid.NewGuid().ToString();
                            et.dc_main_id = id;
                            string tsql = "insert into health.dental_cert_dtl(" + ObjectSqlMapping.MapInsert<DentalCertificateExam>() + ")";
                            int tres = (int)QueryModule.Execute<int>(tsql, et);
                        }
                    }



                    hc.Commit();
                }
                catch (Exception ex)
                {
                    hc.Rollback();
                    return false;
                }
            }

            return true;
        }

        public List<object> GetDataById(string dtl_id)
        {
            string sql = @"SELECT * FROM health.dental_cert_dtl 
                            where dc_dtl_id = @dtl_id";
            List<object> result = (List<object>)QueryModule.DataSource<object>(sql, new { dtl_id = dtl_id });
            return result;
        }

        public bool UpdateDtlById(string id, DentalCertificateExam model)
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
                    sql = "delete from health.dental_cert_dtl where dc_dtl_id = '" + id + "'";
                    QueryModule.Execute<int>(sql);


                    model.dc_dtl_id = Guid.NewGuid().ToString();
                    model.dc_main_id = model.dc_main_id;
                    string tsql = "insert into health.dental_cert_dtl(" + ObjectSqlMapping.MapInsert<DentalCertificateExam>() + ")";
                    int tres = (int)QueryModule.Execute<int>(tsql, model);

                    hc.Commit();
                }
                catch (Exception ex)
                {
                    hc.Rollback();
                    return false;
                }
            }

            return true;
        }


        public bool PayedTrans(string id)
        {
            string sql = "UPDATE health.dental_cert_main SET dc_transaction_status = '1' where health.dental_cert_main.dc_pk_id= @transid";
            if (InsertUpdate.ExecuteSql(sql, new { transid = id }))
                return true;
            return false;
        }

        public bool DeleteTrans(string id)
        {
            string sql = "UPDATE health.dental_cert_main SET dc_transaction_status = '3' where health.dental_cert_main.dc_pk_id= @transid";
            if (InsertUpdate.ExecuteSql(sql, new { transid = id }))
                return true;
            return false;
        }

        public bool RevertTrans(string id)
        {
            string sql = "UPDATE health.dental_cert_main SET dc_transaction_status = '0' where health.dental_cert_main.dc_pk_id= @transid";
            if (InsertUpdate.ExecuteSql(sql, new { transid = id }))
                return true;
            return false;

        }
        }
    }
