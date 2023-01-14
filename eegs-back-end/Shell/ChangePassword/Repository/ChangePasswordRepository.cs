using eegs_back_end.DbModule;
using eegs_back_end.Helpers;
using eegs_back_end.Shell.Domain.Model;
using eegs_back_end.Shell.Form.Model;
using eegs_back_end.Shell.ChangePassword.Model;
using eegs_back_end.Shell.Project.Model;
using eegs_back_end.Shell.RoleType.Model;
using eegs_back_end.Shell.User.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eegs_back_end.GlobalHandler.Error.ErrorException;

namespace eegs_back_end.Shell.ChangePassword.Repository
{
    public interface IChangePasswordRepository : IGlobalInterface
    {
        bool Edit(string GUID, ChangePasswordModel model);

    }
    public class ChangePasswordRepository : FormNumberGenerator, IChangePasswordRepository
    {
        public bool Edit(string GUID, ChangePasswordModel model)
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
                    model.user_guid = GUID;
                    model.confirm = Cryptography.EncryptString(GlobalObject.key, model.confirm);

                    var sql = @"UPDATE general.users SET Password_User='" + model.confirm + "' " +
                        "WHERE general.users.user_guid = '" + GUID + "'   ";
                    int res = (int)QueryModule.DataObject<int>(sql, model);

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
