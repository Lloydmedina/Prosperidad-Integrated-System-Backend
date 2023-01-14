using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using eegs_back_end.Health.medical_abstract.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Dynamic;

namespace eegs_back_end.Health.medical_abstract.Repository
{
    public interface IMedicalAbstractRepository : IGlobalInterface
    {
     
       bool Insert(NewMedicalAbstract model);
       List<object> GetMedicineTypeList();
       List<object> GetList();
       List<object> GetById(string ID);
        bool UpdateDtl(string id, MedicalAbstractDtl model);

        bool PayedTrans(string id);
        bool DeleteTrans(string id);
        bool RevertTrans(string id);
        List<object> Getsignatory();
    }
    public class MedicalAbstractRepository : FormNumberGenerator, IMedicalAbstractRepository
    {
        public List<object> GetById(string ID)
        {
            string sql = @"SELECT * FROM health.med_abstract_main
                        LEFT JOIN general.transaction_status ts on  health.med_abstract_main.ma_transaction_status = general.ts.status_id
                        LEFT JOIN health.health_payment ON health.med_abstract_main.ma_pk_id = health.health_payment.main_pk_id
                        where ma_pk_id = @id";
            List<MedicalAbstract> res = (List<MedicalAbstract>)QueryModule.DataSource<MedicalAbstract>(sql, new { id = ID});
            if (res == null) return null;
            foreach (MedicalAbstract model in res)
            {
                sql = @"SELECT * FROM health.med_abstract_hpi_dtl where ma_main_id = @hpi";
                model.ma_hpi_data = (List<HpiModel>)QueryModule.DataSource<HpiModel>(sql, new { hpi = model.ma_pk_id });
            }
            if (res == null) return null;
            foreach (MedicalAbstract model in res)
            {
                sql = @"SELECT * FROM health.med_abstract_pe_dtl where ma_main_id = @pe";
                model.ma_pe_data = (List<PeModel>)QueryModule.DataSource<PeModel>(sql, new { pe = model.ma_pk_id });
            }
            if (res == null) return null;
            foreach (MedicalAbstract model in res)
            {
                sql = @"SELECT * FROM health.med_abstract_dx_dtl where ma_main_id = @dx";
                model.ma_dx_data = (List<DiagnosisModel>)QueryModule.DataSource<DiagnosisModel>(sql, new { dx = model.ma_pk_id });
            }
            if (res == null) return null;
            foreach (MedicalAbstract model in res)
            {
                sql = @"SELECT * FROM health.med_abstract_meds_dtl where ma_main_id = @meds";
                model.ma_meds_data = (List<MedicationModel>)QueryModule.DataSource<MedicationModel>(sql, new { meds = model.ma_pk_id });
                string persql = @"SELECT * FROM general.person
	            inner join general.gender on general.person.gender_id = general.gender.gender_id
	            inner join general.civil_status on general.person.civil_status_id = general.civil_status.civil_status_id
	            inner join general_address.lgu_province_setup_temp on general.person.province_id = general_address.lgu_province_setup_temp.province_id
	            inner join general_address.lgu_city_mun_setup_temp on general.person.citmun_id = general_address.lgu_city_mun_setup_temp.city_mun_id
	            inner join general_address.lgu_brgy_setup_temp on general.person.barangay_id = general_address.lgu_brgy_setup_temp.brgy_id where general.person.person_guid = @pid";
                model.ma_person_data = (PersonModel)QueryModule.DataObject<PersonModel>(persql, new { pid = model.ma_person_id });

                string reqsql = @"SELECT * FROM health.med_abstract_req where ma_pk_id = @req";
                model.ma_requestor = (TransactionRequestor)QueryModule.DataObject<TransactionRequestor>(reqsql, new { req = model.ma_pk_id });

                /*     string paysql = @"SELECT * FROM health.health_payment where ma_pk_id = @req";
                     model.ma_payment_data = (HealthPayment)QueryModule.DataObject<HealthPayment>(paysql, new { req = model.ma_pk_id });
                   */
            }

            ExpandoObject form = Forms.getForm("medical-abstract");
            List<object> list = new List<object>();
            list.Add(res);
            list.Add(form);
            return list;
        }

