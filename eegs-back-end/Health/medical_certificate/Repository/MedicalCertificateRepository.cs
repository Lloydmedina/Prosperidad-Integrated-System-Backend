using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using eegs_back_end.Health.medical_certificate.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Dynamic;

namespace eegs_back_end.Health.medical_certificate.Repository
{
    public interface IMedicalCertificateRepository : IGlobalInterface
    {
        List<object> GetList();
        List<object> GetByID(string id);
        bool Insert(NewMedicalCertificate model);
        List<object> GetExamData(string dtl_id);
        List<object> GetDatas(string ID);
        bool UpdateDtlById(string id, MedicalCertificateExam model);
        List<object> GetExamListData(string trans_id);
        bool UpdateDetails(string id, List<MedicalCertificateExam> model);
        bool PayedTrans(string id);
        bool DeleteTrans(string id);
        bool RevertTrans(string id);
    }
    public class MedicalCertificateRepository : FormNumberGenerator, IMedicalCertificateRepository
    {
        public List<object> GetByID(string ID)
        {
            string sql = @"SELECT * FROM health.med_cert_main
            LEFT JOIN general.transaction_status ts on health.med_cert_main.mc_transaction_status = general.ts.status_id
            LEFT JOIN health.health_payment ON health.med_cert_main.mc_payment_id = health.health_payment.main_pk_id
            WHERE health.med_cert_main.mc_person_id = @id
            ORDER BY health.med_cert_main.mc_transaction_date DESC";
            List<MedicalCertificate> res = (List<MedicalCertificate>)QueryModule.DataSource<MedicalCertificate>(sql, new { id = ID});
            if (res == null) return null;
            foreach (MedicalCertificate model in res)
            {
                sql = @"SELECT * FROM health.med_cert_dtl
                        INNER JOIN health.lab_exam_setup ON health.med_cert_dtl.mc_lab_exam_id = health.lab_exam_setup.lab_exam_id
                        WHERE health.med_cert_dtl.mc_main_id = @dtl_id
                        ";
                model.mc_exams_data = (List<MedicalCertificateExam>)QueryModule.DataSource<MedicalCertificateExam>(sql, new { dtl_id = model.mc_pk_id });
                /* string paysql = @"SELECT * FROM health.health_payment WHERE main_pk_id = @payid";
                 model.mc_payment_data = (HealthPayment)QueryModule.DataObject<HealthPayment>(paysql, new { payid = model.mc_pk_id});*/
                string persql = @"SELECT * FROM general.person
	            inner join general.gender on general.person.gender_id = general.gender.gender_id
	            inner join general.civil_status on general.person.civil_status_id = general.civil_status.civil_status_id
	            inner join general_address.lgu_province_setup_temp on general.person.province_id = general_address.lgu_province_setup_temp.province_id
	            inner join general_address.lgu_city_mun_setup_temp on general.person.citmun_id = general_address.lgu_city_mun_setup_temp.city_mun_id
	            inner join general_address.lgu_brgy_setup_temp on general.person.barangay_id = general_address.lgu_brgy_setup_temp.brgy_id where general.person.person_guid = @pid";
                model.mc_person_data = (PersonModel)QueryModule.DataObject<PersonModel>(persql, new { pid = model.mc_person_id });
            }
            ExpandoObject from = Forms.getForm("medical-certificate");
            List<object> list = new List<object>();
            list.Add(res);
            list.Add(from);
            return list;
        }

