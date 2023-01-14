using eegs_back_end.Admin.WaitlistedReportSetup.Model;
using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.WaitlistedReportSetup.Repository
{
    public interface IWaitlistedReportRepository :IGlobalInterface
    {
        bool Insert(WaitlistedReportModel model);
        List<object> GetListOfDisability();
        List<object> GetListOfApplication();
        List<object> GetListOfApplicationType();
        List<object> GetListOfRecommendations();
        List<object> GetEducationalType();
        List<object> GetList();
        List<object> GetAllList();
        List<object> GetListDeleted();
        List<object> GetListGenerated(int? filter_type_status_id = 0, int? status_id = 0, int? status_deleted_id = 0, string? this_month = "", string? this_year = "", string? monthly = "", string? monthlyYear = "", string? year_quarterly = "", int? quarter = 0, string? yearly = "", string? from = "", string? to = "");
        List<object> GetDetails(string GUID);
        object GetPersonAdd(string GUID);
        List<object> GetHistoryLogs(string GUID);
        bool Edit(string GUID, WaitlistedReportModel model);
        bool Delete(string GUID);
        object GetWaitlistedReport(string aics_letter_guid);
    }
    public class WaitlistedReportRepository : FormNumberGenerator, IWaitlistedReportRepository
    {
        public bool Edit(string GUID, WaitlistedReportModel model)
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
                    //model.WaitlistedReport_guid = GUID;
                    //model.status = "Active";

                    //var sql = "UPDATE mswd.WaitlistedReport SET status='Trash' " +
                    //    "WHERE mswd.WaitlistedReport.WaitlistedReport_guid = '" + GUID + "'   ";
                    //int res = (int)QueryModule.DataObject<int>(sql, model);

                    //sql = @"INSERT INTO mswd.WaitlistedReport (`WaitlistedReport_guid`, `person_guid`, `form_trans_no`, `status`, `fourps_beneficiary`, `ips`, `member_count`, `application_id`, `application_type_id`, `educational_attainment`, `house_ownership`, `monthly_family_income`, `occupation`, `recommendation_id`, `total_family_income`, `type_of_ethnicity`, `date_recommended`) 
                    //    VALUES(@WaitlistedReport_guid, @person_guid, @form_trans_no, @status, @fourps_beneficiary, @ips, @member_count, @application_id, @application_type_id, @educational_attainment, @house_ownership, @monthly_family_income, @occupation, @recommendation_id, @total_family_income, @type_of_ethnicity, @date_recommended)";
                    //QueryModule.Execute<int>(sql, model);

                    //sql = "DELETE FROM mswd.WaitlistedReport_details WHERE mswd.WaitlistedReport_details.main_guid = '" + GUID + "' ";
                    //QueryModule.Execute<int>(sql, model);

                    //sql = "DELETE FROM mswd.WaitlistedReport_casualties WHERE mswd.WaitlistedReport_casualties.main_guid = '" + GUID + "' ";
                    //QueryModule.Execute<int>(sql, model);

                    //sql = "DELETE FROM mswd.WaitlistedReport_logs WHERE mswd.WaitlistedReport_logs.main_guid = '" + GUID + "' ";
                    //QueryModule.Execute<int>(sql, model);

                    //sql = "INSERT INTO mswd.WaitlistedReport_logs (`main_guid`, `user_id`) VALUES('" + model.WaitlistedReport_guid + "', '" + GlobalObject.user_id + "')";
                    //QueryModule.Execute<int>(sql, model);

                    //if (model.family_details.Count > 0)
                    //{
                    //    foreach (var dt in model.family_details)
                    //    {
                    //        dt.main_guid = model.WaitlistedReport_guid;
                    //        sql = @"INSERT INTO mswd.WaitlistedReport_details (`main_guid`, `person_guid`, `relation`, `educational_attainment`, `occupational_skills`, `remarks`, `occupation_income`)
                    //            VALUES(@main_guid, @person_guid, @relation, @educational_attainment, @occupational_skills, @remarks, @occupation_income)";
                    //        QueryModule.Execute<int>(sql, dt);
                    //    }
                    //}

                    //if (model.casualties_details.Count > 0)
                    //{
                    //    foreach (var dt in model.casualties_details)
                    //    {
                    //        dt.main_guid = model.WaitlistedReport_guid;
                    //        sql = @"INSERT INTO mswd.WaitlistedReport_casualties (`main_guid`, `person_guid`)
                    //            VALUES(@main_guid, @person_guid)";
                    //        QueryModule.Execute<int>(sql, dt);
                    //    }
                    //}

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

        public bool Delete(string GUID)
        {
            string sql = "UPDATE mswd.WaitlistedReport SET status = 'Deleted' where mswd.WaitlistedReport.WaitlistedReport_guid = @WaitlistedReport_guid";
            if (InsertUpdate.ExecuteSql(sql, new { family_composition_guid = GUID }))
                return true;
            return false;
        }

        public List<object> GetListOfDisability()
        {
            string sql = "SELECT * FROM mswd.general_intake_disability";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public List<object> GetListOfApplication()
        {
            string sql = "SELECT * FROM mswd.WaitlistedReport_application";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public List<object> GetListOfApplicationType()
        {
            string sql = "SELECT * FROM mswd.WaitlistedReport_application_type";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public List<object> GetListOfRecommendations()
        {
            string sql = "SELECT * FROM mswd.WaitlistedReport_recommendations";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public List<object> GetEducationalType()
        {
            string sql = "SELECT * FROM general.educational_type";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public List<object> GetList()
        {
            string sql = @"SELECT wr.waitlisted_report_guid, wr.form_trans_no, wr.application_date, wr.`status`, wr.osca_intake_guid, wr.person_guid,
                            general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.waitlisted_report WHERE mswd.waitlisted_report.status = 'Active') as 'count'
                            FROM mswd.waitlisted_report as wr
                            LEFT JOIN general.person ON general.person.person_guid = wr.person_guid
                            WHERE wr.status = 'Active' ORDER BY wr.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("waitlisted-report");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.waitlisted_report wr
                    LEFT JOIN general.person ON general.person.person_guid = wr.person_guid
                    WHERE wr.status = 'Active' 

                    ) z";
            List<object> searches = (List<object>)QueryModule.DataSource<object>(sql);

            List<object> list = new List<object>();
            list.Add(obj);
            list.Add(form);
            list.Add(searches);

            return list;
        }

        public List<object> GetAllList()
        {
            string sql = @"SELECT wr.waitlisted_report_guid, wr.form_trans_no, wr.application_date, wr.`status`, wr.osca_intake_guid, wr.person_guid,
                            general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.waitlisted_report WHERE mswd.waitlisted_report.status = 'Active' OR mswd.waitlisted_report.status = 'Deleted') as 'count'
                            FROM mswd.waitlisted_report as wr
                            LEFT JOIN general.person ON general.person.person_guid = wr.person_guid
                            WHERE wr.status = 'Active' OR wr.status = 'Deleted' ORDER BY wr.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("waitlisted-report");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.waitlisted_report wr
                    LEFT JOIN general.person ON general.person.person_guid = wr.person_guid
                    WHERE wr.status = 'Active' 

                    ) z";
            List<object> searches = (List<object>)QueryModule.DataSource<object>(sql);

            List<object> list = new List<object>();
            list.Add(obj);
            list.Add(form);
            list.Add(searches);

            return list;
        }

        public List<object> GetListDeleted()
        {

            string sql = @"SELECT wr.waitlisted_report_guid, wr.form_trans_no, wr.application_date, wr.`status`, wr.osca_intake_guid, wr.person_guid,
                            general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.waitlisted_report WHERE mswd.waitlisted_report.status = 'Deleted') as 'count'
                            FROM mswd.waitlisted_report as wr
                            LEFT JOIN general.person ON general.person.person_guid = wr.person_guid
                            WHERE wr.status = 'Deleted' ORDER BY wr.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("waitlisted-report");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.waitlisted_report wr
                    LEFT JOIN general.person ON general.person.person_guid = wr.person_guid
                    WHERE wr.status = 'Deleted' 

                    ) z";
            List<object> searches = (List<object>)QueryModule.DataSource<object>(sql);

            List<object> list = new List<object>();
            list.Add(obj);
            list.Add(form);
            list.Add(searches);

            return list;
        }

        public List<object> GetListGenerated(int? filter_type_status_id = 0, int? status_id = 0, int? status_deleted_id = 0, string? this_month = "", string? this_year = "", string? monthly = "", string? monthlyYear = "", string? year_quarterly = "", int? quarter = 0, string? yearly = "", string? from = "", string? to = "")
        {
            string filterStatus = "";
            string dateFilter = "";
            if (status_id != 0)
            {
                filterStatus = "wr.status = 'Active'";
            }
            else if (status_deleted_id != 0)
            {
                filterStatus = "wr.status = 'Deleted'";
            }

            if (filter_type_status_id == 1)
            {
                dateFilter = "AND MONTH(wr.application_date) = '" + this_month + "'";
            }
            else if (filter_type_status_id == 2)
            {
                dateFilter = "AND YEAR(wr.application_date) = '" + this_year + "'";
            }
            else if (filter_type_status_id == 3)
            {
                dateFilter = "AND MONTH(wr.application_date) = '" + monthly + "' AND YEAR(wr.application_date) = '" + monthlyYear + "'";
            }
            else if (filter_type_status_id == 4)
            {
                dateFilter = "AND YEAR(wr.application_date) = '" + year_quarterly + "' AND QUARTER(wr.application_date) = '" + quarter + "'";
            }
            else if (filter_type_status_id == 5)
            {
                dateFilter = "AND YEAR(wr.application_date) = '" + yearly + "'";
            }
            else if (filter_type_status_id == 6)
            {
                dateFilter = "AND wr.application_date >= '" + from + "' AND wr.application_date <= '" + to + "'";
            }

            string sql = @"SELECT wr.waitlisted_report_guid, wr.form_trans_no, wr.application_date, wr.`status`, wr.osca_intake_guid, wr.person_guid,
                            general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.waitlisted_report as wr WHERE " + filterStatus + @" " + dateFilter  + @") as 'count'
                            FROM mswd.waitlisted_report as wr
                            LEFT JOIN general.person ON general.person.person_guid = wr.person_guid
                            WHERE " + filterStatus + @" " + dateFilter  + @" ORDER BY wr.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("waitlisted-report");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.waitlisted_report wr
                    LEFT JOIN general.person ON general.person.person_guid = wr.person_guid
                    WHERE wr.status = 'Active' 

                    ) z";
            List<object> searches = (List<object>)QueryModule.DataSource<object>(sql);

            List<object> list = new List<object>();
            list.Add(obj);
            list.Add(form);
            list.Add(searches);

            return list;
        }

        public List<object> GetDetails(string GUID)
        {
            string sql = @"SELECT mswd.WaitlistedReport_details.person_guid, mswd.WaitlistedReport_details.relation, mswd.WaitlistedReport_details.educational_attainment, mswd.WaitlistedReport_details.occupational_skills,
                            mswd.WaitlistedReport_details.remarks, general.person.prefix, general.person.suffix, general.person.first_name, general.person.middle_name, general.person.last_name,
                            general.person.gender_id, general.gender.gender_name, general.person.age
                            FROM mswd.WaitlistedReport_details
                            INNER JOIN general.person ON general.person.person_guid = mswd.WaitlistedReport_details.person_guid
                            INNER JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            WHERE mswd.WaitlistedReport_details.main_guid = '" + GUID + "' ";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public object GetWaitlistedReport(string waitlisited_report_guid)
        {
            string sql = @"SELECT wr.waitlisted_report_guid, wr.form_trans_no, wr.application_date, wr.`status`, wr.osca_intake_guid, wr.person_guid,
                        general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix,
                        mswd.osca_intake.application_date as 'osca_intake_date', general.person.barangay_id, general.person.street,
                        general_address.lgu_brgy_setup_temp.brgy_name
                        FROM mswd.waitlisted_report as wr
                        LEFT JOIN general.person ON general.person.person_guid = wr.person_guid
                        LEFT JOIN mswd.osca_intake ON mswd.osca_intake.osca_intake_guid = wr.osca_intake_guid
                        LEFT JOIN general_address.lgu_brgy_setup_temp ON general.person.barangay_id = general_address.lgu_brgy_setup_temp.brgy_id
                        WHERE wr.status = 'Active' AND wr.waitlisited_report_guid = @waitlisited_report_guid ";
            var obj = (WaitlistedReportModel)QueryModule.DataObject<WaitlistedReportModel>(sql, new { waitlisited_report_guid = waitlisited_report_guid });

            return obj;
        }

        public object GetPersonAdd(string GUID)
        {
            string sql = @"SELECT mswd.family_composition_head.person_guid, mswd.family_composition_head.family_composition_guid, mswd.family_composition_head.monthly_income, mswd.family_composition_head.fourps_member,
                            mswd.family_composition_head.ips, mswd.family_composition_head.house_occupancy, mswd.family_composition_head.educational_attainment,
                            mswd.family_composition_head.occupation, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix,
                            general.person.gender_id, general.gender.gender_name, general.person.civil_status_id, general.civil_status.civil_status_name,
                            general.person.birth_date, general.person.place_of_birth, general.person.province_id, general_address.lgu_province_setup_temp.province_name,
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name,
                            general.person.religion, general.person.age, general.person.person_image, general.person.street FROM mswd.family_composition_head 
                            INNER JOIN general.person ON general.person.person_guid = mswd.family_composition_head.person_guid
                            INNER JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            INNER JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            INNER JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            INNER JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            INNER JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            WHERE mswd.family_composition_head.status = 'Active' AND mswd.family_composition_head.person_guid = '" + GUID + "' ";
            var obj = (WaitlistedReportModel)QueryModule.DataObject<WaitlistedReportModel>(sql);

            if (obj == null)
                return null;

            //sql = "select mswd.family_composition_details.main_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix, " +
            //    "general.person.age, general.person.civil_status_id, general.civil_status.civil_status_name, mswd.family_composition_details.person_guid, mswd.family_composition_details.status, mswd.family_composition_details.educational_attainment, " +
            //    "mswd.family_composition_details.relation, mswd.family_composition_details.occupation, mswd.family_composition_details.occupation_income " +
            //    "from mswd.family_composition_details " +
            //    "INNER JOIN general.person ON general.person.person_guid = mswd.family_composition_details.person_guid " +
            //    "INNER JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id " +
            //    "where mswd.family_composition_details.main_guid = '" + obj.family_composition_guid + "' ";
            //obj.details = (List<WaitlistedReportDetails>)QueryModule.DataSource<WaitlistedReportDetails>(sql);

            return obj;
        }

        public List<object> GetHistoryLogs(string GUID)
        {
            string sql = @"SELECT mswd.WaitlistedReport_logs.main_guid, mswd.WaitlistedReport.application_date, mswd.WaitlistedReport_logs.user_id, mswd.WaitlistedReport.form_trans_no,
                            mswd.WaitlistedReport.status, mswd.WaitlistedReport.person_guid, general.person.prefix, general.person.suffix, general.person.first_name,
                            general.person.middle_name, general.person.last_name, mswd.WaitlistedReport.ips, mswd.WaitlistedReport.educational_attainment, mswd.WaitlistedReport.total_family_income
                            FROM mswd.WaitlistedReport_logs  
                            INNER JOIN mswd.WaitlistedReport ON mswd.WaitlistedReport.WaitlistedReport_guid = mswd.WaitlistedReport_logs.main_guid  
                            INNER JOIN general.person ON general.person.person_guid = mswd.WaitlistedReport.person_guid
                            WHERE mswd.WaitlistedReport_logs.main_guid = '" + GUID + "' ORDER BY mswd.WaitlistedReport.id DESC";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public bool Insert(WaitlistedReportModel model)
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
                    model.form_trans_no = generateFormNumber("waitlisted-report");
                    model.waitlisted_report_guid = Guid.NewGuid().ToString();
                    model.status = "Active";

                    string sql = @"INSERT INTO mswd.waitlisted_report (`waitlisted_report_guid`, `person_guid`, `form_trans_no`, `status`, `osca_intake_guid`) 
                        VALUES(@waitlisted_report_guid, @person_guid, @form_trans_no, @status, @osca_intake_guid)";
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
