using eegs_back_end.Admin.IndigentSetup.Model;
using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.IndigentSetup.Repository
{
    public interface IIndigentRepository :IGlobalInterface
    {
        bool Insert(IndigentModel model);
        List<object> GetListOfDisability();
        List<object> GetPhilhealthMembership();
        List<object> GetList();
        List<object> GetAllList();
        List<object> GetListDeleted();
        List<object> GetListGenerated(int? filter_type_status_id = 0, int? status_id = 0, int? status_deleted_id = 0, string? this_month = "", string? this_year = "", string? monthly = "", string? monthlyYear = "", string? year_quarterly = "", int? quarter = 0, string? yearly = "", string? from = "", string? to = "");
        List<object> GetDetails(string GUID);
        object GetPersonAdd(string GUID);
        List<object> GetHistoryLogs(string GUID);
        bool Edit(string GUID, IndigentModel model);
        bool Delete(string GUID);
        object GetIndigent(string Indigent_guid);
    }
    public class IndigentRepository : FormNumberGenerator, IIndigentRepository
    {
        public bool Edit(string GUID, IndigentModel model)
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
                    model.indigent_guid = GUID;
                    model.status = "Active";

                    var sql = "UPDATE mswd.Indigent SET status='Trash' " +
                        "WHERE mswd.Indigent.Indigent_guid = '" + GUID + "'   ";
                    int res = (int)QueryModule.DataObject<int>(sql, model);

                    sql = @"INSERT INTO mswd.Indigent (`Indigent_guid`, `person_guid`, `form_trans_no`, `status`, `fourps_beneficiary`, `ips`, `member_count`, `educational_attainment`, `annual_income`, `occupation`, `other_source_of_income`, `application_type`, `name_of_association`, `association_address`, `date_of_membership`, `date_elected`, `ip_name`, 
                        `sss_pensioner`, `sss_monthly_pension`, `gsis_pensioner`, `gsis_monthly_pension`, `pvao_pensioner`, `pvao_monthly_pension`, `philhealth_membership_id`, `household_id_no`, `pantawid_beneficiary_type`, `total_family_income`, `house_ownership`, `monthly_income`) 
                        VALUES(@Indigent_guid, @person_guid, @form_trans_no, @status, @fourps_beneficiary, @ips, @member_count, @educational_attainment, @annual_income, @occupation, @other_source_of_income, @application_type, @name_of_association, @association_address, @date_of_membership, @date_elected, @ip_name, 
                        @sss_pensioner, @sss_monthly_pension, @gsis_pensioner, @gsis_monthly_pension, @pvao_pensioner, @pvao_monthly_pension, @philhealth_membership_id, @household_id_no, @pantawid_beneficiary_type, @total_family_income, @house_ownership, @monthly_income )";
                    QueryModule.Execute<int>(sql, model);

                    sql = "DELETE FROM mswd.Indigent_details WHERE mswd.Indigent_details.main_guid = '" + GUID + "' ";
                    QueryModule.Execute<int>(sql, model);

                    sql = "DELETE FROM mswd.Indigent_logs WHERE mswd.Indigent_logs.main_guid = '" + GUID + "' ";
                    QueryModule.Execute<int>(sql, model);

                    sql = "INSERT INTO mswd.Indigent_logs (`main_guid`, `user_id`) VALUES('" + model.indigent_guid + "', '" + GlobalObject.user_id + "')";
                    QueryModule.Execute<int>(sql, model);

                    if (model.family_details.Count > 0)
                    {
                        foreach (var dt in model.family_details)
                        {
                            dt.main_guid = model.indigent_guid;
                            sql = @"INSERT INTO mswd.Indigent_details (`main_guid`, `person_guid`, `relation`, `educational_attainment`, `occupational_skills`, `remarks`, `occupation_income`)
                                VALUES(@main_guid, @person_guid, @relation, @educational_attainment, @occupational_skills, @remarks, @occupation_income)";
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
            string sql = "UPDATE mswd.Indigent SET status = 'Deleted' where mswd.Indigent.Indigent_guid = @Indigent_guid";
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

        public List<object> GetPhilhealthMembership()
        {
            string sql = "SELECT * FROM mswd.philhealth_membership";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public List<object> GetList()
        {
            string sql = @"SELECT mswd.osca_registration.osca_registration_guid, mswd.osca_registration.person_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, 
                            general.person.suffix, general_address.lgu_province_setup_temp.province_name, general_address.lgu_city_mun_setup_temp.city_mun_name, general_address.lgu_brgy_setup_temp.brgy_name,
                            mswd.osca_registration.form_trans_no, general.person.birth_date, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.gender_id, 
                            general.gender.gender_name, mswd.osca_registration.application_date, mswd.osca_registration.status, mswd.osca_registration.member_count, mswd.osca_registration.fourps_beneficiary,
                            mswd.osca_registration.classification_id, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.osca_registration WHERE mswd.osca_registration.status = 'Active' AND mswd.osca_registration.classification_id = '1') as 'count'
                            FROM mswd.osca_registration
                            LEFT JOIN general.person ON general.person.person_guid = mswd.osca_registration.person_guid
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            WHERE mswd.osca_registration.status = 'Active' AND mswd.osca_registration.classification_id = '1'
                            ORDER BY mswd.osca_registration.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("sc-indigent");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.osca_registration osca_reg
                    INNER JOIN general.person ON general.person.person_guid = osca_reg.person_guid
                    WHERE osca_reg.status = 'Active' AND osca_reg.classification_id = '1'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.osca_registration osca_reg
                    LEFT JOIN general.person ON general.person.person_guid = osca_reg.person_guid
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
            string sql = @"SELECT mswd.osca_registration.osca_registration_guid, mswd.osca_registration.person_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, 
                            general.person.suffix, general_address.lgu_province_setup_temp.province_name, general_address.lgu_city_mun_setup_temp.city_mun_name, general_address.lgu_brgy_setup_temp.brgy_name,
                            mswd.osca_registration.form_trans_no, general.person.birth_date, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.gender_id, 
                            general.gender.gender_name, mswd.osca_registration.application_date, mswd.osca_registration.status, mswd.osca_registration.member_count, mswd.osca_registration.fourps_beneficiary,
                            mswd.osca_registration.classification_id, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.osca_registration WHERE mswd.osca_registration.status = 'Active' OR mswd.osca_registration.status = 'Deleted' AND mswd.osca_registration.classification_id = '1') as 'count'
                            FROM mswd.osca_registration
                            LEFT JOIN general.person ON general.person.person_guid = mswd.osca_registration.person_guid
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            WHERE mswd.osca_registration.status = 'Active' OR mswd.osca_registration.status = 'Deleted' AND mswd.osca_registration.classification_id = '1'
                            ORDER BY mswd.osca_registration.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("sc-indigent");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.osca_registration osca_reg
                    INNER JOIN general.person ON general.person.person_guid = osca_reg.person_guid
                    WHERE osca_reg.status = 'Active' AND osca_reg.classification_id = '1'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.osca_registration osca_reg
                    LEFT JOIN general.person ON general.person.person_guid = osca_reg.person_guid
                    LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                    ) z";
            List<object> searches = (List<object>)QueryModule.DataSource<object>(sql);

            List<object> list = new List<object>();
            list.Add(obj);
            list.Add(form);
            list.Add(searches);

            return list;
        }

        //public List<object> GetList()
        //{
        //    string sql = @"SELECT mswd.indigent.indigent_guid, mswd.indigent.person_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, 
        //                    general.person.suffix, general_address.lgu_province_setup_temp.province_name, general_address.lgu_city_mun_setup_temp.city_mun_name, general_address.lgu_brgy_setup_temp.brgy_name,
        //                    mswd.indigent.form_trans_no, general.person.birth_date, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.gender_id, 
        //                    general.gender.gender_name, mswd.indigent.application_date, mswd.indigent.status
        //                    FROM mswd.indigent
        //                    INNER JOIN general.person ON general.person.person_guid = mswd.indigent.person_guid
        //                    INNER JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
        //                    INNER JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
        //                    INNER JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
        //                    INNER JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
        //                    INNER JOIN general.gender ON general.gender.gender_id = general.person.gender_id
        //                    WHERE mswd.indigent.status = 'Active' ORDER BY mswd.indigent.id DESC";

        //    List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

        //    ExpandoObject form = Forms.getForm("indigent-registration");

        //    List<object> list = new List<object>();
        //    list.Add(obj);
        //    list.Add(form);


        //    return list;
        //}

        //public List<object> GetAllList()
        //{
        //    string sql = @"SELECT mswd.indigent.indigent_guid, mswd.indigent.person_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, 
        //                    general.person.suffix, general_address.lgu_province_setup_temp.province_name, general_address.lgu_city_mun_setup_temp.city_mun_name, general_address.lgu_brgy_setup_temp.brgy_name,
        //                    mswd.indigent.form_trans_no, general.person.birth_date, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.gender_id, 
        //                    general.gender.gender_name, mswd.indigent.application_date, mswd.indigent.status
        //                    FROM mswd.indigent
        //                    LEFT JOIN general.person ON general.person.person_guid = mswd.indigent.person_guid
        //                    LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
        //                    LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
        //                    LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
        //                    LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
        //                    LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
        //                    WHERE mswd.indigent.status = 'Active' OR mswd.indigent.status = 'Deleted' ORDER BY mswd.indigent.id DESC";

        //    List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

        //    ExpandoObject form = Forms.getForm("indigent-registration");

        //    List<object> list = new List<object>();
        //    list.Add(obj);
        //    list.Add(form);


        //    return list;
        //}

        public List<object> GetListDeleted()
        {

            string sql = @"SELECT mswd.osca_registration.osca_registration_guid, mswd.osca_registration.person_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, 
                            general.person.suffix, general_address.lgu_province_setup_temp.province_name, general_address.lgu_city_mun_setup_temp.city_mun_name, general_address.lgu_brgy_setup_temp.brgy_name,
                            mswd.osca_registration.form_trans_no, general.person.birth_date, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.gender_id, 
                            general.gender.gender_name, mswd.osca_registration.application_date, mswd.osca_registration.status, mswd.osca_registration.member_count, mswd.osca_registration.fourps_beneficiary,
                            mswd.osca_registration.classification_id, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.osca_registration WHERE mswd.osca_registration.status = 'Deleted' AND mswd.osca_registration.classification_id = '1') as 'count'
                            FROM mswd.osca_registration
                            LEFT JOIN general.person ON general.person.person_guid = mswd.osca_registration.person_guid
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            WHERE mswd.osca_registration.status = 'Deleted' AND mswd.osca_registration.classification_id = '1'
                            ORDER BY mswd.osca_registration.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("sc-indigent");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.osca_registration osca_reg
                    INNER JOIN general.person ON general.person.person_guid = osca_reg.person_guid
                    WHERE osca_reg.status = 'Deleted' AND osca_reg.classification_id = '1'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.osca_registration osca_reg
                    LEFT JOIN general.person ON general.person.person_guid = osca_reg.person_guid
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
                filterStatus = "mswd.osca_registration.status = 'Active' AND mswd.osca_registration.classification_id = '1'";
            }
            else if (status_deleted_id != 0)
            {
                filterStatus = "mswd.osca_registration.status = 'Deleted' AND mswd.osca_registration.classification_id = '1'";
            }

            if (filter_type_status_id == 1)
            {
                dateFilter = "AND MONTH(mswd.osca_registration.application_date) = '" + this_month + "'";
            }
            else if (filter_type_status_id == 2)
            {
                dateFilter = "AND YEAR(mswd.osca_registration.application_date) = '" + this_year + "'";
            }
            else if (filter_type_status_id == 3)
            {
                dateFilter = "AND MONTH(mswd.osca_registration.application_date) = '" + monthly + "' AND YEAR(mswd.osca_registration.application_date) = '" + monthlyYear + "'";
            }
            else if (filter_type_status_id == 4)
            {
                dateFilter = "AND YEAR(mswd.osca_registration.application_date) = '" + year_quarterly + "' AND QUARTER(mswd.osca_registration.application_date) = '" + quarter + "'";
            }
            else if (filter_type_status_id == 5)
            {
                dateFilter = "AND YEAR(mswd.osca_registration.application_date) = '" + yearly + "'";
            }
            else if (filter_type_status_id == 6)
            {
                dateFilter = "AND mswd.osca_registration.application_date >= '" + from + "' AND mswd.osca_registration.application_date <= '" + to + "'";
            }

            string sql = @"SELECT mswd.osca_registration.osca_registration_guid, mswd.osca_registration.person_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, 
                            general.person.suffix, general_address.lgu_province_setup_temp.province_name, general_address.lgu_city_mun_setup_temp.city_mun_name, general_address.lgu_brgy_setup_temp.brgy_name,
                            mswd.osca_registration.form_trans_no, general.person.birth_date, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.gender_id, 
                            general.gender.gender_name, mswd.osca_registration.application_date, mswd.osca_registration.status, mswd.osca_registration.member_count, mswd.osca_registration.fourps_beneficiary,
                            mswd.osca_registration.classification_id, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.osca_registration WHERE " + filterStatus + @" " + dateFilter  + @") as 'count'
                            FROM mswd.osca_registration
                            LEFT JOIN general.person ON general.person.person_guid = mswd.osca_registration.person_guid
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            WHERE " + filterStatus + @" " + dateFilter  + @"
                            ORDER BY mswd.osca_registration.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("sc-indigent");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.osca_registration osca_reg
                    INNER JOIN general.person ON general.person.person_guid = osca_reg.person_guid
                    WHERE osca_reg.status = 'Active' AND osca_reg.classification_id = '1'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.osca_registration osca_reg
                    LEFT JOIN general.person ON general.person.person_guid = osca_reg.person_guid
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
            string sql = @"SELECT mswd.Indigent_details.person_guid, mswd.Indigent_details.relation, mswd.Indigent_details.educational_attainment, mswd.Indigent_details.occupational_skills,
                            mswd.Indigent_details.remarks, general.person.prefix, general.person.suffix, general.person.first_name, general.person.middle_name, general.person.last_name,
                            general.person.gender_id, general.gender.gender_name, general.person.age
                            FROM mswd.Indigent_details
                            INNER JOIN general.person ON general.person.person_guid = mswd.Indigent_details.person_guid
                            INNER JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            WHERE mswd.Indigent_details.main_guid = '" + GUID + "' ";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public object GetIndigent(string Indigent_guid)
        {
            string sql = @"SELECT mswd.Indigent.Indigent_guid, mswd.Indigent.person_guid, general.person.prefix, general.person.suffix, general.person.first_name, general.person.middle_name, general.person.last_name,
                            mswd.Indigent.form_trans_no, mswd.Indigent.application_date, mswd.Indigent.status, mswd.Indigent.fourps_beneficiary, mswd.Indigent.ips, mswd.Indigent.member_count, mswd.Indigent.educational_attainment,
                            mswd.Indigent.total_family_income, general.person.birth_date, general.person.place_of_birth, general.person.gender_id, general.gender.gender_name,
                            general.person.age, general.person.province_id, general_address.lgu_province_setup_temp.province_name, general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name,
                            general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.street, mswd.Indigent.annual_income, mswd.Indigent.occupation,
                            mswd.Indigent.other_source_of_income, mswd.Indigent.application_type, mswd.Indigent.name_of_association, mswd.Indigent.association_address, mswd.Indigent.date_of_membership,
                            mswd.Indigent.date_elected, mswd.Indigent.ip_name, mswd.Indigent.sss_pensioner, mswd.Indigent.sss_monthly_pension, mswd.Indigent.gsis_pensioner, mswd.Indigent.gsis_monthly_pension, mswd.Indigent.pvao_pensioner, mswd.Indigent.pvao_monthly_pension,
                            mswd.Indigent.philhealth_membership_id, mswd.philhealth_membership.name, mswd.Indigent.household_id_no, mswd.Indigent.pantawid_beneficiary_type, general.person.civil_status_id, general.civil_status.civil_status_name, mswd.Indigent.house_ownership,
                            mswd.Indigent.monthly_income, general.person.person_image
                            FROM mswd.Indigent
                            INNER JOIN general.person ON general.person.person_guid = mswd.Indigent.person_guid
                            INNER JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            INNER JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            INNER JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            INNER JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            INNER JOIN mswd.philhealth_membership ON mswd.philhealth_membership.id = mswd.Indigent.philhealth_membership_id
                            INNER JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            WHERE mswd.Indigent.status = 'Active' AND mswd.Indigent.Indigent_guid = @Indigent_guid ";
            var obj = (IndigentModel)QueryModule.DataObject<IndigentModel>(sql, new { Indigent_guid = Indigent_guid });

            sql = @"SELECT mswd.Indigent_details.main_guid, mswd.Indigent_details.person_guid, mswd.Indigent_details.relation, mswd.Indigent_details.educational_attainment,
                    mswd.Indigent_details.occupational_skills, mswd.Indigent_details.remarks, general.person.prefix, general.person.suffix, general.person.first_name,
                    general.person.middle_name, general.person.last_name, general.person.age, general.person.gender_id, general.gender.gender_name, mswd.Indigent_details.occupation_income,
                    general.person.civil_status_id, general.civil_status.civil_status_name
                    FROM mswd.Indigent_details
                    INNER JOIN general.person ON general.person.person_guid = mswd.Indigent_details.person_guid
                    INNER JOIN general.gender ON general.gender.gender_id =  general.person.gender_id
                    INNER JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                    WHERE mswd.Indigent_details.main_guid = '" + obj.indigent_guid + "' ";
            obj.family_details = (List<IndigentDetails>)QueryModule.DataSource<IndigentDetails>(sql);

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
            var obj = (IndigentModel)QueryModule.DataObject<IndigentModel>(sql);

            if (obj == null)
                return null;

            //sql = "select mswd.family_composition_details.main_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix, " +
            //    "general.person.age, general.person.civil_status_id, general.civil_status.civil_status_name, mswd.family_composition_details.person_guid, mswd.family_composition_details.status, mswd.family_composition_details.educational_attainment, " +
            //    "mswd.family_composition_details.relation, mswd.family_composition_details.occupation, mswd.family_composition_details.occupation_income " +
            //    "from mswd.family_composition_details " +
            //    "INNER JOIN general.person ON general.person.person_guid = mswd.family_composition_details.person_guid " +
            //    "INNER JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id " +
            //    "where mswd.family_composition_details.main_guid = '" + obj.family_composition_guid + "' ";
            //obj.details = (List<IndigentDetails>)QueryModule.DataSource<IndigentDetails>(sql);

            return obj;
        }

        public List<object> GetHistoryLogs(string GUID)
        {
            string sql = @"SELECT mswd.Indigent_logs.main_guid, mswd.Indigent.application_date, mswd.Indigent_logs.user_id, mswd.Indigent.form_trans_no,
                            mswd.Indigent.status, mswd.Indigent.person_guid, general.person.prefix, general.person.suffix, general.person.first_name,
                            general.person.middle_name, general.person.last_name, mswd.Indigent.ips, mswd.Indigent.educational_attainment, mswd.Indigent.total_family_income
                            FROM mswd.Indigent_logs  
                            INNER JOIN mswd.Indigent ON mswd.Indigent.Indigent_guid = mswd.Indigent_logs.main_guid  
                            INNER JOIN general.person ON general.person.person_guid = mswd.Indigent.person_guid
                            WHERE mswd.Indigent_logs.main_guid = '" + GUID + "' ORDER BY mswd.Indigent.id DESC";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public bool Insert(IndigentModel model)
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
                    model.form_trans_no = generateFormNumber("Indigent-id");
                    model.indigent_guid = Guid.NewGuid().ToString();
                    model.status = "Active";

                    string sql = @"INSERT INTO mswd.Indigent (`Indigent_guid`, `person_guid`, `form_trans_no`, `status`, `fourps_beneficiary`, `ips`, `member_count`, `educational_attainment`, `annual_income`, `occupation`, `other_source_of_income`, `application_type`, `name_of_association`, `association_address`, `date_of_membership`, `date_elected`, `ip_name`, 
                        `sss_pensioner`, `sss_monthly_pension`, `gsis_pensioner`, `gsis_monthly_pension`, `pvao_pensioner`, `pvao_monthly_pension`, `philhealth_membership_id`, `household_id_no`, `pantawid_beneficiary_type`, `total_family_income`, `house_ownership`, `monthly_income`) 
                        VALUES(@Indigent_guid, @person_guid, @form_trans_no, @status, @fourps_beneficiary, @ips, @member_count, @educational_attainment, @annual_income, @occupation, @other_source_of_income, @application_type, @name_of_association, @association_address, @date_of_membership, @date_elected, @ip_name, 
                        @sss_pensioner, @sss_monthly_pension, @gsis_pensioner, @gsis_monthly_pension, @pvao_pensioner, @pvao_monthly_pension, @philhealth_membership_id, @household_id_no, @pantawid_beneficiary_type, @total_family_income, @house_ownership, @monthly_income )";
                    int res = (int)QueryModule.DataObject<int>(sql, model);

                    if (model.family_details.Count > 0)
                    {
                        foreach (var dt in model.family_details)
                        {
                            dt.main_guid = model.indigent_guid;
                            sql = @"INSERT INTO mswd.Indigent_details (`main_guid`, `person_guid`, `relation`, `educational_attainment`, `occupational_skills`, `remarks`, `occupation_income`)
                                VALUES(@main_guid, @person_guid, @relation, @educational_attainment, @occupational_skills, @remarks, @occupation_income)";
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
