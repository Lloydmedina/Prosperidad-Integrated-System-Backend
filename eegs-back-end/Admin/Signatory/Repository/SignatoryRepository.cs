using eegs_back_end.Admin.Signatory.Model;
using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.Signatory.Repository
{
    public interface ISignatoryRepository : IGlobalInterface
    {
        bool Insert(SignatoryModel model);
        bool Edit(string id, SignatoryModel model);
        object GetList();
        List<object> GetDepartment();
        List<object> GetDropdown();
        SignatoryModel GetByID(string id);
        bool Delete(string id);

    }
    public class SignatoryRepository : ISignatoryRepository
    {
        public bool Delete(string id)
        {
            string sql = "UPDATE general.signatory_main SET status = 'Deleted' where general.signatory_main.signatory_main_id = @id";
            if ((int)QueryModule.Execute<int>(sql, new { id = id }) == 1)
                return true;
            return false;
        }

        public bool Edit(string id, SignatoryModel model)
        {
            if (QueryModule.connection.State == System.Data.ConnectionState.Open)
            {
                QueryModule.connection.Close();
            }
            QueryModule.connection.Open();
            using (var transaction = QueryModule.connection.BeginTransaction())
            {
                try
                {
                    model.signatory_main_id = id;
                    model.status = "Active";

                    string sql = "UPDATE general.signatory_main SET " + ObjectSqlMapping.MapUpdate<SignatoryModel>() + " WHERE general.signatory_main.signatory_main_id = @signatory_main_id ";
                    int res = (int)QueryModule.Execute<int>(sql, model);


                    sql = "delete from general.signatory_dtl where signatory_main_id = '" + id + "'  ";
                    QueryModule.Execute<int>(sql);

                    if (model.signatoryDetails.Count > 0)
                    {
                        foreach (SignatoryDetail obj in model.signatoryDetails)
                        {
                            obj.signatory_dtl_id = Guid.NewGuid().ToString();
                            obj.signatory_main_id = model.signatory_main_id;
                            sql = "insert into general.signatory_dtl (" + ObjectSqlMapping.MapInsert<SignatoryDetail>() + ")";
                            QueryModule.Execute<int>(sql, obj);
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, ex.Message);
                }
            }
            QueryModule.connection.Close();

            return true;
        }

        public SignatoryModel GetByID(string id)
        {
            string sql = @"select * from general.signatory_main where signatory_main_id = @id";
            SignatoryModel model = (SignatoryModel)QueryModule.DataObject<SignatoryModel>(sql, new {id = id });

            sql = "select * from general.signatory_dtl where signatory_main_id = @id";
            model.signatoryDetails = (List<SignatoryDetail>)QueryModule.DataSource<SignatoryDetail>(sql, new { id = id });

            return model;
        }

        public List<object> GetDepartment()
        {
            string sql = @"select department.dept_id, dept_name, short_desc
                    --    , emp_head.employee_id as dept_head_id, emp_head.employee_name as dept_head_name
                                  -- elected.employee_id as mayor_id, elected.employee_name as mayor_name
                         from humanresource.department
                      --   inner join humanresource.department_head deptHead on deptHead.dept_id = department.dept_id and deptHead.end_time is null
                      --   inner join humanresource.employees emp_head on emp_head.employee_id = deptHead.employee_id
                        -- inner join humanresource.elected_official elected on elected.project_title_guid = department.project_title_guid and elected.position_id = 'pos1'";
            List<object> list = (List<object>)QueryModule.DataSource<object>(sql);

            return list;
        }

        public List<object> GetDropdown()
        {
            string sql = "select * from general.signatory_type";
            List<object> sig_type = (List<object>)QueryModule.DataSource<object>(sql);

            sql = "select * from general.signatory_assign_type";
            List<object> assign_type = (List<object>)QueryModule.DataSource<object>(sql);

            sql = @"select form.form_guid, form.form_name, dept.dept_id from humanresource.department dept
                inner join general.domain dom on dom.domain_guid = dept.domain_id
                inner join general.tbl_user_form form on form.domain_guid = dom.domain_guid
                and form.show_signatory = 'true' and form.form_name NOT LIKE '%dashboard%'
                ";
            List<object> forms = (List<object>)QueryModule.DataSource<object>(sql);

            List<object> list = new List<object>();

            list.Add(new { sig_type = sig_type });
            list.Add(new { assign_type = assign_type });
            list.Add(new { forms = forms });
            return list;
        }

        public object GetList()
        {
            string sql = @"select signatory_main_id, signatory_name, savedate, dept_name, status from general.signatory_main where status = 'Active' ";
            List<object> list = (List<object>)QueryModule.DataSource<object>(sql);

            return list;
        }

        public bool Insert(SignatoryModel model)
        {
            if (QueryModule.connection.State == System.Data.ConnectionState.Open)
            {
                QueryModule.connection.Close();
            }
            QueryModule.connection.Open();
            using (var transaction = QueryModule.connection.BeginTransaction())
            {
                try
                {
                    model.signatory_main_id = Guid.NewGuid().ToString();
                    model.status = "Active";

                    string sql = "insert into general.signatory_main (" + ObjectSqlMapping.MapInsert<SignatoryModel>() + ")";
                    int res = (int)QueryModule.Execute<int>(sql, model);

                    if(model.signatoryDetails.Count > 0)
                    {
                        foreach(SignatoryDetail obj in model.signatoryDetails)
                        {
                            obj.signatory_dtl_id = Guid.NewGuid().ToString();
                            obj.signatory_main_id = model.signatory_main_id;
                            sql = "insert into general.signatory_dtl (" + ObjectSqlMapping.MapInsert<SignatoryDetail>() + ")";
                            QueryModule.Execute<int>(sql, obj);
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, ex.Message);
                }
            }
            QueryModule.connection.Close();

            return true;
        }
    }
}
