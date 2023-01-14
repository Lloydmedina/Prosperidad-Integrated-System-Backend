using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using eegs_back_end.Shell.RoleType.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Shell.RoleType.Repository
{
    public interface IRoleTypeRepository : IGlobalInterface
    {
        List<object> GetList();
        RoleTypeModel GetByID(string ID);

        RoleTypeModel GetByID2(string ID);
        object GetActivityType(string DomainGUID);
        bool Insert(RoleTypeModel model);
        bool Edit(string GUID, RoleTypeModel model);
        public bool Delete(string GUID);
    }
    public class RoleTypeRepository : IRoleTypeRepository
    {
        public bool Delete(string GUID)
        {
            string sql = "UPDATE general.roletype SET status = 'Deleted' where general.roletype.roletype_guid = @roletype_guid";
            if (InsertUpdate.ExecuteSql(sql, new { roletype_guid = GUID }))
                return true;
            return false;
        }
        public bool Edit(string GUID, RoleTypeModel model)
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
                    model.roletype_guid = GUID;
                    model.status = "Active";
                    var sql = "UPDATE general.roletype SET " + ObjectSqlMapping.MapUpdate<RoleTypeModel>() + " WHERE general.roletype.roletype_guid = @roletype_guid ";
                    int res = (int)QueryModule.Execute<int>(sql, model);


                    sql = "DELETE FROM general.roletype_activity WHERE general.roletype_activity.roletype_guid = @roletype_guid";
                    QueryModule.Execute<int>(sql, model);



                    if (res > 0)
                    {
                        foreach (string i in model.activity_guid)
                        {
                            sql = "insert into general.roletype_activity (`roletype_guid`, `activity_guid`) VALUES (@roletype_guid, @activity_guid)";
                            QueryModule.Execute<int>(sql, new { roletype_guid = model.roletype_guid, activity_guid = i });
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
        public object GetActivityType(string DomainGUID)
        {
            //string sql = @"select	form_name as title,
            //       form_number, form_guid as `key`, parent_guid, domain_guid
            //from(select * from general.tbl_user_form ) form,
            //        (select @pv:= '" + DomainGUID + @"') initialisation
            //where   find_in_set(domain_guid, @pv)
            //and length(@pv := concat(@pv, ',', domain_guid)) ";
            string sql = @"select	form_name as title,
                   form_number, form_guid as `key`, parent_guid, domain_guid
            from(select * from general.tbl_user_form ) form";

            List<NodeKey> nodes = (List<NodeKey>)QueryModule.DataSource<NodeKey>(sql);

            List<int> removeIndexs = new List<int>();

            string[] nodeKeys = nodes.Select(x => x.key).ToArray();
            string param = ObjectSqlMapping.SetParam(nodeKeys);
       

            sql = @"select 
                        activity_name as title, 
                        activity_guid as `key`, 
                        form_guid as parent_guid
                    from general.activity 
                    where general.activity.form_guid IN (" + param + ") ";

            List<NodeKey> activity = (List<NodeKey>)QueryModule.DataSource<NodeKey>(sql);



            if (nodes.Count > 0)
            {
                int cnt = 0;
                int k = 0;
                foreach (var z in nodes)
                {
                    if (z.parent_guid != null)
                    {

                        int index = nodes.FindIndex(x => x.key == z.parent_guid);
                        if (index > -1)
                        {
                            if ((nodes[index].children == null))
                            {
                                nodes[index].children = new List<NodeKey>();
                            }

                            if ((z.children == null))
                            {
                                //z.Route = new List<ActivityType>();
                            }

                            z.children = activity.AsEnumerable().Where(r => r.parent_guid == z.key).ToList();


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
                            nodes[index].children.Add(z);
                            //nodes.RemoveAt(cnt);
                            removeIndexs.Add(cnt);


                        }
                    }
                    cnt++;
                }
            }

            for (int i = removeIndexs.Count - 1; i >= 0; i--)
            {
                nodes.RemoveAt(removeIndexs[i]);
            }

            int counter = 0;
            List<int> removeNoChild = new List<int>();
            for(int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].children == null)
                {
                    removeNoChild.Add(counter);
                }
                counter++;
            }

            for(int x = removeNoChild.Count - 1; x >=0; x--)
            {
                nodes.RemoveAt(removeNoChild[x]);
            }


            return nodes;
        }

        public RoleTypeModel GetByID(string ID)
        {
            string sql = "select * from general.roletype where general.roletype.roletype_guid = @roletype_guid";
            RoleTypeModel obj = (RoleTypeModel)QueryModule.DataObject<RoleTypeModel>(sql, new { roletype_guid = ID });

            sql = "select general.roletype_activity.activity_guid from general.roletype_activity where general.roletype_activity.roletype_guid  = @roletype_guid";
            obj.activity_guid = (List<string>)QueryModule.DataSource<string>(sql, new { roletype_guid = ID });
            return obj;
        }

        public RoleTypeModel GetByID2(string ID)
        {
            string sql = "select * from general.roletype where general.roletype.roletype_guid = @roletype_guid";
            RoleTypeModel obj = (RoleTypeModel)QueryModule.DataObject<RoleTypeModel>(sql, new { roletype_guid = ID });

            sql = "select general.roletype_activity.activity_guid from general.roletype_activity where general.roletype_activity.roletype_guid  = @roletype_guid";
            obj.activity_guid = (List<string>)QueryModule.DataSource<string>(sql, new { roletype_guid = ID });
            return obj;
        }

        public List<object> GetList()
        {
            string sql = "select * from general.roletype where general.roletype.status = 'Active'";
            List<object> list = (List<object>)QueryModule.DataSource<object>(sql);
            return list;
        }

        public bool Insert(RoleTypeModel model)
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
                    model.roletype_guid = Guid.NewGuid().ToString();
                    model.status = "Active";
                    string sql = "insert into general.roletype (" + ObjectSqlMapping.MapInsert<RoleTypeModel>() + ")";
                    int res = (int)QueryModule.Execute<int>(sql, model);

                    if(res > 0)
                    {
                        foreach(string i in model.activity_guid)
                        {
                            sql = "insert into general.roletype_activity (`roletype_guid`, `activity_guid`) VALUES (@roletype_guid, @activity_guid)";
                            QueryModule.Execute<int>(sql, new {roletype_guid = model.roletype_guid, activity_guid = i });
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
