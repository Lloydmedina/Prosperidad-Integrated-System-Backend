using eegs_back_end.SocialWelfare.Wcc.wcc_registration.Model;
using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.SocialWelfare.Wcc.wcc_registration.Repository
{
    public interface IWccRegistrationRepository : IGlobalInterface
    {
        //do somthing here
        List<object> GetList();
        object GetFC(string pid);
        object GetFCHead(string pid);
        List<object> GetFCList(string fcid);
        bool Insert(WccRegistrationModel model);
        object GetData(string tid);
        object GetReferrer(string pid);
        bool UpdateData(string id, WccRegistrationModel model);
    }
    public class WccRegistrationRepository : FormNumberGenerator, IWccRegistrationRepository
    {
        //do somthing here
        public List<object> GetList()
        {
            string sql = @"SELECT * From mswd.wcc_registration ORDER BY mswd.wcc_registration.transaction_date DESC";
            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("wcc-registration");

            List<object> list = new List<object>();
            list.Add(obj);
            list.Add(form);


            return list;
        }

        public object GetFC(string pid)
        {
            string sql = @"SELECT main_guid From mswd.family_composition_details where person_guid = @id";
            object obj = QueryModule.DataObject<object>(sql, new { id = pid });
            return obj;
        }

        public object GetFCHead(string pid)
        {
            string sql = @"SELECT * From mswd.family_composition_head where person_guid = @id and status != 'Trash'";
            object obj = QueryModule.DataObject<object>(sql, new { id = pid });
            return obj;
        }

        public List<object> GetFCList(string fcid)
        {
            string sql = @"SELECT * From mswd.family_composition_details
                  inner join general.person on mswd.family_composition_details.person_guid = general.person.person_guid
                    inner join general.gender on general.person.gender_id = general.gender.gender_id
                    inner join general.civil_status on general.person.civil_status_id = general.civil_status.civil_status_id
                    inner join general_address.lgu_province_setup_temp on general.person.province_id = general_address.lgu_province_setup_temp.province_id
                    inner join general_address.lgu_city_mun_setup_temp on general.person.citmun_id = general_address.lgu_city_mun_setup_temp.city_mun_id
                    inner join general_address.lgu_brgy_setup_temp on general.person.barangay_id = general_address.lgu_brgy_setup_temp.brgy_id  
                    where main_guid = @id ";
            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql, new { id = fcid });
            return obj;
        }

        public bool Insert(WccRegistrationModel model)
        {
            if (QueryModule.connection.State == System.Data.ConnectionState.Open)
            { 
                QueryModule.connection.Close();
            }
            QueryModule.connection.Open();
            using (var wcc = QueryModule.connection.BeginTransaction())
            {
                string sql = "";

                try
                {
                    model.form_trans_no = generateFormNumber("wcc-registration");
                    model.main_pk_id = Guid.NewGuid().ToString();
                    sql = "insert into mswd.wcc_registration (" + ObjectSqlMapping.MapInsert<WccRegistrationModel>() + ")";
                    int res = (int)QueryModule.Execute<int>(sql, model);
                    wcc.Commit();
                }
                catch (Exception ex){
                    Console.WriteLine(sql);
                    wcc.Rollback();
                    throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, ex.Message);

                }
            }
            QueryModule.connection.Close();
                return true;
        }

        public object GetData(string tid)
        {
            string sql = @"SELECT * From mswd.wcc_registration where main_pk_id = @id";
            object obj = QueryModule.DataObject<object>(sql, new { id = tid });
            ExpandoObject form = Forms.getForm("wcc-registration");

            List<object> res = new List<object>();
            res.Add(obj);
            res.Add(form);


            return res;
        }

        public object GetReferrer(string pid)
        {
            string sql = @"select 
			emp.employee_id,
			emp.emp_account_no,
			emp.person_guid as `person_id`,
			emp.employee_name,
			emp.trans_date,
			`status`.status_name as `status`, 
			pos.position_desc as `position_name`, 
			dept.short_desc as `dept_name`
 
            from humanresource.employees emp

            left join humanresource.employee_status stat on stat.employee_id = emp.employee_id and stat.end_time is NULL
            left join humanresource.`status` `status` on `status`.status_id = stat.status_id
            left join humanresource.person_position perpos on perpos.person_id = emp.person_guid and perpos.end_time is NULL
            left join humanresource.position pos on pos.position_id = perpos.position_id
            left join humanresource.person_dept perdept on perdept.person_id = emp.person_guid and perdept.end_time is NULL
            left join humanresource.department dept on dept.dept_id = perdept.dept_id 
                where emp.employee_id = @id
            ";
            object obj = QueryModule.DataObject<object>(sql, new { id = pid });
            ExpandoObject form = Forms.getForm("wcc-registration");
            List<object> res = new List<object>();
            res.Add(obj);
            res.Add(form);


            return res;
        }

        public bool UpdateData(string id, WccRegistrationModel model)
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
                    sql = "delete from mswd.wcc_registration where main_pk_id = '" + id + "'";
                    QueryModule.Execute<int>(sql);

                    model.form_trans_no = model.form_trans_no;
                    model.main_pk_id = id;
                    sql = "insert into mswd.wcc_registration(" + ObjectSqlMapping.MapInsert<WccRegistrationModel>() + ")";
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
    }
}