        public List<object> GetList()
        {
            string sql = @"SELECT * FROM health.med_cert_main
            LEFT JOIN general.transaction_status ts on health.med_cert_main.mc_transaction_status = general.ts.status_id
            LEFT JOIN health.health_payment ON health.med_cert_main.mc_payment_id = health.health_payment.main_pk_id
            ORDER BY health.med_cert_main.mc_transaction_date DESC";
            List<MedicalCertificate> res = (List<MedicalCertificate>)QueryModule.DataSource<MedicalCertificate>(sql);
            if (res == null) return null;
            foreach (MedicalCertificate model in res) {
                sql = @"SELECT * FROM health.med_cert_dtl
                        INNER JOIN health.lab_exam_setup ON health.med_cert_dtl.mc_lab_exam_id = health.lab_exam_setup.lab_exam_id
                        WHERE health.med_cert_dtl.mc_main_id = @dtl_id
                        ";
                model.mc_exams_data = (List<MedicalCertificateExam>)QueryModule.DataSource<MedicalCertificateExam>(sql, new { dtl_id = model.mc_pk_id });
                string paysql = @"SELECT * FROM health.health_payment WHERE main_pk_id = @payid";
                model.mc_payment_data = (HealthPayment)QueryModule.DataObject<HealthPayment>(paysql, new { payid = model.mc_pk_id});
                
                string persql = @"SELECT * FROM general.person
	            inner join general.gender on general.person.gender_id = general.gender.gender_id
	            inner join general.civil_status on general.person.civil_status_id = general.civil_status.civil_status_id
	            inner join general_address.lgu_province_setup_temp on general.person.province_id = general_address.lgu_province_setup_temp.province_id
	            inner join general_address.lgu_city_mun_setup_temp on general.person.citmun_id = general_address.lgu_city_mun_setup_temp.city_mun_id
	            inner join general_address.lgu_brgy_setup_temp on general.person.barangay_id = general_address.lgu_brgy_setup_temp.brgy_id where general.person.person_guid = @pid";
                model.mc_person_data = (PersonModel)QueryModule.DataObject<PersonModel>(persql, new { pid = model.mc_person_id });

                
            }
            ExpandoObject from = Forms.getForm("medical-certificate");
            List<object> list = new List<object>();
            list.Add(res);
            list.Add(from);
            return list;
        }

        public List<object> GetExamData(string dtl_id)
        {
            string sql = "SELECT * FROM health.med_cert_dtl where mc_dtl_id = @dtl_id";
            List<object> result = (List<object>)QueryModule.DataSource<object>(sql, new { dtl_id = dtl_id });
            return result;
        }

        public bool Insert(NewMedicalCertificate model)
        {
            if (QueryModule.connection.State == System.Data.ConnectionState.Open) { 
                QueryModule.connection.Close();
            }
            QueryModule.connection.Open();
            using (var mc = QueryModule.connection.BeginTransaction()) {
                string sql = "";

                try
                {
                    model.form_trans_no = generateFormNumber("medical-certificate");
                    model.mc_pk_id = Guid.NewGuid().ToString();
                    model.mc_or_pkid = model.mc_pk_id;
                    model.transaction_log = model.mc_pk_id;
                    model.mc_payment_id = model.mc_pk_id;
                    sql = "insert into health.med_cert_main(" + ObjectSqlMapping.MapInsert<NewMedicalCertificate>() + ")";
                    int res = (int)QueryModule.Execute<int>(sql, model);

                    if (model.mc_exams_data.Count > 0)
                    {
                        foreach (var ed in model.mc_exams_data)
                        {
                            ed.mc_dtl_id = Guid.NewGuid().ToString();
                             ed.mc_main_id = model.mc_pk_id;
                            string esql = "insert into health.med_cert_dtl(" + ObjectSqlMapping.MapInsert<MedicalCertificateExam>() + ")";
                            int eres = (int)QueryModule.Execute<int>(esql, ed);
                        }
                        model.mc_requestor.mc_pk_id = model.mc_pk_id;
                        string rsql = "insert into health.med_cert_req(" + ObjectSqlMapping.MapInsert<TransactionRequestor>() + ")";
                        int rres = (int)QueryModule.Execute<int>(rsql, model.mc_requestor);
                    }
                    mc.Commit();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(sql);
                    mc.Rollback();
                    throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, ex.Message);
                }
            }
            QueryModule.connection.Close();
            return true;
        }

