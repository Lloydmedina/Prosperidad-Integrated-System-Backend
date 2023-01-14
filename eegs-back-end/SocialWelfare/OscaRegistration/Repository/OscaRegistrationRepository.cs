using eegs_back_end.Admin.OscaRegistrationSetup.Model;
using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.OscaRegistrationSetup.Repository
{
    public interface IOscaRegistrationRepository :IGlobalInterface
    {
        bool Insert(OscaRegistrationModel model);
        List<object> GetListOfDisability();
        List<object> GetPhilhealthMembership();
        List<object> GetList();

        List<object> GetListDeleted();
        List<object> GetListGenerated(int? filter_type_status_id = 0, int? status_id = 0, int? status_deleted_id = 0, string? this_month = "", string? this_year = "", string? monthly = "", string? monthlyYear = "", string? year_quarterly = "", int? quarter = 0, string? yearly = "", string? from = "", string? to = "");
        List<object> GetEducationalType();
        List<object> GetEmploymentStatus();
        List<object> GetClassification();
        List<object> GetLivingArrangement();
        List<object> GetAllList();
        List<object> GetDetails(string GUID);
        object GetPersonAdd(string GUID);
        List<object> GetHistoryLogs(string GUID);
        bool Edit(string GUID, OscaRegistrationModel model);
        bool Delete(string GUID);
        object GetOscaRegistration(string OscaRegistration_guid);
    }
    public class OscaRegistrationRepository : FormNumberGenerator, IOscaRegistrationRepository
    {
        public bool Edit(string GUID, OscaRegistrationModel model)
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
                    model.osca_registration_guid = GUID;
                    model.status = "Active";

                    var sql = "UPDATE mswd.osca_registration SET status='Trash' " +
                        "WHERE mswd.osca_registration.osca_registration_guid = '" + GUID + "'   ";
                    int res = (int)QueryModule.DataObject<int>(sql, model);

                    sql = @"INSERT INTO mswd.osca_registration (`osca_registration_guid`, `gsis_no`, `sss_no`, `tin_no`, `gsis_monthly_pension`, `sss_monthly_pension`, `incase_of_emergency`, `contact`, `person_guid`, `form_trans_no`, `status`, `employment_status_id`, `classification_id`,
                        `annual_income`, `other_source_of_income`, `name_of_association`, `address_of_association`, `household_id_no`, `philhealth_membership_id`, `fourps_beneficiary`, `fourps_beneficiary_type`, `date_of_membership`, `date_elected`, `living_arrangement_id`, `specify_others`, `gsis_pensioner`, `sss_pensioner`, `member_count`, `philhealth_no`) 
                        VALUES(@osca_registration_guid, @gsis_no, @sss_no, @tin_no, @gsis_monthly_pension, @sss_monthly_pension, @incase_of_emergency, @contact, @person_guid, @form_trans_no, @status, @employment_status_id, @classification_id, @annual_income, @other_source_of_income, @name_of_association, @address_of_association, @household_id_no,
                        @philhealth_membership_id, @fourps_beneficiary, @fourps_beneficiary_type, @date_of_membership, @date_elected, @living_arrangement_id, @specify_others, @gsis_pensioner, @sss_pensioner, @member_count, @philhealth_no)";
                    QueryModule.Execute<int>(sql, model);

                    sql = "DELETE FROM mswd.osca_registration_details WHERE mswd.osca_registration_details.main_guid = '" + GUID + "' ";
                    QueryModule.Execute<int>(sql, model);

                    sql = "DELETE FROM mswd.osca_registration_logs WHERE mswd.osca_registration_logs.main_guid = '" + GUID + "' ";
                    QueryModule.Execute<int>(sql, model);

                    sql = "INSERT INTO mswd.osca_registration_logs (`main_guid`, `user_id`) VALUES('" + model.osca_registration_guid + "', '" + GlobalObject.user_id + "')";
                    QueryModule.Execute<int>(sql, model);

                    if (model.family_details.Count > 0)
                    {
                        foreach (var dt in model.family_details)
                        {
                            dt.main_guid = model.osca_registration_guid;
                            sql = @"INSERT INTO mswd.osca_registration_details (`main_guid`, `person_guid`, `relation`, `educational_attainment`, `occupational_skills`, `remarks`, `occupation_income`)
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
            string sql = "UPDATE mswd.OscaRegistration SET status = 'Deleted' where mswd.OscaRegistration.OscaRegistration_guid = @OscaRegistration_guid";
            if (InsertUpdate.ExecuteSql(sql, new { family_composition_guid = GUID }))
                return true;
            return false;
        }

        public List<object> GetEmploymentStatus()
        {
            string sql = "SELECT * FROM mswd.employment_status_type";
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

        public List<object> GetLivingArrangement()
        {
            string sql = "SELECT * FROM mswd.living_arrangement";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public List<object> GetClassification()
        {
            string sql = "SELECT * FROM mswd.osca_classification";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
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
            string sql = @"SELECT mswd.osca_registration.osca_registration_guid, mswd.osca_registration.person_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, 
                            general.person.suffix, general_address.lgu_province_setup_temp.province_name, general_address.lgu_city_mun_setup_temp.city_mun_name, general_address.lgu_brgy_setup_temp.brgy_name,
                            mswd.osca_registration.form_trans_no, general.person.birth_date, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.gender_id, 
                            general.gender.gender_name, mswd.osca_registration.application_date, mswd.osca_registration.status, mswd.osca_registration.member_count, mswd.osca_registration.fourps_beneficiary, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.osca_registration WHERE mswd.osca_registration.status = 'Active') as 'count'
                            FROM mswd.osca_registration
                            LEFT JOIN general.person ON general.person.person_guid = mswd.osca_registration.person_guid
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            WHERE mswd.osca_registration.status = 'Active' ORDER BY mswd.osca_registration.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("osca-registration");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.osca_registration osca_reg
                    INNER JOIN general.person ON general.person.person_guid = osca_reg.person_guid
                    WHERE osca_reg.status = 'Active'

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
                            general.gender.gender_name, mswd.osca_registration.application_date, mswd.osca_registration.status, mswd.osca_registration.member_count, mswd.osca_registration.fourps_beneficiary, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.osca_registration WHERE mswd.osca_registration.status = 'Active' OR mswd.osca_registration.status = 'Deleted') as 'count'
                            FROM mswd.osca_registration
                            LEFT JOIN general.person ON general.person.person_guid = mswd.osca_registration.person_guid
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            WHERE mswd.osca_registration.status = 'Active' OR mswd.osca_registration.status = 'Deleted' ORDER BY mswd.osca_registration.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("osca-registration");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.osca_registration osca_reg
                    INNER JOIN general.person ON general.person.person_guid = osca_reg.person_guid
                    WHERE osca_reg.status = 'Active'

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

        public List<object> GetListDeleted()
        {

            string sql = @"SELECT mswd.osca_registration.osca_registration_guid, mswd.osca_registration.person_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, 
                            general.person.suffix, general_address.lgu_province_setup_temp.province_name, general_address.lgu_city_mun_setup_temp.city_mun_name, general_address.lgu_brgy_setup_temp.brgy_name,
                            mswd.osca_registration.form_trans_no, general.person.birth_date, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.gender_id, 
                            general.gender.gender_name, mswd.osca_registration.application_date, mswd.osca_registration.status, mswd.osca_registration.member_count, mswd.osca_registration.fourps_beneficiary, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.osca_registration WHERE mswd.osca_registration.status = 'Deleted') as 'count'
                            FROM mswd.osca_registration
                            LEFT JOIN general.person ON general.person.person_guid = mswd.osca_registration.person_guid
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            WHERE mswd.osca_registration.status = 'Deleted' ORDER BY mswd.osca_registration.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("osca-registration");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.osca_registration osca_reg
                    INNER JOIN general.person ON general.person.person_guid = osca_reg.person_guid
                    WHERE osca_reg.status = 'Deleted'

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
                filterStatus = "mswd.osca_registration.status = 'Active'";
            }
            else if (status_deleted_id != 0)
            {
                filterStatus = "mswd.osca_registration.status = 'Deleted'";
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
                            general.gender.gender_name, mswd.osca_registration.application_date, mswd.osca_registration.status, mswd.osca_registration.member_count, mswd.osca_registration.fourps_beneficiary, general.person.full_name,
                            (SELECT COUNT(*) FROM mswd.osca_registration WHERE " + filterStatus + @" " + dateFilter  + @") as 'count'
                            FROM mswd.osca_registration
                            LEFT JOIN general.person ON general.person.person_guid = mswd.osca_registration.person_guid
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            WHERE " + filterStatus + @" " + dateFilter  + @" ORDER BY mswd.osca_registration.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("osca-registration");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT general.person.full_name as q, 'fn' as tag
                    FROM mswd.osca_registration osca_reg
                    INNER JOIN general.person ON general.person.person_guid = osca_reg.person_guid
                    WHERE osca_reg.status = 'Active'

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
            string sql = @"SELECT mswd.OscaRegistration_details.person_guid, mswd.OscaRegistration_details.relation, mswd.OscaRegistration_details.educational_attainment, mswd.OscaRegistration_details.occupational_skills,
                            mswd.OscaRegistration_details.remarks, general.person.prefix, general.person.suffix, general.person.first_name, general.person.middle_name, general.person.last_name,
                            general.person.gender_id, general.gender.gender_name, general.person.age
                            FROM mswd.OscaRegistration_details
                            LEFT JOIN general.person ON general.person.person_guid = mswd.OscaRegistration_details.person_guid
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            WHERE mswd.OscaRegistration_details.main_guid = '" + GUID + "' ";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public object GetOscaRegistration(string osca_registration_guid)
        {
            string sql = @"SELECT mswd.osca_registration.osca_registration_guid, mswd.osca_registration.person_guid, general.person.prefix, general.person.suffix, general.person.first_name, general.person.middle_name, general.person.last_name,
                            mswd.osca_registration.form_trans_no, mswd.osca_registration.application_date, mswd.osca_registration.status, mswd.osca_registration.gsis_no, mswd.osca_registration.sss_no, mswd.osca_registration.tin_no,
                            mswd.osca_registration.gsis_pensioner, mswd.osca_registration.gsis_monthly_pension, mswd.osca_registration.sss_monthly_pension, mswd.osca_registration.incase_of_emergency, mswd.osca_registration.contact,
                            mswd.osca_registration.employment_status_id, mswd.osca_registration.classification_id, general.person.birth_date, general.person.place_of_birth, general.person.gender_id, general.gender.gender_name,
                            general.person.age, general.person.province_id, general_address.lgu_province_setup_temp.province_name, general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name,
                            general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.street, general.person.phone_no, general.person.telephone_no, general.person.email_address, general.person.blood_type_id,
                            general.blood_type.blood_type_name, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.religion, general.person.educational_attainment, general.educational_type.educational_name,
                            general.person.region_id, general_address.lgu_region_setup_temp.reg_name, general_address.lgu_region_setup_temp.reg_code, general.person.profession, general.educational_type.educational_name, mswd.employment_status_type.employment_status_name,
                            mswd.osca_classification.classification_name, mswd.osca_registration.annual_income, mswd.osca_registration.other_source_of_income, mswd.osca_registration.name_of_association, mswd.osca_registration.address_of_association,
                            mswd.osca_registration.household_id_no, mswd.osca_registration.philhealth_membership_id, mswd.philhealth_membership.name, mswd.osca_registration.fourps_beneficiary, mswd.osca_registration.fourps_beneficiary_type,
                            mswd.osca_registration.date_of_membership, mswd.osca_registration.date_elected, mswd.osca_registration.living_arrangement_id, mswd.living_arrangement.name as 'living_arrangement_status',
                            mswd.osca_registration.specify_others, mswd.osca_registration.sss_pensioner, mswd.osca_registration.member_count, general.person.citizenship, mswd.osca_registration.philhealth_no, general.person.person_image
                            FROM mswd.osca_registration
                            LEFT JOIN general.person ON general.person.person_guid = mswd.osca_registration.person_guid
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general.blood_type ON general.blood_type.blood_type_id = general.person.blood_type_id
                            LEFT JOIN general.educational_type ON general.educational_type.id = general.person.educational_attainment
                            LEFT JOIN general_address.lgu_region_setup_temp ON general_address.lgu_region_setup_temp.SysPK_region = general.person.region_id
                            LEFT JOIN mswd.employment_status_type ON mswd.employment_status_type.id = mswd.osca_registration.employment_status_id
                            LEFT JOIN mswd.osca_classification ON mswd.osca_classification.id = mswd.osca_registration.classification_id
                            LEFT JOIN mswd.philhealth_membership ON mswd.philhealth_membership.id = mswd.osca_registration.philhealth_membership_id
                            LEFT JOIN mswd.living_arrangement ON mswd.living_arrangement.id = mswd.osca_registration.living_arrangement_id
                            WHERE mswd.osca_registration.status = 'Active' AND mswd.osca_registration.osca_registration_guid = @osca_registration_guid ";
            var obj = (OscaRegistrationModel)QueryModule.DataObject<OscaRegistrationModel>(sql, new { osca_registration_guid = osca_registration_guid });

            sql = @"SELECT mswd.osca_registration_details.main_guid, mswd.osca_registration_details.person_guid, mswd.osca_registration_details.relation, general.person.educational_attainment,
                    mswd.osca_registration_details.occupational_skills, mswd.osca_registration_details.remarks, general.person.prefix, general.person.suffix, general.person.first_name,
                    general.person.middle_name, general.person.last_name, general.person.age, general.person.gender_id, general.gender.gender_name, mswd.osca_registration_details.occupation_income,
                    general.person.civil_status_id, general.civil_status.civil_status_name
                    FROM mswd.osca_registration_details
                    LEFT JOIN general.person ON general.person.person_guid = mswd.osca_registration_details.person_guid
                    LEFT JOIN general.gender ON general.gender.gender_id =  general.person.gender_id
                    LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                    WHERE mswd.osca_registration_details.main_guid = '" + obj.osca_registration_guid + "' ";
            obj.family_details = (List<OscaRegistrationDetails>)QueryModule.DataSource<OscaRegistrationDetails>(sql);

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
            var obj = (OscaRegistrationModel)QueryModule.DataObject<OscaRegistrationModel>(sql);

            if (obj == null)
                return null;

            //sql = "select mswd.family_composition_details.main_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix, " +
            //    "general.person.age, general.person.civil_status_id, general.civil_status.civil_status_name, mswd.family_composition_details.person_guid, mswd.family_composition_details.status, mswd.family_composition_details.educational_attainment, " +
            //    "mswd.family_composition_details.relation, mswd.family_composition_details.occupation, mswd.family_composition_details.occupation_income " +
            //    "from mswd.family_composition_details " +
            //    "INNER JOIN general.person ON general.person.person_guid = mswd.family_composition_details.person_guid " +
            //    "INNER JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id " +
            //    "where mswd.family_composition_details.main_guid = '" + obj.family_composition_guid + "' ";
            //obj.details = (List<OscaRegistrationDetails>)QueryModule.DataSource<OscaRegistrationDetails>(sql);

            return obj;
        }

        public List<object> GetHistoryLogs(string GUID)
        {
            string sql = @"SELECT mswd.osca_registration_logs.main_guid, mswd.osca_registration.application_date, mswd.osca_registration_logs.user_id, mswd.osca_registration.form_trans_no,
                            mswd.osca_registration.status, mswd.osca_registration.person_guid, general.person.prefix, general.person.suffix, general.person.first_name,
                            general.person.middle_name, general.person.last_name
                            FROM mswd.osca_registration_logs  
                            LEFT JOIN mswd.osca_registration ON mswd.osca_registration.osca_registration_guid = mswd.osca_registration_logs.main_guid  
                            LEFT JOIN general.person ON general.person.person_guid = mswd.osca_registration.person_guid
                            WHERE mswd.osca_registration_logs.main_guid = '" + GUID + "' ORDER BY mswd.osca_registration.id DESC";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public bool Insert(OscaRegistrationModel model)
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
                    model.form_trans_no = generateFormNumber("osca-registration");
                    model.osca_registration_guid = Guid.NewGuid().ToString();
                    model.status = "Active";

                    string sql = @"INSERT INTO mswd.osca_registration (`osca_registration_guid`, `gsis_no`, `sss_no`, `tin_no`, `gsis_monthly_pension`, `sss_monthly_pension`, `incase_of_emergency`, `contact`, `person_guid`, `form_trans_no`, `status`, `employment_status_id`, `classification_id`,
                        `annual_income`, `other_source_of_income`, `name_of_association`, `address_of_association`, `household_id_no`, `philhealth_membership_id`, `fourps_beneficiary`, `fourps_beneficiary_type`, `date_of_membership`, `date_elected`, `living_arrangement_id`, `specify_others`, `gsis_pensioner`, `sss_pensioner`, `member_count`, `philhealth_no`) 
                        VALUES(@osca_registration_guid, @gsis_no, @sss_no, @tin_no, @gsis_monthly_pension, @sss_monthly_pension, @incase_of_emergency, @contact, @person_guid, @form_trans_no, @status, @employment_status_id, @classification_id, @annual_income, @other_source_of_income, @name_of_association, @address_of_association, @household_id_no,
                        @philhealth_membership_id, @fourps_beneficiary, @fourps_beneficiary_type, @date_of_membership, @date_elected, @living_arrangement_id, @specify_others, @gsis_pensioner, @sss_pensioner, @member_count, @philhealth_no)";
                    int res = (int)QueryModule.DataObject<int>(sql, model);

                    if (model.family_details.Count > 0)
                    {
                        foreach (var dt in model.family_details)
                        {
                            dt.main_guid = model.osca_registration_guid;
                            sql = @"INSERT INTO mswd.osca_registration_details (`main_guid`, `person_guid`, `relation`, `educational_attainment`, `occupational_skills`, `remarks`, `occupation_income`)
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
