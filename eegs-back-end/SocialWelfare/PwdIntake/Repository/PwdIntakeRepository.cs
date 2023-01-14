using eegs_back_end.Admin.PwdIntakeSetup.Model;
using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.PwdIntakeSetup.Repository
{
    public interface IPwdIntakeRepository :IGlobalInterface
    {
        bool Insert(PwdIntakeModel model);
        List<object> GetListOfDisability();
        List<object> GetList();
        List<object> GetAllList();
        List<object> GetListDeleted();
        List<object> GetListGenerated(int? filter_type_status_id = 0, int? status_id = 0, int? status_deleted_id = 0, string? this_month = "", string? this_year = "", string? monthly = "", string? monthlyYear = "", string? year_quarterly = "", int? quarter = 0, string? yearly = "", string? from = "", string? to = "");
        List<object> GetEducationalType();
        List<object> GetDetails(string GUID);
        object GetPersonAdd(string GUID);
        List<object> GetHistoryLogs(string GUID);
        bool Edit(string GUID, PwdIntakeModel model);
        bool Delete(string GUID);
        object GetPwdIntake(string PwdIntake_guid);
    }
    public class PwdIntakeRepository : FormNumberGenerator, IPwdIntakeRepository
    {
        public bool Edit(string GUID, PwdIntakeModel model)
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
                    model.pwd_intake_guid = GUID;
                    model.status = "Active";

                    var sql = "UPDATE mswd.pwd_intake SET status='Trash' " +
                        "WHERE mswd.pwd_intake.pwd_intake_guid = '" + GUID + "'   ";
                    int res = (int)QueryModule.DataObject<int>(sql, model);

                    sql = "insert into mswd.pwd_intake (`pwd_intake_guid`, `person_guid`, `owner`, `renter`, `estimated_damaged`, `if_distressed`, `physical_disability`, `type_of_disability_id`, " +
                        "`sources_of_income`, `total_family_income`, `no_of_hectares`, `crops_planted`, `area_of_location`, `other_sources_of_income`, `status`, `form_trans_no`, `member_count`) " +
                        "VALUES(@pwd_intake_guid, @person_guid, @owner, @renter, @estimated_damaged, @if_distressed, @physical_disability, @type_of_disability_id, @sources_of_income, " +
                        "@total_family_income, @no_of_hectares, @crops_planted, @area_of_location, @other_sources_of_income, @status, @form_trans_no, @member_count)";
                    QueryModule.Execute<int>(sql, model);

                    sql = "DELETE FROM mswd.pwd_intake_details WHERE mswd.pwd_intake_details.main_guid = '" + GUID + "' ";
                    QueryModule.Execute<int>(sql, model);

                    sql = "DELETE FROM mswd.pwd_intake_logs WHERE mswd.pwd_intake_logs.main_guid = '" + GUID + "' ";
                    QueryModule.Execute<int>(sql, model);

                    sql = "INSERT INTO mswd.pwd_intake_logs (`main_guid`, `user_id`) VALUES('" + model.pwd_intake_guid + "', '" + GlobalObject.user_id + "')";
                    QueryModule.Execute<int>(sql, model);

                    if (model.details.Count > 0)
                    {
                        foreach (var dt in model.details)
                        {
                            dt.main_guid = model.pwd_intake_guid;
                            sql = "insert into mswd.pwd_intake_details (`main_guid`, `person_guid`, `relation`, `educational_attainment`, `occupation_income`, `occupation`) " +
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
            string sql = "UPDATE mswd.pwd_intake SET status = 'Deleted' where mswd.pwd_intake.pwd_intake_guid = @pwd_intake_guid";
            if (InsertUpdate.ExecuteSql(sql, new { pwd_intake_guid = GUID }))
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
            string sql = @"SELECT mswd.pwd_intake.pwd_intake_guid, mswd.pwd_intake.status, mswd.pwd_intake.person_guid, mswd.pwd_intake.form_trans_no, mswd.pwd_intake.member_count, mswd.pwd_intake.application_date, general.person.first_name,  
                            general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix, general.person.gender_id, general.gender.gender_name, general.person.birth_date, general.person.place_of_birth,  
                            general.person.province_id, general.person.age, general_address.lgu_province_setup_temp.province_name,  
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.religion, general.person.profession,
                            general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.pwd_intake LEFT JOIN mswd.pwd ON mswd.pwd.person_guid = mswd.pwd_intake.person_guid WHERE mswd.pwd_intake.status = 'Active' AND mswd.pwd.status = 'Active') as 'count'
                            FROM mswd.pwd_intake  
                            LEFT JOIN general.person ON general.person.person_guid = mswd.pwd_intake.person_guid  
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id  
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id  
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id  
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id  
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id  
                            LEFT JOIN mswd.pwd ON mswd.pwd.person_guid = mswd.pwd_intake.person_guid  
                            WHERE mswd.pwd_intake.status = 'Active' AND mswd.pwd.status = 'Active' ORDER BY general.person.age DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("pwd-intake");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.pwd_intake pwd_intake
                    INNER JOIN general.person ON general.person.person_guid = pwd_intake.person_guid
                    WHERE pwd_intake.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.pwd_intake pwd_intake
                    LEFT JOIN general.person ON general.person.person_guid = pwd_intake.person_guid
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
            string sql = @"SELECT mswd.pwd_intake.pwd_intake_guid, mswd.pwd_intake.status, mswd.pwd_intake.person_guid, mswd.pwd_intake.form_trans_no, mswd.pwd_intake.member_count, mswd.pwd_intake.application_date, general.person.first_name,  
                            general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix, general.person.gender_id, general.gender.gender_name, general.person.birth_date, general.person.place_of_birth,  
                            general.person.province_id, general.person.age, general_address.lgu_province_setup_temp.province_name,  
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.religion, general.person.profession,
                            general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.pwd_intake LEFT JOIN mswd.pwd ON mswd.pwd.person_guid = mswd.pwd_intake.person_guid WHERE mswd.pwd_intake.status = 'Active' AND mswd.pwd.status = 'Active' OR mswd.pwd.status = 'Deleted') as 'count'
                            FROM mswd.pwd_intake  
                            LEFT JOIN general.person ON general.person.person_guid = mswd.pwd_intake.person_guid  
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id  
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id  
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id  
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id  
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id  
                            LEFT JOIN mswd.pwd ON mswd.pwd.person_guid = mswd.pwd_intake.person_guid  
                            WHERE mswd.pwd_intake.status = 'Active' AND mswd.pwd.status = 'Active' OR mswd.pwd.status = 'Deleted' ORDER BY general.person.age DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("pwd-intake");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.pwd_intake pwd_intake
                    INNER JOIN general.person ON general.person.person_guid = pwd_intake.person_guid
                    WHERE pwd_intake.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.pwd_intake pwd_intake
                    LEFT JOIN general.person ON general.person.person_guid = pwd_intake.person_guid
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

            string sql = @"SELECT mswd.pwd_intake.pwd_intake_guid, mswd.pwd_intake.status, mswd.pwd_intake.person_guid, mswd.pwd_intake.form_trans_no, mswd.pwd_intake.member_count, mswd.pwd_intake.application_date, general.person.first_name,  
                            general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix, general.person.gender_id, general.gender.gender_name, general.person.birth_date, general.person.place_of_birth,  
                            general.person.province_id, general.person.age, general_address.lgu_province_setup_temp.province_name,  
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.religion, general.person.profession,
                            general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.pwd_intake LEFT JOIN mswd.pwd ON mswd.pwd.person_guid = mswd.pwd_intake.person_guid WHERE mswd.pwd_intake.status = 'Deleted' AND mswd.pwd.status = 'Active') as 'count'
                            FROM mswd.pwd_intake  
                            LEFT JOIN general.person ON general.person.person_guid = mswd.pwd_intake.person_guid  
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id  
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id  
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id  
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id  
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id  
                            LEFT JOIN mswd.pwd ON mswd.pwd.person_guid = mswd.pwd_intake.person_guid  
                            WHERE mswd.pwd_intake.status = 'Deleted' AND mswd.pwd.status = 'Active' ORDER BY general.person.age DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("pwd-intake");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.pwd_intake pwd_intake
                    INNER JOIN general.person ON general.person.person_guid = pwd_intake.person_guid
                    WHERE pwd_intake.status = 'Deleted'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.pwd_intake pwd_intake
                    LEFT JOIN general.person ON general.person.person_guid = pwd_intake.person_guid
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
                filterStatus = "mswd.pwd_intake.status = 'Active' AND mswd.pwd.status = 'Active'";
            }
            else if (status_deleted_id != 0)
            {
                filterStatus = "mswd.pwd_intake.status = 'Deleted' AND mswd.pwd.status = 'Active'";
            }

            if (filter_type_status_id == 1)
            {
                dateFilter = "AND MONTH(mswd.pwd_intake.application_date) = '" + this_month + "'";
            }
            else if (filter_type_status_id == 2)
            {
                dateFilter = "AND YEAR(mswd.pwd_intake.application_date) = '" + this_year + "'";
            }
            else if (filter_type_status_id == 3)
            {
                dateFilter = "AND MONTH(mswd.pwd_intake.application_date) = '" + monthly + "' AND YEAR(mswd.pwd_intake.application_date) = '" + monthlyYear + "'";
            }
            else if (filter_type_status_id == 4)
            {
                dateFilter = "AND YEAR(mswd.pwd_intake.application_date) = '" + year_quarterly + "' AND QUARTER(mswd.pwd_intake.application_date) = '" + quarter + "'";
            }
            else if (filter_type_status_id == 5)
            {
                dateFilter = "AND YEAR(mswd.pwd_intake.application_date) = '" + yearly + "'";
            }
            else if (filter_type_status_id == 6)
            {
                dateFilter = "AND mswd.pwd_intake.application_date >= '" + from + "' AND mswd.pwd_intake.application_date <= '" + to + "'";
            }

            string sql = @"SELECT mswd.pwd_intake.pwd_intake_guid, mswd.pwd_intake.status, mswd.pwd_intake.person_guid, mswd.pwd_intake.form_trans_no, mswd.pwd_intake.member_count, mswd.pwd_intake.application_date, general.person.first_name,  
                            general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix, general.person.gender_id, general.gender.gender_name, general.person.birth_date, general.person.place_of_birth,  
                            general.person.province_id, general.person.age, general_address.lgu_province_setup_temp.province_name,  
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.religion, general.person.profession,
                            general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.pwd_intake LEFT JOIN mswd.pwd ON mswd.pwd.person_guid = mswd.pwd_intake.person_guid WHERE " + filterStatus + @" " + dateFilter  + @") as 'count'
                            FROM mswd.pwd_intake  
                            LEFT JOIN general.person ON general.person.person_guid = mswd.pwd_intake.person_guid  
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id  
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id  
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id  
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id  
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id  
                            LEFT JOIN mswd.pwd ON mswd.pwd.person_guid = mswd.pwd_intake.person_guid  
                            WHERE " + filterStatus + @" " + dateFilter  + @" ORDER BY general.person.age DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("pwd-intake");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.pwd_intake pwd_intake
                    INNER JOIN general.person ON general.person.person_guid = pwd_intake.person_guid
                    WHERE pwd_intake.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.pwd_intake pwd_intake
                    LEFT JOIN general.person ON general.person.person_guid = pwd_intake.person_guid
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
            string sql = @"SELECT mswd.pwd_intake_details.person_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix,  
                            general.person.gender_id, general.gender.gender_name, general.person.birth_date, general.person.place_of_birth,  
                            general.person.province_id, general_address.lgu_province_setup_temp.province_name,  
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.civil_status_id, general.civil_status.civil_status_name,  
                            general.person.religion, general.person.profession, general.person.age,  
                            mswd.pwd_intake_details.main_guid, mswd.pwd_intake_details.relation, mswd.pwd_intake_details.educational_attainment, mswd.pwd_intake_details.occupation_income, mswd.pwd_intake_details.occupation
                            FROM mswd.pwd_intake_details
                            LEFT JOIN general.person ON general.person.person_guid = mswd.pwd_intake_details.person_guid
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            WHERE mswd.pwd_intake_details.main_guid = '" + GUID + "' ";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public object GetPwdIntake(string pwd_intake_guid)
        {
            string sql = @"SELECT mswd.pwd_intake.pwd_intake_guid, mswd.pwd_intake.person_guid, mswd.pwd_intake.owner, mswd.pwd_intake.renter,
                            mswd.pwd_intake.estimated_damaged, mswd.pwd_intake.if_distressed, mswd.pwd_intake.physical_disability, general.person.educational_attainment,
                            mswd.pwd_intake.type_of_disability_id, mswd.general_intake_disability.disability_name, mswd.pwd_intake.sources_of_income,
                            mswd.pwd_intake.total_family_income, mswd.pwd_intake.no_of_hectares, mswd.pwd_intake.crops_planted, mswd.pwd_intake.area_of_location,
                            mswd.pwd_intake.other_sources_of_income, mswd.pwd_intake.form_trans_no, general.person.profession,
                            general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix,
                            general.person.gender_id, general.gender.gender_name, general.person.civil_status_id, general.civil_status.civil_status_name,
                            general.person.birth_date, general.person.place_of_birth, general.person.province_id, general_address.lgu_province_setup_temp.province_name,
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name,
                            general.person.religion, general.person.age, general.person.street, mswd.pwd_intake.application_date, general.educational_type.educational_name
                            FROM mswd.pwd_intake
                            LEFT JOIN mswd.general_intake_disability ON mswd.general_intake_disability.id = mswd.pwd_intake.type_of_disability_id
                            LEFT JOIN general.person ON general.person.person_guid = mswd.pwd_intake.person_guid
                            LEFT JOIN mswd.pwd ON mswd.pwd.person_guid = mswd.pwd_intake.person_guid
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            LEFT JOIN general.educational_type ON general.educational_type.id = general.person.educational_attainment
                            WHERE mswd.pwd.status = 'Active' AND mswd.pwd_intake.status = 'Active' AND mswd.pwd_intake.pwd_intake_guid = @pwd_intake_guid ";
            var obj = (PwdIntakeModel)QueryModule.DataObject<PwdIntakeModel>(sql, new { pwd_intake_guid = pwd_intake_guid });

            sql = @"SELECT mswd.pwd_intake_details.main_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix,  
                    general.person.age, general.person.civil_status_id, general.civil_status.civil_status_name, mswd.pwd_intake_details.person_guid, mswd.pwd_intake_details.educational_attainment,  
                    mswd.pwd_intake_details.relation, mswd.pwd_intake_details.occupation, mswd.pwd_intake_details.occupation_income, general.educational_type.educational_name
                    FROM mswd.pwd_intake_details  
                    LEFT JOIN general.person ON general.person.person_guid = mswd.pwd_intake_details.person_guid  
                    LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id  
                    LEFT JOIN general.educational_type ON general.educational_type.id = mswd.pwd_intake_details.educational_attainment
                    WHERE mswd.pwd_intake_details.main_guid = '" + obj.pwd_intake_guid + "' ";
            obj.details = (List<PwdIntakeDetails>)QueryModule.DataSource<PwdIntakeDetails>(sql);

            return obj;
        }

        public object GetPersonAdd(string GUID)
        {
            string sql = @"SELECT mswd.pwd.person_guid, mswd.pwd.pwd_guid, general.person.first_name, general.person.middle_name, general.person.last_name, 
                            general.person.suffix, general.person.prefix, general.person.profession, general.person.educational_attainment,
                            general.person.gender_id, general.gender.gender_name, general.person.civil_status_id, general.civil_status.civil_status_name,
                            general.person.birth_date, general.person.place_of_birth, general.person.province_id, general_address.lgu_province_setup_temp.province_name,
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name,
                            general.person.religion, general.person.age, general.person.person_image, general.person.street
                            FROM mswd.pwd 
                            LEFT JOIN general.person ON general.person.person_guid = mswd.pwd.person_guid
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            WHERE mswd.pwd.status = 'Active' AND mswd.pwd.person_guid = '" + GUID + "' ";
            var obj = (PwdIntakeModel)QueryModule.DataObject<PwdIntakeModel>(sql);

            if (obj == null)
                return null;

            sql = @"SELECT mswd.pwd_details.main_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix,  
                    general.person.age, general.person.civil_status_id, general.civil_status.civil_status_name, mswd.pwd_details.person_guid, mswd.pwd_details.educational_attainment,  
                    mswd.pwd_details.relation, mswd.pwd_details.occupational_skills, mswd.pwd_details.occupation_income
                    from mswd.pwd_details
                    LEFT JOIN general.person ON general.person.person_guid = mswd.pwd_details.person_guid
                    LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                    WHERE mswd.pwd_details.main_guid = '" + obj.pwd_guid + "' ";
            obj.details = (List<PwdIntakeDetails>)QueryModule.DataSource<PwdIntakeDetails>(sql);

            return obj;
        }

        public List<object> GetHistoryLogs(string GUID)
        {
            string sql = @"SELECT mswd.pwd_intake_logs.main_guid, mswd.pwd_intake.application_date, mswd.pwd_intake_logs.user_id, mswd.pwd_intake.form_trans_no, mswd.pwd_intake.status,
                            mswd.pwd_intake.owner, mswd.pwd_intake.renter, mswd.pwd_intake.estimated_damaged, mswd.pwd_intake.if_distressed, mswd.pwd_intake.physical_disability,
                            mswd.pwd_intake.type_of_disability_id, mswd.general_intake_disability.disability_name, mswd.pwd_intake.sources_of_income, mswd.pwd_intake.total_family_income,
                            mswd.pwd_intake.no_of_hectares, mswd.pwd_intake.crops_planted, mswd.pwd_intake.area_of_location, mswd.pwd_intake.other_sources_of_income
                            FROM mswd.pwd_intake_logs  
                            LEFT JOIN mswd.pwd_intake ON mswd.pwd_intake.pwd_intake_guid = mswd.pwd_intake_logs.main_guid  
                            LEFT JOIN mswd.general_intake_disability ON mswd.general_intake_disability.id = mswd.pwd_intake.type_of_disability_id
                            WHERE mswd.pwd_intake_logs.main_guid = '" + GUID + "' ORDER BY mswd.pwd_intake.id DESC";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public bool Insert(PwdIntakeModel model)
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
                    model.form_trans_no = generateFormNumber("pwd-intake");
                    model.pwd_intake_guid = Guid.NewGuid().ToString();
                    model.status = "Active";

                    string sql = "insert into mswd.pwd_intake (`pwd_intake_guid`, `person_guid`, `owner`, `renter`, `estimated_damaged`, `if_distressed`, `physical_disability`, `type_of_disability_id`, " +
                        "`sources_of_income`, `total_family_income`, `no_of_hectares`, `crops_planted`, `area_of_location`, `other_sources_of_income`, `status`, `form_trans_no`, `member_count`) " +
                        "VALUES(@pwd_intake_guid, @person_guid, @owner, @renter, @estimated_damaged, @if_distressed, @physical_disability, @type_of_disability_id, @sources_of_income, " +
                        "@total_family_income, @no_of_hectares, @crops_planted, @area_of_location, @other_sources_of_income, @status, @form_trans_no, @member_count)";
                    int res = (int)QueryModule.DataObject<int>(sql, model);

                    if (model.details.Count > 0)
                    {
                        foreach (var dt in model.details)
                        {
                            dt.main_guid = model.pwd_intake_guid;
                            sql = "insert into mswd.pwd_intake_details (`main_guid`, `person_guid`, `relation`, `educational_attainment`, `occupation_income`, `occupation`) " +
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
