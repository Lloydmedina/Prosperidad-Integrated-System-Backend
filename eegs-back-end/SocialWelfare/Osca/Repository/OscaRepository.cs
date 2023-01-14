using eegs_back_end.Admin.OscaSetup.Model;
using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.OscaSetup.Repository
{
    public interface IOscaRepository :IGlobalInterface
    {
        bool Insert(OscaModel model);
        List<object> GetListOfDisability();
        List<object> GetPhilhealthMembership();
        List<object> GetList();
        List<object> GetListDeleted();
        List<object> GetListGenerated(int? filter_type_status_id = 0, int? status_id = 0, int? status_deleted_id = 0, string? this_month = "", string? this_year = "", string? monthly = "", string? monthlyYear = "", string? year_quarterly = "", int? quarter = 0, string? yearly = "", string? from = "", string? to = "");
        List<object> GetEducationalType();
        List<object> GetAllList();
        List<object> GetDetails(string GUID);
        object GetPersonAdd(string GUID);
        List<object> GetHistoryLogs(string GUID);
        bool Edit(string GUID, OscaModel model);
        bool Delete(string GUID);
        object GetOsca(string Osca_guid);
    }
    public class OscaRepository : FormNumberGenerator, IOscaRepository
    {
        public bool Edit(string GUID, OscaModel model)
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
                    model.osca_guid = GUID;
                    model.status = "Active";

                    var sql = "UPDATE mswd.osca SET status='Trash' " +
                        "WHERE mswd.osca.osca_guid = '" + GUID + "'   ";
                    int res = (int)QueryModule.DataObject<int>(sql, model);

                    sql = "UPDATE mswd.osca_registration SET annual_income='" + model.annual_income + "', other_source_of_income='" + model.other_source_of_income + "', name_of_association='" + model.name_of_association + "'," +
                        "address_of_association='" + model.association_address + "', date_of_membership='" + model.date_of_membership + "', date_elected='" + model.date_elected + "' " +
                        "WHERE mswd.osca_registration.osca_registration_guid = '" + model.osca_registration_guid + "' ";
                    QueryModule.DataObject<int>(sql, model);

                    sql = @"INSERT INTO mswd.osca (`osca_guid`, `osca_registration_guid`, `form_trans_no`, `status`, `member_count`, `application_type`, `ips`, `ip_name`, `sss_pensioner`, `sss_monthly_pension`, `gsis_pensioner`, `gsis_monthly_pension`, `pvao_pensioner`, 
                        `pvao_monthly_pension`, `philhealth_membership_id`, `fourps_beneficiary`, `household_id_no`, `pantawid_beneficiary_type`, `total_family_income`) 
                        VALUES(@osca_guid, @osca_registration_guid, @form_trans_no, @status, @member_count, @application_type, @ips, @ip_name, @sss_pensioner, @sss_monthly_pension, @gsis_pensioner, @gsis_monthly_pension, @pvao_pensioner, 
                        @pvao_monthly_pension, @philhealth_membership_id, @fourps_beneficiary, @household_id_no, @pantawid_beneficiary_type, @total_family_income)";
                    QueryModule.Execute<int>(sql, model);

                    sql = "DELETE FROM mswd.osca_details WHERE mswd.osca_details.main_guid = '" + GUID + "' ";
                    QueryModule.Execute<int>(sql, model);

                    sql = "DELETE FROM mswd.osca_logs WHERE mswd.osca_logs.main_guid = '" + GUID + "' ";
                    QueryModule.Execute<int>(sql, model);

                    sql = "INSERT INTO mswd.osca_logs (`main_guid`, `user_id`) VALUES('" + model.osca_guid + "', '" + GlobalObject.user_id + "')";
                    QueryModule.Execute<int>(sql, model);

                    if (model.family_details.Count > 0)
                    {
                        foreach (var dt in model.family_details)
                        {
                            dt.main_guid = model.osca_guid;
                            sql = @"INSERT INTO mswd.osca_details (`main_guid`, `person_guid`, `relation`, `educational_attainment`, `occupational_skills`, `remarks`, `occupation_income`)
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
            string sql = "UPDATE mswd.Osca SET status = 'Deleted' where mswd.Osca.Osca_guid = @Osca_guid";
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
            string sql = @"SELECT mswd.osca.osca_guid, mswd.osca.osca_registration_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, 
                            general.person.suffix, general_address.lgu_province_setup_temp.province_name, general_address.lgu_city_mun_setup_temp.city_mun_name, general_address.lgu_brgy_setup_temp.brgy_name,
                            mswd.osca.form_trans_no, general.person.birth_date, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.gender_id, 
                            general.gender.gender_name, mswd.osca.application_date, mswd.osca.member_count, mswd.osca.status, mswd.osca.ips, mswd.osca.fourps_beneficiary, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.osca LEFT JOIN mswd.osca_registration ON mswd.osca_registration.osca_registration_guid = mswd.osca.osca_registration_guid WHERE mswd.osca.status = 'Active' AND mswd.osca_registration.status = 'Active') as 'count'
                            FROM mswd.osca
                            LEFT JOIN mswd.osca_registration ON mswd.osca_registration.osca_registration_guid = mswd.osca.osca_registration_guid
                            LEFT JOIN general.person ON general.person.person_guid = mswd.osca_registration.person_guid
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            WHERE mswd.osca.status = 'Active' AND mswd.osca_registration.status = 'Active' ORDER BY mswd.osca.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("osca-id");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.osca osca
                    LEFT JOIN mswd.osca_registration osca_reg ON osca_reg.osca_registration_guid = osca.osca_registration_guid
                    LEFT JOIN general.person ON general.person.person_guid = osca_reg.person_guid
                    WHERE osca.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.osca osca
                    LEFT JOIN mswd.osca_registration osca_reg ON osca_reg.osca_registration_guid = osca.osca_registration_guid
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
            string sql = @"SELECT mswd.osca.osca_guid, mswd.osca.osca_registration_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, 
                            general.person.suffix, general_address.lgu_province_setup_temp.province_name, general_address.lgu_city_mun_setup_temp.city_mun_name, general_address.lgu_brgy_setup_temp.brgy_name,
                            mswd.osca.form_trans_no, general.person.birth_date, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.gender_id, 
                            general.gender.gender_name, mswd.osca.application_date, mswd.osca.member_count, mswd.osca.status, mswd.osca.ips, mswd.osca.fourps_beneficiary, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.osca LEFT JOIN mswd.osca_registration ON mswd.osca_registration.osca_registration_guid = mswd.osca.osca_registration_guid WHERE mswd.osca.status = 'Active' AND mswd.osca_registration.status = 'Active' OR mswd.osca.status = 'Deleted' OR mswd.osca_registration.status = 'Deleted') as 'count'
                            FROM mswd.osca
                            LEFT JOIN mswd.osca_registration ON mswd.osca_registration.osca_registration_guid = mswd.osca.osca_registration_guid
                            LEFT JOIN general.person ON general.person.person_guid = mswd.osca_registration.person_guid
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            WHERE mswd.osca.status = 'Active' AND mswd.osca_registration.status = 'Active' 
                            OR mswd.osca.status = 'Deleted' OR mswd.osca_registration.status = 'Deleted' ORDER BY mswd.osca.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("osca-id");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.osca osca
                    LEFT JOIN mswd.osca_registration osca_reg ON osca_reg.osca_registration_guid = osca.osca_registration_guid
                    LEFT JOIN general.person ON general.person.person_guid = osca_reg.person_guid
                    WHERE osca.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.osca osca
                    LEFT JOIN mswd.osca_registration osca_reg ON osca_reg.osca_registration_guid = osca.osca_registration_guid
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

        public List<object> GetListDeleted()
        {

            string sql = @"SELECT mswd.osca.osca_guid, mswd.osca.osca_registration_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, 
                            general.person.suffix, general_address.lgu_province_setup_temp.province_name, general_address.lgu_city_mun_setup_temp.city_mun_name, general_address.lgu_brgy_setup_temp.brgy_name,
                            mswd.osca.form_trans_no, general.person.birth_date, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.gender_id, 
                            general.gender.gender_name, mswd.osca.application_date, mswd.osca.member_count, mswd.osca.status, mswd.osca.ips, mswd.osca.fourps_beneficiary, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.osca LEFT JOIN mswd.osca_registration ON mswd.osca_registration.osca_registration_guid = mswd.osca.osca_registration_guid WHERE mswd.osca.status = 'Deleted' AND mswd.osca_registration.status = 'Active') as 'count'
                            FROM mswd.osca
                            LEFT JOIN mswd.osca_registration ON mswd.osca_registration.osca_registration_guid = mswd.osca.osca_registration_guid
                            LEFT JOIN general.person ON general.person.person_guid = mswd.osca_registration.person_guid
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            WHERE mswd.osca.status = 'Deleted' AND mswd.osca_registration.status = 'Active' ORDER BY mswd.osca.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("osca-id");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.osca osca
                    LEFT JOIN mswd.osca_registration osca_reg ON osca_reg.osca_registration_guid = osca.osca_registration_guid
                    LEFT JOIN general.person ON general.person.person_guid = osca_reg.person_guid
                    WHERE osca.status = 'Deleted'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.osca osca
                    LEFT JOIN mswd.osca_registration osca_reg ON osca_reg.osca_registration_guid = osca.osca_registration_guid
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
                filterStatus = "mswd.osca.status = 'Active' AND mswd.osca_registration.status = 'Active'";
            }
            else if (status_deleted_id != 0)
            {
                filterStatus = "mswd.osca.status = 'Deleted' AND mswd.osca_registration.status = 'Active'";
            }

            if (filter_type_status_id == 1)
            {
                dateFilter = "AND MONTH(mswd.osca.application_date) = '" + this_month + "'";
            }
            else if (filter_type_status_id == 2)
            {
                dateFilter = "AND YEAR(mswd.osca.application_date) = '" + this_year + "'";
            }
            else if (filter_type_status_id == 3)
            {
                dateFilter = "AND MONTH(mswd.osca.application_date) = '" + monthly + "' AND YEAR(mswd.osca.application_date) = '" + monthlyYear + "'";
            }
            else if (filter_type_status_id == 4)
            {
                dateFilter = "AND YEAR(mswd.osca.application_date) = '" + year_quarterly + "' AND QUARTER(mswd.osca.application_date) = '" + quarter + "'";
            }
            else if (filter_type_status_id == 5)
            {
                dateFilter = "AND YEAR(mswd.osca.application_date) = '" + yearly + "'";
            }
            else if (filter_type_status_id == 6)
            {
                dateFilter = "AND mswd.osca.application_date >= '" + from + "' AND mswd.osca.application_date <= '" + to + "'";
            }

            string sql = @"SELECT mswd.osca.osca_guid, mswd.osca.osca_registration_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, 
                            general.person.suffix, general_address.lgu_province_setup_temp.province_name, general_address.lgu_city_mun_setup_temp.city_mun_name, general_address.lgu_brgy_setup_temp.brgy_name,
                            mswd.osca.form_trans_no, general.person.birth_date, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.gender_id, 
                            general.gender.gender_name, mswd.osca.application_date, mswd.osca.member_count, mswd.osca.status, mswd.osca.ips, mswd.osca.fourps_beneficiary, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.osca LEFT JOIN mswd.osca_registration ON mswd.osca_registration.osca_registration_guid = mswd.osca.osca_registration_guid WHERE " + filterStatus + @" " + dateFilter  + @") as 'count'
                            FROM mswd.osca
                            LEFT JOIN mswd.osca_registration ON mswd.osca_registration.osca_registration_guid = mswd.osca.osca_registration_guid
                            LEFT JOIN general.person ON general.person.person_guid = mswd.osca_registration.person_guid
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            WHERE " + filterStatus + @" " + dateFilter  + @" ORDER BY mswd.osca.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("osca-id");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.osca osca
                    LEFT JOIN mswd.osca_registration osca_reg ON osca_reg.osca_registration_guid = osca.osca_registration_guid
                    LEFT JOIN general.person ON general.person.person_guid = osca_reg.person_guid
                    WHERE osca.status = 'Active'

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM mswd.osca osca
                    LEFT JOIN mswd.osca_registration osca_reg ON osca_reg.osca_registration_guid = osca.osca_registration_guid
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
            string sql = @"SELECT mswd.osca_details.person_guid, mswd.osca_details.relation, mswd.osca_details.educational_attainment, mswd.osca_details.occupational_skills,
                            mswd.osca_details.remarks, general.person.prefix, general.person.suffix, general.person.first_name, general.person.middle_name, general.person.last_name,
                            general.person.gender_id, general.gender.gender_name, general.person.age
                            FROM mswd.osca_details
                            LEFT JOIN general.person ON general.person.person_guid = mswd.osca_details.person_guid
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            WHERE mswd.osca_details.main_guid = '" + GUID + "' ";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public object GetOsca(string osca_guid)
        {
            string sql = @"SELECT mswd.osca.osca_guid, mswd.osca_registration.person_guid, general.person.prefix, general.person.suffix, general.person.first_name, general.person.middle_name, general.person.last_name,
                            mswd.osca.form_trans_no, mswd.osca.application_date, mswd.osca.status, mswd.osca.fourps_beneficiary, mswd.osca.ips, mswd.osca.member_count, general.person.educational_attainment,
                            mswd.osca.total_family_income, general.person.birth_date, general.person.place_of_birth, general.person.gender_id, general.gender.gender_name, mswd.osca.osca_registration_guid,
                            general.person.age, general.person.province_id, general_address.lgu_province_setup_temp.province_name, general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name,
                            general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.street, mswd.osca_registration.annual_income, general.person.profession,
                            mswd.osca_registration.other_source_of_income, mswd.osca.application_type, mswd.osca_registration.name_of_association, mswd.osca_registration.address_of_association, mswd.osca_registration.date_of_membership,
                            mswd.osca_registration.date_elected, mswd.osca.ip_name, mswd.osca.sss_pensioner, mswd.osca.sss_monthly_pension, mswd.osca.gsis_pensioner, mswd.osca.gsis_monthly_pension, mswd.osca.pvao_pensioner, mswd.osca.pvao_monthly_pension,
                            mswd.osca.philhealth_membership_id, mswd.philhealth_membership.name, mswd.osca.household_id_no, mswd.osca.pantawid_beneficiary_type, general.person.civil_status_id, general.civil_status.civil_status_name,
                            general.educational_type.educational_name, general.person.person_image
                            FROM mswd.osca
                            LEFT JOIN mswd.osca_registration ON mswd.osca_registration.osca_registration_guid = mswd.osca.osca_registration_guid
                            LEFT JOIN general.person ON general.person.person_guid = mswd.osca_registration.person_guid
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            LEFT JOIN mswd.philhealth_membership ON mswd.philhealth_membership.id = mswd.osca.philhealth_membership_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general.educational_type ON general.educational_type.id = general.person.educational_attainment
                            WHERE mswd.osca.status = 'Active' AND mswd.osca_registration.status = 'Active' AND mswd.osca.osca_guid = @osca_guid ";
            var obj = (OscaModel)QueryModule.DataObject<OscaModel>(sql, new { osca_guid = osca_guid });

            sql = @"SELECT mswd.osca_details.main_guid, mswd.osca_details.person_guid, mswd.osca_details.relation, mswd.osca_details.educational_attainment,
                    mswd.osca_details.occupational_skills, mswd.osca_details.remarks, general.person.prefix, general.person.suffix, general.person.first_name,
                    general.person.middle_name, general.person.last_name, general.person.age, general.person.gender_id, general.gender.gender_name, mswd.osca_details.occupation_income,
                    general.person.civil_status_id, general.civil_status.civil_status_name
                    FROM mswd.osca_details
                    LEFT JOIN general.person ON general.person.person_guid = mswd.osca_details.person_guid
                    LEFT JOIN general.gender ON general.gender.gender_id =  general.person.gender_id
                    LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                    WHERE mswd.osca_details.main_guid = '" + obj.osca_guid + "' ";
            obj.family_details = (List<OscaDetails>)QueryModule.DataSource<OscaDetails>(sql);

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
            var obj = (OscaModel)QueryModule.DataObject<OscaModel>(sql);

            if (obj == null)
                return null;

            //sql = "select mswd.family_composition_details.main_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix, " +
            //    "general.person.age, general.person.civil_status_id, general.civil_status.civil_status_name, mswd.family_composition_details.person_guid, mswd.family_composition_details.status, mswd.family_composition_details.educational_attainment, " +
            //    "mswd.family_composition_details.relation, mswd.family_composition_details.occupation, mswd.family_composition_details.occupation_income " +
            //    "from mswd.family_composition_details " +
            //    "INNER JOIN general.person ON general.person.person_guid = mswd.family_composition_details.person_guid " +
            //    "INNER JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id " +
            //    "where mswd.family_composition_details.main_guid = '" + obj.family_composition_guid + "' ";
            //obj.details = (List<OscaDetails>)QueryModule.DataSource<OscaDetails>(sql);

            return obj;
        }

        public List<object> GetHistoryLogs(string GUID)
        {
            string sql = @"SELECT mswd.osca_logs.main_guid, mswd.osca.application_date, mswd.osca_logs.user_id, mswd.osca.form_trans_no,
                            mswd.osca.status, mswd.osca.person_guid, general.person.prefix, general.person.suffix, general.person.first_name,
                            general.person.middle_name, general.person.last_name, mswd.osca.ips, mswd.osca.educational_attainment, mswd.osca.total_family_income
                            FROM mswd.osca_logs  
                            LEFT JOIN mswd.osca ON mswd.osca.osca_guid = mswd.osca_logs.main_guid  
                            LEFT JOIN general.person ON general.person.person_guid = mswd.osca.person_guid
                            WHERE mswd.osca_logs.main_guid = '" + GUID + "' ORDER BY mswd.osca.id DESC";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public bool Insert(OscaModel model)
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
                    model.form_trans_no = generateFormNumber("osca-id");
                    model.osca_guid = Guid.NewGuid().ToString();
                    model.status = "Active";

                    string sql = "UPDATE mswd.osca_registration SET annual_income='" + model.annual_income + "', other_source_of_income='" + model.other_source_of_income + "', name_of_association='" + model.name_of_association + "'," +
                        "address_of_association='" + model.association_address + "', date_of_membership='" + model.date_of_membership + "', date_elected='" + model.date_elected + "' " +
                        "WHERE mswd.osca_registration.osca_registration_guid = '" + model.osca_registration_guid + "' ";
                    QueryModule.DataObject<int>(sql, model);

                    sql = @"INSERT INTO mswd.osca (`osca_guid`, `osca_registration_guid`, `form_trans_no`, `status`, `member_count`, `application_type`, `ips`, `ip_name`, `sss_pensioner`, `sss_monthly_pension`, `gsis_pensioner`, `gsis_monthly_pension`, `pvao_pensioner`, 
                        `pvao_monthly_pension`, `philhealth_membership_id`, `fourps_beneficiary`, `household_id_no`, `pantawid_beneficiary_type`, `total_family_income`) 
                        VALUES(@osca_guid, @osca_registration_guid, @form_trans_no, @status, @member_count, @application_type, @ips, @ip_name, @sss_pensioner, @sss_monthly_pension, @gsis_pensioner, @gsis_monthly_pension, @pvao_pensioner, 
                        @pvao_monthly_pension, @philhealth_membership_id, @fourps_beneficiary, @household_id_no, @pantawid_beneficiary_type, @total_family_income)";
                    int res = (int)QueryModule.DataObject<int>(sql, model);

                    

                    if (model.family_details.Count > 0)
                    {
                        foreach (var dt in model.family_details)
                        {
                            dt.main_guid = model.osca_guid;
                            sql = @"INSERT INTO mswd.Osca_details (`main_guid`, `person_guid`, `relation`, `educational_attainment`, `occupational_skills`, `remarks`, `occupation_income`)
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
