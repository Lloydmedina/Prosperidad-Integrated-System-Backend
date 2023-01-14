using eegs_back_end.Accounting.AccountTitle.Model;
using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Accounting.AccountTitle.Repository
{
    public interface IAccountRepository: IGlobalInterface
    {
        List<object> GetList();
        AccountModel GetByID(string ID);
        bool Insert(AccountModel model);
        bool Edit(string ID, AccountModel model);
        bool Delete(string ID);
        List<object> GetDropDown();
        object GenerateAccountCode(int parent_id);
    }
    public class AccountRepository : IAccountRepository
    {

        public bool Delete(string ID)
        {
            string sql = "update accounting.account_title SET status = 'Deleted' WHERE account_id = " + ID;
            int i = (int)QueryModule.Execute<int>(sql, new { account_id = ID });
            if (i == 0) return false;

            return true;
        }

        public bool Edit(string ID, AccountModel model)
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
                    model.status = "Active";
                    string sql = "update accounting.account_title SET " + ObjectSqlMapping.MapUpdate<AccountModel>() + " WHERE account_id = " + ID;
                    QueryModule.Execute<int>(sql, model);

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

        public AccountModel GetByID(string ID)
        {
            string sql = @"select * from accounting.account_title acc where acc.account_id = @account_id ";
            AccountModel model = (AccountModel)QueryModule.DataObject<AccountModel>(sql, new { account_id = ID });


            return model;
        }


        public List<object> GetList()
        {

            string sql = @"select acc.*, parent.account_name as parent from accounting.account_title acc
                        left join accounting.account_title parent on parent.account_id = acc.parent_id
                        order by acc.account_id asc
                        ";
            List<object> data = (List<object>)QueryModule.DataSource<object>(sql);


            ExpandoObject form = Forms.getForm("account-title");

            List<object> list = new List<object>();
            list.Add(data);
            list.Add(form);
            return list;
        }

        public List<object> GetDropDown()
        {
            string sql = @"SELECT account_id, account_name FROM accounting.`account_title`
            where length(account_code) <= 5;";
            List<object> parent = (List<object>)QueryModule.DataSource<object>(sql);


            sql = "select * from accounting.account_type";
            List<object> acc_type = (List<object>)QueryModule.DataSource<object>(sql);

            sql = "select * from accounting.account_activity";
            List<object> activity = (List<object>)QueryModule.DataSource<object>(sql);

            List<object> list = new List<object>();

            list.Add(new {parent= parent, acc_type = acc_type, activity = activity });

            return list;
        }

        public bool Insert(AccountModel model)
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
                    model.status = "Active";
                    string sql = "insert into accounting.account_title (" + ObjectSqlMapping.MapInsert<AccountModel>() + ")";
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

        public object GenerateAccountCode(int parent_id)
        {
            string new_acc_code = "";
            string sql = @"SELECT z.*, parent.account_code as parent_acc_code FROM accounting.`account_title` z
            left join (select * from accounting.account_title where account_id = " + parent_id  + @") parent
            on parent.account_id = z.parent_id
            where z.parent_id = " + parent_id + @"
            order by LENGTH(z.account_code) DESC, z.account_id DESC";

            dynamic obj = (dynamic)QueryModule.DataObject<dynamic>(sql);

            if(obj == null)
            {
                sql = @"select * from accounting.account_title where account_id = " + parent_id;
                obj = (dynamic)QueryModule.DataObject<dynamic>(sql);

                new_acc_code = obj.account_code + "10";
            }
            else
            {
                int incre = 1;
                string? parentLength = obj.parent_acc_code;
                if (parentLength == null) return new { new_code = "" };
                int subStr = 1;
                string latestCode = obj.account_code;
                if (parentLength.Length >= 3)
                {
                    subStr = 0;
                    incre = 10;

                    if (parentLength.Length == 3)
                    {
                        incre = 1;
                    }
                }
                else
                {
                    if (parentLength.Length <= 2)
                    {
                        parentLength = parentLength.Substring(1, 1);
                    }
                }
                string trimmed = latestCode.Substring((parentLength.Length), (latestCode.Length - (parentLength.Length)));
                if (trimmed.Length == 0) return new { new_code = obj.parent_acc_code };
                string padded = "D" + trimmed.Length;
                string newCode = Convert.ToInt32(Convert.ToInt32(trimmed) + incre).ToString(padded);
                new_acc_code = obj.parent_acc_code + newCode;
                new_acc_code = new_acc_code.Substring(subStr, new_acc_code.Length - subStr);
            }

            return new {new_code = new_acc_code };
        }
    }
}
