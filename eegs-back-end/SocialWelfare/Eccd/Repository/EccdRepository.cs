using eegs_back_end.Admin.EccdSetup.Model;
using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.EccdSetup.Repository
{
    public interface IEccdRepository :IGlobalInterface
    {
        bool Insert(EccdModel model);
        List<object> GetListOfDisability();
        List<object> GetPhilhealthMembership();
        List<object> GetList();
        List<object> GetAllList();
        List<object> GetEducationalType();
        List<object> GetOccupationList();
        List<object> GetCauseOfDisability();
        List<object> GetEmploymentStatus();
        List<object> GetEmploymentType();
        List<object> GetEmployerType();
        List<object> GetDetails(string GUID);
        object GetPersonAdd(string GUID);
        List<object> GetHistoryLogs(string GUID);
        bool Edit(string GUID, EccdModel model);
        bool Delete(string GUID);
        object GetEccd(string Eccd_guid);
    }
    public class EccdRepository : FormNumberGenerator, IEccdRepository
    {
        public bool Edit(string GUID, EccdModel model)
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
                    model.child_info_guid = GUID;
                    model.status = "Active";

                    var sql = "UPDATE mswd.solo_parent SET status='Trash' " +
                        "WHERE mswd.solo_parent.solo_parent_guid = '" + GUID + "'   ";
                    int res = (int)QueryModule.DataObject<int>(sql, model);

                    sql = @"INSERT INTO mswd.solo_parent (`solo_parent_guid`, `person_guid`, `form_trans_no`, `status`, `member_count`, `circumstances`, `need_or_problem`, `reason_for_applying`, `family_resources`, `total_family_income`) 
                        VALUES(@solo_parent_guid, @person_guid, @form_trans_no, @status, @member_count, @circumstances, @need_or_problem, @reason_for_applying, @family_resources, @total_family_income)";
                    QueryModule.Execute<int>(sql, model);

                    sql = "DELETE FROM mswd.solo_parent_details WHERE mswd.solo_parent_details.main_guid = '" + GUID + "' ";
                    QueryModule.Execute<int>(sql, model);

                    sql = "DELETE FROM mswd.solo_parent_logs WHERE mswd.solo_parent_logs.main_guid = '" + GUID + "' ";
                    QueryModule.Execute<int>(sql, model);

                    sql = "INSERT INTO mswd.solo_parent_logs (`main_guid`, `user_id`) VALUES('" + model.child_info_guid + "', '" + GlobalObject.user_id + "')";
                    QueryModule.Execute<int>(sql, model);

                    if (model.family_details.Count > 0)
                    {
                        foreach (var dt in model.family_details)
                        {
                            dt.main_guid = model.child_info_guid;
                            sql = @"INSERT INTO mswd.solo_parent_details (`main_guid`, `person_guid`, `relation`, `educational_attainment`, `occupational_skills`, `occupation_income`)
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
            string sql = "UPDATE mswd.Eccd SET status = 'Deleted' where mswd.Eccd.Eccd_guid = @Eccd_guid";
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
            string sql = @"SELECT mswd.eccd_facility.eccd_facility_guid, mswd.eccd_facility.form_trans_no, mswd.eccd_facility.application_date, mswd.eccd_facility.status
                            FROM mswd.eccd_facility
                            WHERE mswd.eccd_facility.`status` = 'Active' ORDER BY mswd.eccd_facility.id DESC;";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("eccd-facility");

            List<object> list = new List<object>();
            list.Add(obj);
            list.Add(form);


            return list;
        }

        public List<object> GetAllList()
        {
            string sql = @"SELECT mswd.solo_parent.solo_parent_guid, mswd.solo_parent.person_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.prefix, 
                            general.person.suffix, general_address.lgu_province_setup_temp.province_name, general_address.lgu_city_mun_setup_temp.city_mun_name, general_address.lgu_brgy_setup_temp.brgy_name,
                            mswd.solo_parent.form_trans_no, general.person.birth_date, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.gender_id, 
                            general.gender.gender_name, mswd.solo_parent.application_date, mswd.solo_parent.member_count, mswd.solo_parent.status
                            FROM mswd.solo_parent
                            INNER JOIN general.person ON general.person.person_guid = mswd.solo_parent.person_guid
                            INNER JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            INNER JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            INNER JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            INNER JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            INNER JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            WHERE mswd.solo_parent.status = 'Active' OR mswd.solo_parent.status = 'Deleted' ORDER BY mswd.solo_parent.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("solo-parent-registration");

            List<object> list = new List<object>();
            list.Add(obj);
            list.Add(form);


            return list;
        }

        public List<object> GetDetails(string GUID)
        {
            string sql = @"SELECT mswd.solo_parent_details.person_guid, mswd.solo_parent_details.relation, mswd.solo_parent_details.educational_attainment, mswd.solo_parent_details.occupational_skills,
                            general.person.prefix, general.person.suffix, general.person.first_name, general.person.middle_name, general.person.last_name,
                            general.person.gender_id, general.gender.gender_name, general.person.age
                            FROM mswd.solo_parent_details
                            INNER JOIN general.person ON general.person.person_guid = mswd.solo_parent_details.person_guid
                            INNER JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            WHERE mswd.solo_parent_details.main_guid = '" + GUID + "' ";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public object GetEccd(string child_info_guid)
        {
            string sql = @"SELECT mswd.solo_parent.solo_parent_guid, mswd.solo_parent.person_guid, general.person.prefix, general.person.suffix, general.person.first_name, general.person.middle_name, general.person.last_name,
                            mswd.solo_parent.form_trans_no, mswd.solo_parent.application_date, mswd.solo_parent.status, mswd.solo_parent.member_count, general.person.birth_date, general.person.place_of_birth, general.person.gender_id, 
                            general.gender.gender_name, general.person.age, general.person.province_id, general_address.lgu_province_setup_temp.province_name, general.person.citmun_id, 
                            general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.street, general.person.civil_status_id, 
                            general.civil_status.civil_status_name, general.person.educational_attainment, general.person.profession, mswd.solo_parent.total_family_income, mswd.solo_parent.circumstances,
                            mswd.solo_parent.need_or_problem, mswd.solo_parent.reason_for_applying, mswd.solo_parent.family_resources, general.educational_type.educational_name, general.person.profession
                            FROM mswd.solo_parent
                            INNER JOIN general.person ON general.person.person_guid = mswd.solo_parent.person_guid
                            INNER JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            INNER JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            INNER JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            INNER JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            INNER JOIN general_address.lgu_region_setup_temp ON general_address.lgu_region_setup_temp.SysPK_region = general.person.region_id
                            INNER JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            INNER JOIN general.educational_type ON general.educational_type.id = general.person.educational_attainment
                            WHERE mswd.solo_parent.status = 'Active' AND mswd.solo_parent.solo_parent_guid = @solo_parent_guid ";
            var obj = (EccdModel)QueryModule.DataObject<EccdModel>(sql, new { child_info_guid = child_info_guid });

            sql = @"SELECT mswd.solo_parent_details.main_guid, mswd.solo_parent_details.person_guid, mswd.solo_parent_details.relation, mswd.solo_parent_details.educational_attainment,
                    mswd.solo_parent_details.occupational_skills, general.person.prefix, general.person.suffix, general.person.first_name,
                    general.person.middle_name, general.person.last_name, general.person.age, general.person.gender_id, general.gender.gender_name, mswd.solo_parent_details.occupation_income,
                    general.person.civil_status_id, general.civil_status.civil_status_name, general.person.birth_date
                    FROM mswd.solo_parent_details
                    INNER JOIN general.person ON general.person.person_guid = mswd.solo_parent_details.person_guid
                    INNER JOIN general.gender ON general.gender.gender_id =  general.person.gender_id
                    INNER JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                    WHERE mswd.solo_parent_details.main_guid = '" + obj.child_info_guid + "' ";
            obj.family_details = (List<EccdDetails>)QueryModule.DataSource<EccdDetails>(sql);

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
            var obj = (EccdModel)QueryModule.DataObject<EccdModel>(sql);

            if (obj == null)
                return null;

            //sql = "select mswd.family_composition_details.main_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix, " +
            //    "general.person.age, general.person.civil_status_id, general.civil_status.civil_status_name, mswd.family_composition_details.person_guid, mswd.family_composition_details.status, mswd.family_composition_details.educational_attainment, " +
            //    "mswd.family_composition_details.relation, mswd.family_composition_details.occupation, mswd.family_composition_details.occupation_income " +
            //    "from mswd.family_composition_details " +
            //    "INNER JOIN general.person ON general.person.person_guid = mswd.family_composition_details.person_guid " +
            //    "INNER JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id " +
            //    "where mswd.family_composition_details.main_guid = '" + obj.family_composition_guid + "' ";
            //obj.details = (List<EccdDetails>)QueryModule.DataSource<EccdDetails>(sql);

            return obj;
        }

        public List<object> GetHistoryLogs(string GUID)
        {
            string sql = @"SELECT mswd.solo_parent_logs.main_guid, mswd.solo_parent.application_date, mswd.solo_parent_logs.user_id, mswd.solo_parent.form_trans_no,
                            mswd.solo_parent.status, mswd.solo_parent.person_guid, general.person.prefix, general.person.suffix, general.person.first_name,
                            general.person.middle_name, general.person.last_name, mswd.solo_parent.total_family_income
                            FROM mswd.solo_parent_logs  
                            INNER JOIN mswd.solo_parent ON mswd.solo_parent.solo_parent_guid = mswd.solo_parent_logs.main_guid  
                            INNER JOIN general.person ON general.person.person_guid = mswd.solo_parent.person_guid
                            WHERE mswd.solo_parent_logs.main_guid = '" + GUID + "' ORDER BY mswd.solo_parent.id DESC";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public bool Insert(EccdModel model)
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
                    model.form_trans_no = generateFormNumber("solo-parent-registration");
                    model.child_info_guid = Guid.NewGuid().ToString();
                    model.status = "Active";

                    string sql = @"INSERT INTO mswd.solo_parent (`solo_parent_guid`, `person_guid`, `form_trans_no`, `status`, `member_count`, `circumstances`, `need_or_problem`, `reason_for_applying`, `family_resources`, `total_family_income`) 
                        VALUES(@solo_parent_guid, @person_guid, @form_trans_no, @status, @member_count, @circumstances, @need_or_problem, @reason_for_applying, @family_resources, @total_family_income)";
                    int res = (int)QueryModule.DataObject<int>(sql, model);

                    if (model.family_details.Count > 0)
                    {
                        foreach (var dt in model.family_details)
                        {
                            dt.main_guid = model.child_info_guid;
                            sql = @"INSERT INTO mswd.solo_parent_details (`main_guid`, `person_guid`, `relation`, `educational_attainment`, `occupational_skills`, `occupation_income`)
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