        public List<object> GetList()
        {
            string sql = @"SELECT * FROM health.med_abstract_main
                        LEFT JOIN general.transaction_status ts on  health.med_abstract_main.ma_transaction_status = general.ts.status_id
                        LEFT JOIN health.health_payment ON health.med_abstract_main.ma_pk_id = health.health_payment.main_pk_id
                        ORDER BY health.med_abstract_main.ma_transaction_date DESC";
            List<MedicalAbstract> res = (List<MedicalAbstract>)QueryModule.DataSource<MedicalAbstract>(sql);
            if (res == null) return null;
            foreach (MedicalAbstract model in res) {
                sql = @"SELECT * FROM health.med_abstract_hpi_dtl where ma_main_id = @hpi";
                model.ma_hpi_data = (List<HpiModel>)QueryModule.DataSource<HpiModel>(sql, new { hpi = model.ma_pk_id});
            }
            if (res == null) return null;
            foreach (MedicalAbstract model in res) {
                sql = @"SELECT * FROM health.med_abstract_pe_dtl where ma_main_id = @pe";
                model.ma_pe_data = (List<PeModel>)QueryModule.DataSource<PeModel>(sql, new { pe = model.ma_pk_id });
            }
            if (res == null) return null;
            foreach (MedicalAbstract model in res) {
                sql = @"SELECT * FROM health.med_abstract_dx_dtl where ma_main_id = @dx";
                model.ma_dx_data = (List<DiagnosisModel>)QueryModule.DataSource<DiagnosisModel>(sql, new { dx = model.ma_pk_id });
            }
            if (res == null) return null;
            foreach (MedicalAbstract model in res){
                sql = @"SELECT * FROM health.med_abstract_meds_dtl where ma_main_id = @meds";
                model.ma_meds_data = (List<MedicationModel>)QueryModule.DataSource<MedicationModel>(sql, new { meds = model.ma_pk_id });
                string persql = @"SELECT * FROM general.person
	            inner join general.gender on general.person.gender_id = general.gender.gender_id
	            inner join general.civil_status on general.person.civil_status_id = general.civil_status.civil_status_id
	            inner join general_address.lgu_province_setup_temp on general.person.province_id = general_address.lgu_province_setup_temp.province_id
	            inner join general_address.lgu_city_mun_setup_temp on general.person.citmun_id = general_address.lgu_city_mun_setup_temp.city_mun_id
	            inner join general_address.lgu_brgy_setup_temp on general.person.barangay_id = general_address.lgu_brgy_setup_temp.brgy_id where general.person.person_guid = @pid";
                model.ma_person_data = (PersonModel)QueryModule.DataObject<PersonModel>(persql, new { pid = model.ma_person_id });

                string reqsql = @"SELECT * FROM health.med_abstract_req where ma_pk_id = @req";
                model.ma_requestor = (TransactionRequestor)QueryModule.DataObject<TransactionRequestor>(reqsql, new { req = model.ma_pk_id});

                string paysql = @"SELECT * FROM health.health_payment where main_pk_id = @req";
                model.ma_payment_data = (HealthPayment)QueryModule.DataObject<HealthPayment>(paysql, new { req = model.ma_pk_id });
            
            }
            
            ExpandoObject form = Forms.getForm("medical-abstract");
            List<object> list = new List<object>();
            list.Add(res);
            list.Add(form);
            return list;
        }

        public List<object> GetMedicineTypeList()
        {
            string sql = "select * from health.medications_setup";
            List<object> res = (List<object>)QueryModule.DataSource<object>(sql);
            return res;
        }

        public bool Insert(NewMedicalAbstract model)
        {
            if (QueryModule.connection.State == System.Data.ConnectionState.Open) { 
                QueryModule.connection.Close();
            }
            QueryModule.connection.Open();
            using (var ma = QueryModule.connection.BeginTransaction()) {
                string sql = "";

                try
                {
                    model.form_trans_no = generateFormNumber("medical-abstract");
                    model.ma_pk_id = Guid.NewGuid().ToString();
                    model.ma_or_pkid = model.ma_pk_id;
                    model.transaction_log = model.ma_pk_id;
                    model.ma_payment_id = model.ma_pk_id;
                    sql = "insert into health.med_abstract_main(" + ObjectSqlMapping.MapInsert<NewMedicalAbstract>() + ")";
                    int res = (int)QueryModule.Execute<int>(sql, model);

                    if (model.ma_requestor.requestor_id != null)
                    { 
                        model.ma_requestor.ma_pk_id = model.ma_pk_id;
                        string rqsql = "insert into health.med_abstract_req(" + ObjectSqlMapping.MapInsert<TransactionRequestor>() + ")";
                        int rqres = (int)QueryModule.Execute<int>(rqsql, model.ma_requestor);
                    }

                    if (model.ma_hpi_data.Count > 0)
                    {
                        foreach (var hpi in model.ma_hpi_data)
                        {
                            hpi.ma_hpi_dtl_id = Guid.NewGuid().ToString();
                            hpi.ma_main_id = model.ma_pk_id;
                            string hpisql = "insert into health.med_abstract_hpi_dtl("+ObjectSqlMapping.MapInsert<HpiModel>()+")";
                            int hpires = (int)QueryModule.Execute<int>(hpisql, hpi);
                        }
                    }
                    if (model.ma_pe_data.Count > 0)
                    {
                        foreach (var pe in model.ma_pe_data)
                        { 
                            pe.ma_pe_dtl_id = Guid.NewGuid().ToString();
                            pe.ma_main_id = model.ma_pk_id;
                            string pesql = "insert into health.med_abstract_pe_dtl(" + ObjectSqlMapping.MapInsert<PeModel>() + ")";
                            int peres = (int)QueryModule.Execute<int>(pesql, pe);
                        }
                    
                    }
                    if (model.ma_dx_data.Count > 0)
                    {
                        foreach (var dx in model.ma_dx_data)
                        { 
                            dx.ma_dx_dtl_id = Guid.NewGuid().ToString();
                            dx.ma_main_id = model.ma_pk_id;
                            string dxsql = "insert into health.med_abstract_dx_dtl(" + ObjectSqlMapping.MapInsert<DiagnosisModel>() + ")";
                            int dxres = (int)QueryModule.Execute<int>(dxsql, dx);
                        }
                    }
                    if (model.ma_meds_data.Count > 0)
                    {
                        foreach (var meds in model.ma_meds_data)
                        { 
                            meds.ma_meds_dtl_id= Guid.NewGuid().ToString();
                            meds.ma_main_id = model.ma_pk_id;
                            string medssql = "insert into health.med_abstract_meds_dtl(" + ObjectSqlMapping.MapInsert<MedicationModel>() + ")";
                            int medsres = (int)QueryModule.Execute<int>(medssql, meds);
                        }
                    }
                    ma.Commit();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(sql);
                    ma.Rollback();
                    throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, ex.Message);
                }
            }
            QueryModule.connection.Close();
            return true;
        }

