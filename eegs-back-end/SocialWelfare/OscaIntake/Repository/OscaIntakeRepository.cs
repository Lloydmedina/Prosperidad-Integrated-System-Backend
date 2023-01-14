using eegs_back_end.Admin.OscaIntakeSetup.Model;
using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.OscaIntakeSetup.Repository
{
    public interface IOscaIntakeRepository :IGlobalInterface
    {
        bool Insert(OscaIntakeModel model);
        List<object> GetListOfDisability();
        List<object> GetList();
        List<object> GetAllList();

        List<object> GetListDeleted();
        List<object> GetListGenerated(int? filter_type_status_id = 0, int? status_id = 0, int? status_deleted_id = 0, string? this_month = "", string? this_year = "", string? monthly = "", string? monthlyYear = "", string? year_quarterly = "", int? quarter = 0, string? yearly = "", string? from = "", string? to = "");
        List<object> GetDetails(string GUID);
        object GetPersonAdd(string GUID);
        List<object> GetHistoryLogs(string GUID);
        bool Edit(string GUID, OscaIntakeModel model);
        bool Delete(string GUID);
        object GetOscaIntake(string OscaIntake_guid);
    }
    public class OscaIntakeRepository : FormNumberGenerator, IOscaIntakeRepository
    {
        public bool Edit(string GUID, OscaIntakeModel model)
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
                    model.osca_intake_guid = GUID;
                    model.status = "Active";

                    var sql = "UPDATE mswd.osca_intake SET status='Trash' " +
                        "WHERE mswd.osca_intake.osca_intake_guid = '" + GUID + "'   ";
                    int res = (int)QueryModule.DataObject<int>(sql, model);

                    sql = "insert into mswd.osca_intake (`osca_intake_guid`, `osca_registration_guid`, `total_family_income`, `status`, `form_trans_no`, `member_count`, `fathers_name`, `mothers_name`, " +
                        "`listahanan`, `fourps_beneficiary`, `senior_citizen_organization`, `ip`, `other`, `osca_no`, `tin_no`, `gsis_no`, `sss_no`, `philhealth_no`, `others_no`, `living_arrangement_id`) " +
                        "VALUES(@osca_intake_guid, @osca_registration_guid, @total_family_income, @status, @form_trans_no, @member_count, @fathers_name, @mothers_name, @listahanan, " +
                        "@fourps_beneficiary, @senior_citizen_organization, @ip, @other, @osca_no, @tin_no, @gsis_no, @sss_no, @philhealth_no, @others_no, @living_arrangement_id)";
                    QueryModule.Execute<int>(sql, model);

                    sql = "insert into mswd.osca_intake_info (`main_guid`, `specify_listahanan`, `specify_ip`, " +
                        "`specify_other`, `pensioner`, `pensioner_amount`, `source_gsis`, `source_sss`, `source_afpslai`, `source_others`, `specify_other_source`, `permanent_source`, `what_source`, `family_support`, `support_type`, `how_much`, " +
                        "`how_often`, `in_kind`, `condition`, `with_maintenance`, `specify_maintenance`, `assessment_description`, `specify_living_others`) " +
                        "VALUES(@osca_intake_guid, @specify_listahanan, @specify_ip, @specify_other, " +
                        "@pensioner, @pensioner_amount, @source_gsis, @source_sss, @source_afpslai, @source_others, @specify_other_source, @permanent_source, @what_source, @family_support, @support_type, @how_much, " +
                        "@how_often, @in_kind, @condition, @with_maintenance, @specify_maintenance, @assessment_description, @specify_living_others)";
                    QueryModule.Execute<int>(sql, model);

                    sql = "DELETE FROM mswd.osca_intake_details WHERE mswd.osca_intake_details.main_guid = '" + GUID + "' ";
                    QueryModule.Execute<int>(sql, model);

                    sql = "DELETE FROM mswd.osca_intake_logs WHERE mswd.osca_intake_logs.main_guid = '" + GUID + "' ";
                    QueryModule.Execute<int>(sql, model);

                    sql = "INSERT INTO mswd.osca_intake_logs (`main_guid`, `user_id`) VALUES('" + model.osca_intake_guid + "', '" + GlobalObject.user_id + "')";
                    QueryModule.Execute<int>(sql, model);

                    if (model.details.Count > 0)
                    {
                        foreach (var dt in model.details)
                        {
                            dt.main_guid = model.osca_intake_guid;
                            sql = "insert into mswd.osca_intake_details (`main_guid`, `person_guid`, `relation`, `educational_attainment`, `occupation_income`, `occupation`) " +
                                "VALUES(@main_guid, @person_guid, @relation, @educational_attainment, @occupation_income, @occupational_skills)";
                            QueryModule.Execute<int>(sql, dt);
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

        public bool Delete(string GUID)
        {
            string sql = "UPDATE mswd.osca_intake SET status = 'Deleted' where mswd.osca_intake.osca_intake_guid = @osca_intake_guid";
            if (InsertUpdate.ExecuteSql(sql, new { osca_intake_guid = GUID }))
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

        public List<object> GetList()
        {
            string sql = @"SELECT mswd.osca_intake.osca_intake_guid, mswd.osca_intake.osca_registration_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, 
                            general.person.suffix, general_address.lgu_province_setup_temp.province_name, general_address.lgu_city_mun_setup_temp.city_mun_name, general_address.lgu_brgy_setup_temp.brgy_name,
                            mswd.osca_intake.form_trans_no, general.person.birth_date, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.gender_id, 
                            general.gender.gender_name, mswd.osca_intake.application_date, mswd.osca_intake.member_count, mswd.osca_intake.status, mswd.osca_intake.ip, mswd.osca_intake.fourps_beneficiary, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.osca_intake LEFT JOIN mswd.osca_registration ON mswd.osca_registration.osca_registration_guid = mswd.osca_intake.osca_registration_guid WHERE mswd.osca_intake.status = 'Active' AND mswd.osca_registration.status = 'Active') as 'count'
                            FROM mswd.osca_intake
                            LEFT JOIN mswd.osca_registration ON mswd.osca_registration.osca_registration_guid = mswd.osca_intake.osca_registration_guid
                            LEFT JOIN general.person ON general.person.person_guid = mswd.osca_registration.person_guid
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            WHERE mswd.osca_intake.status = 'Active' AND mswd.osca_registration.status = 'Active' ORDER BY mswd.osca_intake.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("osca-intake");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.osca_intake osca_intake
                    LEFT JOIN mswd.osca_registration ON mswd.osca_registration.osca_registration_guid = osca_intake.osca_registration_guid
                    LEFT JOIN general.person ON general.person.person_guid = mswd.osca_registration.person_guid
                    WHERE osca_intake.status = 'Active' AND mswd.osca_registration.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.osca_intake osca_intake
                    LEFT JOIN mswd.osca_registration ON mswd.osca_registration.osca_registration_guid = osca_intake.osca_registration_guid
                    LEFT JOIN general.person ON general.person.person_guid = mswd.osca_registration.person_guid
                    LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
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
            string sql = @"SELECT mswd.osca_intake.osca_intake_guid, mswd.osca_intake.osca_registration_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, 
                            general.person.suffix, general_address.lgu_province_setup_temp.province_name, general_address.lgu_city_mun_setup_temp.city_mun_name, general_address.lgu_brgy_setup_temp.brgy_name,
                            mswd.osca_intake.form_trans_no, general.person.birth_date, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.gender_id, 
                            general.gender.gender_name, mswd.osca_intake.application_date, mswd.osca_intake.member_count, mswd.osca_intake.status, mswd.osca_intake.ip, mswd.osca_intake.fourps_beneficiary, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.osca_intake LEFT JOIN mswd.osca_registration ON mswd.osca_registration.osca_registration_guid = mswd.osca_intake.osca_registration_guid WHERE mswd.osca_intake.status = 'Active' AND mswd.osca_registration.status = 'Active' OR mswd.osca_intake.status = 'Deleted' OR mswd.osca_registration.status = 'Deleted') as 'count'
                            FROM mswd.osca_intake
                            LEFT JOIN mswd.osca_registration ON mswd.osca_registration.osca_registration_guid = mswd.osca_intake.osca_registration_guid
                            LEFT JOIN general.person ON general.person.person_guid = mswd.osca_registration.person_guid
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            WHERE mswd.osca_intake.status = 'Active' AND mswd.osca_registration.status = 'Active' 
                            OR mswd.osca_intake.status = 'Deleted' OR mswd.osca_registration.status = 'Deleted' ORDER BY mswd.osca_intake.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("osca-intake");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.osca_intake osca_intake
                    LEFT JOIN mswd.osca_registration ON mswd.osca_registration.osca_registration_guid = osca_intake.osca_registration_guid
                    LEFT JOIN general.person ON general.person.person_guid = mswd.osca_registration.person_guid
                    WHERE osca_intake.status = 'Active' AND mswd.osca_registration.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.osca_intake osca_intake
                    LEFT JOIN mswd.osca_registration ON mswd.osca_registration.osca_registration_guid = osca_intake.osca_registration_guid
                    LEFT JOIN general.person ON general.person.person_guid = mswd.osca_registration.person_guid
                    LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
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

            string sql = @"SELECT mswd.osca_intake.osca_intake_guid, mswd.osca_intake.osca_registration_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, 
                            general.person.suffix, general_address.lgu_province_setup_temp.province_name, general_address.lgu_city_mun_setup_temp.city_mun_name, general_address.lgu_brgy_setup_temp.brgy_name,
                            mswd.osca_intake.form_trans_no, general.person.birth_date, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.gender_id, 
                            general.gender.gender_name, mswd.osca_intake.application_date, mswd.osca_intake.member_count, mswd.osca_intake.status, mswd.osca_intake.ip, mswd.osca_intake.fourps_beneficiary, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.osca_intake LEFT JOIN mswd.osca_registration ON mswd.osca_registration.osca_registration_guid = mswd.osca_intake.osca_registration_guid WHERE mswd.osca_intake.status = 'Deleted' AND mswd.osca_registration.status = 'Active') as 'count'
                            FROM mswd.osca_intake
                            LEFT JOIN mswd.osca_registration ON mswd.osca_registration.osca_registration_guid = mswd.osca_intake.osca_registration_guid
                            LEFT JOIN general.person ON general.person.person_guid = mswd.osca_registration.person_guid
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            WHERE mswd.osca_intake.status = 'Deleted' AND mswd.osca_registration.status = 'Active' ORDER BY mswd.osca_intake.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("osca-intake");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.osca_intake osca_intake
                    LEFT JOIN mswd.osca_registration ON mswd.osca_registration.osca_registration_guid = osca_intake.osca_registration_guid
                    LEFT JOIN general.person ON general.person.person_guid = mswd.osca_registration.person_guid
                    WHERE osca_intake.status = 'Deleted' AND mswd.osca_registration.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.osca_intake osca_intake
                    LEFT JOIN mswd.osca_registration ON mswd.osca_registration.osca_registration_guid = osca_intake.osca_registration_guid
                    LEFT JOIN general.person ON general.person.person_guid = mswd.osca_registration.person_guid
                    LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
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
                filterStatus = "mswd.osca_intake.status = 'Active' AND mswd.osca_registration.status = 'Active'";
            }
            else if (status_deleted_id != 0)
            {
                filterStatus = "mswd.osca_intake.status = 'Deleted' AND mswd.osca_registration.status = 'Active'";
            }

            if (filter_type_status_id == 1)
            {
                dateFilter = "AND MONTH(mswd.osca_intake.application_date) = '" + this_month + "'";
            }
            else if (filter_type_status_id == 2)
            {
                dateFilter = "AND YEAR(mswd.osca_intake.application_date) = '" + this_year + "'";
            }
            else if (filter_type_status_id == 3)
            {
                dateFilter = "AND MONTH(mswd.osca_intake.application_date) = '" + monthly + "' AND YEAR(mswd.osca_intake.application_date) = '" + monthlyYear + "'";
            }
            else if (filter_type_status_id == 4)
            {
                dateFilter = "AND YEAR(mswd.osca_intake.application_date) = '" + year_quarterly + "' AND QUARTER(mswd.osca_intake.application_date) = '" + quarter + "'";
            }
            else if (filter_type_status_id == 5)
            {
                dateFilter = "AND YEAR(mswd.osca_intake.application_date) = '" + yearly + "'";
            }
            else if (filter_type_status_id == 6)
            {
                dateFilter = "AND mswd.osca_intake.application_date >= '" + from + "' AND mswd.osca_intake.application_date <= '" + to + "'";
            }

            string sql = @"SELECT mswd.osca_intake.osca_intake_guid, mswd.osca_intake.osca_registration_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, 
                            general.person.suffix, general_address.lgu_province_setup_temp.province_name, general_address.lgu_city_mun_setup_temp.city_mun_name, general_address.lgu_brgy_setup_temp.brgy_name,
                            mswd.osca_intake.form_trans_no, general.person.birth_date, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.gender_id, 
                            general.gender.gender_name, mswd.osca_intake.application_date, mswd.osca_intake.member_count, mswd.osca_intake.status, mswd.osca_intake.ip, mswd.osca_intake.fourps_beneficiary, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.osca_intake LEFT JOIN mswd.osca_registration ON mswd.osca_registration.osca_registration_guid = mswd.osca_intake.osca_registration_guid WHERE " + filterStatus + @" " + dateFilter  + @") as 'count'
                            FROM mswd.osca_intake
                            LEFT JOIN mswd.osca_registration ON mswd.osca_registration.osca_registration_guid = mswd.osca_intake.osca_registration_guid
                            LEFT JOIN general.person ON general.person.person_guid = mswd.osca_registration.person_guid
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            WHERE " + filterStatus + @" " + dateFilter  + @" ORDER BY mswd.osca_intake.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("osca-intake");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.osca_intake osca_intake
                    LEFT JOIN mswd.osca_registration ON mswd.osca_registration.osca_registration_guid = osca_intake.osca_registration_guid
                    LEFT JOIN general.person ON general.person.person_guid = mswd.osca_registration.person_guid
                    WHERE osca_intake.status = 'Active' AND mswd.osca_registration.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.osca_intake osca_intake
                    LEFT JOIN mswd.osca_registration ON mswd.osca_registration.osca_registration_guid = osca_intake.osca_registration_guid
                    LEFT JOIN general.person ON general.person.person_guid = mswd.osca_registration.person_guid
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
            string sql = @"SELECT mswd.osca_intake_details.person_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix,  
                            general.person.gender_id, general.gender.gender_name, general.person.birth_date, general.person.place_of_birth,  
                            general.person.province_id, general_address.lgu_province_setup_temp.province_name,  
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.civil_status_id, general.civil_status.civil_status_name,  
                            general.person.religion, general.person.profession, general.person.age,  
                            mswd.osca_intake_details.main_guid, mswd.osca_intake_details.relation, mswd.osca_intake_details.educational_attainment, mswd.osca_intake_details.occupation_income, mswd.osca_intake_details.occupation
                            FROM mswd.osca_intake_details
                            LEFT JOIN general.person ON general.person.person_guid = mswd.osca_intake_details.person_guid
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            WHERE mswd.osca_intake_details.main_guid = '" + GUID + "' ";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public object GetOscaIntake(string osca_intake_guid)
        {
            string sql = @"SELECT mswd.osca_intake.osca_intake_guid, mswd.osca_registration.person_guid, general.person.prefix, general.person.suffix, general.person.first_name, general.person.middle_name, general.person.last_name,
                            mswd.osca_intake.form_trans_no, mswd.osca_intake.application_date, mswd.osca_intake.status, mswd.osca_intake.fourps_beneficiary, mswd.osca_intake.ip, mswd.osca_intake.member_count, general.person.educational_attainment,
                            mswd.osca_intake.total_family_income, general.person.birth_date, general.person.place_of_birth, general.person.gender_id, general.gender.gender_name, mswd.osca_intake.osca_registration_guid,
                            general.person.age, general.person.province_id, general_address.lgu_province_setup_temp.province_name, general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name,
                            general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.street, general.person.profession, mswd.osca_intake.fathers_name, mswd.osca_intake.mothers_name,
                            mswd.osca_intake.listahanan, mswd.osca_intake.senior_citizen_organization, mswd.osca_intake.other, mswd.osca_intake.osca_no, mswd.osca_intake.tin_no, mswd.osca_intake.gsis_no, mswd.osca_intake.sss_no,
                            mswd.osca_intake.philhealth_no, mswd.osca_intake.others_no, mswd.osca_intake.living_arrangement_id, mswd.osca_intake_info.specify_listahanan, mswd.osca_intake_info.specify_ip, mswd.osca_intake_info.specify_other,
                            mswd.osca_intake_info.pensioner, mswd.osca_intake_info.pensioner_amount, mswd.osca_intake_info.source_gsis, mswd.osca_intake_info.source_sss, mswd.osca_intake_info.source_afpslai, mswd.osca_intake_info.source_others, mswd.osca_intake_info.specify_other_source,
                            mswd.osca_intake_info.permanent_source, mswd.osca_intake_info.what_source, mswd.osca_intake_info.family_support, mswd.osca_intake_info.support_type, mswd.osca_intake_info.how_much, mswd.osca_intake_info.how_often, mswd.osca_intake_info.in_kind, mswd.osca_intake_info.condition,
                            mswd.osca_intake_info.with_maintenance, mswd.osca_intake_info.specify_maintenance, mswd.osca_intake_info.assessment_description, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.profession, general.person.citizenship,
                            general.person.religion, general.educational_type.educational_name, mswd.osca_intake_info.specify_living_others
                            FROM mswd.osca_intake
                            LEFT JOIN mswd.osca_registration ON mswd.osca_registration.osca_registration_guid = mswd.osca_intake.osca_registration_guid
                            LEFT JOIN mswd.osca_intake_info ON mswd.osca_intake_info.main_guid = mswd.osca_intake.osca_intake_guid
                            LEFT JOIN general.person ON general.person.person_guid = mswd.osca_registration.person_guid
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general.educational_type ON general.educational_type.id = general.person.educational_attainment
                            WHERE mswd.osca_intake.status = 'Active' AND mswd.osca_registration.status = 'Active' AND mswd.osca_intake.osca_intake_guid = @osca_intake_guid ";
            var obj = (OscaIntakeModel)QueryModule.DataObject<OscaIntakeModel>(sql, new { osca_intake_guid = osca_intake_guid });

            sql = @"SELECT mswd.osca_intake_details.main_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix,  
                    general.person.age, general.person.civil_status_id, general.civil_status.civil_status_name, mswd.osca_intake_details.person_guid, mswd.osca_intake_details.educational_attainment,  
                    mswd.osca_intake_details.relation, mswd.osca_intake_details.occupation, mswd.osca_intake_details.occupation_income  
                    FROM mswd.osca_intake_details  
                    LEFT JOIN general.person ON general.person.person_guid = mswd.osca_intake_details.person_guid  
                    LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id  
                    WHERE mswd.osca_intake_details.main_guid = '" + obj.osca_intake_guid + "' ";
            obj.details = (List<OscaIntakeDetails>)QueryModule.DataSource<OscaIntakeDetails>(sql);

            return obj;
        }

        public object GetPersonAdd(string GUID)
        {
            string sql = @"SELECT mswd.osca.person_guid, mswd.osca.osca_guid, mswd.osca.annual_income, mswd.osca.fourps_beneficiary,
                            mswd.osca.ips, mswd.osca.house_ownership, mswd.osca.educational_attainment, mswd.osca.monthly_income,
                            mswd.osca.occupation, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix,
                            general.person.gender_id, general.gender.gender_name, general.person.civil_status_id, general.civil_status.civil_status_name,
                            general.person.birth_date, general.person.place_of_birth, general.person.province_id, general_address.lgu_province_setup_temp.province_name,
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name,
                            general.person.religion, general.person.age, general.person.person_image, general.person.street
                            FROM mswd.osca 
                            LEFT JOIN general.person ON general.person.person_guid = mswd.osca.person_guid
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            WHERE mswd.osca.status = 'Active' AND mswd.osca.person_guid = '" + GUID + "' ";
            var obj = (OscaIntakeModel)QueryModule.DataObject<OscaIntakeModel>(sql);

            if (obj == null)
                return null;

            sql = @"SELECT mswd.osca_details.main_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix,  
                    general.person.age, general.person.civil_status_id, general.civil_status.civil_status_name, mswd.osca_details.person_guid, mswd.osca_details.educational_attainment,  
                    mswd.osca_details.relation, mswd.osca_details.occupational_skills, mswd.osca_details.remarks, mswd.osca_details.occupation_income
                    from mswd.osca_details
                    LEFT JOIN general.person ON general.person.person_guid = mswd.osca_details.person_guid
                    LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                    WHERE mswd.osca_details.main_guid = '" + obj.osca_guid + "' ";
            obj.details = (List<OscaIntakeDetails>)QueryModule.DataSource<OscaIntakeDetails>(sql);

            return obj;
        }

        public List<object> GetHistoryLogs(string GUID)
        {
            string sql = @"SELECT mswd.osca_intake_logs.main_guid, mswd.osca_intake.application_date, mswd.osca_intake_logs.user_id, mswd.osca_intake.form_trans_no, mswd.osca_intake.status,
                            mswd.osca_intake.total_family_income
                            FROM mswd.osca_intake_logs  
                            LEFT JOIN mswd.osca_intake ON mswd.osca_intake.osca_intake_guid = mswd.osca_intake_logs.main_guid  
                            WHERE mswd.osca_intake_logs.main_guid = '" + GUID + "' ORDER BY mswd.osca_intake.id DESC";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public bool Insert(OscaIntakeModel model)
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
                    model.form_trans_no = generateFormNumber("osca-intake");
                    model.osca_intake_guid = Guid.NewGuid().ToString();
                    model.status = "Active";

                    string sql = "insert into mswd.osca_intake (`osca_intake_guid`, `osca_registration_guid`, `total_family_income`, `status`, `form_trans_no`, `member_count`, `fathers_name`, `mothers_name`, " +
                        "`listahanan`, `fourps_beneficiary`, `senior_citizen_organization`, `ip`, `other`, `osca_no`, `tin_no`, `gsis_no`, `sss_no`, `philhealth_no`, `others_no`, `living_arrangement_id`) " +
                        "VALUES(@osca_intake_guid, @osca_registration_guid, @total_family_income, @status, @form_trans_no, @member_count, @fathers_name, @mothers_name, @listahanan, " +
                        "@fourps_beneficiary, @senior_citizen_organization, @ip, @other, @osca_no, @tin_no, @gsis_no, @sss_no, @philhealth_no, @others_no, @living_arrangement_id)";
                    int res = (int)QueryModule.DataObject<int>(sql, model);

                    sql = "insert into mswd.osca_intake_info (`main_guid`, `specify_listahanan`, `specify_ip`, " +
                        "`specify_other`, `pensioner`, `pensioner_amount`, `source_gsis`, `source_sss`, `source_afpslai`, `source_others`, `specify_other_source`, `permanent_source`, `what_source`, `family_support`, `support_type`, `how_much`, " +
                        "`how_often`, `in_kind`, `condition`, `with_maintenance`, `specify_maintenance`, `assessment_description`, `specify_living_others`) " +
                        "VALUES(@osca_intake_guid, @specify_listahanan, @specify_ip, @specify_other, " +
                        "@pensioner, @pensioner_amount, @source_gsis, @source_sss, @source_afpslai, @source_others, @specify_other_source, @permanent_source, @what_source, @family_support, @support_type, @how_much, " +
                        "@how_often, @in_kind, @condition, @with_maintenance, @specify_maintenance, @assessment_description, @specify_living_others)";
                    QueryModule.Execute<int>(sql, model);

                    if (model.details.Count > 0)
                    {
                        foreach (var dt in model.details)
                        {
                            dt.main_guid = model.osca_intake_guid;
                            sql = "insert into mswd.osca_intake_details (`main_guid`, `person_guid`, `relation`, `educational_attainment`, `occupation_income`, `occupation`) " +
                                "VALUES(@main_guid, @person_guid, @relation, @educational_attainment, @occupation_income, @occupational_skills)";
                            QueryModule.Execute<int>(sql, dt);
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