        public List<object> GetDatas(string ID)
        {
            string sql = @"SELECT * FROM health.med_cert_main
            WHERE health.med_cert_main.mc_pk_id = @id";
            List<MedicalCertificate> res = (List<MedicalCertificate>)QueryModule.DataSource<MedicalCertificate>(sql, new { id = ID });
            if (res == null) return null;
            foreach (MedicalCertificate model in res)
            {
                sql = @"SELECT * FROM health.med_cert_dtl
                        INNER JOIN health.lab_exam_setup ON health.med_cert_dtl.mc_lab_exam_id = health.lab_exam_setup.lab_exam_id
                        WHERE health.med_cert_dtl.mc_main_id = @dtl_id
                        ";
                model.mc_exams_data = (List<MedicalCertificateExam>)QueryModule.DataSource<MedicalCertificateExam>(sql, new { dtl_id = model.mc_pk_id });
                /* string paysql = @"SELECT * FROM health.health_payment WHERE main_pk_id = @payid";
                 model.mc_payment_data = (HealthPayment)QueryModule.DataObject<HealthPayment>(paysql, new { payid = model.mc_pk_id});*/
                string persql = @"SELECT * FROM general.person
	            inner join general.gender on general.person.gender_id = general.gender.gender_id
	            inner join general.civil_status on general.person.civil_status_id = general.civil_status.civil_status_id
	            inner join general_address.lgu_province_setup_temp on general.person.province_id = general_address.lgu_province_setup_temp.province_id
	            inner join general_address.lgu_city_mun_setup_temp on general.person.citmun_id = general_address.lgu_city_mun_setup_temp.city_mun_id
	            inner join general_address.lgu_brgy_setup_temp on general.person.barangay_id = general_address.lgu_brgy_setup_temp.brgy_id where general.person.person_guid = @pid";
                model.mc_person_data = (PersonModel)QueryModule.DataObject<PersonModel>(persql, new { pid = model.mc_person_id });
            }
            ExpandoObject from = Forms.getForm("medical-certificate");
            List<object> list = new List<object>();
            list.Add(res);
            list.Add(from);
            return list;
        }

        public bool UpdateDtlById(string id, MedicalCertificateExam model)
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
                    sql = "delete from health.med_cert_dtl where mc_dtl_id = '" + id + "'";
                    QueryModule.Execute<int>(sql);


                    model.mc_dtl_id = Guid.NewGuid().ToString();
                    model.mc_main_id = model.mc_main_id;
                    string tsql = "insert into health.med_cert_dtl(" + ObjectSqlMapping.MapInsert<MedicalCertificateExam>() + ")";
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

        public List<object> GetExamListData(string trans_id)
        {
            string sql = @"SELECT * FROM health.med_cert_dtl 
                            INNER JOIN health.lab_exam_setup ON health.med_cert_dtl.mc_lab_exam_id = health.lab_exam_setup.lab_exam_id
                            where mc_main_id = @main_id
                            ";
            List<object> result = (List<object>)QueryModule.DataSource<object>(sql, new { main_id = trans_id });
            return result;
        }

        public bool UpdateDetails(string id, List<MedicalCertificateExam> model)
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
                    sql = "delete from health.med_cert_dtl where mc_main_id = '" + id + "'";
                    QueryModule.Execute<int>(sql);


                    if (model.Count > 0)
                    {
                        foreach (var et in model)
                        {
                            et.mc_dtl_id = Guid.NewGuid().ToString();
                            et.mc_main_id = id;
                            string tsql = "insert into health.med_cert_dtl(" + ObjectSqlMapping.MapInsert<MedicalCertificateExam>() + ")";
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

        public bool PayedTrans(string id)
        {
            string sql = "UPDATE health.med_cert_main SET mc_transaction_status = '1' where health.med_cert_main.mc_pk_id= @transid";
            if (InsertUpdate.ExecuteSql(sql, new { transid = id }))
                return true;
            return false;
        }

        public bool DeleteTrans(string id)
        {
            string sql = "UPDATE health.med_cert_main SET mc_transaction_status = '3' where health.med_cert_main.mc_pk_id= @transid";
            if (InsertUpdate.ExecuteSql(sql, new { transid = id }))
                return true;
            return false;
        }

        public bool RevertTrans(string id)
        {
            string sql = "UPDATE health.med_cert_main SET mc_transaction_status = '0' where health.med_cert_main.mc_pk_id= @transid";
            if (InsertUpdate.ExecuteSql(sql, new { transid = id }))
                return true;
            return false;
        }

    }
}
