using eegs_back_end.Admin.SoloParentIntakeSetup.Model;
using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.SoloParentIntakeSetup.Repository
{
    public interface ISoloParentIntakeRepository :IGlobalInterface
    {
        bool Insert(SoloParentIntakeModel model);
        List<object> GetListOfDisability();
        List<object> GetList();
        List<object> GetAllList();
        List<object> GetListDeleted();
        List<object> GetListGenerated(int? filter_type_status_id = 0, int? status_id = 0, int? status_deleted_id = 0, string? this_month = "", string? this_year = "", string? monthly = "", string? monthlyYear = "", string? year_quarterly = "", int? quarter = 0, string? yearly = "", string? from = "", string? to = "");
        List<object> GetEducationalType();
        List<object> GetDetails(string GUID);
        object GetPersonAdd(string GUID);
        List<object> GetHistoryLogs(string GUID);
        bool Edit(string GUID, SoloParentIntakeModel model);
        bool Delete(string GUID);
        object GetSoloParentIntake(string SoloParentIntake_guid);
    }
    public class SoloParentIntakeRepository : FormNumberGenerator, ISoloParentIntakeRepository
    {
        public bool Edit(string GUID, SoloParentIntakeModel model)
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
                    model.solo_parent_intake_guid = GUID;
                    model.status = "Active";

                    var sql = "UPDATE mswd.solo_parent_intake SET status='Trash' " +
                        "WHERE mswd.solo_parent_intake.solo_parent_intake_guid = '" + GUID + "'   ";
                    int res = (int)QueryModule.DataObject<int>(sql, model);

                    sql = "insert into mswd.solo_parent_intake (`solo_parent_intake_guid`, `person_guid`, `owner`, `renter`, `estimated_damaged`, `if_distressed`, `physical_disability`, `type_of_disability_id`, " +
                        "`sources_of_income`, `total_family_income`, `no_of_hectares`, `crops_planted`, `area_of_location`, `other_sources_of_income`, `status`, `form_trans_no`, `member_count`) " +
                        "VALUES(@solo_parent_intake_guid, @person_guid, @owner, @renter, @estimated_damaged, @if_distressed, @physical_disability, @type_of_disability_id, @sources_of_income, " +
                        "@total_family_income, @no_of_hectares, @crops_planted, @area_of_location, @other_sources_of_income, @status, @form_trans_no, @member_count)";
                    QueryModule.Execute<int>(sql, model);

                    sql = "DELETE FROM mswd.solo_parent_intake_details WHERE mswd.solo_parent_intake_details.main_guid = '" + GUID + "' ";
                    QueryModule.Execute<int>(sql, model);

                    sql = "DELETE FROM mswd.solo_parent_intake_logs WHERE mswd.solo_parent_intake_logs.main_guid = '" + GUID + "' ";
                    QueryModule.Execute<int>(sql, model);

                    sql = "INSERT INTO mswd.solo_parent_intake_logs (`main_guid`, `user_id`) VALUES('" + model.solo_parent_intake_guid + "', '" + GlobalObject.user_id + "')";
                    QueryModule.Execute<int>(sql, model);

                    if (model.details.Count > 0)
                    {
                        foreach (var dt in model.details)
                        {
                            dt.main_guid = model.solo_parent_intake_guid;
                            sql = "insert into mswd.solo_parent_intake_details (`main_guid`, `person_guid`, `relation`, `educational_attainment`, `occupation_income`, `occupation`) " +
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
            string sql = "UPDATE mswd.solo_parent_intake SET status = 'Deleted' where mswd.solo_parent_intake.solo_parent_intake_guid = @solo_parent_intake_guid";
            if (InsertUpdate.ExecuteSql(sql, new { solo_parent_intake_guid = GUID }))
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
            string sql = @"SELECT mswd.solo_parent_intake.solo_parent_intake_guid, mswd.solo_parent_intake.status, mswd.solo_parent_intake.person_guid, mswd.solo_parent_intake.form_trans_no, mswd.solo_parent_intake.member_count, mswd.solo_parent_intake.application_date, general.person.first_name,  
                            general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix, general.person.gender_id, general.gender.gender_name, general.person.birth_date, general.person.place_of_birth,  
                            general.person.province_id, general.person.age, general_address.lgu_province_setup_temp.province_name,  
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.religion, general.person.profession,
                            general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.solo_parent_intake LEFT JOIN mswd.solo_parent ON mswd.solo_parent.person_guid = mswd.solo_parent_intake.person_guid WHERE mswd.solo_parent_intake.status = 'Active' AND mswd.solo_parent.status = 'Active') as 'count'
                            FROM mswd.solo_parent_intake  
                            LEFT JOIN general.person ON general.person.person_guid = mswd.solo_parent_intake.person_guid  
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id  
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id  
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id  
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id  
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id  
                            LEFT JOIN mswd.solo_parent ON mswd.solo_parent.person_guid = mswd.solo_parent_intake.person_guid  
                            WHERE mswd.solo_parent_intake.status = 'Active' AND mswd.solo_parent.status = 'Active' ORDER BY mswd.solo_parent_intake.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("solo-parent-intake");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.solo_parent_intake sp_intake
                    LEFT JOIN general.person ON general.person.person_guid = sp_intake.person_guid
                    WHERE sp_intake.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.solo_parent_intake sp_intake
                    LEFT JOIN general.person ON general.person.person_guid = sp_intake.person_guid
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
            string sql = @"SELECT mswd.solo_parent_intake.solo_parent_intake_guid, mswd.solo_parent_intake.status, mswd.solo_parent_intake.person_guid, mswd.solo_parent_intake.form_trans_no, mswd.solo_parent_intake.member_count, mswd.solo_parent_intake.application_date, general.person.first_name,  
                            general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix, general.person.gender_id, general.gender.gender_name, general.person.birth_date, general.person.place_of_birth,  
                            general.person.province_id, general.person.age, general_address.lgu_province_setup_temp.province_name,  
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.religion, general.person.profession,
                            general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.solo_parent_intake LEFT JOIN mswd.solo_parent ON mswd.solo_parent.person_guid = mswd.solo_parent_intake.person_guid WHERE mswd.solo_parent_intake.status = 'Active' AND mswd.solo_parent.status = 'Active' OR mswd.solo_parent_intake.status = 'Deleted') as 'count'
                            FROM mswd.solo_parent_intake  
                            LEFT JOIN general.person ON general.person.person_guid = mswd.solo_parent_intake.person_guid  
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id  
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id  
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id  
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id  
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id  
                            LEFT JOIN mswd.solo_parent ON mswd.solo_parent.person_guid = mswd.solo_parent_intake.person_guid  
                            WHERE mswd.solo_parent_intake.status = 'Active' AND mswd.solo_parent.status = 'Active' OR mswd.solo_parent_intake.status = 'Deleted' ORDER BY mswd.solo_parent_intake.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("solo-parent-intake");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.solo_parent_intake sp_intake
                    LEFT JOIN general.person ON general.person.person_guid = sp_intake.person_guid
                    WHERE sp_intake.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.solo_parent_intake sp_intake
                    LEFT JOIN general.person ON general.person.person_guid = sp_intake.person_guid
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

            string sql = @"SELECT mswd.solo_parent_intake.solo_parent_intake_guid, mswd.solo_parent_intake.status, mswd.solo_parent_intake.person_guid, mswd.solo_parent_intake.form_trans_no, mswd.solo_parent_intake.member_count, mswd.solo_parent_intake.application_date, general.person.first_name,  
                            general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix, general.person.gender_id, general.gender.gender_name, general.person.birth_date, general.person.place_of_birth,  
                            general.person.province_id, general.person.age, general_address.lgu_province_setup_temp.province_name,  
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.religion, general.person.profession,
                            general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.solo_parent_intake LEFT JOIN mswd.solo_parent ON mswd.solo_parent.person_guid = mswd.solo_parent_intake.person_guid WHERE mswd.solo_parent_intake.status = 'Deleted' AND mswd.solo_parent.status = 'Active') as 'count'
                            FROM mswd.solo_parent_intake  
                            LEFT JOIN general.person ON general.person.person_guid = mswd.solo_parent_intake.person_guid  
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id  
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id  
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id  
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id  
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id  
                            LEFT JOIN mswd.solo_parent ON mswd.solo_parent.person_guid = mswd.solo_parent_intake.person_guid  
                            WHERE mswd.solo_parent_intake.status = 'Deleted' AND mswd.solo_parent.status = 'Active' ORDER BY mswd.solo_parent_intake.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("solo-parent-intake");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.solo_parent_intake sp_intake
                    LEFT JOIN general.person ON general.person.person_guid = sp_intake.person_guid
                    WHERE sp_intake.status = 'Deleted'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.solo_parent_intake sp_intake
                    LEFT JOIN general.person ON general.person.person_guid = sp_intake.person_guid
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
                filterStatus = "mswd.solo_parent_intake.status = 'Active' AND mswd.solo_parent.status = 'Active'";
            }
            else if (status_deleted_id != 0)
            {
                filterStatus = "mswd.solo_parent_intake.status = 'Deleted' AND mswd.solo_parent.status = 'Active'";
            }

            if (filter_type_status_id == 1)
            {
                dateFilter = "AND MONTH(mswd.solo_parent_intake.application_date) = '" + this_month + "'";
            }
            else if (filter_type_status_id == 2)
            {
                dateFilter = "AND YEAR(mswd.solo_parent_intake.application_date) = '" + this_year + "'";
            }
            else if (filter_type_status_id == 3)
            {
                dateFilter = "AND MONTH(mswd.solo_parent_intake.application_date) = '" + monthly + "' AND YEAR(mswd.solo_parent_intake.application_date) = '" + monthlyYear + "'";
            }
            else if (filter_type_status_id == 4)
            {
                dateFilter = "AND YEAR(mswd.solo_parent_intake.application_date) = '" + year_quarterly + "' AND QUARTER(mswd.solo_parent_intake.application_date) = '" + quarter + "'";
            }
            else if (filter_type_status_id == 5)
            {
                dateFilter = "AND YEAR(mswd.solo_parent_intake.application_date) = '" + yearly + "'";
            }
            else if (filter_type_status_id == 6)
            {
                dateFilter = "AND mswd.solo_parent_intake.application_date >= '" + from + "' AND mswd.solo_parent_intake.application_date <= '" + to + "'";
            }

            string sql = @"SELECT mswd.solo_parent_intake.solo_parent_intake_guid, mswd.solo_parent_intake.status, mswd.solo_parent_intake.person_guid, mswd.solo_parent_intake.form_trans_no, mswd.solo_parent_intake.member_count, mswd.solo_parent_intake.application_date, general.person.first_name,  
                            general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix, general.person.gender_id, general.gender.gender_name, general.person.birth_date, general.person.place_of_birth,  
                            general.person.province_id, general.person.age, general_address.lgu_province_setup_temp.province_name,  
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.religion, general.person.profession,
                            general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.solo_parent_intake LEFT JOIN mswd.solo_parent ON mswd.solo_parent.person_guid = mswd.solo_parent_intake.person_guid WHERE " + filterStatus + @" " + dateFilter  + @") as 'count'
                            FROM mswd.solo_parent_intake 
                            LEFT JOIN general.person ON general.person.person_guid = mswd.solo_parent_intake.person_guid  
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id  
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id  
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id  
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id  
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id  
                            LEFT JOIN mswd.solo_parent ON mswd.solo_parent.person_guid = mswd.solo_parent_intake.person_guid  
                            WHERE " + filterStatus + @" " + dateFilter  + @" ORDER BY mswd.solo_parent_intake.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("solo-parent-intake");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.solo_parent_intake sp_intake
                    LEFT JOIN general.person ON general.person.person_guid = sp_intake.person_guid
                    WHERE sp_intake.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.solo_parent_intake sp_intake
                    LEFT JOIN general.person ON general.person.person_guid = sp_intake.person_guid
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
            string sql = @"SELECT mswd.solo_parent_intake_details.person_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix,  
                            general.person.gender_id, general.gender.gender_name, general.person.birth_date, general.person.place_of_birth,  
                            general.person.province_id, general_address.lgu_province_setup_temp.province_name,  
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.civil_status_id, general.civil_status.civil_status_name,  
                            general.person.religion, general.person.profession, general.person.age,  
                            mswd.solo_parent_intake_details.main_guid, mswd.solo_parent_intake_details.relation, mswd.solo_parent_intake_details.educational_attainment, mswd.solo_parent_intake_details.occupation_income, mswd.solo_parent_intake_details.occupation
                            FROM mswd.solo_parent_intake_details
                            LEFT JOIN general.person ON general.person.person_guid = mswd.solo_parent_intake_details.person_guid
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            WHERE mswd.solo_parent_intake_details.main_guid = '" + GUID + "' ";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public object GetSoloParentIntake(string solo_parent_intake_guid)
        {
            string sql = @"SELECT mswd.solo_parent_intake.solo_parent_intake_guid, mswd.solo_parent_intake.person_guid, mswd.solo_parent_intake.owner, mswd.solo_parent_intake.renter,
                            mswd.solo_parent_intake.estimated_damaged, mswd.solo_parent_intake.if_distressed, mswd.solo_parent_intake.physical_disability, general.person.educational_attainment,
                            mswd.solo_parent_intake.type_of_disability_id, mswd.general_intake_disability.disability_name, mswd.solo_parent_intake.sources_of_income,
                            mswd.solo_parent_intake.total_family_income, mswd.solo_parent_intake.no_of_hectares, mswd.solo_parent_intake.crops_planted, mswd.solo_parent_intake.area_of_location,
                            mswd.solo_parent_intake.other_sources_of_income, mswd.solo_parent_intake.form_trans_no, general.person.profession,
                            general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix,
                            general.person.gender_id, general.gender.gender_name, general.person.civil_status_id, general.civil_status.civil_status_name,
                            general.person.birth_date, general.person.place_of_birth, general.person.province_id, general_address.lgu_province_setup_temp.province_name,
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name,
                            general.person.religion, general.person.age, general.person.street, mswd.solo_parent_intake.application_date, general.educational_type.educational_name
                            FROM mswd.solo_parent_intake
                            LEFT JOIN mswd.general_intake_disability ON mswd.general_intake_disability.id = mswd.solo_parent_intake.type_of_disability_id
                            LEFT JOIN general.person ON general.person.person_guid = mswd.solo_parent_intake.person_guid
                            LEFT JOIN mswd.solo_parent ON mswd.solo_parent.person_guid = mswd.solo_parent_intake.person_guid
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            LEFT JOIN general.educational_type ON general.educational_type.id = general.person.educational_attainment
                            WHERE mswd.solo_parent.status = 'Active' AND mswd.solo_parent_intake.status = 'Active' AND mswd.solo_parent_intake.solo_parent_intake_guid = @solo_parent_intake_guid ";
            var obj = (SoloParentIntakeModel)QueryModule.DataObject<SoloParentIntakeModel>(sql, new { solo_parent_intake_guid = solo_parent_intake_guid });

            sql = @"SELECT mswd.solo_parent_intake_details.main_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix,  
                    general.person.age, general.person.civil_status_id, general.civil_status.civil_status_name, mswd.solo_parent_intake_details.person_guid, mswd.solo_parent_intake_details.educational_attainment,  
                    mswd.solo_parent_intake_details.relation, mswd.solo_parent_intake_details.occupation, mswd.solo_parent_intake_details.occupation_income, general.educational_type.educational_name
                    FROM mswd.solo_parent_intake_details  
                    LEFT JOIN general.person ON general.person.person_guid = mswd.solo_parent_intake_details.person_guid  
                    LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id  
                    LEFT JOIN general.educational_type ON general.educational_type.id = mswd.solo_parent_intake_details.educational_attainment
                    WHERE mswd.solo_parent_intake_details.main_guid = '" + obj.solo_parent_intake_guid + "' ";
            obj.details = (List<SoloParentIntakeDetails>)QueryModule.DataSource<SoloParentIntakeDetails>(sql);

            return obj;
        }

        public object GetPersonAdd(string GUID)
        {
            string sql = @"SELECT mswd.solo_parent.person_guid, mswd.solo_parent.solo_parent_guid, general.person.educational_attainment, general.person.profession,
                            general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix,
                            general.person.gender_id, general.gender.gender_name, general.person.civil_status_id, general.civil_status.civil_status_name,
                            general.person.birth_date, general.person.place_of_birth, general.person.province_id, general_address.lgu_province_setup_temp.province_name,
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name,
                            general.person.religion, general.person.age, general.person.person_image, general.person.street
                            FROM mswd.solo_parent
                            LEFT JOIN general.person ON general.person.person_guid = mswd.solo_parent.person_guid
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            WHERE mswd.solo_parent.status = 'Active' AND mswd.solo_parent.person_guid = '" + GUID + "' ";
            //string sql = @"SELECT mswd.SoloParent.person_guid, mswd.SoloParent.SoloParent_guid, mswd.SoloParent.annual_income, mswd.SoloParent.fourps_beneficiary,
            //                mswd.SoloParent.ips, mswd.SoloParent.house_ownership, mswd.SoloParent.educational_attainment, mswd.SoloParent.monthly_income,
            //                mswd.SoloParent.occupation, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix,
            //                general.person.gender_id, general.gender.gender_name, general.person.civil_status_id, general.civil_status.civil_status_name,
            //                general.person.birth_date, general.person.place_of_birth, general.person.province_id, general_address.lgu_province_setup_temp.province_name,
            //                general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name,
            //                general.person.religion, general.person.age, general.person.person_image, general.person.street
            //                FROM mswd.SoloParent 
            //                INNER JOIN general.person ON general.person.person_guid = mswd.SoloParent.person_guid
            //                INNER JOIN general.gender ON general.gender.gender_id = general.person.gender_id
            //                INNER JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
            //                INNER JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
            //                INNER JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
            //                INNER JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
            //                WHERE mswd.SoloParent.status = 'Active' AND mswd.SoloParent.person_guid = '" + GUID + "' ";
            var obj = (SoloParentIntakeModel)QueryModule.DataObject<SoloParentIntakeModel>(sql);

            if (obj == null)
                return null;

            sql = @"SELECT mswd.solo_parent_details.main_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix,  
                    general.person.age, general.person.civil_status_id, general.civil_status.civil_status_name, mswd.solo_parent_details.person_guid, mswd.solo_parent_details.educational_attainment,  
                    mswd.solo_parent_details.relation, mswd.solo_parent_details.occupational_skills, mswd.solo_parent_details.occupation_income
                    from mswd.solo_parent_details
                    LEFT JOIN general.person ON general.person.person_guid = mswd.solo_parent_details.person_guid
                    LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                    WHERE mswd.solo_parent_details.main_guid = '" + obj.solo_parent_guid + "' ";
            obj.details = (List<SoloParentIntakeDetails>)QueryModule.DataSource<SoloParentIntakeDetails>(sql);

            return obj;
        }

        public List<object> GetHistoryLogs(string GUID)
        {
            string sql = @"SELECT mswd.solo_parent_intake_logs.main_guid, mswd.solo_parent_intake.application_date, mswd.solo_parent_intake_logs.user_id, mswd.solo_parent_intake.form_trans_no, mswd.solo_parent_intake.status,
                            mswd.solo_parent_intake.owner, mswd.solo_parent_intake.renter, mswd.solo_parent_intake.estimated_damaged, mswd.solo_parent_intake.if_distressed, mswd.solo_parent_intake.physical_disability,
                            mswd.solo_parent_intake.type_of_disability_id, mswd.general_intake_disability.disability_name, mswd.solo_parent_intake.sources_of_income, mswd.solo_parent_intake.total_family_income,
                            mswd.solo_parent_intake.no_of_hectares, mswd.solo_parent_intake.crops_planted, mswd.solo_parent_intake.area_of_location, mswd.solo_parent_intake.other_sources_of_income
                            FROM mswd.solo_parent_intake_logs  
                            LEFT JOIN mswd.solo_parent_intake ON mswd.solo_parent_intake.solo_parent_intake_guid = mswd.solo_parent_intake_logs.main_guid  
                            LEFT JOIN mswd.general_intake_disability ON mswd.general_intake_disability.id = mswd.solo_parent_intake.type_of_disability_id
                            WHERE mswd.solo_parent_intake_logs.main_guid = '" + GUID + "' ORDER BY mswd.solo_parent_intake.id DESC";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public bool Insert(SoloParentIntakeModel model)
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
                    model.form_trans_no = generateFormNumber("solo-parent-intake");
                    model.solo_parent_intake_guid = Guid.NewGuid().ToString();
                    model.status = "Active";

                    string sql = "insert into mswd.solo_parent_intake (`solo_parent_intake_guid`, `person_guid`, `owner`, `renter`, `estimated_damaged`, `if_distressed`, `physical_disability`, `type_of_disability_id`, " +
                        "`sources_of_income`, `total_family_income`, `no_of_hectares`, `crops_planted`, `area_of_location`, `other_sources_of_income`, `status`, `form_trans_no`, `member_count`) " +
                        "VALUES(@solo_parent_intake_guid, @person_guid, @owner, @renter, @estimated_damaged, @if_distressed, @physical_disability, @type_of_disability_id, @sources_of_income, " +
                        "@total_family_income, @no_of_hectares, @crops_planted, @area_of_location, @other_sources_of_income, @status, @form_trans_no, @member_count)";
                    int res = (int)QueryModule.DataObject<int>(sql, model);

                    if (model.details.Count > 0)
                    {
                        foreach (var dt in model.details)
                        {
                            dt.main_guid = model.solo_parent_intake_guid;
                            sql = "insert into mswd.solo_parent_intake_details (`main_guid`, `person_guid`, `relation`, `educational_attainment`, `occupation_income`, `occupation`) " +
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
