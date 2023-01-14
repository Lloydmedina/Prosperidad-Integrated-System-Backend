using eegs_back_end.Admin.GeneralIntakeSetup.Model;
using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.GeneralIntakeSetup.Repository
{
    public interface IGeneralIntakeRepository :IGlobalInterface
    {
        bool Insert(GeneralIntakeModel model);
        List<object> GetListOfDisability();
        List<object> GetList();
        List<object> GetAllList();
        List<object> GetListDeleted();
        List<object> GetListGenerated(int? filter_type_status_id = 0, int? status_id = 0, int? status_deleted_id = 0, string? this_month = "", string? this_year = "", string? monthly = "", string? monthlyYear = "", string? year_quarterly = "", int? quarter = 0, string? yearly = "", string? from = "", string? to = "");
        List<object> GetEducationalType();
        List<object> GetDetails(string GUID);
        object GetPersonAdd(string GUID);
        List<object> GetHistoryLogs(string GUID);
        bool Edit(string GUID, GeneralIntakeModel model);
        bool Delete(string GUID);
        object GetGeneralIntake(string GeneralIntake_guid);
    }
    public class GeneralIntakeRepository : FormNumberGenerator, IGeneralIntakeRepository
    {
        public bool Edit(string GUID, GeneralIntakeModel model)
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
                    model.general_intake_guid = GUID;
                    model.status = "Active";

                    var sql = "UPDATE mswd.general_intake SET status='Trash' " +
                        "WHERE mswd.general_intake.general_intake_guid = '" + GUID + "'   ";
                    int res = (int)QueryModule.DataObject<int>(sql, model);

                    sql = "insert into mswd.general_intake (`general_intake_guid`, `person_guid`, `owner`, `renter`, `estimated_damaged`, `if_distressed`, `physical_disability`, `type_of_disability_id`, `sources_of_income`, `total_family_income`, `no_of_hectares`, `crops_planted`, `area_of_location`, `other_sources_of_income`, `status`, `form_trans_no`, `member_count`) " +
                        "VALUES(@general_intake_guid, @person_guid, @owner, @renter, @estimated_damaged, @if_distressed, @physical_disability, @type_of_disability_id, @sources_of_income, @total_family_income, @no_of_hectares, @crops_planted, @area_of_location, @other_sources_of_income, @status, '" + model.form_trans_no + "', @member_count)";
                    QueryModule.Execute<int>(sql, model);

                    sql = "DELETE FROM mswd.general_intake_details WHERE mswd.general_intake_details.main_guid = '" + GUID + "' ";
                    QueryModule.Execute<int>(sql, model);

                    sql = "DELETE FROM mswd.general_intake_logs WHERE mswd.general_intake_logs.main_guid = '" + GUID + "' ";
                    QueryModule.Execute<int>(sql, model);

                    sql = "INSERT INTO mswd.general_intake_logs (`main_guid`, `user_id`) VALUES('" + model.general_intake_guid + "', '" + GlobalObject.user_id + "')";
                    QueryModule.Execute<int>(sql, model);

                    if (model.details.Count > 0)
                    {
                        foreach (var dt in model.details)
                        {
                            dt.main_guid = model.general_intake_guid;
                            sql = "insert into mswd.general_intake_details (`main_guid`, `person_guid`, `status`, `relation`, `educational_attainment`, `occupation_income`, `occupation`) " +
                                "VALUES(@main_guid, @person_guid, @status, @relation, @educational_attainment, @occupation_income, @occupation)";
                            QueryModule.Execute<int>(sql, dt);
                        }
                    }

                    //var sql = "UPDATE mswd.family_composition_head SET person_guid='"+ model.person_guid +"', monthly_income='"+model.monthly_income+"', fourps_member='"+model.fourps_member+"', " +
                    //    "ips='"+model.ips+"', house_occupancy='"+model.house_occupancy+"', property_cost='"+model.property_cost+"', total_family_income='"+model.total_family_income+"', " +
                    //    "family_composition_guid='"+model.general_intake_guid + "', status='"+model.status+"' " +
                    //    "WHERE general.family_composition_head.family_composition_guid = '" + GUID + "'   ";
                    //int res = (int)QueryModule.DataObject<int>(sql, model);

                    //sql = "DELETE FROM general.family_composition_details WHERE general.family_composition_details.main_guid = '" + GUID + "' ";
                    //QueryModule.Execute<int>(sql, model);

                    //if (model.details.Count > 0)
                    //{
                    //    foreach (var dt in model.details)
                    //    {
                    //        dt.main_guid = model.general_intake_guid;
                    //        sql = "insert into general.family_composition_details (`main_guid`, `person_guid`, `status`, `relation`, `educational_attainment`, `occupation_income`, `occupation`) " +
                    //            "VALUES(@main_guid, @person_guid, @status, @relation, @educational_attainment, @occupation_income, @occupation)";
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
            string sql = "UPDATE mswd.general_intake SET status = 'Deleted' where mswd.general_intake.general_intake_guid = @general_intake_guid";
            if (InsertUpdate.ExecuteSql(sql, new { general_intake_guid = GUID }))
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
            string sql = "SELECT mswd.general_intake.general_intake_guid, mswd.general_intake.status, mswd.general_intake.person_guid, mswd.general_intake.form_trans_no, mswd.general_intake.member_count, mswd.general_intake.application_date, general.person.first_name, " +
                "general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix, general.person.gender_id, general.gender.gender_name, general.person.birth_date, general.person.place_of_birth, " +
                "general.person.province_id, general.person.age, general_address.lgu_province_setup_temp.province_name, " +
                "general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.religion, general.person.profession, " +
                "mswd.family_composition_head.fourps_member, mswd.family_composition_head.ips, general.person.full_name, " +
                "(SELECT COUNT(*) FROM mswd.general_intake LEFT JOIN mswd.family_composition_head ON mswd.family_composition_head.person_guid = mswd.general_intake.person_guid WHERE mswd.general_intake.status = 'Active' AND mswd.family_composition_head.status = 'Active') as 'count' " +
                "FROM mswd.general_intake " +
                "LEFT JOIN general.person ON general.person.person_guid = mswd.general_intake.person_guid " +
                "LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id " +
                "LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id " +
                "LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id " +
                "LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id " +
                "LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id " +
                "LEFT JOIN mswd.family_composition_head ON mswd.family_composition_head.person_guid = mswd.general_intake.person_guid " +
                "WHERE mswd.general_intake.status = 'Active' AND mswd.family_composition_head.status = 'Active' ORDER BY mswd.general_intake.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("fc-intake");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.general_intake gen
                    LEFT JOIN general.person ON general.person.person_guid = gen.person_guid
                    WHERE gen.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.general_intake gen
                    LEFT JOIN general.person ON general.person.person_guid = gen.person_guid
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
            string sql = "SELECT mswd.general_intake.general_intake_guid, mswd.general_intake.status, mswd.general_intake.person_guid, mswd.general_intake.form_trans_no, mswd.general_intake.member_count, mswd.general_intake.application_date, general.person.first_name, " +
                "general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix, general.person.gender_id, general.gender.gender_name, general.person.birth_date, general.person.place_of_birth, " +
                "general.person.province_id, general.person.age, general_address.lgu_province_setup_temp.province_name, " +
                "general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.religion, general.person.profession, " +
                "mswd.family_composition_head.fourps_member, mswd.family_composition_head.ips, general.person.full_name, " +
                "(SELECT COUNT(*) FROM mswd.general_intake LEFT JOIN mswd.family_composition_head ON mswd.family_composition_head.person_guid = mswd.general_intake.person_guid WHERE mswd.general_intake.status = 'Active' AND mswd.family_composition_head.status = 'Active' OR mswd.general_intake.status = 'Deleted') as 'count' " +
                "FROM mswd.general_intake " +
                "LEFT JOIN general.person ON general.person.person_guid = mswd.general_intake.person_guid " +
                "LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id " +
                "LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id " +
                "LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id " +
                "LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id " +
                "LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id " +
                "LEFT JOIN mswd.family_composition_head ON mswd.family_composition_head.person_guid = mswd.general_intake.person_guid " +
                "WHERE mswd.general_intake.status = 'Active' AND mswd.family_composition_head.status = 'Active' OR mswd.general_intake.status = 'Deleted' ORDER BY mswd.general_intake.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("fc-intake");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.general_intake gen
                    LEFT JOIN general.person ON general.person.person_guid = gen.person_guid
                    WHERE gen.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.general_intake gen
                    LEFT JOIN general.person ON general.person.person_guid = gen.person_guid
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
            string sql = "SELECT mswd.general_intake_details.person_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, " +
                "general.person.gender_id, general.gender.gender_name, general.person.birth_date, general.person.place_of_birth, " +
                "general.person.province_id, general_address.lgu_province_setup_temp.province_name, " +
                "general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.civil_status_id, general.civil_status.civil_status_name, " +
                "general.person.religion, general.person.profession, general.person.age, " +
                "mswd.general_intake_details.main_guid, mswd.general_intake_details.status, mswd.general_intake_details.relation, mswd.general_intake_details.educational_attainment, mswd.general_intake_details.occupation_income, mswd.general_intake_details.occupation " +
                "FROM mswd.general_intake_details " +
                "LEFT JOIN general.person ON general.person.person_guid = mswd.general_intake_details.person_guid " +
                "LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id " +
                "LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id " +
                "LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id " +
                "LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id " +
                "LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id " +
                "WHERE mswd.general_intake_details.main_guid = '" + GUID + "'";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public List<object> GetListDeleted()
        {

            string sql = "SELECT mswd.general_intake.general_intake_guid, mswd.general_intake.status, mswd.general_intake.person_guid, mswd.general_intake.form_trans_no, mswd.general_intake.member_count, mswd.general_intake.application_date, general.person.first_name, " +
                "general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix, general.person.gender_id, general.gender.gender_name, general.person.birth_date, general.person.place_of_birth, " +
                "general.person.province_id, general.person.age, general_address.lgu_province_setup_temp.province_name, " +
                "general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.religion, general.person.profession, " +
                "mswd.family_composition_head.fourps_member, mswd.family_composition_head.ips, general.person.full_name, " +
                "(SELECT COUNT(*) FROM mswd.general_intake LEFT JOIN mswd.family_composition_head ON mswd.family_composition_head.person_guid = mswd.general_intake.person_guid WHERE mswd.general_intake.status = 'Deleted' AND mswd.family_composition_head.status = 'Active') as 'count' " +
                "FROM mswd.general_intake " +
                "LEFT JOIN general.person ON general.person.person_guid = mswd.general_intake.person_guid " +
                "LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id " +
                "LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id " +
                "LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id " +
                "LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id " +
                "LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id " +
                "LEFT JOIN mswd.family_composition_head ON mswd.family_composition_head.person_guid = mswd.general_intake.person_guid " +
                "WHERE mswd.general_intake.status = 'Deleted' AND mswd.family_composition_head.status = 'Active' ORDER BY mswd.general_intake.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("fc-intake");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.general_intake gen
                    LEFT JOIN general.person ON general.person.person_guid = gen.person_guid
                    WHERE gen.status = 'Deleted'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.general_intake gen
                    LEFT JOIN general.person ON general.person.person_guid = gen.person_guid
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
                filterStatus = "mswd.general_intake.status = 'Active' AND mswd.family_composition_head.status = 'Active'";
            }
            else if (status_deleted_id != 0)
            {
                filterStatus = "mswd.general_intake.status = 'Deleted'";
            }

            if (filter_type_status_id == 1)
            {
                dateFilter = "AND MONTH(mswd.general_intake.application_date) = '" + this_month + "'";
            }
            else if (filter_type_status_id == 2)
            {
                dateFilter = "AND YEAR(mswd.general_intake.application_date) = '" + this_year + "'";
            }
            else if (filter_type_status_id == 3)
            {
                dateFilter = "AND MONTH(mswd.general_intake.application_date) = '" + monthly + "' AND YEAR(mswd.general_intake.application_date) = '" + monthlyYear + "'";
            }
            else if (filter_type_status_id == 4)
            {
                dateFilter = "AND YEAR(mswd.general_intake.application_date) = '" + year_quarterly + "' AND QUARTER(mswd.general_intake.application_date) = '" + quarter + "'";
            }
            else if (filter_type_status_id == 5)
            {
                dateFilter = "AND YEAR(mswd.general_intake.application_date) = '" + yearly + "'";
            }
            else if (filter_type_status_id == 6)
            {
                dateFilter = "AND mswd.general_intake.application_date >= '" + from + "' AND mswd.general_intake.application_date <= '" + to + "'";
            }

            string sql = "SELECT mswd.general_intake.general_intake_guid, mswd.general_intake.status, mswd.general_intake.person_guid, mswd.general_intake.form_trans_no, mswd.general_intake.member_count, mswd.general_intake.application_date, general.person.first_name, " +
                "general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix, general.person.gender_id, general.gender.gender_name, general.person.birth_date, general.person.place_of_birth, " +
                "general.person.province_id, general.person.age, general_address.lgu_province_setup_temp.province_name, " +
                "general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.religion, general.person.profession, " +
                "mswd.family_composition_head.fourps_member, mswd.family_composition_head.ips, general.person.full_name, " +
                "(SELECT COUNT(*) FROM mswd.general_intake LEFT JOIN mswd.family_composition_head ON mswd.family_composition_head.person_guid = mswd.general_intake.person_guid WHERE " + filterStatus + @" " + dateFilter  + @") as 'count' " +
                "FROM mswd.general_intake " +
                "LEFT JOIN general.person ON general.person.person_guid = mswd.general_intake.person_guid " +
                "LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id " +
                "LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id " +
                "LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id " +
                "LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id " +
                "LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id " +
                "LEFT JOIN mswd.family_composition_head ON mswd.family_composition_head.person_guid = mswd.general_intake.person_guid " +
                "WHERE " + filterStatus + @" " + dateFilter  + @" ORDER BY mswd.general_intake.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("fc-intake");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.general_intake gen
                    LEFT JOIN general.person ON general.person.person_guid = gen.person_guid
                    WHERE gen.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.general_intake gen
                    LEFT JOIN general.person ON general.person.person_guid = gen.person_guid
                    LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id

                    ) z";
            List<object> searches = (List<object>)QueryModule.DataSource<object>(sql);

            List<object> list = new List<object>();
            list.Add(obj);
            list.Add(form);
            list.Add(searches);


            return list;
        }

        public object GetGeneralIntake(string general_intake_guid)
        {
            string sql = @"SELECT mswd.general_intake.general_intake_guid, mswd.general_intake.person_guid, mswd.general_intake.owner, mswd.general_intake.renter,
                            mswd.general_intake.estimated_damaged, mswd.general_intake.if_distressed, mswd.general_intake.physical_disability, 
                            mswd.general_intake.type_of_disability_id, mswd.general_intake_disability.disability_name, mswd.general_intake.sources_of_income,
                            mswd.general_intake.total_family_income, mswd.general_intake.no_of_hectares, mswd.general_intake.crops_planted, mswd.general_intake.area_of_location,
                            mswd.general_intake.other_sources_of_income, mswd.general_intake.form_trans_no, mswd.family_composition_head.fourps_member, mswd.family_composition_head.ips,
                            mswd.general_intake.application_date, mswd.family_composition_head.monthly_income, mswd.family_composition_head.house_occupancy, mswd.family_composition_head.educational_attainment,
                            mswd.family_composition_head.occupation, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix,
                            general.person.gender_id, general.gender.gender_name, general.person.civil_status_id, general.civil_status.civil_status_name,
                            general.person.birth_date, general.person.place_of_birth, general.person.province_id, general_address.lgu_province_setup_temp.province_name,
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name,
                            general.person.religion, general.person.age, general.person.street, mswd.general_intake.application_date, general.person.person_image, general.educational_type.educational_name
                            FROM mswd.general_intake
                            LEFT JOIN mswd.general_intake_disability ON mswd.general_intake_disability.id = mswd.general_intake.type_of_disability_id
                            LEFT JOIN general.person ON general.person.person_guid = mswd.general_intake.person_guid
                            LEFT JOIN mswd.family_composition_head ON mswd.family_composition_head.person_guid = mswd.general_intake.person_guid
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            LEFT JOIN general.educational_type ON general.educational_type.id = mswd.family_composition_head.educational_attainment
                            WHERE mswd.family_composition_head.status = 'Active' AND mswd.general_intake.status = 'Active' 
                            AND mswd.general_intake.general_intake_guid = @general_intake_guid ";
            var obj = (GeneralIntakeModel)QueryModule.DataObject<GeneralIntakeModel>(sql, new { general_intake_guid = general_intake_guid });

            sql = @"SELECT mswd.general_intake_details.main_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix,  
                    general.person.age, general.person.civil_status_id, general.civil_status.civil_status_name, mswd.general_intake_details.person_guid, mswd.general_intake_details.status, mswd.general_intake_details.educational_attainment,  
                    mswd.general_intake_details.relation, mswd.general_intake_details.occupation, mswd.general_intake_details.occupation_income  
                    FROM mswd.general_intake_details  
                    LEFT JOIN general.person ON general.person.person_guid = mswd.general_intake_details.person_guid  
                    LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id  
                    WHERE mswd.general_intake_details.main_guid = '" + obj.general_intake_guid + "' ";
            obj.details = (List<GeneralIntakeDetails>)QueryModule.DataSource<GeneralIntakeDetails>(sql);

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
            var obj = (GeneralIntakeModel)QueryModule.DataObject<GeneralIntakeModel>(sql);

            if (obj == null)
                return null;

            sql = "select mswd.family_composition_details.main_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix, " +
                "general.person.age, general.person.civil_status_id, general.civil_status.civil_status_name, mswd.family_composition_details.person_guid, mswd.family_composition_details.status, mswd.family_composition_details.educational_attainment, " +
                "mswd.family_composition_details.relation, mswd.family_composition_details.occupation, mswd.family_composition_details.occupation_income " +
                "from mswd.family_composition_details " +
                "LEFT JOIN general.person ON general.person.person_guid = mswd.family_composition_details.person_guid " +
                "LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id " +
                "where mswd.family_composition_details.main_guid = '" + obj.family_composition_guid + "' ";
            obj.details = (List<GeneralIntakeDetails>)QueryModule.DataSource<GeneralIntakeDetails>(sql);

            return obj;
        }

        public List<object> GetHistoryLogs(string GUID)
        {
            string sql = @"SELECT mswd.general_intake_logs.main_guid, mswd.general_intake.application_date, mswd.general_intake_logs.user_id, mswd.general_intake.form_trans_no, mswd.general_intake.status,
                            mswd.general_intake.owner, mswd.general_intake.renter, mswd.general_intake.estimated_damaged, mswd.general_intake.if_distressed, mswd.general_intake.physical_disability,
                            mswd.general_intake.type_of_disability_id, mswd.general_intake_disability.disability_name, mswd.general_intake.sources_of_income, mswd.general_intake.total_family_income,
                            mswd.general_intake.no_of_hectares, mswd.general_intake.crops_planted, mswd.general_intake.area_of_location, mswd.general_intake.other_sources_of_income
                            FROM mswd.general_intake_logs  
                            LEFT JOIN mswd.general_intake ON mswd.general_intake.general_intake_guid = mswd.general_intake_logs.main_guid  
                            LEFT JOIN mswd.general_intake_disability ON mswd.general_intake_disability.id = mswd.general_intake.type_of_disability_id
                            WHERE mswd.general_intake_logs.main_guid = '" + GUID +"' ORDER BY mswd.general_intake.id DESC";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public bool Insert(GeneralIntakeModel model)
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
                    model.form_trans_no = generateFormNumber("fc-intake");
                    model.general_intake_guid = Guid.NewGuid().ToString();
                    model.status = "Active";

                    string sql = "insert into mswd.general_intake (`person_guid`, `owner`, `renter`, `estimated_damaged`, `if_distressed`, `physical_disability`, `type_of_disability_id`, `sources_of_income`, " +
                        "`total_family_income`, `no_of_hectares`, `crops_planted`, `area_of_location`, `other_sources_of_income`, `status`, `member_count`, `form_trans_no`, `general_intake_guid`) " +
                        "VALUES(@person_guid, @owner, @renter, @estimated_damaged, @if_distressed, @physical_disability, @type_of_disability_id, @sources_of_income, @total_family_income, " +
                        "@no_of_hectares, @crops_planted, @area_of_location, @other_sources_of_income, @status, @member_count, @form_trans_no, @general_intake_guid)";
                    int res = (int)QueryModule.DataObject<int>(sql, model);

                    if (model.details.Count > 0)
                    {
                        foreach (var dt in model.details)
                        {
                            dt.main_guid = model.general_intake_guid;
                            sql = "insert into mswd.general_intake_details (`main_guid`, `person_guid`, `status`, `relation`, `educational_attainment`, `occupation_income`, `occupation`) " +
                                "VALUES(@main_guid, @person_guid, @status, @relation, @educational_attainment, @occupation_income, @occupation)";
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