        public bool UpdateDtl(string id, MedicalAbstractDtl model)
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

                    sql = "delete from health.med_abstract_hpi_dtl where ma_main_id = '" + id + "'";
                    QueryModule.Execute<int>(sql);

                    sql = "delete from health.med_abstract_pe_dtl where ma_main_id = '" + id + "'";
                    QueryModule.Execute<int>(sql);

                    sql = "delete from health.med_abstract_dx_dtl where ma_main_id = '" + id + "'";
                    QueryModule.Execute<int>(sql);

                    sql = "delete from health.med_abstract_meds_dtl where ma_main_id = '" + id + "'";
                    QueryModule.Execute<int>(sql);


                    if (model.ma_hpi_data.Count > 0)
                    {
                        foreach (var hpi in model.ma_hpi_data)
                        {
                            hpi.ma_hpi_dtl_id = Guid.NewGuid().ToString();
                            hpi.ma_main_id = id;
                            string hpisql = "insert into health.med_abstract_hpi_dtl(" + ObjectSqlMapping.MapInsert<HpiModel>() + ")";
                            int hpires = (int)QueryModule.Execute<int>(hpisql, hpi);
                        }
                    }
                    if (model.ma_pe_data.Count > 0)
                    {
                        foreach (var pe in model.ma_pe_data)
                        {
                            pe.ma_pe_dtl_id = Guid.NewGuid().ToString();
                            pe.ma_main_id = id;
                            string pesql = "insert into health.med_abstract_pe_dtl(" + ObjectSqlMapping.MapInsert<PeModel>() + ")";
                            int peres = (int)QueryModule.Execute<int>(pesql, pe);
                        }

                    }
                    if (model.ma_dx_data.Count > 0)
                    {
                        foreach (var dx in model.ma_dx_data)
                        {
                            dx.ma_dx_dtl_id = Guid.NewGuid().ToString();
                            dx.ma_main_id = id;
                            string dxsql = "insert into health.med_abstract_dx_dtl(" + ObjectSqlMapping.MapInsert<DiagnosisModel>() + ")";
                            int dxres = (int)QueryModule.Execute<int>(dxsql, dx);
                        }
                    }
                    if (model.ma_meds_data.Count > 0)
                    {
                        foreach (var meds in model.ma_meds_data)
                        {
                            meds.ma_meds_dtl_id = Guid.NewGuid().ToString();
                            meds.ma_main_id = id;
                            string medssql = "insert into health.med_abstract_meds_dtl(" + ObjectSqlMapping.MapInsert<MedicationModel>() + ")";
                            int medsres = (int)QueryModule.Execute<int>(medssql, meds);
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


        public bool PayedTrans(string id)
        {
            string sql = "UPDATE health.med_abstract_main SET ma_transaction_status = '1' where health.med_abstract_main.ma_pk_id= @transid";
            if (InsertUpdate.ExecuteSql(sql, new { transid = id }))
                return true;
            return false;
        }

        public bool DeleteTrans(string id)
        {
            string sql = "UPDATE health.med_abstract_main SET ma_transaction_status = '3' where health.med_abstract_main.ma_pk_id= @transid";
            if (InsertUpdate.ExecuteSql(sql, new { transid = id }))
                return true;
            return false;
        }

        public bool RevertTrans(string id)
        {
            string sql = "UPDATE health.med_abstract_main SET ma_transaction_status = '0' where health.med_abstract_main.ma_pk_id= @transid";
            if (InsertUpdate.ExecuteSql(sql, new { transid = id }))
                return true;
            return false;
        }

        public List<object> Getsignatory()

        {
            string sql = "select * from health.singatory_";
            List<object> res = (List<object>)QueryModule.DataSource<object>(sql);
            return res;
        }
    }
}
