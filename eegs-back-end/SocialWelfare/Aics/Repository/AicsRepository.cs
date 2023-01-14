using eegs_back_end.Admin.AicsSetup.Model;
using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.AicsSetup.Repository
{
    public interface IAicsRepository :IGlobalInterface
    {
        bool Insert(AicsModel model);
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
        bool Edit(string GUID, AicsModel model);
        bool Delete(string GUID);
        object GetAics(string Aics_guid);
    }
    public class AicsRepository : FormNumberGenerator, IAicsRepository
    {
        public bool Edit(string GUID, AicsModel model)
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
                    model.aics_guid = GUID;
                    model.status = "Active";

                    var sql = "UPDATE mswd.aics SET status='Trash' " +
                        "WHERE mswd.aics.aics_guid = '" + GUID + "'   ";
                    int res = (int)QueryModule.DataObject<int>(sql, model);

                    sql = @"INSERT INTO mswd.aics (`aics_guid`, `person_guid`, `form_trans_no`, `status`, `fourps_beneficiary`, `ips`, `member_count`, `application_id`, `application_type_id`, `educational_attainment`, `house_ownership`, `monthly_family_income`, `occupation`, `recommendation_id`, `total_family_income`, `type_of_ethnicity`, `date_recommended`) 
                        VALUES(@aics_guid, @person_guid, @form_trans_no, @status, @fourps_beneficiary, @ips, @member_count, @application_id, @application_type_id, @educational_attainment, @house_ownership, @monthly_family_income, @occupation, @recommendation_id, @total_family_income, @type_of_ethnicity, @date_recommended)";
                    QueryModule.Execute<int>(sql, model);

                    sql = "DELETE FROM mswd.aics_details WHERE mswd.aics_details.main_guid = '" + GUID + "' ";
                    QueryModule.Execute<int>(sql, model);

                    sql = "DELETE FROM mswd.aics_casualties WHERE mswd.aics_casualties.main_guid = '" + GUID + "' ";
                    QueryModule.Execute<int>(sql, model);

                    sql = "DELETE FROM mswd.aics_logs WHERE mswd.aics_logs.main_guid = '" + GUID + "' ";
                    QueryModule.Execute<int>(sql, model);

                    sql = "INSERT INTO mswd.aics_logs (`main_guid`, `user_id`) VALUES('" + model.aics_guid + "', '" + GlobalObject.user_id + "')";
                    QueryModule.Execute<int>(sql, model);

                    if (model.family_details.Count > 0)
                    {
                        foreach (var dt in model.family_details)
                        {
                            dt.main_guid = model.aics_guid;
                            sql = @"INSERT INTO mswd.aics_details (`main_guid`, `person_guid`, `relation`, `educational_attainment`, `occupational_skills`, `remarks`, `occupation_income`)
                                VALUES(@main_guid, @person_guid, @relation, @educational_attainment, @occupational_skills, @remarks, @occupation_income)";
                            QueryModule.Execute<int>(sql, dt);
                        }
                    }

                    if (model.casualties_details.Count > 0)
                    {
                        foreach (var dt in model.casualties_details)
                        {
                            dt.main_guid = model.aics_guid;
                            sql = @"INSERT INTO mswd.aics_casualties (`main_guid`, `person_guid`)
                                VALUES(@main_guid, @person_guid)";
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
            string sql = "UPDATE mswd.aics SET status = 'Deleted' where mswd.aics.aics_guid = @aics_guid";
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
            string sql = "SELECT * FROM mswd.aics_application";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public List<object> GetListOfApplicationType()
        {
            string sql = "SELECT * FROM mswd.aics_application_type";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public List<object> GetListOfRecommendations()
        {
            string sql = "SELECT * FROM mswd.aics_recommendations";
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
            string sql = @"SELECT mswd.aics.aics_guid, mswd.aics.person_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, 
                            general.person.suffix, general_address.lgu_province_setup_temp.province_name, general_address.lgu_city_mun_setup_temp.city_mun_name, general_address.lgu_brgy_setup_temp.brgy_name,
                            mswd.aics.form_trans_no, general.person.birth_date, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.gender_id, 
                            general.gender.gender_name, mswd.aics.application_date, mswd.aics.fourps_beneficiary, mswd.aics.ips, mswd.aics.member_count, mswd.aics.status, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.aics WHERE mswd.aics.status = 'Active') as 'count'
                            FROM mswd.aics
                            LEFT JOIN general.person ON general.person.person_guid = mswd.aics.person_guid
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            WHERE mswd.aics.status = 'Active' ORDER BY mswd.aics.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("aics-registration");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.aics aics
                    LEFT JOIN general.person ON general.person.person_guid = aics.person_guid
                    WHERE aics.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.aics aics
                    LEFT JOIN general.person ON general.person.person_guid = aics.person_guid
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
            string sql = @"SELECT mswd.aics.aics_guid, mswd.aics.person_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, 
                            general.person.suffix, general_address.lgu_province_setup_temp.province_name, general_address.lgu_city_mun_setup_temp.city_mun_name, general_address.lgu_brgy_setup_temp.brgy_name,
                            mswd.aics.form_trans_no, general.person.birth_date, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.gender_id, 
                            general.gender.gender_name, mswd.aics.application_date, mswd.aics.fourps_beneficiary, mswd.aics.ips, mswd.aics.member_count, mswd.aics.status, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.aics WHERE mswd.aics.status = 'Active' OR mswd.aics.status = 'Deleted') as 'count'
                            FROM mswd.aics
                            LEFT JOIN general.person ON general.person.person_guid = mswd.aics.person_guid
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            WHERE mswd.aics.status = 'Active' OR mswd.aics.status = 'Deleted' ORDER BY mswd.aics.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("aics-registration");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.aics aics
                    LEFT JOIN general.person ON general.person.person_guid = aics.person_guid
                    WHERE aics.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.aics aics
                    LEFT JOIN general.person ON general.person.person_guid = aics.person_guid
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

            string sql = @"SELECT mswd.aics.aics_guid, mswd.aics.person_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, 
                            general.person.suffix, general_address.lgu_province_setup_temp.province_name, general_address.lgu_city_mun_setup_temp.city_mun_name, general_address.lgu_brgy_setup_temp.brgy_name,
                            mswd.aics.form_trans_no, general.person.birth_date, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.gender_id, 
                            general.gender.gender_name, mswd.aics.application_date, mswd.aics.fourps_beneficiary, mswd.aics.ips, mswd.aics.member_count, mswd.aics.status, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.aics WHERE mswd.aics.status = 'Deleted') as 'count'
                            FROM mswd.aics
                            LEFT JOIN general.person ON general.person.person_guid = mswd.aics.person_guid
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            WHERE mswd.aics.status = 'Deleted' ORDER BY mswd.aics.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("aics-registration");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.aics aics
                    LEFT JOIN general.person ON general.person.person_guid = aics.person_guid
                    WHERE aics.status = 'Deleted'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.aics aics
                    LEFT JOIN general.person ON general.person.person_guid = aics.person_guid
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
                filterStatus = "mswd.aics.status = 'Active'";
            }
            else if (status_deleted_id != 0)
            {
                filterStatus = "mswd.aics.status = 'Deleted'";
            }

            if (filter_type_status_id == 1)
            {
                dateFilter = "AND MONTH(mswd.aics.application_date) = '" + this_month + "'";
            }
            else if (filter_type_status_id == 2)
            {
                dateFilter = "AND YEAR(mswd.aics.application_date) = '" + this_year + "'";
            }
            else if (filter_type_status_id == 3)
            {
                dateFilter = "AND MONTH(mswd.aics.application_date) = '" + monthly + "' AND YEAR(mswd.aics.application_date) = '" + monthlyYear + "'";
            }
            else if (filter_type_status_id == 4)
            {
                dateFilter = "AND YEAR(mswd.aics.application_date) = '" + year_quarterly + "' AND QUARTER(mswd.aics.application_date) = '" + quarter + "'";
            }
            else if (filter_type_status_id == 5)
            {
                dateFilter = "AND YEAR(mswd.aics.application_date) = '" + yearly + "'";
            }
            else if (filter_type_status_id == 6)
            {
                dateFilter = "AND mswd.aics.application_date >= '" + from + "' AND mswd.aics.application_date <= '" + to + "'";
            }

            string sql = @"SELECT mswd.aics.aics_guid, mswd.aics.person_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, 
                            general.person.suffix, general_address.lgu_province_setup_temp.province_name, general_address.lgu_city_mun_setup_temp.city_mun_name, general_address.lgu_brgy_setup_temp.brgy_name,
                            mswd.aics.form_trans_no, general.person.birth_date, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.gender_id, 
                            general.gender.gender_name, mswd.aics.application_date, mswd.aics.fourps_beneficiary, mswd.aics.ips, mswd.aics.member_count, mswd.aics.status, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.aics WHERE " + filterStatus + @" " + dateFilter  + @") as 'count'
                            FROM mswd.aics
                            LEFT JOIN general.person ON general.person.person_guid = mswd.aics.person_guid
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            WHERE " + filterStatus + @" " + dateFilter  + @" ORDER BY mswd.aics.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("aics-registration");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.aics aics
                    LEFT JOIN general.person ON general.person.person_guid = aics.person_guid
                    WHERE aics.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.aics aics
                    LEFT JOIN general.person ON general.person.person_guid = aics.person_guid
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
            string sql = @"SELECT mswd.aics_details.person_guid, mswd.aics_details.relation, mswd.aics_details.educational_attainment, mswd.aics_details.occupational_skills,
                            mswd.aics_details.remarks, general.person.prefix, general.person.suffix, general.person.first_name, general.person.middle_name, general.person.last_name,
                            general.person.gender_id, general.gender.gender_name, general.person.age
                            FROM mswd.aics_details
                            LEFT JOIN general.person ON general.person.person_guid = mswd.aics_details.person_guid
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            WHERE mswd.aics_details.main_guid = '" + GUID + "' ";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public object GetAics(string aics_guid)
        {
            string sql = @"SELECT mswd.aics.aics_guid, mswd.aics.person_guid, general.person.prefix, general.person.suffix, general.person.first_name, general.person.middle_name, general.person.last_name,
                            mswd.aics.form_trans_no, mswd.aics.application_date, mswd.aics.status, mswd.aics.fourps_beneficiary, mswd.aics.ips, mswd.aics.member_count, mswd.aics.application_id,
                            mswd.aics_application.application_name, mswd.aics.application_type_id, mswd.aics_application_type.application_type, mswd.aics.educational_attainment,
                            mswd.aics.house_ownership, mswd.aics.monthly_family_income, mswd.aics.occupation, mswd.aics.recommendation_id, mswd.aics_recommendations.recommendation_name,
                            mswd.aics.total_family_income, mswd.aics.type_of_ethnicity, general.person.birth_date, general.person.place_of_birth, general.person.gender_id, general.gender.gender_name,
                            general.person.age, general.person.province_id, general_address.lgu_province_setup_temp.province_name, general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name,
                            general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.street, mswd.aics.date_recommended,
                            (SELECT general.person.application_date FROM general.person WHERE general.person.person_guid = mswd.aics.person_guid) as date_occurence
                            FROM mswd.aics
                            LEFT JOIN general.person ON general.person.person_guid = mswd.aics.person_guid
                            LEFT JOIN mswd.aics_application ON mswd.aics_application.id = mswd.aics.application_id
                            LEFT JOIN mswd.aics_application_type ON mswd.aics_application_type.id = mswd.aics.application_type_id
                            LEFT JOIN mswd.aics_recommendations ON mswd.aics_recommendations.id = mswd.aics.recommendation_id
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            WHERE mswd.aics.status = 'Active' AND mswd.aics.aics_guid = @aics_guid ";
            var obj = (AicsModel)QueryModule.DataObject<AicsModel>(sql, new { aics_guid = aics_guid });

            sql = @"SELECT mswd.aics_details.main_guid, mswd.aics_details.person_guid, mswd.aics_details.relation, mswd.aics_details.educational_attainment,
                    mswd.aics_details.occupational_skills, mswd.aics_details.remarks, general.person.prefix, general.person.suffix, general.person.first_name,
                    general.person.middle_name, general.person.last_name, general.person.age, general.person.gender_id, general.gender.gender_name, mswd.aics_details.occupation_income,
                    general.educational_type.educational_name
                    FROM mswd.aics_details
                    LEFT JOIN general.person ON general.person.person_guid = mswd.aics_details.person_guid
                    LEFT JOIN general.gender ON general.gender.gender_id =  general.person.gender_id
                    LEFT JOIN general.educational_type ON general.educational_type.id = mswd.aics_details.educational_attainment
                    WHERE mswd.aics_details.main_guid = '" + obj.aics_guid + "' ";
            obj.family_details = (List<AicsDetails>)QueryModule.DataSource<AicsDetails>(sql);

            sql = @"SELECT mswd.aics_casualties.main_guid, mswd.aics_casualties.person_guid, general.person.prefix, general.person.suffix, general.person.first_name,
                    general.person.middle_name, general.person.last_name, general.person.age
                    FROM mswd.aics_casualties
                    LEFT JOIN general.person ON general.person.person_guid = mswd.aics_casualties.person_guid
                    WHERE mswd.aics_casualties.main_guid = '" + obj.aics_guid + "' ";
            obj.casualties_details = (List<CasualtiesDetails>)QueryModule.DataSource<CasualtiesDetails>(sql);

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
                            LEFT JOIN general.person ON general.person.person_guid = mswd.family_composition_head.person_guid
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            WHERE mswd.family_composition_head.status = 'Active' AND mswd.family_composition_head.person_guid = '" + GUID + "' ";
            var obj = (AicsModel)QueryModule.DataObject<AicsModel>(sql);

            if (obj == null)
                return null;

            //sql = "select mswd.family_composition_details.main_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix, " +
            //    "general.person.age, general.person.civil_status_id, general.civil_status.civil_status_name, mswd.family_composition_details.person_guid, mswd.family_composition_details.status, mswd.family_composition_details.educational_attainment, " +
            //    "mswd.family_composition_details.relation, mswd.family_composition_details.occupation, mswd.family_composition_details.occupation_income " +
            //    "from mswd.family_composition_details " +
            //    "INNER JOIN general.person ON general.person.person_guid = mswd.family_composition_details.person_guid " +
            //    "INNER JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id " +
            //    "where mswd.family_composition_details.main_guid = '" + obj.family_composition_guid + "' ";
            //obj.details = (List<AicsDetails>)QueryModule.DataSource<AicsDetails>(sql);

            return obj;
        }

        public List<object> GetHistoryLogs(string GUID)
        {
            string sql = @"SELECT mswd.aics_logs.main_guid, mswd.aics.application_date, mswd.aics_logs.user_id, mswd.aics.form_trans_no,
                            mswd.aics.status, mswd.aics.person_guid, general.person.prefix, general.person.suffix, general.person.first_name,
                            general.person.middle_name, general.person.last_name, mswd.aics.ips, mswd.aics.educational_attainment, mswd.aics.total_family_income
                            FROM mswd.aics_logs  
                            LEFT JOIN mswd.aics ON mswd.aics.aics_guid = mswd.aics_logs.main_guid  
                            LEFT JOIN general.person ON general.person.person_guid = mswd.aics.person_guid
                            WHERE mswd.aics_logs.main_guid = '" + GUID + "' ORDER BY mswd.aics.id DESC";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public bool Insert(AicsModel model)
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
                    model.form_trans_no = generateFormNumber("aics-registration");
                    model.aics_guid = Guid.NewGuid().ToString();
                    model.status = "Active";

                    string sql = @"INSERT INTO mswd.aics (`aics_guid`, `person_guid`, `form_trans_no`, `status`, `fourps_beneficiary`, `ips`, `member_count`, `application_id`, `application_type_id`, `educational_attainment`, `house_ownership`, `monthly_family_income`, `occupation`, `recommendation_id`, `total_family_income`, `type_of_ethnicity`, `date_recommended`) 
                        VALUES(@aics_guid, @person_guid, @form_trans_no, @status, @fourps_beneficiary, @ips, @member_count, @application_id, @application_type_id, @educational_attainment, @house_ownership, @monthly_family_income, @occupation, @recommendation_id, @total_family_income, @type_of_ethnicity, @date_recommended)";
                    int res = (int)QueryModule.DataObject<int>(sql, model);

                    if (model.family_details.Count > 0)
                    {
                        foreach (var dt in model.family_details)
                        {
                            dt.main_guid = model.aics_guid;
                            sql = @"INSERT INTO mswd.aics_details (`main_guid`, `person_guid`, `relation`, `educational_attainment`, `occupational_skills`, `remarks`, `occupation_income`)
                                VALUES(@main_guid, @person_guid, @relation, @educational_attainment, @occupational_skills, @remarks, @occupation_income)";
                            QueryModule.Execute<int>(sql, dt);
                        }
                    }

                    if (model.casualties_details.Count > 0)
                    {
                        foreach (var dt in model.casualties_details)
                        {
                            dt.main_guid = model.aics_guid;
                            sql = @"INSERT INTO mswd.aics_casualties (`main_guid`, `person_guid`)
                                VALUES(@main_guid, @person_guid)";
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
