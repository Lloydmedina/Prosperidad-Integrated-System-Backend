using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using eegs_back_end.Shell.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Shell.Domain.Repository
{
    public interface IDomainRepository : IGlobalInterface
    {
        DomainModel GetByID(string ID);
        List<object> GetList();
        
        
        bool Insert(DomainModel model);
        bool Edit(string GUID, DomainModel model);
        bool Delete(string GUID);
        List<object> GetProject();  

    }
    public class DomainRepository : FormNumberGenerator, IDomainRepository
    {
        public bool Delete(string GUID)
        {
            string sql = "UPDATE general.domain SET status = 'Deleted' where general.domain.domain_guid = @domain_guid";
            if (InsertUpdate.ExecuteSql(sql, new {domain_guid = GUID }))
                return true;
            return false;
        }

        public bool Edit(string GUID, DomainModel model)
        {
            model.domain_guid = GUID;
            model.status = "Active";
            string sql = @"UPDATE general.domain SET domain_name = @domain_name, description = @description, route_reference = @route_reference WHERE general.domain.domain_guid = @domain_guid";

            if (InsertUpdate.ExecuteSql(sql, model)) 
                return true;

            return false;
        }

        public DomainModel GetByID(string ID)
        { 
            string sql = "select * from general.domain where general.domain.domain_guid = @id";
            DomainModel domain = (DomainModel)QueryModule.DataObject<DomainModel>(sql, new { id = ID });

            return domain;
        }

        public List<object> GetList()
        {
            string sql = @"SELECT * From general.domain where general.domain.status = 'Active'";

            var Domain = (List<object>)QueryModule.DataSource<object>(sql);

            if (Domain == null)
                return null;
            return Domain;
        }

        public List<object> GetProject()
        {
            string sql = "select general.project_title.title_name, general.project_title.project_title_guid from general.project_title  ";
            List<object> list = (List<object>)QueryModule.DataSource<object>(sql);
            return list;
        }

        public bool Insert(DomainModel model)
        {
            model.form_trans_no = generateFormNumber("domain");



            model.domain_guid = Guid.NewGuid().ToString();
            model.status = "Active";
            string sql = @"INSERT INTO general.domain (`domain_name`, `description`, `domain_guid`, `project_title_guid`, `transaction_date`, `status`, `route_reference`)" +
                "VALUES (@domain_name, @description, @domain_guid, @project_title_guid, @transaction_date, @status, @route_reference)";
            if (InsertUpdate.ExecuteSql(sql, model))
                return true;

            return false;

        }
    }
}
