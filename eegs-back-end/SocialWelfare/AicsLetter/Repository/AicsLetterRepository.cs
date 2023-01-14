using eegs_back_end.Admin.AicsLetterSetup.Model;
using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.AicsLetterSetup.Repository
{
    public interface IAicsLetterRepository :IGlobalInterface
    {
        bool Insert(AicsLetterModel model);
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
        bool Edit(string GUID, AicsLetterModel model);
        bool Delete(string GUID);
        object GetAicsLetter(string aics_letter_guid);
    }
    public class AicsLetterRepository : FormNumberGenerator, IAicsLetterRepository
    {
        public bool Edit(string GUID, AicsLetterModel model)
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
                    //model.AicsLetter_guid = GUID;
                    //model.status = "Active";

                    //var sql = "UPDATE mswd.AicsLetter SET status='Trash' " +
                    //    "WHERE mswd.AicsLetter.AicsLetter_guid = '" + GUID + "'   ";
                    //int res = (int)QueryModule.DataObject<int>(sql, model);

                    //sql = @"INSERT INTO mswd.AicsLetter (`AicsLetter_guid`, `person_guid`, `form_trans_no`, `status`, `fourps_beneficiary`, `ips`, `member_count`, `application_id`, `application_type_id`, `educational_attainment`, `house_ownership`, `monthly_family_income`, `occupation`, `recommendation_id`, `total_family_income`, `type_of_ethnicity`, `date_recommended`) 
                    //    VALUES(@AicsLetter_guid, @person_guid, @form_trans_no, @status, @fourps_beneficiary, @ips, @member_count, @application_id, @application_type_id, @educational_attainment, @house_ownership, @monthly_family_income, @occupation, @recommendation_id, @total_family_income, @type_of_ethnicity, @date_recommended)";
                    //QueryModule.Execute<int>(sql, model);

                    //sql = "DELETE FROM mswd.AicsLetter_details WHERE mswd.AicsLetter_details.main_guid = '" + GUID + "' ";
                    //QueryModule.Execute<int>(sql, model);

                    //sql = "DELETE FROM mswd.AicsLetter_casualties WHERE mswd.AicsLetter_casualties.main_guid = '" + GUID + "' ";
                    //QueryModule.Execute<int>(sql, model);

                    //sql = "DELETE FROM mswd.AicsLetter_logs WHERE mswd.AicsLetter_logs.main_guid = '" + GUID + "' ";
                    //QueryModule.Execute<int>(sql, model);

                    //sql = "INSERT INTO mswd.AicsLetter_logs (`main_guid`, `user_id`) VALUES('" + model.AicsLetter_guid + "', '" + GlobalObject.user_id + "')";
                    //QueryModule.Execute<int>(sql, model);

                    //if (model.family_details.Count > 0)
                    //{
                    //    foreach (var dt in model.family_details)
                    //    {
                    //        dt.main_guid = model.AicsLetter_guid;
                    //        sql = @"INSERT INTO mswd.AicsLetter_details (`main_guid`, `person_guid`, `relation`, `educational_attainment`, `occupational_skills`, `remarks`, `occupation_income`)
                    //            VALUES(@main_guid, @person_guid, @relation, @educational_attainment, @occupational_skills, @remarks, @occupation_income)";
                    //        QueryModule.Execute<int>(sql, dt);
                    //    }
                    //}

                    //if (model.casualties_details.Count > 0)
                    //{
                    //    foreach (var dt in model.casualties_details)
                    //    {
                    //        dt.main_guid = model.AicsLetter_guid;
                    //        sql = @"INSERT INTO mswd.AicsLetter_casualties (`main_guid`, `person_guid`)
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
            string sql = "UPDATE mswd.AicsLetter SET status = 'Deleted' where mswd.AicsLetter.AicsLetter_guid = @AicsLetter_guid";
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
            string sql = "SELECT * FROM mswd.AicsLetter_application";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public List<object> GetListOfApplicationType()
        {
            string sql = "SELECT * FROM mswd.AicsLetter_application_type";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public List<object> GetListOfRecommendations()
        {
            string sql = "SELECT * FROM mswd.AicsLetter_recommendations";
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
            string sql = @"SELECT al.aics_letter_guid, al.form_trans_no, al.application_date, al.`status`, al.aics_intake_guid, al.person_guid,
                            general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix, general.person.full_name,
                            general_address.lgu_province_setup_temp.province_name, general_address.lgu_city_mun_setup_temp.city_mun_name, general_address.lgu_brgy_setup_temp.brgy_name,
                            (SELECT COUNT(*) FROM mswd.aics_letter as al WHERE al.status = 'Active') as 'count'
                            FROM mswd.aics_letter as al
                            LEFT JOIN general.person ON general.person.person_guid = al.person_guid
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id  
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id  
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id  
                            WHERE al.status = 'Active' ORDER BY al.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("aics-letter");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.aics_letter aics_letter
                    LEFT JOIN general.person ON general.person.person_guid = aics_letter.person_guid
                    WHERE aics_letter.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.aics_letter aics_letter
                    LEFT JOIN general.person ON general.person.person_guid = aics_letter.person_guid
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
            string sql = @"SELECT al.aics_letter_guid, al.form_trans_no, al.application_date, al.`status`, al.aics_intake_guid, al.person_guid,
                            general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix, general.person.full_name,
                            general_address.lgu_province_setup_temp.province_name, general_address.lgu_city_mun_setup_temp.city_mun_name, general_address.lgu_brgy_setup_temp.brgy_name,
                            (SELECT COUNT(*) FROM mswd.aics_letter as al WHERE al.status = 'Active' OR al.status = 'Deleted') as 'count'
                            FROM mswd.aics_letter as al
                            LEFT JOIN general.person ON general.person.person_guid = al.person_guid
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id  
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id  
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id  
                            WHERE al.status = 'Active' OR al.status = 'Deleted' ORDER BY al.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("aics-letter");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.aics_letter aics_letter
                    LEFT JOIN general.person ON general.person.person_guid = aics_letter.person_guid
                    WHERE aics_letter.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.aics_letter aics_letter
                    LEFT JOIN general.person ON general.person.person_guid = aics_letter.person_guid
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

            string sql = @"SELECT al.aics_letter_guid, al.form_trans_no, al.application_date, al.`status`, al.aics_intake_guid, al.person_guid,
                            general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix, general.person.full_name,
                            general_address.lgu_province_setup_temp.province_name, general_address.lgu_city_mun_setup_temp.city_mun_name, general_address.lgu_brgy_setup_temp.brgy_name,
                            (SELECT COUNT(*) FROM mswd.aics_letter as al WHERE al.status = 'Deleted') as 'count'
                            FROM mswd.aics_letter as al
                            LEFT JOIN general.person ON general.person.person_guid = al.person_guid
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id  
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id  
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id  
                            WHERE al.status = 'Deleted' ORDER BY al.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("aics-letter");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.aics_letter aics_letter
                    LEFT JOIN general.person ON general.person.person_guid = aics_letter.person_guid
                    WHERE aics_letter.status = 'Deleted'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.aics_letter aics_letter
                    LEFT JOIN general.person ON general.person.person_guid = aics_letter.person_guid
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
                filterStatus = "al.status = 'Active'";
            }
            else if (status_deleted_id != 0)
            {
                filterStatus = "al.status = 'Deleted'";
            }

            if (filter_type_status_id == 1)
            {
                dateFilter = "AND MONTH(al.application_date) = '" + this_month + "'";
            }
            else if (filter_type_status_id == 2)
            {
                dateFilter = "AND YEAR(al.application_date) = '" + this_year + "'";
            }
            else if (filter_type_status_id == 3)
            {
                dateFilter = "AND MONTH(al.application_date) = '" + monthly + "' AND YEAR(al.application_date) = '" + monthlyYear + "'";
            }
            else if (filter_type_status_id == 4)
            {
                dateFilter = "AND YEAR(al.application_date) = '" + year_quarterly + "' AND QUARTER(al.application_date) = '" + quarter + "'";
            }
            else if (filter_type_status_id == 5)
            {
                dateFilter = "AND YEAR(al.application_date) = '" + yearly + "'";
            }
            else if (filter_type_status_id == 6)
            {
                dateFilter = "AND al.application_date >= '" + from + "' AND al.application_date <= '" + to + "'";
            }

            string sql = @"SELECT al.aics_letter_guid, al.form_trans_no, al.application_date, al.`status`, al.aics_intake_guid, al.person_guid,
                            general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix, general.person.full_name,
                            general_address.lgu_province_setup_temp.province_name, general_address.lgu_city_mun_setup_temp.city_mun_name, general_address.lgu_brgy_setup_temp.brgy_name,
                            (SELECT COUNT(*) FROM mswd.aics_letter as al WHERE " + filterStatus + @" " + dateFilter  + @") as 'count'
                            FROM mswd.aics_letter as al
                            LEFT JOIN general.person ON general.person.person_guid = al.person_guid
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id  
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id  
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id  
                            WHERE " + filterStatus + @" " + dateFilter  + @" ORDER BY al.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("aics-letter");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.aics_letter aics_letter
                    LEFT JOIN general.person ON general.person.person_guid = aics_letter.person_guid
                    WHERE aics_letter.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.aics_letter aics_letter
                    LEFT JOIN general.person ON general.person.person_guid = aics_letter.person_guid
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
            string sql = @"SELECT mswd.AicsLetter_details.person_guid, mswd.AicsLetter_details.relation, mswd.AicsLetter_details.educational_attainment, mswd.AicsLetter_details.occupational_skills,
                            mswd.AicsLetter_details.remarks, general.person.prefix, general.person.suffix, general.person.first_name, general.person.middle_name, general.person.last_name,
                            general.person.gender_id, general.gender.gender_name, general.person.age
                            FROM mswd.AicsLetter_details
                            LEFT JOIN general.person ON general.person.person_guid = mswd.AicsLetter_details.person_guid
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            WHERE mswd.AicsLetter_details.main_guid = '" + GUID + "' ";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public object GetAicsLetter(string aics_letter_guid)
        {
            string sql = @"SELECT al.aics_letter_guid, al.form_trans_no, al.application_date, al.`status`, al.aics_intake_guid, al.person_guid,
                            general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix,
                            mswd.aics_intake.application_date as 'aics_intake_date', general.person.barangay_id, general.person.street,
                            general_address.lgu_brgy_setup_temp.brgy_name
                            FROM mswd.aics_letter as al
                            INNER JOIN general.person ON general.person.person_guid = al.person_guid
                            INNER JOIN mswd.aics_intake ON mswd.aics_intake.aics_intake_guid = al.aics_intake_guid
                            INNER JOIN general_address.lgu_brgy_setup_temp ON general.person.barangay_id = general_address.lgu_brgy_setup_temp.brgy_id
                            WHERE al.status = 'Active' AND al.aics_letter_guid = @aics_letter_guid ";
            var obj = (AicsLetterModel)QueryModule.DataObject<AicsLetterModel>(sql, new { aics_letter_guid = aics_letter_guid });

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
            var obj = (AicsLetterModel)QueryModule.DataObject<AicsLetterModel>(sql);

            if (obj == null)
                return null;

            //sql = "select mswd.family_composition_details.main_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix, " +
            //    "general.person.age, general.person.civil_status_id, general.civil_status.civil_status_name, mswd.family_composition_details.person_guid, mswd.family_composition_details.status, mswd.family_composition_details.educational_attainment, " +
            //    "mswd.family_composition_details.relation, mswd.family_composition_details.occupation, mswd.family_composition_details.occupation_income " +
            //    "from mswd.family_composition_details " +
            //    "INNER JOIN general.person ON general.person.person_guid = mswd.family_composition_details.person_guid " +
            //    "INNER JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id " +
            //    "where mswd.family_composition_details.main_guid = '" + obj.family_composition_guid + "' ";
            //obj.details = (List<AicsLetterDetails>)QueryModule.DataSource<AicsLetterDetails>(sql);

            return obj;
        }

        public List<object> GetHistoryLogs(string GUID)
        {
            string sql = @"SELECT mswd.AicsLetter_logs.main_guid, mswd.AicsLetter.application_date, mswd.AicsLetter_logs.user_id, mswd.AicsLetter.form_trans_no,
                            mswd.AicsLetter.status, mswd.AicsLetter.person_guid, general.person.prefix, general.person.suffix, general.person.first_name,
                            general.person.middle_name, general.person.last_name, mswd.AicsLetter.ips, mswd.AicsLetter.educational_attainment, mswd.AicsLetter.total_family_income
                            FROM mswd.AicsLetter_logs  
                            INNER JOIN mswd.AicsLetter ON mswd.AicsLetter.AicsLetter_guid = mswd.AicsLetter_logs.main_guid  
                            INNER JOIN general.person ON general.person.person_guid = mswd.AicsLetter.person_guid
                            WHERE mswd.AicsLetter_logs.main_guid = '" + GUID + "' ORDER BY mswd.AicsLetter.id DESC";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public bool Insert(AicsLetterModel model)
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
                    model.form_trans_no = generateFormNumber("aics-letter");
                    model.aics_letter_guid = Guid.NewGuid().ToString();
                    model.status = "Active";

                    string sql = @"INSERT INTO mswd.aics_letter (`aics_letter_guid`, `person_guid`, `form_trans_no`, `status`, `aics_intake_guid`) 
                        VALUES(@aics_letter_guid, @person_guid, @form_trans_no, @status, @aics_intake_guid)";
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
