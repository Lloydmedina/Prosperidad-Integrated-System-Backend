using eegs_back_end.Admin.DafacSetup.Model;
using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.DafacSetup.Repository
{
    public interface IDafacRepository :IGlobalInterface
    {
        bool Insert(DafacModel model);
        List<object> GetListOfDisability();
        List<object> GetList();
        List<object> GetAllList();
        List<object> GetListDeleted();
        List<object> GetListGenerated(int? filter_type_status_id = 0, int? status_id = 0, int? status_deleted_id = 0, string? this_month = "", string? this_year = "", string? monthly = "", string? monthlyYear = "", string? year_quarterly = "", int? quarter = 0, string? yearly = "", string? from = "", string? to = "");
        List<object> GetEducationalType();
        List<object> GetDetails(string GUID);
        object GetPersonAdd(string GUID);
        List<object> GetHistoryLogs(string GUID);
        bool Edit(string GUID, DafacModel model);
        bool Delete(string GUID);
        object GetDafac(string Dafac_guid);
    }
    public class DafacRepository : FormNumberGenerator, IDafacRepository
    {
        public bool Edit(string GUID, DafacModel model)
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
                    model.dafac_guid = GUID;
                    model.status = "Active";

                    var sql = "UPDATE mswd.dafac SET status='Trash' " +
                        "WHERE mswd.dafac.dafac_guid = '" + GUID + "'   ";
                    int res = (int)QueryModule.DataObject<int>(sql, model);

                    sql = "INSERT INTO mswd.dafac (`person_guid`, `evacuation_center_guid`, `mothers_maiden_name`, `monthly_family_income`, `id_card_presented`, `id_card_number`, `primary_contact`, `alternate_contact`, `fourps_beneficiary`, `ips`, `type_of_ethnicity`, `no_of_older`, `no_of_pregnant_or_lactating`, `no_of_pwds_and_conditions`, `house_ownership`, `housing_conditioning`, `dafac_guid`, " +
                        "`status`, `member_count`, `form_trans_no`, `name_extension`, `region_id`, `province_id`, `district`, `barangay_id`, `citmun_id`, `occupation`, `educational_attainment`, `total_family_income`) " +
                        "VALUES(@person_guid, @evacuation_center_guid, @mothers_maiden_name, @monthly_family_income, @id_card_presented, @id_card_number, @primary_contact, @alternate_contact, @fourps_beneficiary, @ips, @type_of_ethnicity, @no_of_older, @no_of_pregnant_or_lactating, @no_of_pwds_and_conditions, @house_ownership, @housing_conditioning, @dafac_guid, " +
                        "@status, @member_count, @form_trans_no, @name_extension, @region_id, @province_id, @district, @barangay_id, @citmun_id, @occupation, @educational_attainment, @total_family_income)";
                    QueryModule.Execute<int>(sql, model);

                    sql = "DELETE FROM mswd.dafac_details WHERE mswd.dafac_details.main_guid = '" + GUID + "' ";
                    QueryModule.Execute<int>(sql, model);

                    sql = "DELETE FROM mswd.dafac_family_assistance WHERE mswd.dafac_family_assistance.main_guid = '" + GUID + "' ";
                    QueryModule.Execute<int>(sql, model);

                    sql = "DELETE FROM mswd.dafac_logs WHERE mswd.dafac_logs.main_guid = '" + GUID + "' ";
                    QueryModule.Execute<int>(sql, model);

                    sql = "INSERT INTO mswd.dafac_logs (`main_guid`, `user_id`) VALUES('" + model.dafac_guid + "', '" + GlobalObject.user_id + "')";
                    QueryModule.Execute<int>(sql, model);

                    if (model.family_details.Count > 0)
                    {
                        foreach (var dt in model.family_details)
                        {
                            dt.main_guid = model.dafac_guid;
                            sql = @"INSERT INTO mswd.dafac_details (`main_guid`, `person_guid`, `relation`, `educational_attainment`, `occupational_skills`, `remarks`, `occupation_income`)
                                VALUES(@main_guid, @person_guid, @relation, @educational_attainment, @occupational_skills, @remarks, @occupation_income)";
                            QueryModule.Execute<int>(sql, dt);
                        }
                    }

                    if (model.assistance_details.Count > 0)
                    {
                        foreach (var dt in model.assistance_details)
                        {
                            dt.main_guid = model.dafac_guid;
                            sql = @"INSERT INTO mswd.dafac_family_assistance (`main_guid`, `person_guid`, `kind_type`, `qty`, `cost`, `provider`)
                                VALUES(@main_guid, @person_guid, @kind_type, @qty, @cost, @provider)";
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
            string sql = "UPDATE mswd.dafac SET status = 'Deleted' where mswd.dafac.dafac_guid = @dafac_guid";
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
            string sql = @"SELECT mswd.dafac.dafac_guid, mswd.dafac.person_guid, mswd.dafac.evacuation_center_guid, mswd.dafac.mothers_maiden_name, mswd.dafac.monthly_family_income,
                            mswd.dafac.id_card_presented, mswd.dafac.id_card_number, mswd.dafac.primary_contact, mswd.dafac.alternate_contact, mswd.dafac.fourps_beneficiary,
                            mswd.dafac.ips, mswd.dafac.type_of_ethnicity, mswd.dafac.no_of_older, mswd.dafac.no_of_pregnant_or_lactating, mswd.dafac.no_of_pwds_and_conditions,
                            mswd.dafac.house_ownership, mswd.dafac.housing_conditioning, mswd.dafac.application_date, mswd.dafac.status, mswd.dafac.member_count,
                            general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix,
                            general.evacuation_center.province_id, general.evacuation_center.citmun_id, general.evacuation_center.barangay_id, general.evacuation_center.region_id,
                            general_address.lgu_province_setup_temp.province_name, general_address.lgu_city_mun_setup_temp.city_mun_name, general_address.lgu_brgy_setup_temp.brgy_name,
                            general_address.lgu_region_setup_temp.reg_name, general_address.lgu_region_setup_temp.reg_code, mswd.dafac.form_trans_no, general.person.birth_date,
                            general.person.civil_status_id, general.civil_status.civil_status_name, general.person.gender_id, general.gender.gender_name, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.dafac WHERE mswd.dafac.status = 'Active') as 'count'
                            FROM mswd.dafac
                            LEFT JOIN general.person ON general.person.person_guid = mswd.dafac.person_guid
                            LEFT JOIN general.evacuation_center ON general.evacuation_center.evacuation_center_guid = mswd.dafac.evacuation_center_guid
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.evacuation_center.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.evacuation_center.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.evacuation_center.barangay_id
                            LEFT JOIN general_address.lgu_region_setup_temp ON general_address.lgu_region_setup_temp.SysPK_region = general.evacuation_center.region_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            WHERE mswd.dafac.status = 'Active' ORDER BY mswd.dafac.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("dafac-registration");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.dafac dafac
                    LEFT JOIN general.person ON general.person.person_guid = dafac.person_guid
                    WHERE dafac.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.dafac dafac
                    LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = dafac.barangay_id

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
            string sql = @"SELECT mswd.dafac.dafac_guid, mswd.dafac.person_guid, mswd.dafac.evacuation_center_guid, mswd.dafac.mothers_maiden_name, mswd.dafac.monthly_family_income,
                            mswd.dafac.id_card_presented, mswd.dafac.id_card_number, mswd.dafac.primary_contact, mswd.dafac.alternate_contact, mswd.dafac.fourps_beneficiary,
                            mswd.dafac.ips, mswd.dafac.type_of_ethnicity, mswd.dafac.no_of_older, mswd.dafac.no_of_pregnant_or_lactating, mswd.dafac.no_of_pwds_and_conditions,
                            mswd.dafac.house_ownership, mswd.dafac.housing_conditioning, mswd.dafac.application_date, mswd.dafac.status, mswd.dafac.member_count,
                            general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix,
                            general.evacuation_center.province_id, general.evacuation_center.citmun_id, general.evacuation_center.barangay_id, general.evacuation_center.region_id,
                            general_address.lgu_province_setup_temp.province_name, general_address.lgu_city_mun_setup_temp.city_mun_name, general_address.lgu_brgy_setup_temp.brgy_name,
                            general_address.lgu_region_setup_temp.reg_name, general_address.lgu_region_setup_temp.reg_code, mswd.dafac.form_trans_no, general.person.birth_date,
                            general.person.civil_status_id, general.civil_status.civil_status_name, general.person.gender_id, general.gender.gender_name, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.dafac WHERE mswd.dafac.status = 'Active' OR mswd.dafac.status = 'Deleted') as 'count'
                            FROM mswd.dafac
                            LEFT JOIN general.person ON general.person.person_guid = mswd.dafac.person_guid
                            LEFT JOIN general.evacuation_center ON general.evacuation_center.evacuation_center_guid = mswd.dafac.evacuation_center_guid
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.evacuation_center.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.evacuation_center.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.evacuation_center.barangay_id
                            LEFT JOIN general_address.lgu_region_setup_temp ON general_address.lgu_region_setup_temp.SysPK_region = general.evacuation_center.region_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            WHERE mswd.dafac.status = 'Active' OR mswd.dafac.status = 'Deleted' ORDER BY mswd.dafac.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("dafac-registration");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.dafac dafac
                    LEFT JOIN general.person ON general.person.person_guid = dafac.person_guid
                    WHERE dafac.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.dafac dafac
                    LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = dafac.barangay_id

                    ) z";
            List<object> searches = (List<object>)QueryModule.DataSource<object>(sql);

            List<object> list = new List<object>();
            list.Add(obj);
            list.Add(form);


            return list;
        }

        public List<object> GetDetails(string GUID)
        {
            string sql = @"SELECT mswd.dafac_details.person_guid, mswd.dafac_details.relation, mswd.dafac_details.educational_attainment, mswd.dafac_details.occupational_skills,
                            mswd.dafac_details.remarks, general.person.prefix, general.person.suffix, general.person.first_name, general.person.middle_name, general.person.last_name,
                            general.person.gender_id, general.gender.gender_name, general.person.age
                            FROM mswd.dafac_details
                            LEFT JOIN general.person ON general.person.person_guid = mswd.dafac_details.person_guid
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            WHERE mswd.dafac_details.main_guid = '" + GUID + "' ";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public List<object> GetListDeleted()
        {

            string sql = @"SELECT mswd.dafac.dafac_guid, mswd.dafac.person_guid, mswd.dafac.evacuation_center_guid, mswd.dafac.mothers_maiden_name, mswd.dafac.monthly_family_income,
                            mswd.dafac.id_card_presented, mswd.dafac.id_card_number, mswd.dafac.primary_contact, mswd.dafac.alternate_contact, mswd.dafac.fourps_beneficiary,
                            mswd.dafac.ips, mswd.dafac.type_of_ethnicity, mswd.dafac.no_of_older, mswd.dafac.no_of_pregnant_or_lactating, mswd.dafac.no_of_pwds_and_conditions,
                            mswd.dafac.house_ownership, mswd.dafac.housing_conditioning, mswd.dafac.application_date, mswd.dafac.status, mswd.dafac.member_count,
                            general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix,
                            general.evacuation_center.province_id, general.evacuation_center.citmun_id, general.evacuation_center.barangay_id, general.evacuation_center.region_id,
                            general_address.lgu_province_setup_temp.province_name, general_address.lgu_city_mun_setup_temp.city_mun_name, general_address.lgu_brgy_setup_temp.brgy_name,
                            general_address.lgu_region_setup_temp.reg_name, general_address.lgu_region_setup_temp.reg_code, mswd.dafac.form_trans_no, general.person.birth_date,
                            general.person.civil_status_id, general.civil_status.civil_status_name, general.person.gender_id, general.gender.gender_name, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.dafac WHERE mswd.dafac.status = 'Deleted') as 'count'
                            FROM mswd.dafac
                            LEFT JOIN general.person ON general.person.person_guid = mswd.dafac.person_guid
                            LEFT JOIN general.evacuation_center ON general.evacuation_center.evacuation_center_guid = mswd.dafac.evacuation_center_guid
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.evacuation_center.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.evacuation_center.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.evacuation_center.barangay_id
                            LEFT JOIN general_address.lgu_region_setup_temp ON general_address.lgu_region_setup_temp.SysPK_region = general.evacuation_center.region_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            WHERE mswd.dafac.status = 'Deleted' ORDER BY mswd.dafac.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("dafac-registration");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.dafac dafac
                    LEFT JOIN general.person ON general.person.person_guid = dafac.person_guid
                    WHERE dafac.status = 'Deleted'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.dafac dafac
                    LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = dafac.barangay_id

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
                filterStatus = "mswd.dafac.status = 'Active'";
            }
            else if (status_deleted_id != 0)
            {
                filterStatus = "mswd.dafac.status = 'Deleted'";
            }

            if (filter_type_status_id == 1)
            {
                dateFilter = "AND MONTH(mswd.dafac.application_date) = '" + this_month + "'";
            }
            else if (filter_type_status_id == 2)
            {
                dateFilter = "AND YEAR(mswd.dafac.application_date) = '" + this_year + "'";
            }
            else if (filter_type_status_id == 3)
            {
                dateFilter = "AND MONTH(mswd.dafac.application_date) = '" + monthly + "' AND YEAR(mswd.dafac.application_date) = '" + monthlyYear + "'";
            }
            else if (filter_type_status_id == 4)
            {
                dateFilter = "AND YEAR(mswd.dafac.application_date) = '" + year_quarterly + "' AND QUARTER(mswd.dafac.application_date) = '" + quarter + "'";
            }
            else if (filter_type_status_id == 5)
            {
                dateFilter = "AND YEAR(mswd.dafac.application_date) = '" + yearly + "'";
            }
            else if (filter_type_status_id == 6)
            {
                dateFilter = "AND mswd.dafac.application_date >= '" + from + "' AND mswd.dafac.application_date <= '" + to + "'";
            }

            string sql = @"SELECT mswd.dafac.dafac_guid, mswd.dafac.person_guid, mswd.dafac.evacuation_center_guid, mswd.dafac.mothers_maiden_name, mswd.dafac.monthly_family_income,
                            mswd.dafac.id_card_presented, mswd.dafac.id_card_number, mswd.dafac.primary_contact, mswd.dafac.alternate_contact, mswd.dafac.fourps_beneficiary,
                            mswd.dafac.ips, mswd.dafac.type_of_ethnicity, mswd.dafac.no_of_older, mswd.dafac.no_of_pregnant_or_lactating, mswd.dafac.no_of_pwds_and_conditions,
                            mswd.dafac.house_ownership, mswd.dafac.housing_conditioning, mswd.dafac.application_date, mswd.dafac.status, mswd.dafac.member_count,
                            general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix,
                            general.evacuation_center.province_id, general.evacuation_center.citmun_id, general.evacuation_center.barangay_id, general.evacuation_center.region_id,
                            general_address.lgu_province_setup_temp.province_name, general_address.lgu_city_mun_setup_temp.city_mun_name, general_address.lgu_brgy_setup_temp.brgy_name,
                            general_address.lgu_region_setup_temp.reg_name, general_address.lgu_region_setup_temp.reg_code, mswd.dafac.form_trans_no, general.person.birth_date,
                            general.person.civil_status_id, general.civil_status.civil_status_name, general.person.gender_id, general.gender.gender_name, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.dafac WHERE " + filterStatus + @" " + dateFilter  + @") as 'count'
                            FROM mswd.dafac
                            LEFT JOIN general.person ON general.person.person_guid = mswd.dafac.person_guid
                            LEFT JOIN general.evacuation_center ON general.evacuation_center.evacuation_center_guid = mswd.dafac.evacuation_center_guid
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.evacuation_center.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.evacuation_center.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.evacuation_center.barangay_id
                            LEFT JOIN general_address.lgu_region_setup_temp ON general_address.lgu_region_setup_temp.SysPK_region = general.evacuation_center.region_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            WHERE " + filterStatus + @" " + dateFilter  + @" ORDER BY mswd.dafac.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("dafac-registration");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.dafac dafac
                    LEFT JOIN general.person ON general.person.person_guid = dafac.person_guid
                    WHERE dafac.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.dafac dafac
                    LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = dafac.barangay_id

                    ) z";
            List<object> searches = (List<object>)QueryModule.DataSource<object>(sql);

            List<object> list = new List<object>();
            list.Add(obj);
            list.Add(form);
            list.Add(searches);


            return list;
        }

        public object GetDafac(string dafac_guid)
        {
            string sql = @"SELECT mswd.dafac.region_id, general_address.lgu_region_setup_temp.reg_name, mswd.dafac.province_id, general_address.lgu_province_setup_temp.province_name,
                            mswd.dafac.district, mswd.dafac.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, mswd.dafac.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name,
                            mswd.dafac.evacuation_center_guid, general.evacuation_center.venue, mswd.dafac.person_guid, general.person.prefix, general.person.suffix, general.person.first_name,
                            general.person.middle_name, general.person.last_name, mswd.dafac.name_extension, mswd.dafac.fourps_beneficiary, mswd.dafac.ips, mswd.dafac.type_of_ethnicity,
                            general.person.birth_date, general.person.place_of_birth, general.person.gender_id, general.gender.gender_name, mswd.dafac.mothers_maiden_name,
                            mswd.dafac.occupation, mswd.dafac.monthly_family_income, mswd.dafac.id_card_presented, mswd.dafac.id_card_number, mswd.dafac.primary_contact, mswd.dafac.alternate_contact,
                            mswd.dafac.house_ownership, mswd.dafac.housing_conditioning, mswd.dafac.application_date, mswd.dafac.status, mswd.dafac.dafac_guid, mswd.dafac.form_trans_no,
                            (SELECT general.person.barangay_id FROM general.person WHERE mswd.dafac.person_guid = general.person.person_guid) as family_head_barangay_id,
                            (SELECT general_address.lgu_brgy_setup_temp.brgy_name FROM general_address.lgu_brgy_setup_temp WHERE general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id) as family_head_barangay_name,
                            (SELECT general.person.street FROM general.person WHERE mswd.dafac.person_guid = general.person.person_guid) as family_head_street,                
                            general.person.age, mswd.dafac.no_of_older, mswd.dafac.no_of_pregnant_or_lactating, mswd.dafac.no_of_pwds_and_conditions, mswd.dafac.educational_attainment
                            FROM mswd.dafac
                            LEFT JOIN general_address.lgu_region_setup_temp ON general_address.lgu_region_setup_temp.SysPK_region = mswd.dafac.region_id
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = mswd.dafac.province_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = mswd.dafac.barangay_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = mswd.dafac.citmun_id
                            LEFT JOIN general.evacuation_center ON general.evacuation_center.evacuation_center_guid = mswd.dafac.evacuation_center_guid
                            LEFT JOIN general.person ON general.person.person_guid = mswd.dafac.person_guid
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            WHERE mswd.dafac.status = 'Active' AND mswd.dafac.dafac_guid = @dafac_guid ";
            var obj = (DafacModel)QueryModule.DataObject<DafacModel>(sql, new { dafac_guid = dafac_guid });

            sql = @"SELECT mswd.dafac_details.main_guid, mswd.dafac_details.person_guid, mswd.dafac_details.relation, mswd.dafac_details.educational_attainment,
                    mswd.dafac_details.occupational_skills, mswd.dafac_details.remarks, general.person.prefix, general.person.suffix, general.person.first_name,
                    general.person.middle_name, general.person.last_name, general.person.age, general.person.gender_id, general.gender.gender_name, mswd.dafac_details.occupation_income,
                    general.educational_type.educational_name
                    FROM mswd.dafac_details
                    LEFT JOIN general.person ON general.person.person_guid = mswd.dafac_details.person_guid
                    LEFT JOIN general.gender ON general.gender.gender_id =  general.person.gender_id
                    LEFT JOIN general.educational_type ON general.educational_type.id = mswd.dafac_details.educational_attainment
                    WHERE mswd.dafac_details.main_guid = '" + obj.dafac_guid + "' ";
            obj.family_details = (List<DafacDetails>)QueryModule.DataSource<DafacDetails>(sql);

            sql = @"SELECT mswd.dafac_family_assistance.main_guid, mswd.dafac_family_assistance.person_guid, mswd.dafac_family_assistance.date, 
                    mswd.dafac_family_assistance.kind_type, mswd.dafac_family_assistance.qty, mswd.dafac_family_assistance.cost,
                    mswd.dafac_family_assistance.provider, general.person.prefix, general.person.suffix, general.person.first_name,
                    general.person.middle_name, general.person.last_name
                    FROM mswd.dafac_family_assistance
                    LEFT JOIN general.person ON general.person.person_guid = mswd.dafac_family_assistance.person_guid
                    WHERE mswd.dafac_family_assistance.main_guid = '" + obj.dafac_guid + "' ";
            obj.assistance_details = (List<AssistanceDetails>)QueryModule.DataSource<AssistanceDetails>(sql);

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
            var obj = (DafacModel)QueryModule.DataObject<DafacModel>(sql);

            if (obj == null)
                return null;

            //sql = "select mswd.family_composition_details.main_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix, " +
            //    "general.person.age, general.person.civil_status_id, general.civil_status.civil_status_name, mswd.family_composition_details.person_guid, mswd.family_composition_details.status, mswd.family_composition_details.educational_attainment, " +
            //    "mswd.family_composition_details.relation, mswd.family_composition_details.occupation, mswd.family_composition_details.occupation_income " +
            //    "from mswd.family_composition_details " +
            //    "INNER JOIN general.person ON general.person.person_guid = mswd.family_composition_details.person_guid " +
            //    "INNER JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id " +
            //    "where mswd.family_composition_details.main_guid = '" + obj.family_composition_guid + "' ";
            //obj.details = (List<DafacDetails>)QueryModule.DataSource<DafacDetails>(sql);

            return obj;
        }

        public List<object> GetHistoryLogs(string GUID)
        {
            string sql = @"SELECT mswd.dafac_logs.main_guid, mswd.dafac.application_date, mswd.dafac_logs.user_id, mswd.dafac.form_trans_no,
                            mswd.dafac.status, mswd.dafac.evacuation_center_guid, general.evacuation_center.venue, general.evacuation_center.venue_condition,
                            general.evacuation_center.capacity, mswd.dafac.person_guid, general.person.prefix, general.person.suffix, general.person.first_name,
                            general.person.middle_name, general.person.last_name
                            FROM mswd.dafac_logs  
                            LEFT JOIN mswd.dafac ON mswd.dafac.dafac_guid = mswd.dafac_logs.main_guid  
                            LEFT JOIN general.evacuation_center ON general.evacuation_center.evacuation_center_guid = mswd.dafac.evacuation_center_guid 
                            LEFT JOIN general.person ON general.person.person_guid = mswd.dafac.person_guid
                            WHERE mswd.dafac_logs.main_guid = '" + GUID + "' ORDER BY mswd.dafac.id DESC";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public bool Insert(DafacModel model)
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
                    model.form_trans_no = generateFormNumber("dafac-registration");
                    model.dafac_guid = Guid.NewGuid().ToString();
                    model.status = "Active";

                    string sql = @"INSERT INTO mswd.dafac (`person_guid`, `evacuation_center_guid`, `mothers_maiden_name`, `monthly_family_income`, `id_card_presented`, `id_card_number`, `primary_contact`, `alternate_contact`, 
                        `fourps_beneficiary`, `ips`, `type_of_ethnicity`, `no_of_older`, `no_of_pregnant_or_lactating`, `no_of_pwds_and_conditions`, `house_ownership`, `housing_conditioning`, `dafac_guid`, `status`, 
                        `member_count`, `form_trans_no`, `name_extension`, `region_id`, `province_id`, `district`, `barangay_id`, `citmun_id`, `occupation`, `educational_attainment`, `total_family_income`) 
                        VALUES(@person_guid, @evacuation_center_guid, @mothers_maiden_name, @monthly_family_income, @id_card_presented, @id_card_number, @primary_contact, @alternate_contact, @fourps_beneficiary, @ips, 
                        @type_of_ethnicity, @no_of_older, @no_of_pregnant_or_lactating, @no_of_pwds_and_conditions, @house_ownership, @housing_conditioning, @dafac_guid, @status, @member_count, @form_trans_no, @name_extension, 
                        @region_id, @province_id, @district, @barangay_id, @citmun_id, @occupation, @educational_attainment, @total_family_income)";
                    int res = (int)QueryModule.DataObject<int>(sql, model);

                    if (model.family_details.Count > 0)
                    {
                        foreach (var dt in model.family_details)
                        {
                            dt.main_guid = model.dafac_guid;
                            sql = @"INSERT INTO mswd.dafac_details (`main_guid`, `person_guid`, `relation`, `educational_attainment`, `occupational_skills`, `remarks`, `occupation_income`)
                                VALUES(@main_guid, @person_guid, @relation, @educational_attainment, @occupational_skills, @remarks, @occupation_income)";
                            QueryModule.Execute<int>(sql, dt);
                        }
                    }

                    if (model.assistance_details.Count > 0)
                    {
                        foreach (var dt in model.assistance_details)
                        {
                            dt.main_guid = model.dafac_guid;
                            sql = @"INSERT INTO mswd.dafac_family_assistance (`main_guid`, `person_guid`, `kind_type`, `qty`, `cost`, `provider`)
                                VALUES(@main_guid, @person_guid, @kind_type, @qty, @cost, @provider)";
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
