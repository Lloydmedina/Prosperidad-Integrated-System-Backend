using eegs_back_end.Admin.PwdSetup.Model;
using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.PwdSetup.Repository
{
    public interface IPwdRepository :IGlobalInterface
    {
        bool Insert(PwdModel model);
        List<object> GetListOfDisability();
        List<object> GetPhilhealthMembership();
        List<object> GetList();
        List<object> GetAllList();
        List<object> GetListDeleted();
        List<object> GetListGenerated(int? filter_type_status_id = 0, int? status_id = 0, int? status_deleted_id = 0, string? this_month = "", string? this_year = "", string? monthly = "", string? monthlyYear = "", string? year_quarterly = "", int? quarter = 0, string? yearly = "", string? from = "", string? to = "");
        List<object> GetEducationalType();
        List<object> GetOccupationList();
        List<object> GetCauseOfDisability();
        List<object> GetEmploymentStatus();
        List<object> GetEmploymentType();
        List<object> GetEmployerType();
        List<object> GetDetails(string GUID);
        object GetPersonAdd(string GUID);
        List<object> GetHistoryLogs(string GUID);
        bool Edit(string GUID, PwdModel model);
        bool Delete(string GUID);
        object GetPwd(string Pwd_guid);
    }
    public class PwdRepository : FormNumberGenerator, IPwdRepository
    {
        public bool Edit(string GUID, PwdModel model)
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
                    model.pwd_guid = GUID;
                    model.status = "Active";

                    var sql = "UPDATE mswd.pwd SET status='Trash' " +
                        "WHERE mswd.pwd.pwd_guid = '" + GUID + "'   ";
                    int res = (int)QueryModule.DataObject<int>(sql, model);

                    sql = @"INSERT INTO mswd.pwd (`pwd_guid`, `person_guid`, `form_trans_no`, `status`, `member_count`, `type_of_disability_id`, `cause_of_disability_id`, `employment_status_id`, `employment_type_id`, `employer_type_id`, `occupation_id`, 
                        `blood_type_id`, `sss_no`, `gsis_no`, `pagibig_no`, `philhealth_no`, `philhealth_membership`, `organization`, `contact_person`, `office_address`, `tel_no`, `reporting_unit`, `mobile_no`, `specify_disability`, `occupation_others`) 
                        VALUES(@pwd_guid, @person_guid, @form_trans_no, @status, @member_count, @type_of_disability_id, @cause_of_disability_id, @employment_status_id, @employment_type_id, @employer_type_id, @occupation_id, 
                        @blood_type_id, @sss_no, @gsis_no, @pagibig_no, @philhealth_no, @philhealth_membership, @organization, @contact_person, @office_address, @tel_no, @reporting_unit, @mobile_no, @specify_disability, @occupation_others )";
                    QueryModule.Execute<int>(sql, model);

                    sql = "DELETE FROM mswd.pwd_details WHERE mswd.pwd_details.main_guid = '" + GUID + "' ";
                    QueryModule.Execute<int>(sql, model);

                    sql = "DELETE FROM mswd.pwd_logs WHERE mswd.pwd_logs.main_guid = '" + GUID + "' ";
                    QueryModule.Execute<int>(sql, model);

                    sql = "INSERT INTO mswd.pwd_logs (`main_guid`, `user_id`) VALUES('" + model.pwd_guid + "', '" + GlobalObject.user_id + "')";
                    QueryModule.Execute<int>(sql, model);

                    if (model.family_details.Count > 0)
                    {
                        foreach (var dt in model.family_details)
                        {
                            dt.main_guid = model.pwd_guid;
                            sql = @"INSERT INTO mswd.Pwd_details (`main_guid`, `person_guid`, `relation`, `educational_attainment`, `occupational_skills`, `occupation_income`)
                                VALUES(@main_guid, @person_guid, @relation, @educational_attainment, @occupational_skills, @occupation_income)";
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
            string sql = "UPDATE mswd.Pwd SET status = 'Deleted' where mswd.Pwd.Pwd_guid = @Pwd_guid";
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

        public List<object> GetCauseOfDisability()
        {
            string sql = "SELECT * FROM mswd.cause_of_disability";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public List<object> GetPhilhealthMembership()
        {
            string sql = "SELECT * FROM mswd.philhealth_membership";
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

        public List<object> GetOccupationList()
        {
            string sql = "SELECT * FROM mswd.occupation_list";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public List<object> GetEmploymentStatus()
        {
            string sql = "SELECT * FROM mswd.employment_status_type";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public List<object> GetEmploymentType()
        {
            string sql = "SELECT * FROM mswd.employment_type";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public List<object> GetEmployerType()
        {
            string sql = "SELECT * FROM mswd.employer_type";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public List<object> GetList()
        {
            string sql = @"SELECT mswd.pwd.pwd_guid, mswd.pwd.person_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, 
                            general.person.suffix, general_address.lgu_province_setup_temp.province_name, general_address.lgu_city_mun_setup_temp.city_mun_name, general_address.lgu_brgy_setup_temp.brgy_name,
                            mswd.pwd.form_trans_no, general.person.birth_date, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.gender_id, 
                            general.gender.gender_name, mswd.pwd.application_date, mswd.pwd.member_count, mswd.pwd.status, mswd.pwd.type_of_disability_id, mswd.general_intake_disability.disability_name,
                            general.person.age, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.pwd WHERE mswd.pwd.status = 'Active') as 'count'
                            FROM mswd.pwd
                            LEFT JOIN general.person ON general.person.person_guid = mswd.pwd.person_guid
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            LEFT JOIN mswd.general_intake_disability ON mswd.general_intake_disability.id = mswd.pwd.type_of_disability_id
                            WHERE mswd.pwd.status = 'Active' ORDER BY general.person.age DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("pwd-registration");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.pwd pwd
                    INNER JOIN general.person ON general.person.person_guid = pwd.person_guid
                    WHERE pwd.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.pwd pwd
                    LEFT JOIN general.person ON general.person.person_guid = pwd.person_guid
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
            string sql = @"SELECT mswd.pwd.pwd_guid, mswd.pwd.person_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, 
                            general.person.suffix, general_address.lgu_province_setup_temp.province_name, general_address.lgu_city_mun_setup_temp.city_mun_name, general_address.lgu_brgy_setup_temp.brgy_name,
                            mswd.pwd.form_trans_no, general.person.birth_date, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.gender_id, 
                            general.gender.gender_name, mswd.pwd.application_date, mswd.pwd.member_count, mswd.pwd.status, mswd.pwd.type_of_disability_id, mswd.general_intake_disability.disability_name,
                            general.person.age, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.pwd WHERE mswd.pwd.status = 'Active' OR mswd.pwd.status = 'Deleted') as 'count'
                            FROM mswd.pwd
                            LEFT JOIN general.person ON general.person.person_guid = mswd.pwd.person_guid
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            LEFT JOIN mswd.general_intake_disability ON mswd.general_intake_disability.id = mswd.pwd.type_of_disability_id
                            WHERE mswd.pwd.status = 'Active' OR mswd.pwd.status = 'Deleted' ORDER BY general.person.age DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("pwd-registration");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.pwd pwd
                    INNER JOIN general.person ON general.person.person_guid = pwd.person_guid
                    WHERE pwd.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.pwd pwd
                    LEFT JOIN general.person ON general.person.person_guid = pwd.person_guid
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

            string sql = @"SELECT mswd.pwd.pwd_guid, mswd.pwd.person_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, 
                            general.person.suffix, general_address.lgu_province_setup_temp.province_name, general_address.lgu_city_mun_setup_temp.city_mun_name, general_address.lgu_brgy_setup_temp.brgy_name,
                            mswd.pwd.form_trans_no, general.person.birth_date, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.gender_id, 
                            general.gender.gender_name, mswd.pwd.application_date, mswd.pwd.member_count, mswd.pwd.status, mswd.pwd.type_of_disability_id, mswd.general_intake_disability.disability_name,
                            general.person.age, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.pwd WHERE mswd.pwd.status = 'Deleted') as 'count'
                            FROM mswd.pwd
                            LEFT JOIN general.person ON general.person.person_guid = mswd.pwd.person_guid
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            LEFT JOIN mswd.general_intake_disability ON mswd.general_intake_disability.id = mswd.pwd.type_of_disability_id
                            WHERE mswd.pwd.status = 'Deleted' ORDER BY general.person.age DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("pwd-registration");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.pwd pwd
                    INNER JOIN general.person ON general.person.person_guid = pwd.person_guid
                    WHERE pwd.status = 'Deleted'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.pwd pwd
                    LEFT JOIN general.person ON general.person.person_guid = pwd.person_guid
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
                filterStatus = "mswd.pwd.status = 'Active'";
            }
            else if (status_deleted_id != 0)
            {
                filterStatus = "mswd.pwd.status = 'Deleted'";
            }

            if (filter_type_status_id == 1)
            {
                dateFilter = "AND MONTH(mswd.pwd.application_date) = '" + this_month + "'";
            }
            else if (filter_type_status_id == 2)
            {
                dateFilter = "AND YEAR(mswd.pwd.application_date) = '" + this_year + "'";
            }
            else if (filter_type_status_id == 3)
            {
                dateFilter = "AND MONTH(mswd.pwd.application_date) = '" + monthly + "' AND YEAR(mswd.pwd.application_date) = '" + monthlyYear + "'";
            }
            else if (filter_type_status_id == 4)
            {
                dateFilter = "AND YEAR(mswd.pwd.application_date) = '" + year_quarterly + "' AND QUARTER(mswd.pwd.application_date) = '" + quarter + "'";
            }
            else if (filter_type_status_id == 5)
            {
                dateFilter = "AND YEAR(mswd.pwd.application_date) = '" + yearly + "'";
            }
            else if (filter_type_status_id == 6)
            {
                dateFilter = "AND mswd.pwd.application_date >= '" + from + "' AND mswd.pwd.application_date <= '" + to + "'";
            }

            string sql = @"SELECT mswd.pwd.pwd_guid, mswd.pwd.person_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, 
                            general.person.suffix, general_address.lgu_province_setup_temp.province_name, general_address.lgu_city_mun_setup_temp.city_mun_name, general_address.lgu_brgy_setup_temp.brgy_name,
                            mswd.pwd.form_trans_no, general.person.birth_date, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.gender_id, 
                            general.gender.gender_name, mswd.pwd.application_date, mswd.pwd.member_count, mswd.pwd.status, mswd.pwd.type_of_disability_id, mswd.general_intake_disability.disability_name,
                            general.person.age, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.pwd WHERE " + filterStatus + @" " + dateFilter  + @") as 'count'
                            FROM mswd.pwd
                            LEFT JOIN general.person ON general.person.person_guid = mswd.pwd.person_guid
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            LEFT JOIN mswd.general_intake_disability ON mswd.general_intake_disability.id = mswd.pwd.type_of_disability_id
                            WHERE " + filterStatus + @" " + dateFilter  + @" ORDER BY general.person.age DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("pwd-registration");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.pwd pwd
                    INNER JOIN general.person ON general.person.person_guid = pwd.person_guid
                    WHERE pwd.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.pwd pwd
                    LEFT JOIN general.person ON general.person.person_guid = pwd.person_guid
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
            string sql = @"SELECT mswd.pwd_details.person_guid, mswd.pwd_details.relation, mswd.pwd_details.educational_attainment, mswd.pwd_details.occupational_skills,
                            general.person.prefix, general.person.suffix, general.person.first_name, general.person.middle_name, general.person.last_name,
                            general.person.gender_id, general.gender.gender_name, general.person.age
                            FROM mswd.pwd_details
                            LEFT JOIN general.person ON general.person.person_guid = mswd.pwd_details.person_guid
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            WHERE mswd.pwd_details.main_guid = '" + GUID + "' ";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public object GetPwd(string pwd_guid)
        {
            string sql = @"SELECT mswd.pwd.pwd_guid, mswd.pwd.person_guid, general.person.prefix, general.person.suffix, general.person.first_name, general.person.middle_name, general.person.last_name,
                            mswd.pwd.form_trans_no, mswd.pwd.application_date, mswd.pwd.status, mswd.pwd.member_count, general.person.birth_date, general.person.place_of_birth, general.person.gender_id, 
                            general.gender.gender_name, general.person.age, general.person.province_id, general_address.lgu_province_setup_temp.province_name, general.person.citmun_id, 
                            general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.street, general.person.civil_status_id, 
                            general.civil_status.civil_status_name, mswd.pwd.type_of_disability_id, mswd.general_intake_disability.disability_name, mswd.pwd.cause_of_disability_id, mswd.cause_of_disability.cause_of_disability_name,
                            mswd.pwd.employment_status_id, mswd.employment_status_type.employment_status_name, mswd.pwd.employment_type_id, mswd.employment_type.employment_type_name, mswd.pwd.employer_type_id,
                            mswd.employer_type.employer_type_name, mswd.pwd.occupation_id, mswd.occupation_list.occupation_name, mswd.pwd.blood_type_id, general.blood_type.blood_type_name, mswd.pwd.sss_no,
                            mswd.pwd.gsis_no, mswd.pwd.pagibig_no, mswd.pwd.philhealth_no, mswd.pwd.philhealth_membership, mswd.pwd.organization, mswd.pwd.contact_person, mswd.pwd.office_address,
                            mswd.pwd.tel_no, mswd.pwd.reporting_unit, mswd.pwd.mobile_no, mswd.pwd.specify_disability, mswd.pwd.occupation_others, general.person.region_id,
                            general_address.lgu_region_setup_temp.reg_code, general_address.lgu_region_setup_temp.reg_name, general.person.educational_attainment, general.person.telephone_no,
                            general.person.phone_no, general.person.email_address
                            FROM mswd.pwd
                            LEFT JOIN general.person ON general.person.person_guid = mswd.pwd.person_guid
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            LEFT JOIN general_address.lgu_region_setup_temp ON general_address.lgu_region_setup_temp.SysPK_region = general.person.region_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN mswd.general_intake_disability ON mswd.general_intake_disability.id = mswd.pwd.type_of_disability_id
                            LEFT JOIN mswd.cause_of_disability ON mswd.cause_of_disability.id = mswd.pwd.cause_of_disability_id
                            LEFT JOIN mswd.employment_status_type ON mswd.employment_status_type.id = mswd.pwd.employment_status_id
                            LEFT JOIN mswd.employment_type ON mswd.employment_type.id = mswd.pwd.employment_type_id
                            LEFT JOIN mswd.employer_type ON mswd.employer_type.id = mswd.pwd.employer_type_id
                            LEFT JOIN mswd.occupation_list ON mswd.occupation_list.id = mswd.pwd.occupation_id
                            LEFT JOIN general.blood_type ON general.blood_type.blood_type_id = mswd.pwd.blood_type_id
                            WHERE mswd.pwd.status = 'Active' AND mswd.pwd.pwd_guid = @pwd_guid ";
            var obj = (PwdModel)QueryModule.DataObject<PwdModel>(sql, new { pwd_guid = pwd_guid });

            sql = @"SELECT mswd.pwd_details.main_guid, mswd.pwd_details.person_guid, mswd.pwd_details.relation, mswd.pwd_details.educational_attainment,
                    mswd.pwd_details.occupational_skills, general.person.prefix, general.person.suffix, general.person.first_name,
                    general.person.middle_name, general.person.last_name, general.person.age, general.person.gender_id, general.gender.gender_name, mswd.pwd_details.occupation_income,
                    general.person.civil_status_id, general.civil_status.civil_status_name
                    FROM mswd.pwd_details
                    LEFT JOIN general.person ON general.person.person_guid = mswd.pwd_details.person_guid
                    LEFT JOIN general.gender ON general.gender.gender_id =  general.person.gender_id
                    LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                    WHERE mswd.pwd_details.main_guid = '" + obj.pwd_guid + "' ";
            obj.family_details = (List<PwdDetails>)QueryModule.DataSource<PwdDetails>(sql);

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
            var obj = (PwdModel)QueryModule.DataObject<PwdModel>(sql);

            if (obj == null)
                return null;

            //sql = "select mswd.family_composition_details.main_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix, " +
            //    "general.person.age, general.person.civil_status_id, general.civil_status.civil_status_name, mswd.family_composition_details.person_guid, mswd.family_composition_details.status, mswd.family_composition_details.educational_attainment, " +
            //    "mswd.family_composition_details.relation, mswd.family_composition_details.occupation, mswd.family_composition_details.occupation_income " +
            //    "from mswd.family_composition_details " +
            //    "INNER JOIN general.person ON general.person.person_guid = mswd.family_composition_details.person_guid " +
            //    "INNER JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id " +
            //    "where mswd.family_composition_details.main_guid = '" + obj.family_composition_guid + "' ";
            //obj.details = (List<PwdDetails>)QueryModule.DataSource<PwdDetails>(sql);

            return obj;
        }

        public List<object> GetHistoryLogs(string GUID)
        {
            string sql = @"SELECT mswd.pwd_logs.main_guid, mswd.pwd.application_date, mswd.pwd_logs.user_id, mswd.pwd.form_trans_no,
                            mswd.pwd.status, mswd.pwd.person_guid, general.person.prefix, general.person.suffix, general.person.first_name,
                            general.person.middle_name, general.person.last_name
                            FROM mswd.pwd_logs  
                            LEFT JOIN mswd.pwd ON mswd.pwd.pwd_guid = mswd.pwd_logs.main_guid  
                            LEFT JOIN general.person ON general.person.person_guid = mswd.pwd.person_guid
                            WHERE mswd.pwd_logs.main_guid = '" + GUID + "' ORDER BY mswd.pwd.id DESC";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public bool Insert(PwdModel model)
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
                    model.form_trans_no = generateFormNumber("pwd-registration");
                    model.pwd_guid = Guid.NewGuid().ToString();
                    model.status = "Active";

                    string sql = @"INSERT INTO mswd.pwd (`pwd_guid`, `person_guid`, `form_trans_no`, `status`, `member_count`, `type_of_disability_id`, `cause_of_disability_id`, `employment_status_id`, `employment_type_id`, `employer_type_id`, `occupation_id`, 
                        `blood_type_id`, `sss_no`, `gsis_no`, `pagibig_no`, `philhealth_no`, `philhealth_membership`, `organization`, `contact_person`, `office_address`, `tel_no`, `reporting_unit`, `mobile_no`, `specify_disability`, `occupation_others`) 
                        VALUES(@pwd_guid, @person_guid, @form_trans_no, @status, @member_count, @type_of_disability_id, @cause_of_disability_id, @employment_status_id, @employment_type_id, @employer_type_id, @occupation_id, 
                        @blood_type_id, @sss_no, @gsis_no, @pagibig_no, @philhealth_no, @philhealth_membership, @organization, @contact_person, @office_address, @tel_no, @reporting_unit, @mobile_no, @specify_disability, @occupation_others )";
                    int res = (int)QueryModule.DataObject<int>(sql, model);

                    if (model.family_details.Count > 0)
                    {
                        foreach (var dt in model.family_details)
                        {
                            dt.main_guid = model.pwd_guid;
                            sql = @"INSERT INTO mswd.pwd_details (`main_guid`, `person_guid`, `relation`, `educational_attainment`, `occupational_skills`, `occupation_income`)
                                VALUES(@main_guid, @person_guid, @relation, @educational_attainment, @occupational_skills, @occupation_income)";
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
