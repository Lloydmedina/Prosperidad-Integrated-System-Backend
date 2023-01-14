using eegs_back_end.Admin.FeesCharges.Model;
using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using eegs_back_end.Shell.RoleType.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.FeesCharges.Repository
{
    public interface IFCRepository: IGlobalInterface
    {
        List<object> GetList(int? status_id = 0);
        FCModel GetByID(string ID);
        bool Insert(FCModel model);
        bool Edit(string ID, FCModel model);
        bool Delete(string ID);
        List<object> GetDropDown();
    }
    public class FCRepository : IFCRepository
    {

        public bool Delete(string ID)
        {
            string sql = "update general.fees_charges SET status = 'Deleted' WHERE fees_pk_id = '" + ID + "'";
            int i = (int)QueryModule.Execute<int>(sql);
            if (i == 0) return false;

            return true;
        }

        public bool Edit(string ID, FCModel model)
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
                    model.fees_pk_id = ID;
                    model.status = "Active";
                    string sql = "update general.fees_charges SET " + ObjectSqlMapping.MapUpdate<FCModel>() + " WHERE fees_pk_id = '" + ID + "'";
                    QueryModule.Execute<int>(sql, model);


                    sql = "delete from general.fees_range where fees_pk_id = '" + ID + "' ";
                    QueryModule.Execute<int>(sql);
                    sql = "delete from general.fees_dept_form where fees_pk_id = '" + ID + "'";
                    QueryModule.Execute<int>(sql);


                    if (model.range_fees != null && model.range_fees.Count > 0)
                    {
                        foreach (FCRange obj in model.range_fees)
                        {
                            obj.fees_pk_id = model.fees_pk_id;
                            sql = "insert into general.fees_range (" + ObjectSqlMapping.MapInsert<FCRange>() + ")";
                            QueryModule.Execute<int>(sql, obj);
                        }
                    }


                    if (model.dept_form != null && model.dept_form.Count > 0)
                    {

                        foreach (FCDeptForm obj in model.dept_form)
                        {
                            obj.fees_pk_id = model.fees_pk_id;
                            sql = "insert into general.fees_dept_form (" + ObjectSqlMapping.MapInsert<FCDeptForm>() + ")";
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

        public FCModel GetByID(string ID)
        {
            string sql = @"select * from general.fees_charges fees where fees.fees_pk_id = @fees_pk_id ";
            FCModel model = (FCModel)QueryModule.DataObject<FCModel>(sql, new { fees_pk_id = ID });

            
            sql = @"select * from general.fees_dept_form frm where frm.fees_pk_id = @fees_pk_id ";
            model.dept_form = (List<FCDeptForm>)QueryModule.DataSource<FCDeptForm>(sql, new { fees_pk_id = ID });

            model.range_fees = new List<FCRange>();
            if(model.fees_type_id == 2)
            {
                sql = @"select * from general.fees_range frm where frm.fees_pk_id = @fees_pk_id ";
                model.range_fees = (List<FCRange>)QueryModule.DataSource<FCRange>(sql, new { fees_pk_id = ID });
            }

            return model;
        }


        public List<object> GetList(int? status_id = 0)
        {
            string whr = "";
            if(status_id == 1)
            {
                whr = "WHERE fees.status = 'Active'";
            }
            string sql = @"select fees.fees_pk_id,  fees.fees_code,  fees.fees_name, type.fees_type_name as fees_type,

                        case WHEN fees.fees_type_id = 1 THEN FORMAT(fees.initial_amount, 2)
                        WHEN fees.fees_type_id = 2 THEN 
                        (select CONCAT(FORMAT(min(r.fees), 2),' - ', FORMAT(MAX(r.fees), 2)) from general.fees_range r where r.fees_pk_id = fees.fees_pk_id	)
                        WHEN fees.fees_type_id = 3 THEN 
                        CONCAT(fees.initial_amount,'%') END
                        as initial_amount,
                        acc.account_name as income_account,  fees.status
                         from general.fees_charges fees
                        inner join general.fees_type type on type.fees_type_id = fees.fees_type_id
                        left join accounting.account_title acc on acc.account_id = fees.account_id
                        " + whr + " ";
            List<object> data = (List<object>)QueryModule.DataSource<object>(sql);


            ExpandoObject form = Forms.getForm("fees-charges");

            List<object> list = new List<object>();
            list.Add(data);
            list.Add(form);
            return list;
        }

        public List<object> GetDropDown()
        {

            string sql = "select * from general.fees_type";
            List<object> fees_type = (List<object>)QueryModule.DataSource<object>(sql);

            sql = @"select dept.dept_name as `title`, dept.dept_id as `key`, dept.domain_id as domain_guid from humanresource.department dept
                    inner join general.domain dom on dom.domain_guid = dept.domain_id";
            List<NodeKey> dept = (List<NodeKey>)QueryModule.DataSource<NodeKey>(sql);


            sql = "select * from accounting.account_title";
            List<object> accounts = (List<object>)QueryModule.DataSource<object>(sql);

            sql = "select * from general.fees_charges";
            List<object> parent = (List<object>)QueryModule.DataSource<object>(sql);


            List<object> list = new List<object>();




            List<int> removeIndexs = new List<int>();

            string[] nodeKeys = dept.Select(x => x.domain_guid).ToArray();
            string param = ObjectSqlMapping.SetParam(nodeKeys);


            sql = @"select 
                        form.form_name as title, 
                        form.form_guid as `key`,
                        form.domain_guid,
                        dept.dept_id
                    from general.tbl_user_form form
                    inner join humanresource.department dept on dept.domain_id = form.domain_guid
                    where form.domain_guid IN (" + param + ") and form.form_type = 'Form' and form.with_fees = 'true'";

            List<NodeKey> forms = (List<NodeKey>)QueryModule.DataSource<NodeKey>(sql);



            if (dept.Count > 0)
            {
                int cnt = 0;
                int k = 0;
                foreach (var z in dept)
                {
                    if (z.domain_guid != null)
                    {

                        int index = dept.FindIndex(x => x.domain_guid == z.domain_guid);
                        if (index > -1)
                        {
                            if ((dept[index].children == null))
                            {
                                dept[index].children = new List<NodeKey>();
                            }

                            if ((z.children == null))
                            {
                                //z.Route = new List<ActivityType>();
                            }

                            z.children = forms.AsEnumerable().Where(r => r.domain_guid == z.domain_guid).ToList();


                            if (z.children.Count == 0)
                            {
                                z.children = null;
                            }
                            else
                            {
                                foreach (var x in z.children)
                                {
                                    //Route[k] = x.ExecutablePath;
                                    k++;
                                }
                            }
                            //dept.RemoveAt(cnt);
                        }
                    }
                    cnt++;
                }
            }


            list.Add(new {feesType = fees_type, dept = dept, accounts = accounts, parent = parent });

            return list;
        }

        public bool Insert(FCModel model)
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

                    model.fees_pk_id = Guid.NewGuid().ToString();
                    model.status = "Active";
                    string sql = "insert into general.fees_charges (" + ObjectSqlMapping.MapInsert<FCModel>() + ")";
                    int res = (int)QueryModule.Execute<int>(sql, model);

                    if (model.range_fees != null && model.range_fees.Count > 0)
                    {
                        foreach (FCRange obj in model.range_fees)
                        {
                            obj.fees_pk_id = model.fees_pk_id;
                            sql = "insert into general.fees_range (" + ObjectSqlMapping.MapInsert<FCRange>() + ")";
                            res = (int)QueryModule.Execute<int>(sql, obj);
                        }
                    }


                    if (model.dept_form != null && model.dept_form.Count > 0)
                    {

                        foreach (FCDeptForm obj in model.dept_form)
                        {
                            obj.fees_pk_id = model.fees_pk_id;
                            sql = "insert into general.fees_dept_form (" + ObjectSqlMapping.MapInsert<FCDeptForm>() + ")";
                            res = (int)QueryModule.Execute<int>(sql, obj);
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
