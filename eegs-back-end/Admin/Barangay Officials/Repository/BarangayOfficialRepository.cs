using eegs_back_end.Admin.Barangay_Officials.Model;
using eegs_back_end.Admin.Department.Model;
using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.Barangay_Officials.Repository
{
    public interface IBarangayOfficialRepository: IGlobalInterface
    {
        List<object> GetList();
        BarangayOfficialModel GetByID(string ID);
        bool Insert(BarangayOfficialModel model);
        bool Edit(string ID, BarangayOfficialModel model);
        bool Delete(string ID);
        List<object> GetEmployees();
    }
    public class BarangayOfficialRepository : IBarangayOfficialRepository
    {
        public bool Delete(string ID)
        {
            string sql = "update humanresource.brgy_officials SET status = 'Deleted' where brgy_official_id = @brgy_official_id";
            int i = (int)QueryModule.Execute<int>(sql, new { brgy_official_id = ID });
            if (i == 0) return false;

            return true;
        }

        public bool Edit(string ID, BarangayOfficialModel model)
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
                    model.brgy_official_id = ID;
                    model.status = "Inactive";

                    string sql = "update humanresource.brgy_officials SET end_time = CURRENT_TIMESTAMP where brgy_official_id = @brgy_official_id and end_time is null";
                    QueryModule.Execute<int>(sql, model);

                    model.brgy_official_id = ID;
                    model.status = "Active";
                    sql = "insert into humanresource.brgy_officials (" + ObjectSqlMapping.MapInsert<BarangayOfficialModel>() + ")";
                    int res = (int)QueryModule.Execute<int>(sql, model);

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

        public BarangayOfficialModel GetByID(string ID)
        {
            string sql = @"select * from humanresource.brgy_officials brgy where brgy.brgy_official_id = @brgy_official_id AND brgy.end_time is null";
            BarangayOfficialModel model = (BarangayOfficialModel)QueryModule.DataObject<BarangayOfficialModel>(sql, new { brgy_official_id = ID });


            return model;
        }

        public List<object> GetEmployees()
        {
            string sql = @"select employee_id, employee_name from humanresource.employees";
            List<object> list = (List<object>)QueryModule.DataSource<object>(sql);

            return list;
        }

        public List<object> GetList()
        {
            string sql = @"
                            SELECT b.brgy_official_id,  b.official_id, brgy.brgy_code1, brgy.brgy_name, b.start_time as savedate, b.`status`, official.employee_name as official FROM humanresource.brgy_officials b
                            inner join general_address.lgu_brgy_setup_temp brgy on brgy.brgy_id = b.brgy_id
                            left join humanresource.employees official on official.employee_id = b.official_id
                            where b.end_time is null and b.status = 'Active'
                            ORDER BY brgy.brgy_name asc
                        ";
            List<object> data = (List<object>)QueryModule.DataSource<object>(sql);


            ExpandoObject form = Forms.getForm("barangay-official");

            List<object> list = new List<object>();
            list.Add(data);
            list.Add(form);
            return list;
        }

        public bool Insert(BarangayOfficialModel model)
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
                    model.brgy_official_id = Guid.NewGuid().ToString();
                    model.status = "Active";

                    string sql = "insert into humanresource.brgy_officials (" + ObjectSqlMapping.MapInsert<BarangayOfficialModel>() + ")";
                    int res = (int)QueryModule.Execute<int>(sql, model);

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
