using eegs_back_end.Admin.FamilyCompositionSetup.Model;
using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Dynamic;

namespace eegs_back_end.Admin.FamilyCompositionSetup.Repository
{
    public interface IFamilyCompositionRepository :IGlobalInterface
    {
        bool Insert(FamilyCompositionModel model);
        List<object> GetList();
        List<object> GetAllList();
        List<object> GetListDeleted();
        List<object> GetListGenerated(int? filter_type_status_id = 0, int? status_id = 0, int? status_deleted_id = 0, string? this_month = "", string? this_year = "", string? monthly = "", string? monthlyYear = "", string? year_quarterly = "", int? quarter = 0, string? yearly = "", string? from = "", string? to = "");
        List<object> GetDetails(string GUID);
        List<object> GetEducationalType();
        List<object> GetRegisteredFourps();
        List<object> GetRegisteredIPS();
        List<object> GetHistoryLogs(string GUID);
        List<object> GetDetailsUsingPersonGUID(string GUID);
        bool Edit(string GUID, FamilyCompositionModel model);
        bool Delete(string GUID);
        object GetFamilyComposition(string FamilyComposition_guid);
        object ServerTime();
    }
    public class FamilyCompositionRepository : FormNumberGenerator, IFamilyCompositionRepository
    {
        public bool Edit(string GUID, FamilyCompositionModel model)
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
                    model.family_composition_guid = GUID;
                    model.status = "Active";

                    var sql = "UPDATE mswd.family_composition_head SET status='Trash' " +
                        "WHERE mswd.family_composition_head.family_composition_guid = '" + GUID + "'   ";
                    int res = (int)QueryModule.DataObject<int>(sql, model);

                    sql = @"INSERT into mswd.family_composition_head (`person_guid`, `monthly_income`, `fourps_member`, `ips`, `house_occupancy`, `property_cost`, `total_family_income`, `contact_no`, `secondary_contact_no`, `educational_attainment`, `occupation`, `family_composition_guid`, `status`, `form_trans_no`, `member_count`)  
                        VALUES(@person_guid, @monthly_income, @fourps_member, @ips, @house_occupancy, @property_cost, @total_family_income, @contact_no, @secondary_contact_no, @educational_attainment, @occupation, @family_composition_guid, @status, '" + model.form_trans_no + "', @member_count)";
                    QueryModule.Execute<int>(sql, model);

                    sql = "DELETE FROM mswd.family_composition_details WHERE mswd.family_composition_details.main_guid = '" + GUID + "' ";
                    QueryModule.Execute<int>(sql, model);

                    sql = "DELETE FROM mswd.family_composition_logs WHERE mswd.family_composition_logs.main_guid = '" + GUID + "' ";
                    QueryModule.Execute<int>(sql, model);

                    sql = "INSERT INTO mswd.family_composition_logs (`main_guid`, `user_id`) VALUES('" + model.family_composition_guid + "', '" + GlobalObject.user_id + "')";
                    QueryModule.Execute<int>(sql, model);

