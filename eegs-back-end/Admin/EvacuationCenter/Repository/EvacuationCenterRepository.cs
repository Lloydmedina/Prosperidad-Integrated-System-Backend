using eegs_back_end.Admin.EvacuationCenterSetup.Model;
using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.EvacuationCenterSetup.Repository
{
    public interface IEvacuationCenterRepository :IGlobalInterface
    {
        bool Insert(EvacuationCenterModel model);
        List<object> GetRegion();
        List<object> GetList();
        List<object> GetAllList();
        List<object> GetListDeleted();
        List<object> GetListGenerated(int? filter_type_status_id = 0, int? status_id = 0, int? status_deleted_id = 0, string? this_month = "", string? this_year = "", string? monthly = "", string? monthlyYear = "", string? year_quarterly = "", int? quarter = 0, string? yearly = "", string? from = "", string? to = "");
        List<object> GetDetails(string GUID);
        object GetPersonAdd(string GUID);
        List<object> GetHistoryLogs(string GUID);
        bool Edit(string GUID, EvacuationCenterModel model);
        bool Delete(string GUID);
        object GetEvacuationCenter(string EvacuationCenter_guid);
    }
    public class EvacuationCenterRepository : FormNumberGenerator, IEvacuationCenterRepository
    {
        public bool Edit(string GUID, EvacuationCenterModel model)
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
                    model.evacuation_center_guid = GUID;
                    model.status = "Active";

                    var sql = @"UPDATE general.evacuation_center SET region_id='" + model.region_id + "', province_id='" + model.province_id + "', citmun_id='" + model.citmun_id + "', barangay_id='" + model.barangay_id + "', " +
                        "street='" + model.street + "', description='" + model.description + "', capacity='" + model.capacity + "', status='" + model.status + "', " +
                        "venue='" + model.venue + "', venue_status='" + model.venue_status + "', venue_condition='" + model.venue_condition + "' " +
                        "WHERE general.evacuation_center.evacuation_center_guid = '" + GUID + "'   ";
                    int res = (int)QueryModule.Execute<int>(sql, model);

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
            string sql = "UPDATE general.evacuation_center SET status = 'Deleted' where general.evacuation_center.evacuation_center_guid = @evacuation_center_guid";
            if (InsertUpdate.ExecuteSql(sql, new { evacuation_center_guid = GUID }))
                return true;
            return false;
        }

        public List<object> GetRegion()
        {
            string sql = "SELECT * FROM general_address.lgu_region_setup_temp";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public List<object> GetList()
        {
            string sql = @"SELECT general.evacuation_center.region_id, general_address.lgu_region_setup_temp.reg_name, general_address.lgu_region_setup_temp.reg_code, general.evacuation_center.province_id, 
                            general_address.lgu_province_setup_temp.province_name, general.evacuation_center.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name,
                            general.evacuation_center.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.evacuation_center.street, general.evacuation_center.description,
                            general.evacuation_center.capacity, general.evacuation_center.status, general.evacuation_center.application_date,
                            general.evacuation_center.venue_condition, general.evacuation_center.form_trans_no, general.evacuation_center.evacuation_center_guid,
                            general.evacuation_center.venue, (SELECT COUNT(*) FROM general.evacuation_center WHERE general.evacuation_center.status = 'Active') as 'count'
                            FROM general.evacuation_center
                            LEFT JOIN general_address.lgu_region_setup_temp ON general_address.lgu_region_setup_temp.SysPK_region = general.evacuation_center.region_id
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.evacuation_center.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.evacuation_center.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.evacuation_center.barangay_id
                            WHERE general.evacuation_center.status = 'Active' ORDER BY general.evacuation_center.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("evacuation-setup");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT evac.venue as q, 'vn' as tag
                    FROM general.evacuation_center evac
                    LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = evac.barangay_id
                    WHERE evac.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM general.evacuation_center evac
                    LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = evac.barangay_id
                    WHERE evac.status = 'Active'

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
            string sql = @"SELECT general.evacuation_center.region_id, general_address.lgu_region_setup_temp.reg_name, general_address.lgu_region_setup_temp.reg_code, general.evacuation_center.province_id, 
                            general_address.lgu_province_setup_temp.province_name, general.evacuation_center.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name,
                            general.evacuation_center.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.evacuation_center.street, general.evacuation_center.description,
                            general.evacuation_center.capacity, general.evacuation_center.status, general.evacuation_center.application_date,
                            general.evacuation_center.venue_condition, general.evacuation_center.form_trans_no, general.evacuation_center.evacuation_center_guid,
                            general.evacuation_center.venue, (SELECT COUNT(*) FROM general.evacuation_center WHERE general.evacuation_center.status = 'Active' OR general.evacuation_center.status = 'Deleted') as 'count'
                            FROM general.evacuation_center
                            LEFT JOIN general_address.lgu_region_setup_temp ON general_address.lgu_region_setup_temp.SysPK_region = general.evacuation_center.region_id
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.evacuation_center.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.evacuation_center.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.evacuation_center.barangay_id
                            WHERE general.evacuation_center.status = 'Active' OR general.evacuation_center.status = 'Deleted' ORDER BY general.evacuation_center.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("evacuation-setup");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT evac.venue as q, 'vn' as tag
                    FROM general.evacuation_center evac
                    LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = evac.barangay_id
                    WHERE evac.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM general.evacuation_center evac
                    LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = evac.barangay_id
                    WHERE evac.status = 'Active'

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
                "INNER JOIN general.person ON general.person.person_guid = mswd.general_intake_details.person_guid " +
                "INNER JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id " +
                "INNER JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id " +
                "INNER JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id " +
                "INNER JOIN general.gender ON general.gender.gender_id = general.person.gender_id " +
                "INNER JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id " +
                "WHERE mswd.general_intake_details.main_guid = '" + GUID + "'";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public List<object> GetListDeleted()
        {

            string sql = @"SELECT general.evacuation_center.region_id, general_address.lgu_region_setup_temp.reg_name, general_address.lgu_region_setup_temp.reg_code, general.evacuation_center.province_id, 
                            general_address.lgu_province_setup_temp.province_name, general.evacuation_center.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name,
                            general.evacuation_center.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.evacuation_center.street, general.evacuation_center.description,
                            general.evacuation_center.capacity, general.evacuation_center.status, general.evacuation_center.application_date,
                            general.evacuation_center.venue_condition, general.evacuation_center.form_trans_no, general.evacuation_center.evacuation_center_guid,
                            general.evacuation_center.venue, (SELECT COUNT(*) FROM general.evacuation_center WHERE general.evacuation_center.status = 'Deleted') as 'count'
                            FROM general.evacuation_center
                            LEFT JOIN general_address.lgu_region_setup_temp ON general_address.lgu_region_setup_temp.SysPK_region = general.evacuation_center.region_id
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.evacuation_center.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.evacuation_center.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.evacuation_center.barangay_id
                            WHERE general.evacuation_center.status = 'Deleted' ORDER BY general.evacuation_center.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("evacuation-setup");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT evac.venue as q, 'vn' as tag
                    FROM general.evacuation_center evac
                    LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = evac.barangay_id
                    WHERE evac.status = 'Deleted'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM general.evacuation_center evac
                    LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = evac.barangay_id
                    WHERE evac.status = 'Deleted'

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
                filterStatus = "general.evacuation_center.status = 'Active'";
            }
            else if (status_deleted_id != 0)
            {
                filterStatus = "general.evacuation_center.status = 'Deleted'";
            }

            if (filter_type_status_id == 1)
            {
                dateFilter = "AND MONTH(general.evacuation_center.application_date) = '" + this_month + "'";
            }
            else if (filter_type_status_id == 2)
            {
                dateFilter = "AND YEAR(general.evacuation_center.application_date) = '" + this_year + "'";
            }
            else if (filter_type_status_id == 3)
            {
                dateFilter = "AND MONTH(general.evacuation_center.application_date) = '" + monthly + "' AND YEAR(general.evacuation_center.application_date) = '" + monthlyYear + "'";
            }
            else if (filter_type_status_id == 4)
            {
                dateFilter = "AND YEAR(general.evacuation_center.application_date) = '" + year_quarterly + "' AND QUARTER(general.evacuation_center.application_date) = '" + quarter + "'";
            }
            else if (filter_type_status_id == 5)
            {
                dateFilter = "AND YEAR(general.evacuation_center.application_date) = '" + yearly + "'";
            }
            else if (filter_type_status_id == 6)
            {
                dateFilter = "AND general.evacuation_center.application_date >= '" + from + "' AND general.evacuation_center.application_date <= '" + to + "'";
            }

            string sql = @"SELECT general.evacuation_center.region_id, general_address.lgu_region_setup_temp.reg_name, general_address.lgu_region_setup_temp.reg_code, general.evacuation_center.province_id, 
                            general_address.lgu_province_setup_temp.province_name, general.evacuation_center.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name,
                            general.evacuation_center.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.evacuation_center.street, general.evacuation_center.description,
                            general.evacuation_center.capacity, general.evacuation_center.status, general.evacuation_center.application_date,
                            general.evacuation_center.venue_condition, general.evacuation_center.form_trans_no, general.evacuation_center.evacuation_center_guid,
                            general.evacuation_center.venue, (SELECT COUNT(*) FROM general.evacuation_center WHERE " + filterStatus + @" " + dateFilter  + @") as 'count'
                            FROM general.evacuation_center
                            LEFT JOIN general_address.lgu_region_setup_temp ON general_address.lgu_region_setup_temp.SysPK_region = general.evacuation_center.region_id
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.evacuation_center.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.evacuation_center.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.evacuation_center.barangay_id
                            WHERE " + filterStatus + @" " + dateFilter  + @" ORDER BY general.evacuation_center.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("evacuation-setup");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT evac.venue as q, 'vn' as tag
                    FROM general.evacuation_center evac
                    LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = evac.barangay_id
                    WHERE evac.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM general.evacuation_center evac
                    LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = evac.barangay_id
                    WHERE evac.status = 'Active'

                    ) z";
            List<object> searches = (List<object>)QueryModule.DataSource<object>(sql);

            List<object> list = new List<object>();
            list.Add(obj);
            list.Add(form);
            list.Add(searches);


            return list;
        }

        public object GetEvacuationCenter(string general_intake_guid)
        {
            string sql = @"SELECT * FROM general.evacuation_center WHERE general.evacuation_center.status = 'Active' ";
            var obj = (EvacuationCenterModel)QueryModule.DataObject<EvacuationCenterModel>(sql, new { general_intake_guid = general_intake_guid });

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
            var obj = (EvacuationCenterModel)QueryModule.DataObject<EvacuationCenterModel>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public List<object> GetHistoryLogs(string GUID)
        {
            string sql = @"SELECT mswd.general_intake_logs.main_guid, mswd.general_intake.application_date, mswd.general_intake_logs.user_id, mswd.general_intake.form_trans_no, mswd.general_intake.status,
                            mswd.general_intake.owner, mswd.general_intake.renter, mswd.general_intake.estimated_damaged, mswd.general_intake.if_distressed, mswd.general_intake.physical_disability,
                            mswd.general_intake.type_of_disability_id, mswd.general_intake_disability.disability_name, mswd.general_intake.sources_of_income, mswd.general_intake.total_family_income,
                            mswd.general_intake.no_of_hectares, mswd.general_intake.crops_planted, mswd.general_intake.area_of_location, mswd.general_intake.other_sources_of_income
                            FROM mswd.general_intake_logs  
                            INNER JOIN mswd.general_intake ON mswd.general_intake.general_intake_guid = mswd.general_intake_logs.main_guid  
                            INNER JOIN mswd.general_intake_disability ON mswd.general_intake_disability.id = mswd.general_intake.type_of_disability_id
                            WHERE mswd.general_intake_logs.main_guid = '"+ GUID +"' ORDER BY mswd.general_intake.id DESC";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public bool Insert(EvacuationCenterModel model)
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
                    model.form_trans_no = generateFormNumber("evacuation-center");
                    model.evacuation_center_guid = Guid.NewGuid().ToString();
                    model.status = "Active";

                    string sql = "insert into general.evacuation_center (`region_id`, `province_id`, `citmun_id`, `barangay_id`, `street`, `description`, `capacity`, `status`, `venue_condition`, `form_trans_no`, `evacuation_center_guid`, `venue`, `venue_status`) " +
                        "VALUES(@region_id, @province_id, @citmun_id, @barangay_id, @street, @description, @capacity, @status, @venue_condition, @form_trans_no, @evacuation_center_guid, @venue, @venue_status)";
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
