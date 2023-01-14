using eegs_back_end.Admin.DafacIntakeSetup.Model;
using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.DafacIntakeSetup.Repository
{
    public interface IDafacIntakeRepository :IGlobalInterface
    {
        bool Insert(DafacIntakeModel model);
        List<object> GetListOfDisability();
        List<object> GetEducationalType();
        List<object> GetList();
        List<object> GetAllList();
        List<object> GetListDeleted();
        List<object> GetListGenerated(int? filter_type_status_id = 0, int? status_id = 0, int? status_deleted_id = 0, string? this_month = "", string? this_year = "", string? monthly = "", string? monthlyYear = "", string? year_quarterly = "", int? quarter = 0, string? yearly = "", string? from = "", string? to = "");
        List<object> GetDetails(string GUID);
        object GetPersonAdd(string GUID);
        List<object> GetHistoryLogs(string GUID);
        bool Edit(string GUID, DafacIntakeModel model);
        bool Delete(string GUID);
        object GetDafacIntake(string DafacIntake_guid);
    }
    public class DafacIntakeRepository : FormNumberGenerator, IDafacIntakeRepository
    {
        public bool Edit(string GUID, DafacIntakeModel model)
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
                    model.dafac_intake_guid = GUID;
                    model.status = "Active";

                    var sql = "UPDATE mswd.dafac_intake SET status='Trash' " +
                        "WHERE mswd.dafac_intake.dafac_intake_guid = '" + GUID + "'   ";
                    int res = (int)QueryModule.DataObject<int>(sql, model);

                    sql = "insert into mswd.dafac_intake (`dafac_intake_guid`, `person_guid`, `owner`, `renter`, `estimated_damaged`, `if_distressed`, `physical_disability`, `type_of_disability_id`, `sources_of_income`, `total_family_income`, `no_of_hectares`, `crops_planted`, `area_of_location`, `other_sources_of_income`, `status`, `form_trans_no`, `member_count`) " +
                        "VALUES(@dafac_intake_guid, @person_guid, @owner, @renter, @estimated_damaged, @if_distressed, @physical_disability, @type_of_disability_id, @sources_of_income, @total_family_income, @no_of_hectares, @crops_planted, @area_of_location, @other_sources_of_income, @status, '" + model.form_trans_no + "', @member_count)";
                    QueryModule.Execute<int>(sql, model);

                    sql = "DELETE FROM mswd.dafac_intake_details WHERE mswd.dafac_intake_details.main_guid = '" + GUID + "' ";
                    QueryModule.Execute<int>(sql, model);

                    sql = "DELETE FROM mswd.dafac_intake_logs WHERE mswd.dafac_intake_logs.main_guid = '" + GUID + "' ";
                    QueryModule.Execute<int>(sql, model);

                    sql = "INSERT INTO mswd.dafac_intake_logs (`main_guid`, `user_id`) VALUES('" + model.dafac_intake_guid + "', '" + GlobalObject.user_id + "')";
                    QueryModule.Execute<int>(sql, model);

                    if (model.details.Count > 0)
                    {
                        foreach (var dt in model.details)
                        {
                            dt.main_guid = model.dafac_intake_guid;
                            sql = "insert into mswd.dafac_intake_details (`main_guid`, `person_guid`, `relation`, `educational_attainment`, `occupation_income`, `occupation`) " +
                                "VALUES(@main_guid, @person_guid, @relation, @educational_attainment, @occupation_income, @occupation)";
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
            string sql = "UPDATE mswd.dafac_intake SET status = 'Deleted' where mswd.dafac_intake.dafac_intake_guid = @dafac_intake_guid";
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
            string sql = @"SELECT mswd.dafac_intake.dafac_intake_guid, mswd.dafac_intake.status, mswd.dafac_intake.person_guid, mswd.dafac_intake.form_trans_no, mswd.dafac_intake.member_count, mswd.dafac_intake.application_date, general.person.first_name,  
                            general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix, general.person.gender_id, general.gender.gender_name, general.person.birth_date, general.person.place_of_birth,  
                            general.person.province_id, general.person.age, general_address.lgu_province_setup_temp.province_name,  
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.religion, general.person.profession,  
                            mswd.dafac.fourps_beneficiary, mswd.dafac.ips, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.dafac_intake LEFT JOIN mswd.dafac ON mswd.dafac.person_guid = mswd.dafac_intake.person_guid WHERE mswd.dafac_intake.status = 'Active' AND mswd.dafac.status = 'Active') as 'count'
                            FROM mswd.dafac_intake  
                            LEFT JOIN general.person ON general.person.person_guid = mswd.dafac_intake.person_guid  
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id  
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id  
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id  
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id  
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id  
                            LEFT JOIN mswd.dafac ON mswd.dafac.person_guid = mswd.dafac_intake.person_guid  
                            WHERE mswd.dafac_intake.status = 'Active' AND mswd.dafac.status = 'Active' ORDER BY mswd.dafac_intake.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("dafac-intake");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.dafac_intake dafac_intake
                    LEFT JOIN general.person ON general.person.person_guid = dafac_intake.person_guid
                    WHERE dafac_intake.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.dafac_intake dafac_intake
                    LEFT JOIN general.person ON general.person.person_guid = dafac_intake.person_guid
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
            string sql = @"SELECT mswd.dafac_intake.dafac_intake_guid, mswd.dafac_intake.status, mswd.dafac_intake.person_guid, mswd.dafac_intake.form_trans_no, mswd.dafac_intake.member_count, mswd.dafac_intake.application_date, general.person.first_name,  
                            general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix, general.person.gender_id, general.gender.gender_name, general.person.birth_date, general.person.place_of_birth,  
                            general.person.province_id, general.person.age, general_address.lgu_province_setup_temp.province_name,  
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.religion, general.person.profession,  
                            mswd.dafac.fourps_beneficiary, mswd.dafac.ips, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.dafac_intake LEFT JOIN mswd.dafac ON mswd.dafac.person_guid = mswd.dafac_intake.person_guid WHERE mswd.dafac_intake.status = 'Active' AND mswd.dafac.status = 'Active' OR mswd.dafac_intake.status = 'Deleted') as 'count'
                            FROM mswd.dafac_intake  
                            LEFT JOIN general.person ON general.person.person_guid = mswd.dafac_intake.person_guid  
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id  
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id  
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id  
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id  
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id  
                            LEFT JOIN mswd.dafac ON mswd.dafac.person_guid = mswd.dafac_intake.person_guid  
                            WHERE mswd.dafac_intake.status = 'Active' AND mswd.dafac.status = 'Active' OR mswd.dafac_intake.status = 'Deleted' ORDER BY mswd.dafac_intake.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("dafac-intake");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.dafac_intake dafac_intake
                    LEFT JOIN general.person ON general.person.person_guid = dafac_intake.person_guid
                    WHERE dafac_intake.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.dafac_intake dafac_intake
                    LEFT JOIN general.person ON general.person.person_guid = dafac_intake.person_guid
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
            string sql = "SELECT mswd.dafac_intake_details.person_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, " +
                "general.person.gender_id, general.gender.gender_name, general.person.birth_date, general.person.place_of_birth, " +
                "general.person.province_id, general_address.lgu_province_setup_temp.province_name, " +
                "general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.civil_status_id, general.civil_status.civil_status_name, " +
                "general.person.religion, general.person.profession, general.person.age, " +
                "mswd.dafac_intake_details.main_guid, mswd.dafac_intake_details.relation, mswd.dafac_intake_details.educational_attainment, mswd.dafac_intake_details.occupation_income, mswd.dafac_intake_details.occupation " +
                "FROM mswd.dafac_intake_details " +
                "LEFT JOIN general.person ON general.person.person_guid = mswd.dafac_intake_details.person_guid " +
                "LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id " +
                "LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id " +
                "LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id " +
                "LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id " +
                "LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id " +
                "WHERE mswd.dafac_intake_details.main_guid = '" + GUID + "'";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public List<object> GetListDeleted()
        {

            string sql = @"SELECT mswd.dafac_intake.dafac_intake_guid, mswd.dafac_intake.status, mswd.dafac_intake.person_guid, mswd.dafac_intake.form_trans_no, mswd.dafac_intake.member_count, mswd.dafac_intake.application_date, general.person.first_name,  
                            general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix, general.person.gender_id, general.gender.gender_name, general.person.birth_date, general.person.place_of_birth,  
                            general.person.province_id, general.person.age, general_address.lgu_province_setup_temp.province_name,  
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.religion, general.person.profession,  
                            mswd.dafac.fourps_beneficiary, mswd.dafac.ips, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.dafac_intake LEFT JOIN mswd.dafac ON mswd.dafac.person_guid = mswd.dafac_intake.person_guid WHERE mswd.dafac_intake.status = 'Deleted' AND mswd.dafac.status = 'Active') as 'count'
                            FROM mswd.dafac_intake  
                            LEFT JOIN general.person ON general.person.person_guid = mswd.dafac_intake.person_guid  
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id  
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id  
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id  
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id  
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id  
                            LEFT JOIN mswd.dafac ON mswd.dafac.person_guid = mswd.dafac_intake.person_guid  
                            WHERE mswd.dafac_intake.status = 'Deleted' AND mswd.dafac.status = 'Active' ORDER BY mswd.dafac_intake.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("dafac-intake");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.dafac_intake dafac_intake
                    LEFT JOIN general.person ON general.person.person_guid = dafac_intake.person_guid
                    WHERE dafac_intake.status = 'Deleted'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.dafac_intake dafac_intake
                    LEFT JOIN general.person ON general.person.person_guid = dafac_intake.person_guid
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
                filterStatus = "mswd.dafac_intake.status = 'Active' AND mswd.dafac.status = 'Active'";
            }
            else if (status_deleted_id != 0)
            {
                filterStatus = "mswd.dafac_intake.status = 'Deleted'";
            }

            if (filter_type_status_id == 1)
            {
                dateFilter = "AND MONTH(mswd.dafac_intake.application_date) = '" + this_month + "'";
            }
            else if (filter_type_status_id == 2)
            {
                dateFilter = "AND YEAR(mswd.dafac_intake.application_date) = '" + this_year + "'";
            }
            else if (filter_type_status_id == 3)
            {
                dateFilter = "AND MONTH(mswd.dafac_intake.application_date) = '" + monthly + "' AND YEAR(mswd.dafac_intake.application_date) = '" + monthlyYear + "'";
            }
            else if (filter_type_status_id == 4)
            {
                dateFilter = "AND YEAR(mswd.dafac_intake.application_date) = '" + year_quarterly + "' AND QUARTER(mswd.dafac_intake.application_date) = '" + quarter + "'";
            }
            else if (filter_type_status_id == 5)
            {
                dateFilter = "AND YEAR(mswd.dafac_intake.application_date) = '" + yearly + "'";
            }
            else if (filter_type_status_id == 6)
            {
                dateFilter = "AND mswd.dafac_intake.application_date >= '" + from + "' AND mswd.dafac_intake.application_date <= '" + to + "'";
            }

            string sql = @"SELECT mswd.dafac_intake.dafac_intake_guid, mswd.dafac_intake.status, mswd.dafac_intake.person_guid, mswd.dafac_intake.form_trans_no, mswd.dafac_intake.member_count, mswd.dafac_intake.application_date, general.person.first_name,  
                            general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix, general.person.gender_id, general.gender.gender_name, general.person.birth_date, general.person.place_of_birth,  
                            general.person.province_id, general.person.age, general_address.lgu_province_setup_temp.province_name,  
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.religion, general.person.profession,  
                            mswd.dafac.fourps_beneficiary, mswd.dafac.ips, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.dafac_intake LEFT JOIN mswd.dafac ON mswd.dafac.person_guid = mswd.dafac_intake.person_guid WHERE " + filterStatus + @" " + dateFilter  + @") as 'count'
                            FROM mswd.dafac_intake  
                            LEFT JOIN general.person ON general.person.person_guid = mswd.dafac_intake.person_guid  
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id  
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id  
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id  
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id  
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id  
                            LEFT JOIN mswd.dafac ON mswd.dafac.person_guid = mswd.dafac_intake.person_guid  
                            WHERE " + filterStatus + @" " + dateFilter  + @" ORDER BY mswd.dafac_intake.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("dafac-intake");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.dafac_intake dafac_intake
                    LEFT JOIN general.person ON general.person.person_guid = dafac_intake.person_guid
                    WHERE dafac_intake.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.dafac_intake dafac_intake
                    LEFT JOIN general.person ON general.person.person_guid = dafac_intake.person_guid
                    LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id

                    ) z";
            List<object> searches = (List<object>)QueryModule.DataSource<object>(sql);

            List<object> list = new List<object>();
            list.Add(obj);
            list.Add(form);
            list.Add(searches);


            return list;
        }

        public object GetDafacIntake(string dafac_intake_guid)
        {
            string sql = @"SELECT mswd.dafac_intake.dafac_intake_guid, mswd.dafac_intake.person_guid, mswd.dafac_intake.owner, mswd.dafac_intake.renter,
                            mswd.dafac_intake.estimated_damaged, mswd.dafac_intake.if_distressed, mswd.dafac_intake.physical_disability, 
                            mswd.dafac_intake.type_of_disability_id, mswd.general_intake_disability.disability_name, mswd.dafac_intake.sources_of_income,
                            mswd.dafac_intake.total_family_income, mswd.dafac_intake.no_of_hectares, mswd.dafac_intake.crops_planted, mswd.dafac_intake.area_of_location,
                            mswd.dafac_intake.other_sources_of_income, mswd.dafac_intake.form_trans_no, mswd.dafac.fourps_beneficiary, mswd.dafac.ips,
                            mswd.dafac_intake.application_date, mswd.dafac.monthly_family_income, mswd.dafac.house_ownership, mswd.dafac.educational_attainment,
                            mswd.dafac.occupation, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix,
                            general.person.gender_id, general.gender.gender_name, general.person.civil_status_id, general.civil_status.civil_status_name,
                            general.person.birth_date, general.person.place_of_birth, general.person.province_id, general_address.lgu_province_setup_temp.province_name,
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name,
                            general.person.religion, general.person.age, general.person.street, mswd.dafac_intake.application_date, general.person.person_image,
                            general.educational_type.educational_name
                            FROM mswd.dafac_intake
                            LEFT JOIN mswd.general_intake_disability ON mswd.general_intake_disability.id = mswd.dafac_intake.type_of_disability_id
                            LEFT JOIN general.person ON general.person.person_guid = mswd.dafac_intake.person_guid
                            LEFT JOIN mswd.dafac ON mswd.dafac.person_guid = mswd.dafac_intake.person_guid
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            LEFT JOIN general.educational_type ON general.educational_type.id = mswd.dafac.educational_attainment
                            WHERE mswd.dafac.status = 'Active' AND mswd.dafac_intake.status = 'Active' AND mswd.dafac_intake.dafac_intake_guid = @dafac_intake_guid ";
            var obj = (DafacIntakeModel)QueryModule.DataObject<DafacIntakeModel>(sql, new { dafac_intake_guid = dafac_intake_guid });

            sql = @"SELECT mswd.dafac_intake_details.main_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix,  
                    general.person.age, general.person.civil_status_id, general.civil_status.civil_status_name, mswd.dafac_intake_details.person_guid, mswd.dafac_intake_details.educational_attainment,  
                    mswd.dafac_intake_details.relation, mswd.dafac_intake_details.occupation, mswd.dafac_intake_details.occupation_income, general.educational_type.educational_name
                    FROM mswd.dafac_intake_details  
                    LEFT JOIN general.person ON general.person.person_guid = mswd.dafac_intake_details.person_guid  
                    LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id  
                    LEFT JOIN general.educational_type ON general.educational_type.id = mswd.dafac_intake_details.educational_attainment
                    WHERE mswd.dafac_intake_details.main_guid = '" + obj.dafac_intake_guid + "' ";
            obj.details = (List<DafacIntakeDetails>)QueryModule.DataSource<DafacIntakeDetails>(sql);

            return obj;
        }

        public object GetPersonAdd(string GUID)
        {
            string sql = @"SELECT mswd.dafac.person_guid, mswd.dafac.dafac_guid, mswd.dafac.monthly_family_income, mswd.dafac.fourps_beneficiary,
                            mswd.dafac.ips, mswd.dafac.house_ownership, mswd.dafac.educational_attainment,
                            mswd.dafac.occupation, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix,
                            general.person.gender_id, general.gender.gender_name, general.person.civil_status_id, general.civil_status.civil_status_name,
                            general.person.birth_date, general.person.place_of_birth, general.person.province_id, general_address.lgu_province_setup_temp.province_name,
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name,
                            general.person.religion, general.person.age, general.person.person_image, general.person.street 
                            FROM mswd.dafac 
                            LEFT JOIN general.person ON general.person.person_guid = mswd.dafac.person_guid
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            WHERE mswd.dafac.status = 'Active' AND mswd.dafac.person_guid = '" + GUID + "' ";
            var obj = (DafacIntakeModel)QueryModule.DataObject<DafacIntakeModel>(sql);

            if (obj == null)
                return null;

            sql = "select mswd.dafac_details.main_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix, " +
                "general.person.age, general.person.civil_status_id, general.civil_status.civil_status_name, mswd.dafac_details.person_guid, mswd.dafac_details.educational_attainment, " +
                "mswd.dafac_details.relation, mswd.dafac_details.occupational_skills, mswd.dafac_details.remarks, mswd.dafac_details.occupation_income " +
                "from mswd.dafac_details " +
                "LEFT JOIN general.person ON general.person.person_guid = mswd.dafac_details.person_guid " +
                "LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id " +
                "where mswd.dafac_details.main_guid = '" + obj.dafac_guid + "' ";
            obj.details = (List<DafacIntakeDetails>)QueryModule.DataSource<DafacIntakeDetails>(sql);

            return obj;
        }

        public List<object> GetHistoryLogs(string GUID)
        {
            string sql = @"SELECT mswd.dafac_intake_logs.main_guid, mswd.dafac_intake.application_date, mswd.dafac_intake_logs.user_id, mswd.dafac_intake.form_trans_no, mswd.dafac_intake.status,
                            mswd.dafac_intake.owner, mswd.dafac_intake.renter, mswd.dafac_intake.estimated_damaged, mswd.dafac_intake.if_distressed, mswd.dafac_intake.physical_disability,
                            mswd.dafac_intake.type_of_disability_id, mswd.general_intake_disability.disability_name, mswd.dafac_intake.sources_of_income, mswd.dafac_intake.total_family_income,
                            mswd.dafac_intake.no_of_hectares, mswd.dafac_intake.crops_planted, mswd.dafac_intake.area_of_location, mswd.dafac_intake.other_sources_of_income
                            FROM mswd.dafac_intake_logs  
                            LEFT JOIN mswd.dafac_intake ON mswd.dafac_intake.dafac_intake_guid = mswd.dafac_intake_logs.main_guid  
                            LEFT JOIN mswd.general_intake_disability ON mswd.general_intake_disability.id = mswd.dafac_intake.type_of_disability_id
                            WHERE mswd.dafac_intake_logs.main_guid = '" + GUID +"' ORDER BY mswd.dafac_intake.id DESC";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public bool Insert(DafacIntakeModel model)
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
                    model.form_trans_no = generateFormNumber("dafac-intake");
                    model.dafac_intake_guid = Guid.NewGuid().ToString();
                    model.status = "Active";

                    string sql = "insert into mswd.dafac_intake (`person_guid`, `owner`, `renter`, `estimated_damaged`, `if_distressed`, `physical_disability`, `type_of_disability_id`, `sources_of_income`, " +
                        "`total_family_income`, `no_of_hectares`, `crops_planted`, `area_of_location`, `other_sources_of_income`, `status`, `member_count`, `form_trans_no`, `dafac_intake_guid`) " +
                        "VALUES(@person_guid, @owner, @renter, @estimated_damaged, @if_distressed, @physical_disability, @type_of_disability_id, @sources_of_income, @total_family_income, " +
                        "@no_of_hectares, @crops_planted, @area_of_location, @other_sources_of_income, @status, @member_count, @form_trans_no, @dafac_intake_guid)";
                    int res = (int)QueryModule.DataObject<int>(sql, model);

                    if (model.details.Count > 0)
                    {
                        foreach (var dt in model.details)
                        {
                            dt.main_guid = model.dafac_intake_guid;
                            sql = "insert into mswd.dafac_intake_details (`main_guid`, `person_guid`, `relation`, `educational_attainment`, `occupation_income`, `occupation`) " +
                                "VALUES(@main_guid, @person_guid, @relation, @educational_attainment, @occupation_income, @occupation)";
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
