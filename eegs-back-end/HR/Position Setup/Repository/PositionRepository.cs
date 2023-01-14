using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using eegs_back_end.HR.Position_Setup.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.HR.Position_Setup.Repository
{
    public interface IPositionRepository: IGlobalInterface
    {
        List<object> GetList();
        PositionModel GetByID(string ID);
        bool Insert(PositionModel model);
        bool Edit(string ID, PositionModel model);
        bool Delete(string ID);
        List<object> GetDropDown();

    }
    public class PositionRepository : IPositionRepository
    {
        public bool Delete(string ID)
        {
            throw new NotImplementedException();
        }

        public bool Edit(string ID, PositionModel model)
        {
            model.position_id = ID;
            string sql = "update humanresource.position SET " + ObjectSqlMapping.MapUpdate<PositionModel>() + " WHERE position_id = '" + ID + "'";
            QueryModule.Execute<int>(sql, model);

            return true;
        }

        public PositionModel GetByID(string ID)
        {
            string sql = "select * from humanresource.position where position_id = '" + ID + "'";
            PositionModel obj = (PositionModel)QueryModule.DataObject<PositionModel>(sql);

            return obj;
        }

        public List<object> GetDropDown()
        {
            string sql = "select * from humanresource.department";
            List<object> dept = (List<object>)QueryModule.DataSource<object>(sql);

            sql = "select * from humanresource.position_type";
            List<object> positionType = (List<object>)QueryModule.DataSource<object>(sql);

            sql = "select * from humanresource.salary_grade_tbl";
            List<object> salary_grade = (List<object>)QueryModule.DataSource<object>(sql);

            List<object> list = new List<object>();
            list.Add(new { department = dept, posType = positionType, salaryGrade = salary_grade });
            return list;
        }

        public List<object> GetList()
        {
            string sql = @"select * from humanresource.position pos
                           inner join humanresource.department dept on dept.dept_id = pos.dept_id 
                           inner join humanresource.position_type postype on postype.position_type_id = pos.position_type_id";
            List<object> list = (List<object>)QueryModule.DataSource<object>(sql);


            return list;
        }

        public bool Insert(PositionModel model)
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
                    model.position_id = Guid.NewGuid().ToString();
                    model.status = "Active";
                    string sql = "insert into humanresource.position (" + ObjectSqlMapping.MapInsert<PositionModel>() + ")";
                    int res = (int)QueryModule.Execute<int>(sql, model);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, ex.Message);
                }
            }
            return true;
        }
    }
}
