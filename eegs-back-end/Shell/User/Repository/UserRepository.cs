using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using eegs_back_end.Shell.RoleType.Model;
using eegs_back_end.Shell.User.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Shell.User.Repository
{
    public interface IUserRepository: IGlobalInterface
    {
        public object GetRoleTypePerDomain(string domain_guid);
        bool Insert(UserModel model);
        bool Edit(string GUID, UserModel model);
        object GetList();
        UserModel GetByID(string GUID);
        bool Delete(string GUID);

    }
    public class UserRepository : IUserRepository
    {


        public object GetRoleTypePerDomain(string domain_guid)
        {
            string sql = @"                    
                            SELECT
	                            *
                            FROM
	                            general.roletype
                            
                       ";
            var RoleType = (object)QueryModule.DataSource<object>(sql, new { domain_guid = domain_guid });
            return RoleType;

        }

        public object GetList()
        {
            string sql = @"select 
                        users.user_guid,
                        UserName_User,
                        UserFull_Name,
                        DATE_FORMAT(savedate, '%m/%d/%Y') as `datesave`,
                        status
                         
                        from general.users
                           inner join general.user_roletype on general.user_roletype.user_guid = general.users.user_guid
                        where general.users.status = 'Active'
                        group by UserName_User, UserFull_Name, savedate, user_guid
                        order by general.users.id DESC
                        ";
            List<UserModel> obj = (List<UserModel>)QueryModule.DataSource<UserModel>(sql);

            string user_guids = ObjectSqlMapping.SetParam(obj.Select(x => x.user_guid).ToArray());

            sql = @"select general.user_roletype.* from general.user_roletype where general.user_roletype.user_guid in ("+ user_guids+")";
            List<UserRoleType> roletype_guids  = (List<UserRoleType>)QueryModule.DataSource<UserRoleType>(sql);
 
            foreach (UserModel x in obj)
            {
                x.roletype_guid = roletype_guids.Where(c => c.user_guid == x.user_guid).Select(z => z.roletype_guid).ToList();
            }

            return obj;
        }

        public bool Insert(UserModel model)
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
                    model.user_guid = Guid.NewGuid().ToString();
                    model.status = "Active";
                    model.Password_User = Cryptography.EncryptString(GlobalObject.key, model.Password_User);
                    //string sql = "insert into general.users (" + ObjectSqlMapping.MapInsert<UserModel>() + ")";
                    string sql = "insert into general.users (`domain_guid`, `UserFull_Name`, `user_guid`, `email_address`, `person_guid`, `employee_guid`, `UserName_User`, `Password_User`, `status`) " +
                            "VALUES(@domain_guid, @UserFull_Name, @user_guid, @email_address, @person_guid, @employee_guid, @UserName_User, @Password_User, @status)";
                    int res = (int)QueryModule.Execute<int>(sql, model);

                    for(int i = 0; i < model.roletype_guid.Count; i++)
                    {
                        sql = "insert into general.user_roletype (`user_guid`, `roletype_guid`) VALUES (@user_guid, @roletype_guid)";
                        QueryModule.Execute<int>(sql, new {user_guid = model.user_guid, roletype_guid = model.roletype_guid[i]});
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

        public bool Edit(string GUID, UserModel model)
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
                    model.user_guid = model.user_guid;
                    model.UserName_User = model.UserName_User;
                    model.Password_User = Cryptography.EncryptString(GlobalObject.key, model.Password_User);
                    var sql = "UPDATE general.users SET domain_guid = @domain_guid, UserFull_Name = @UserFull_Name, user_guid = @user_guid, email_address = @email_address, person_guid = @person_guid, employee_guid = @employee_guid, UserName_User = @UserName_User, Password_User = @Password_User, status = @status WHERE general.users.user_guid = '"+model.user_guid+"'";
                    int res = (int)QueryModule.Execute<int>(sql, model);


                    sql = "DELETE FROM general.user_roletype WHERE general.user_roletype.user_guid = '" + model.user_guid + "'";
                    QueryModule.Execute<int>(sql, model);


                    if (res > 0)
                    {
                        foreach (string i in model.roletype_guid)
                        {
                            sql = "insert into general.user_roletype (`user_guid`, `roletype_guid`) VALUES (@user_guid, @roletype_guid)";
                            QueryModule.Execute<int>(sql, new { user_guid = model.user_guid, roletype_guid = i });
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

        public UserModel GetByID(string GUID)
        {

            string sql = @"select general.users.*, general.person.full_name as userFull_Name from general.users
                            left join general.person on general.person.person_guid = general.users.person_guid
                            where general.users.user_guid = @user_guid";
            UserModel obj = (UserModel)QueryModule.DataObject<UserModel>(sql, new {user_guid = GUID } );

            sql = @"select general.user_roletype.roletype_guid from general.user_roletype where general.user_roletype.user_guid = @user_guid";
            obj.roletype_guid = (List<string>)QueryModule.DataSource<string>(sql, new {user_guid = obj.user_guid });
            obj.Password_User = Cryptography.DecryptString(GlobalObject.key, obj.Password_User);
            return obj;
        }

        public bool Delete(string GUID)
        {
            string sql = "UPDATE general.users SET status = 'Deleted' where general.users.user_guid = @user_guid";
            if ((int)QueryModule.Execute<int>(sql, new { user_guid = GUID }) == 1)
                return true;
            return false;
        }
    }
}