                    if (model.details.Count > 0)
                    {
                        foreach (var dt in model.details)
                        {
                            dt.main_guid = model.family_composition_guid;
                            sql = "insert into mswd.family_composition_details (`main_guid`, `person_guid`, `status`, `relation`, `educational_attainment`, `occupation_income`, `occupation`) " +
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

        public bool Delete(string GUID)
        {
            string sql = "UPDATE mswd.family_composition_head SET status = 'Deleted' where mswd.family_composition_head.family_composition_guid = @family_composition_guid";
            if (InsertUpdate.ExecuteSql(sql, new { family_composition_guid = GUID }))
                return true;
            return false;
        }

        public List<object> GetEducationalType()
        {
            string sql = @"SELECT * from general.educational_type";

            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public List<object> GetRegisteredFourps()
        {
            string sql = @"SELECT COUNT(*) as 'count' from mswd.family_composition_head WHERE mswd.family_composition_head.status = 'Active' AND mswd.family_composition_head.fourps_member = 1";

            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public List<object> GetRegisteredIPS()
        {
            string sql = @"SELECT COUNT(*) as 'count' from mswd.family_composition_head WHERE mswd.family_composition_head.status = 'Active' AND mswd.family_composition_head.ips = 1";

            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public List<object> GetList()
        {
            string sql = @"SELECT mswd.family_composition_head.family_composition_guid, mswd.family_composition_head.person_guid, general.person.first_name,  
                            general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix, general.person.gender_id, general.gender.gender_name, general.person.birth_date, general.person.place_of_birth, 
                            general.person.province_id, general.person.age, general_address.lgu_province_setup_temp.province_name, general.person.phone_no, general.person.telephone_no,  
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.religion, general.person.profession, 
                            mswd.family_composition_head.monthly_income, mswd.family_composition_head.fourps_member, mswd.family_composition_head.ips, mswd.family_composition_head.house_occupancy, 
                            mswd.family_composition_head.property_cost, mswd.family_composition_head.total_family_income, mswd.family_composition_head.application_date, general.person.educational_attainment, general.educational_type.educational_name,
                            mswd.family_composition_head.status, mswd.family_composition_head.form_trans_no, mswd.family_composition_head.member_count, general.person.street, general.person.full_name, 
                            (SELECT COUNT(*) FROM mswd.family_composition_head WHERE mswd.family_composition_head.status = 'Active') as 'count'
                            FROM mswd.family_composition_head  
                            LEFT JOIN general.person ON general.person.person_guid = mswd.family_composition_head.person_guid  
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id  
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id  
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id  
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id  
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id  
                            LEFT JOIN general.educational_type ON general.educational_type.id = general.person.educational_attainment
                            WHERE mswd.family_composition_head.status = 'Active' ORDER BY mswd.family_composition_head.id DESC ";
            
            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("fc-registration");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.family_composition_head fam
                    LEFT JOIN general.person ON general.person.person_guid = fam.person_guid
                    WHERE fam.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.family_composition_head fam
                    LEFT JOIN general.person ON general.person.person_guid = fam.person_guid
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
            string sql = "SELECT mswd.family_composition_head.family_composition_guid, mswd.family_composition_head.person_guid, general.person.first_name, " +
                "general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix, general.person.gender_id, general.gender.gender_name, general.person.birth_date, general.person.place_of_birth," +
                "general.person.province_id, general.person.age, general_address.lgu_province_setup_temp.province_name, general.person.phone_no, general.person.telephone_no, " +
                "general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.religion, general.person.profession," +
                "mswd.family_composition_head.monthly_income, mswd.family_composition_head.fourps_member, mswd.family_composition_head.ips, mswd.family_composition_head.house_occupancy," +
                "mswd.family_composition_head.property_cost, mswd.family_composition_head.total_family_income, mswd.family_composition_head.application_date, " +
                "mswd.family_composition_head.status, mswd.family_composition_head.form_trans_no, mswd.family_composition_head.member_count, general.person.street, general.person.full_name, " +
                "(SELECT COUNT(*) FROM mswd.family_composition_head WHERE mswd.family_composition_head.status = 'Active' OR mswd.family_composition_head.status = 'Deleted') as 'count'" +
                "FROM mswd.family_composition_head " +
                "LEFT JOIN general.person ON general.person.person_guid = mswd.family_composition_head.person_guid " +
                "LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id " +
                "LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id " +
                "LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id " +
                "LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id " +
                "LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id " +
                "WHERE mswd.family_composition_head.status = 'Active' OR mswd.family_composition_head.status = 'Deleted' " +
                "ORDER BY mswd.family_composition_head.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("fc-registration");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.family_composition_head fam
                    LEFT JOIN general.person ON general.person.person_guid = fam.person_guid
                    WHERE fam.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.family_composition_head fam
                    LEFT JOIN general.person ON general.person.person_guid = fam.person_guid
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
            string sql = "SELECT mswd.family_composition_details.person_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, " +
                "general.person.gender_id, general.gender.gender_name, general.person.birth_date, general.person.place_of_birth, " +
                "general.person.province_id, general_address.lgu_province_setup_temp.province_name, general.person.phone_no, general.person.telephone_no, " +
                "general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.civil_status_id, general.civil_status.civil_status_name, " +
                "general.person.religion, general.person.profession, general.person.age, " +
                "mswd.family_composition_details.main_guid, mswd.family_composition_details.status, mswd.family_composition_details.relation, mswd.family_composition_details.educational_attainment, mswd.family_composition_details.occupation_income, mswd.family_composition_details.occupation " +
                "FROM mswd.family_composition_details " +
                "LEFT JOIN general.person ON general.person.person_guid = mswd.family_composition_details.person_guid " +
                "LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id " +
                "LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id " +
                "LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id " +
                "LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id " +
                "LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id " +
                "WHERE mswd.family_composition_details.main_guid = '" + GUID + "'";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public List<object> GetListDeleted()
        {
            string sql = @"SELECT mswd.family_composition_head.family_composition_guid, mswd.family_composition_head.person_guid, general.person.first_name,  
                            general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix, general.person.gender_id, general.gender.gender_name, general.person.birth_date, general.person.place_of_birth, 
                            general.person.province_id, general.person.age, general_address.lgu_province_setup_temp.province_name, general.person.phone_no, general.person.telephone_no,  
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.religion, general.person.profession, 
                            mswd.family_composition_head.monthly_income, mswd.family_composition_head.fourps_member, mswd.family_composition_head.ips, mswd.family_composition_head.house_occupancy, 
                            mswd.family_composition_head.property_cost, mswd.family_composition_head.total_family_income, mswd.family_composition_head.application_date, general.person.educational_attainment, general.educational_type.educational_name,
                            mswd.family_composition_head.status, mswd.family_composition_head.form_trans_no, mswd.family_composition_head.member_count, general.person.street, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.family_composition_head WHERE mswd.family_composition_head.status = 'Deleted') as 'count'
                            FROM mswd.family_composition_head  
                            LEFT JOIN general.person ON general.person.person_guid = mswd.family_composition_head.person_guid  
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id  
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id  
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id  
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id  
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id  
                            LEFT JOIN general.educational_type ON general.educational_type.id = general.person.educational_attainment
                            WHERE mswd.family_composition_head.status = 'Deleted' ORDER BY mswd.family_composition_head.id DESC ";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("fc-registration");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.family_composition_head fam
                    LEFT JOIN general.person ON general.person.person_guid = fam.person_guid
                    WHERE fam.status = 'Deleted'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.family_composition_head fam
                    LEFT JOIN general.person ON general.person.person_guid = fam.person_guid
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
                filterStatus = "mswd.family_composition_head.status = 'Active'";
            }
            else if (status_deleted_id != 0)
            {
                filterStatus = "mswd.family_composition_head.status = 'Deleted'";
            }

            if (filter_type_status_id == 1)
            {
                dateFilter = "AND MONTH(mswd.family_composition_head.application_date) = '" + this_month + "'";
            }
            else if (filter_type_status_id == 2)
            {
                dateFilter = "AND YEAR(mswd.family_composition_head.application_date) = '" + this_year + "'";
            }
            else if (filter_type_status_id == 3)
            {
                dateFilter = "AND MONTH(mswd.family_composition_head.application_date) = '" + monthly + "' AND YEAR(mswd.family_composition_head.application_date) = '" + monthlyYear + "'";
            }
            else if (filter_type_status_id == 4)
            {
                dateFilter = "AND YEAR(mswd.family_composition_head.application_date) = '" + year_quarterly + "' AND QUARTER(mswd.family_composition_head.application_date) = '" + quarter + "'";
            }
            else if (filter_type_status_id == 5)
            {
                dateFilter = "AND YEAR(general.person.application_date) = '" + yearly + "'";
            }
            else if (filter_type_status_id == 6)
            {
                dateFilter = "AND mswd.family_composition_head.application_date >= '" + from + "' AND mswd.family_composition_head.application_date <= '" + to + "'";
            }

            string sql = @"SELECT mswd.family_composition_head.family_composition_guid, mswd.family_composition_head.person_guid, general.person.first_name,  
                            general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix, general.person.gender_id, general.gender.gender_name, general.person.birth_date, general.person.place_of_birth, 
                            general.person.province_id, general.person.age, general_address.lgu_province_setup_temp.province_name, general.person.phone_no, general.person.telephone_no,  
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.religion, general.person.profession, 
                            mswd.family_composition_head.monthly_income, mswd.family_composition_head.fourps_member, mswd.family_composition_head.ips, mswd.family_composition_head.house_occupancy, 
                            mswd.family_composition_head.property_cost, mswd.family_composition_head.total_family_income, mswd.family_composition_head.application_date, general.person.educational_attainment, general.educational_type.educational_name,
                            mswd.family_composition_head.status, mswd.family_composition_head.form_trans_no, mswd.family_composition_head.member_count, general.person.street, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.family_composition_head WHERE " + filterStatus + @" " + dateFilter  + @") as 'count'
                            FROM mswd.family_composition_head  
                            LEFT JOIN general.person ON general.person.person_guid = mswd.family_composition_head.person_guid  
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id  
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id  
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id  
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id  
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id  
                            LEFT JOIN general.educational_type ON general.educational_type.id = general.person.educational_attainment
                            WHERE " + filterStatus + @" " + dateFilter  + @" ORDER BY mswd.family_composition_head.id DESC ";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("fc-registration");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.family_composition_head fam
                    LEFT JOIN general.person ON general.person.person_guid = fam.person_guid
                    WHERE fam.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.family_composition_head fam
                    LEFT JOIN general.person ON general.person.person_guid = fam.person_guid
                    LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                    ) z";
            List<object> searches = (List<object>)QueryModule.DataSource<object>(sql);

            List<object> list = new List<object>();
            list.Add(obj);
            list.Add(form);
            list.Add(searches);


            return list;
        }

        public List<object> GetDetailsUsingPersonGUID(string GUID)
        {
            string sql = @"SELECT mswd.family_composition_details.person_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix,  
                            general.person.gender_id, general.gender.gender_name, general.person.birth_date, general.person.place_of_birth,  
                            general.person.province_id, general_address.lgu_province_setup_temp.province_name, general.person.phone_no, general.person.telephone_no,  
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.civil_status_id, general.civil_status.civil_status_name,  
                            general.person.religion, general.person.profession, general.person.age,  
                            mswd.family_composition_details.main_guid, mswd.family_composition_details.status, mswd.family_composition_details.relation, mswd.family_composition_details.educational_attainment, mswd.family_composition_details.occupation_income, mswd.family_composition_details.occupation,
                            mswd.family_composition_head.educational_attainment as 'educational_attainment_id'
                            FROM mswd.family_composition_head
                            LEFT JOIN mswd.family_composition_details ON mswd.family_composition_details.main_guid = mswd.family_composition_head.family_composition_guid
                            LEFT JOIN general.person ON general.person.person_guid = mswd.family_composition_details.person_guid  
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id  
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id  
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id  
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id  
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id  
                            WHERE mswd.family_composition_head.person_guid = '" + GUID + "' AND mswd.family_composition_head.`status` = 'Active'";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public List<object> GetHistoryLogs(string GUID)
        {
            string sql = @"SELECT mswd.family_composition_logs.main_guid, mswd.family_composition_head.application_date, mswd.family_composition_logs.user_id, mswd.family_composition_head.form_trans_no, mswd.family_composition_head.status,
                        mswd.family_composition_head.monthly_income, mswd.family_composition_head.fourps_member, mswd.family_composition_head.ips, mswd.family_composition_head.house_occupancy,
                        mswd.family_composition_head.property_cost, mswd.family_composition_head.total_family_income, mswd.family_composition_head.contact_no, mswd.family_composition_head.secondary_contact_no, mswd.family_composition_head.educational_attainment,
                        mswd.family_composition_head.occupation, mswd.family_composition_head.member_count
                        FROM mswd.family_composition_logs " +
                        "LEFT JOIN mswd.family_composition_head ON mswd.family_composition_head.family_composition_guid = mswd.family_composition_logs.main_guid " +
                        "WHERE mswd.family_composition_logs.main_guid = '" + GUID + "'" +
                        "ORDER BY mswd.family_composition_head.id DESC";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public object GetFamilyComposition(string family_composition_guid)
        {
            string sql = @"SELECT mswd.family_composition_head.person_guid, mswd.family_composition_head.monthly_income, mswd.family_composition_head.fourps_member, mswd.family_composition_head.ips, general.person.person_image,  
                            mswd.family_composition_head.house_occupancy, mswd.family_composition_head.property_cost, mswd.family_composition_head.total_family_income, mswd.family_composition_head.family_composition_guid,  
                            mswd.family_composition_head.status, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix, general.person.gender_id, general.gender.gender_name,  
                            general.person.civil_status_id, general.civil_status.civil_status_name, general.person.birth_date, general.person.place_of_birth, general.person.province_id, general_address.lgu_province_setup_temp.province_name,  
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.street, general.person.religion, general.person.age,  
                            mswd.family_composition_head.educational_attainment, mswd.family_composition_head.occupation, mswd.family_composition_head.contact_no, mswd.family_composition_head.secondary_contact_no, mswd.family_composition_head.form_trans_no,
                            general.educational_type.educational_name
                            from mswd.family_composition_head  
                            LEFT JOIN general.person ON general.person.person_guid = mswd.family_composition_head.person_guid  
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id  
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id  
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id  
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id  
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id  
                            LEFT JOIN general.educational_type ON general.educational_type.id = mswd.family_composition_head.educational_attainment
                            WHERE mswd.family_composition_head.status = 'Active' AND mswd.family_composition_head.family_composition_guid = @family_composition_guid ";
            var obj = (FamilyCompositionModel)QueryModule.DataObject<FamilyCompositionModel>(sql, new { family_composition_guid = family_composition_guid });

            sql = @"SELECT mswd.family_composition_details.main_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix,  
                    general.person.age, general.person.civil_status_id, general.civil_status.civil_status_name, mswd.family_composition_details.person_guid, mswd.family_composition_details.status, mswd.family_composition_details.educational_attainment,  
                    mswd.family_composition_details.relation, mswd.family_composition_details.occupation, mswd.family_composition_details.occupation_income  
                    FROM mswd.family_composition_details  
                    LEFT JOIN general.person ON general.person.person_guid = mswd.family_composition_details.person_guid  
                    LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id  
                    WHERE mswd.family_composition_details.main_guid = '" + obj.family_composition_guid + "' ";
            obj.details = (List<FamilyCompositionDetails>)QueryModule.DataSource<FamilyCompositionDetails>(sql);

            return obj;
        }

        public bool Insert(FamilyCompositionModel model)
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
                    model.form_trans_no = generateFormNumber("fc-registration");
                    model.family_composition_guid = Guid.NewGuid().ToString();
                    model.status = "Active";

                    string sql = @"INSERT into mswd.family_composition_head (`person_guid`, `monthly_income`, `fourps_member`, `ips`, `house_occupancy`, `property_cost`, `total_family_income`, `contact_no`, `secondary_contact_no`, `educational_attainment`, `occupation`, `family_composition_guid`, `status`, `form_trans_no`, `member_count`)  
                                VALUES(@person_guid, @monthly_income, @fourps_member, @ips, @house_occupancy, @property_cost, @total_family_income, @contact_no, @secondary_contact_no, @educational_attainment, @occupation, @family_composition_guid, @status, @form_trans_no, @member_count)";
                    int res = (int)QueryModule.DataObject<int>(sql, model);

                    if (model.details.Count > 0)
                    {
                        foreach (var dt in model.details)
                        {
                            dt.main_guid = model.family_composition_guid;
                            sql = "insert into mswd.family_composition_details (`main_guid`, `person_guid`, `status`, `relation`, `educational_attainment`, `occupation_income`, `occupation`) " +
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

        public object ServerTime()
        {
            var obj = System.DateTime.Now;
            return obj;
        }

    }
    
}
