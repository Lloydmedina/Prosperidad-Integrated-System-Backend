using eegs_back_end.Admin.FinancialAssistanceSetup.Model;
using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.FinancialAssistanceSetup.Repository
{
    public interface IFinancialAssistanceRepository :IGlobalInterface
    {
        bool Insert(FinancialAssistanceModel model);
        List<object> GetListOfDisability();
        List<object> GetListOfApplication();
        List<object> GetListOfApplicationType();
        List<object> GetListOfRecommendations();
        List<object> GetEducationalType();
        List<object> getFormSettings();
        List<object> GetList(int? filter_type_status_id = 0, int? rec_id = 0, string? this_month = "", string? this_year = "", string? monthly = "", string? monthlyYear = "", string? year_quarterly = "", int? quarter = 0, string? yearly = "", string? from = "", string? to = "");
        List<object> GetAllList();
        List<object> GetDetails(string GUID);
        object GetPersonAdd(string GUID);
        List<object> GetHistoryLogs(string GUID);
        bool Edit(string GUID, FinancialAssistanceModel model);
        bool Delete(string GUID);
        object GetFinancialAssistance(string FinancialAssistance_guid);
    }
    public class FinancialAssistanceRepository : FormNumberGenerator, IFinancialAssistanceRepository
    {
        public bool Edit(string GUID, FinancialAssistanceModel model)
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
                    //model.FinancialAssistance_guid = GUID;
                    //model.status = "Active";

                    //var sql = "UPDATE mswd.FinancialAssistance SET status='Trash' " +
                    //    "WHERE mswd.FinancialAssistance.FinancialAssistance_guid = '" + GUID + "'   ";
                    //int res = (int)QueryModule.DataObject<int>(sql, model);

                    //sql = @"INSERT INTO mswd.FinancialAssistance (`FinancialAssistance_guid`, `person_guid`, `form_trans_no`, `status`, `fourps_beneficiary`, `ips`, `member_count`, `application_id`, `application_type_id`, `educational_attainment`, `house_ownership`, `monthly_family_income`, `occupation`, `recommendation_id`, `total_family_income`, `type_of_ethnicity`, `date_recommended`) 
                    //    VALUES(@FinancialAssistance_guid, @person_guid, @form_trans_no, @status, @fourps_beneficiary, @ips, @member_count, @application_id, @application_type_id, @educational_attainment, @house_ownership, @monthly_family_income, @occupation, @recommendation_id, @total_family_income, @type_of_ethnicity, @date_recommended)";
                    //QueryModule.Execute<int>(sql, model);

                    //sql = "DELETE FROM mswd.FinancialAssistance_details WHERE mswd.FinancialAssistance_details.main_guid = '" + GUID + "' ";
                    //QueryModule.Execute<int>(sql, model);

                    //sql = "DELETE FROM mswd.FinancialAssistance_casualties WHERE mswd.FinancialAssistance_casualties.main_guid = '" + GUID + "' ";
                    //QueryModule.Execute<int>(sql, model);

                    //sql = "DELETE FROM mswd.FinancialAssistance_logs WHERE mswd.FinancialAssistance_logs.main_guid = '" + GUID + "' ";
                    //QueryModule.Execute<int>(sql, model);

                    //sql = "INSERT INTO mswd.FinancialAssistance_logs (`main_guid`, `user_id`) VALUES('" + model.FinancialAssistance_guid + "', '" + GlobalObject.user_id + "')";
                    //QueryModule.Execute<int>(sql, model);

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
            string sql = "UPDATE mswd.FinancialAssistance SET status = 'Deleted' where mswd.FinancialAssistance.FinancialAssistance_guid = @FinancialAssistance_guid";
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
            string sql = "SELECT * FROM mswd.FinancialAssistance_application";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public List<object> GetListOfApplicationType()
        {
            string sql = "SELECT * FROM mswd.FinancialAssistance_application_type";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public List<object> GetListOfRecommendations()
        {
            string sql = "SELECT * FROM mswd.FinancialAssistance_recommendations";
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

        public List<object> GetList(int? filter_type_status_id = 0, int? rec_id = 0, string? this_month = "", string? this_year = "", string? monthly = "", string? monthlyYear = "", string? year_quarterly = "", int? quarter = 0, string? yearly = "", string? from = "", string? to = "")
        {
            string dateFilter = "";

            if (filter_type_status_id == 1)
            {
                dateFilter = "AND MONTH(aics_intake.application_date) = '" + this_month + "'";
            }
            else if (filter_type_status_id == 2)
            {
                dateFilter = "AND YEAR(aics_intake.application_date) = '" + this_year + "'";
            }
            else if (filter_type_status_id == 3)
            {
                dateFilter = "AND MONTH(aics_intake.application_date) = '" + monthly + "'";
            }
            else if (filter_type_status_id == 4)
            {
                dateFilter = "AND YEAR(aics_intake.application_date) = '" + year_quarterly + "' AND QUARTER(general.person.application_date) = '" + quarter + "'";
            }
            else if (filter_type_status_id == 5)
            {
                dateFilter = "AND YEAR(aics_intake.application_date) = '" + yearly + "'";
            }
            else if (filter_type_status_id == 6)
            {
                dateFilter = "AND aics_intake.application_date >= '" + from + "' AND aics_intake.application_date <= '" + to + "'";
            }

            string sql = @"SELECT aics_intake.application_date, aics_intake.`status`, aics_intake.aics_intake_guid, aics_intake.person_guid,
                            general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix, general.person.full_name,
                            general_address.lgu_province_setup_temp.province_name, general_address.lgu_city_mun_setup_temp.city_mun_name, general_address.lgu_brgy_setup_temp.brgy_name,
                            (
	                            SELECT general.person.full_name 
	                            FROM mswd.aics_intake_logs
	                            LEFT JOIN general.users ON general.users.user_guid = mswd.aics_intake_logs.user_id
	                            LEFT JOIN general.person ON general.person.person_guid = general.users.person_guid
	                            WHERE mswd.aics_intake_logs.main_guid = aics_intake.aics_intake_guid 
                            ) as 'released_by',
                            DATEDIFF(CURRENT_DATE(),aics_intake.application_date ) as 'count_days',
                            (SELECT COUNT(*) FROM mswd.aics_intake as aics_intake WHERE aics_intake.status = 'Active' AND mswd.aics.status = 'Active' AND mswd.aics.recommendation_id = " + rec_id + @" " + dateFilter + @") as 'count'
                            FROM mswd.aics_intake as aics_intake
                            LEFT JOIN general.person ON general.person.person_guid = aics_intake.person_guid
                            LEFT JOIN mswd.aics ON mswd.aics.person_guid = aics_intake.person_guid
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id  
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id  
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id 
                            WHERE aics_intake.status = 'Active' AND mswd.aics.status = 'Active' AND mswd.aics.recommendation_id = " + rec_id + @" " + dateFilter + @"  ORDER BY aics_intake.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("financial-assistance");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.aics_intake aics_intake
                    LEFT JOIN general.person ON general.person.person_guid = aics_intake.person_guid
                    LEFT JOIN mswd.aics ON mswd.aics.person_guid = aics_intake.person_guid
                    WHERE aics_intake.status = 'Active' AND mswd.aics.status = 'Active' AND mswd.aics.recommendation_id = "+ rec_id + @"

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.aics_intake aics_intake
                    LEFT JOIN general.person ON general.person.person_guid = aics_intake.person_guid
                    LEFT JOIN mswd.aics ON mswd.aics.person_guid = aics_intake.person_guid
                    LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                    WHERE aics_intake.status = 'Active' AND mswd.aics.status = 'Active' AND mswd.aics.recommendation_id = " + rec_id + @"
                    ) z";
            List<object> searches = (List<object>)QueryModule.DataSource<object>(sql);

            List<object> list = new List<object>();
            list.Add(obj);
            list.Add(form);
            list.Add(searches);

            return list;
        }

        public List<object> getFormSettings()
        {
            ExpandoObject form = Forms.getForm("financial-assistance");

            List<object> list = new List<object>();
            list.Add(form);

            return list;
        }

        public List<object> GetAllList()
        {
            string sql = @"SELECT av.aics_voucher_guid, av.form_trans_no, av.application_date, av.`status`, av.aics_intake_guid, av.person_guid,
                            general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix, general.person.full_name,
                            general_address.lgu_province_setup_temp.province_name, general_address.lgu_city_mun_setup_temp.city_mun_name, general_address.lgu_brgy_setup_temp.brgy_name
                            FROM mswd.aics_voucher as av
                            LEFT JOIN general.person ON general.person.person_guid = av.person_guid
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id  
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id  
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id 
                            WHERE av.status = 'Active' OR av.status = 'Deleted' ORDER BY av.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("aics-voucher");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.aics_voucher aics_voucher
                    LEFT JOIN general.person ON general.person.person_guid = aics_voucher.person_guid
                    WHERE aics_voucher.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.aics_voucher aics_voucher
                    LEFT JOIN general.person ON general.person.person_guid = aics_voucher.person_guid
                    LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
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
            string sql = @"SELECT mswd.FinancialAssistance_details.person_guid, mswd.FinancialAssistance_details.relation, mswd.FinancialAssistance_details.educational_attainment, mswd.FinancialAssistance_details.occupational_skills,
                            mswd.FinancialAssistance_details.remarks, general.person.prefix, general.person.suffix, general.person.first_name, general.person.middle_name, general.person.last_name,
                            general.person.gender_id, general.gender.gender_name, general.person.age
                            FROM mswd.FinancialAssistance_details
                            INNER JOIN general.person ON general.person.person_guid = mswd.FinancialAssistance_details.person_guid
                            INNER JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            WHERE mswd.FinancialAssistance_details.main_guid = '" + GUID + "' ";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public object GetFinancialAssistance(string aics_voucher_guid)
        {
            string sql = @"SELECT av.aics_voucher_guid, av.form_trans_no, av.application_date, av.`status`, av.aics_intake_guid, av.person_guid,
                            general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix,
                            mswd.aics_intake.application_date as 'aics_intake_date', general.person.barangay_id, general.person.street,
                            general_address.lgu_brgy_setup_temp.brgy_name
                            FROM mswd.aics_voucher as av
                            INNER JOIN general.person ON general.person.person_guid = av.person_guid
                            INNER JOIN mswd.aics_intake ON mswd.aics_intake.aics_intake_guid = av.aics_intake_guid
                            INNER JOIN general_address.lgu_brgy_setup_temp ON general.person.barangay_id = general_address.lgu_brgy_setup_temp.brgy_id
                            WHERE av.status = 'Active' AND av.aics_voucher_guid = @aics_voucher_guid";
            var obj = (FinancialAssistanceModel)QueryModule.DataObject<FinancialAssistanceModel>(sql, new { aics_voucher_guid = aics_voucher_guid });

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
            var obj = (FinancialAssistanceModel)QueryModule.DataObject<FinancialAssistanceModel>(sql);

            if (obj == null)
                return null;

            //sql = "select mswd.family_composition_details.main_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix, " +
            //    "general.person.age, general.person.civil_status_id, general.civil_status.civil_status_name, mswd.family_composition_details.person_guid, mswd.family_composition_details.status, mswd.family_composition_details.educational_attainment, " +
            //    "mswd.family_composition_details.relation, mswd.family_composition_details.occupation, mswd.family_composition_details.occupation_income " +
            //    "from mswd.family_composition_details " +
            //    "INNER JOIN general.person ON general.person.person_guid = mswd.family_composition_details.person_guid " +
            //    "INNER JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id " +
            //    "where mswd.family_composition_details.main_guid = '" + obj.family_composition_guid + "' ";
            //obj.details = (List<FinancialAssistanceDetails>)QueryModule.DataSource<FinancialAssistanceDetails>(sql);

            return obj;
        }

        public List<object> GetHistoryLogs(string GUID)
        {
            string sql = @"SELECT mswd.FinancialAssistance_logs.main_guid, mswd.FinancialAssistance.application_date, mswd.FinancialAssistance_logs.user_id, mswd.FinancialAssistance.form_trans_no,
                            mswd.FinancialAssistance.status, mswd.FinancialAssistance.person_guid, general.person.prefix, general.person.suffix, general.person.first_name,
                            general.person.middle_name, general.person.last_name, mswd.FinancialAssistance.ips, mswd.FinancialAssistance.educational_attainment, mswd.FinancialAssistance.total_family_income
                            FROM mswd.FinancialAssistance_logs  
                            INNER JOIN mswd.FinancialAssistance ON mswd.FinancialAssistance.FinancialAssistance_guid = mswd.FinancialAssistance_logs.main_guid  
                            INNER JOIN general.person ON general.person.person_guid = mswd.FinancialAssistance.person_guid
                            WHERE mswd.FinancialAssistance_logs.main_guid = '" + GUID + "' ORDER BY mswd.FinancialAssistance.id DESC";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public bool Insert(FinancialAssistanceModel model)
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
                    model.form_trans_no = generateFormNumber("aics-voucher");
                    model.aics_voucher_guid = Guid.NewGuid().ToString();
                    model.status = "Active";

                    string sql = @"INSERT INTO mswd.aics_voucher (`aics_voucher_guid`, `person_guid`, `form_trans_no`, `status`, `aics_intake_guid`) 
                        VALUES(@aics_voucher_guid, @person_guid, @form_trans_no, @status, @aics_intake_guid)";
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
