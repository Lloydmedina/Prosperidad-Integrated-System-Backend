using eegs_back_end.Admin.IndigentIntakeSetup.Model;
using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.IndigentIntakeSetup.Repository
{
    public interface IIndigentIntakeRepository :IGlobalInterface
    {
        bool Insert(IndigentIntakeModel model);
        List<object> GetListOfDisability();
        List<object> GetList();
        List<object> GetDetails(string GUID);
        object GetPersonAdd(string GUID);
        List<object> GetHistoryLogs(string GUID);
        bool Edit(string GUID, IndigentIntakeModel model);
        bool Delete(string GUID);
        object GetIndigentIntake(string IndigentIntake_guid);
    }
    public class IndigentIntakeRepository : FormNumberGenerator, IIndigentIntakeRepository
    {
        public bool Edit(string GUID, IndigentIntakeModel model)
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
                    model.indigent_intake_guid = GUID;
                    model.status = "Active";

                    var sql = "UPDATE mswd.Indigent_intake SET status='Trash' " +
                        "WHERE mswd.Indigent_intake.Indigent_intake_guid = '" + GUID + "'   ";
                    int res = (int)QueryModule.DataObject<int>(sql, model);

                    sql = "insert into mswd.Indigent_intake (`Indigent_intake_guid`, `person_guid`, `owner`, `renter`, `estimated_damaged`, `if_distressed`, `physical_disability`, `type_of_disability_id`, " +
                        "`sources_of_income`, `total_family_income`, `no_of_hectares`, `crops_planted`, `area_of_location`, `other_sources_of_income`, `status`, `form_trans_no`, `member_count`) " +
                        "VALUES(@Indigent_intake_guid, @person_guid, @owner, @renter, @estimated_damaged, @if_distressed, @physical_disability, @type_of_disability_id, @sources_of_income, " +
                        "@total_family_income, @no_of_hectares, @crops_planted, @area_of_location, @other_sources_of_income, @status, @form_trans_no, @member_count)";
                    QueryModule.Execute<int>(sql, model);

                    sql = "DELETE FROM mswd.Indigent_intake_details WHERE mswd.Indigent_intake_details.main_guid = '" + GUID + "' ";
                    QueryModule.Execute<int>(sql, model);

                    sql = "DELETE FROM mswd.Indigent_intake_logs WHERE mswd.Indigent_intake_logs.main_guid = '" + GUID + "' ";
                    QueryModule.Execute<int>(sql, model);

                    sql = "INSERT INTO mswd.Indigent_intake_logs (`main_guid`, `user_id`) VALUES('" + model.indigent_intake_guid + "', '" + GlobalObject.user_id + "')";
                    QueryModule.Execute<int>(sql, model);

                    if (model.details.Count > 0)
                    {
                        foreach (var dt in model.details)
                        {
                            dt.main_guid = model.indigent_intake_guid;
                            sql = "insert into mswd.Indigent_intake_details (`main_guid`, `person_guid`, `relation`, `educational_attainment`, `occupation_income`, `occupation`) " +
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
            string sql = "UPDATE mswd.Indigent_intake SET status = 'Deleted' where mswd.Indigent_intake.Indigent_intake_guid = @Indigent_intake_guid";
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

        public List<object> GetList()
        {
            string sql = @"SELECT mswd.indigent_intake.indigent_intake_guid, mswd.indigent_intake.status, mswd.indigent_intake.person_guid, mswd.indigent_intake.form_trans_no, mswd.indigent_intake.application_date, general.person.first_name,  
                            general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix, general.person.gender_id, general.gender.gender_name, general.person.birth_date, general.person.place_of_birth,  
                            general.person.province_id, general.person.age, general_address.lgu_province_setup_temp.province_name,  general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, 
                            general_address.lgu_brgy_setup_temp.brgy_name, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.religion, general.person.profession
                            FROM mswd.indigent_intake  
                            INNER JOIN general.person ON general.person.person_guid = mswd.indigent_intake.person_guid  
                            INNER JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id  
                            INNER JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id  
                            INNER JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id  
                            INNER JOIN general.gender ON general.gender.gender_id = general.person.gender_id  
                            INNER JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id  
                            INNER JOIN mswd.indigent ON mswd.indigent.person_guid = mswd.indigent_intake.person_guid  
                            WHERE mswd.indigent_intake.status = 'Active' AND mswd.indigent.status = 'Active' ORDER BY mswd.indigent_intake.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("indigent-intake");

            List<object> list = new List<object>();
            list.Add(obj);
            list.Add(form);


            return list;
        }

        public List<object> GetDetails(string GUID)
        {
            string sql = @"SELECT mswd.Indigent_intake_details.person_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix,  
                            general.person.gender_id, general.gender.gender_name, general.person.birth_date, general.person.place_of_birth,  
                            general.person.province_id, general_address.lgu_province_setup_temp.province_name,  
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.civil_status_id, general.civil_status.civil_status_name,  
                            general.person.religion, general.person.profession, general.person.age,  
                            mswd.Indigent_intake_details.main_guid, mswd.Indigent_intake_details.relation, mswd.Indigent_intake_details.educational_attainment, mswd.Indigent_intake_details.occupation_income, mswd.Indigent_intake_details.occupation
                            FROM mswd.Indigent_intake_details
                            INNER JOIN general.person ON general.person.person_guid = mswd.Indigent_intake_details.person_guid
                            INNER JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            INNER JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            INNER JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            INNER JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            INNER JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            WHERE mswd.Indigent_intake_details.main_guid = '" + GUID + "' ";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public object GetIndigentIntake(string Indigent_intake_guid)
        {
            string sql = @"SELECT mswd.Indigent_intake.Indigent_intake_guid, mswd.Indigent_intake.person_guid, mswd.Indigent_intake.owner, mswd.Indigent_intake.renter,
                            mswd.Indigent_intake.estimated_damaged, mswd.Indigent_intake.if_distressed, mswd.Indigent_intake.physical_disability, 
                            mswd.Indigent_intake.type_of_disability_id, mswd.general_intake_disability.disability_name, mswd.Indigent_intake.sources_of_income,
                            mswd.Indigent_intake.total_family_income, mswd.Indigent_intake.no_of_hectares, mswd.Indigent_intake.crops_planted, mswd.Indigent_intake.area_of_location,
                            mswd.Indigent_intake.other_sources_of_income, mswd.Indigent_intake.form_trans_no, mswd.Indigent.fourps_beneficiary, mswd.Indigent.ips,
                            mswd.Indigent.monthly_income, mswd.Indigent.house_ownership, mswd.Indigent.educational_attainment,
                            mswd.Indigent.occupation, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix,
                            general.person.gender_id, general.gender.gender_name, general.person.civil_status_id, general.civil_status.civil_status_name,
                            general.person.birth_date, general.person.place_of_birth, general.person.province_id, general_address.lgu_province_setup_temp.province_name,
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name,
                            general.person.religion, general.person.age, general.person.street, mswd.Indigent_intake.application_date
                            FROM mswd.Indigent_intake
                            INNER JOIN mswd.general_intake_disability ON mswd.general_intake_disability.id = mswd.Indigent_intake.type_of_disability_id
                            INNER JOIN general.person ON general.person.person_guid = mswd.Indigent_intake.person_guid
                            INNER JOIN mswd.Indigent ON mswd.Indigent.person_guid = mswd.Indigent_intake.person_guid
                            INNER JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            INNER JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            INNER JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            INNER JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            INNER JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            WHERE mswd.Indigent.status = 'Active' AND mswd.Indigent_intake.status = 'Active' AND mswd.Indigent_intake.Indigent_intake_guid = @Indigent_intake_guid ";
            var obj = (IndigentIntakeModel)QueryModule.DataObject<IndigentIntakeModel>(sql, new { Indigent_intake_guid = Indigent_intake_guid });

            sql = @"SELECT mswd.Indigent_intake_details.main_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix,  
                    general.person.age, general.person.civil_status_id, general.civil_status.civil_status_name, mswd.Indigent_intake_details.person_guid, mswd.Indigent_intake_details.educational_attainment,  
                    mswd.Indigent_intake_details.relation, mswd.Indigent_intake_details.occupation, mswd.Indigent_intake_details.occupation_income  
                    FROM mswd.Indigent_intake_details  
                    INNER JOIN general.person ON general.person.person_guid = mswd.Indigent_intake_details.person_guid  
                    INNER JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id  
                    WHERE mswd.Indigent_intake_details.main_guid = '" + obj.indigent_intake_guid + "' ";
            obj.details = (List<IndigentIntakeDetails>)QueryModule.DataSource<IndigentIntakeDetails>(sql);

            return obj;
        }

        public object GetPersonAdd(string GUID)
        {
            string sql = @"SELECT mswd.Indigent.person_guid, mswd.Indigent.Indigent_guid, mswd.Indigent.annual_income, mswd.Indigent.fourps_beneficiary,
                            mswd.Indigent.ips, mswd.Indigent.house_ownership, mswd.Indigent.educational_attainment, mswd.Indigent.monthly_income,
                            mswd.Indigent.occupation, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix,
                            general.person.gender_id, general.gender.gender_name, general.person.civil_status_id, general.civil_status.civil_status_name,
                            general.person.birth_date, general.person.place_of_birth, general.person.province_id, general_address.lgu_province_setup_temp.province_name,
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name,
                            general.person.religion, general.person.age, general.person.person_image, general.person.street
                            FROM mswd.Indigent 
                            INNER JOIN general.person ON general.person.person_guid = mswd.Indigent.person_guid
                            INNER JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            INNER JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            INNER JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            INNER JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            INNER JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            WHERE mswd.Indigent.status = 'Active' AND mswd.Indigent.person_guid = '" + GUID + "' ";
            var obj = (IndigentIntakeModel)QueryModule.DataObject<IndigentIntakeModel>(sql);

            if (obj == null)
                return null;

            sql = @"SELECT mswd.Indigent_details.main_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix,  
                    general.person.age, general.person.civil_status_id, general.civil_status.civil_status_name, mswd.Indigent_details.person_guid, mswd.Indigent_details.educational_attainment,  
                    mswd.Indigent_details.relation, mswd.Indigent_details.occupational_skills, mswd.Indigent_details.remarks, mswd.Indigent_details.occupation_income
                    from mswd.Indigent_details
                    INNER JOIN general.person ON general.person.person_guid = mswd.Indigent_details.person_guid
                    INNER JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                    WHERE mswd.Indigent_details.main_guid = '" + obj.indigent_guid + "' ";
            obj.details = (List<IndigentIntakeDetails>)QueryModule.DataSource<IndigentIntakeDetails>(sql);

            return obj;
        }

        public List<object> GetHistoryLogs(string GUID)
        {
            string sql = @"SELECT mswd.Indigent_intake_logs.main_guid, mswd.Indigent_intake.application_date, mswd.Indigent_intake_logs.user_id, mswd.Indigent_intake.form_trans_no, mswd.Indigent_intake.status,
                            mswd.Indigent_intake.owner, mswd.Indigent_intake.renter, mswd.Indigent_intake.estimated_damaged, mswd.Indigent_intake.if_distressed, mswd.Indigent_intake.physical_disability,
                            mswd.Indigent_intake.type_of_disability_id, mswd.general_intake_disability.disability_name, mswd.Indigent_intake.sources_of_income, mswd.Indigent_intake.total_family_income,
                            mswd.Indigent_intake.no_of_hectares, mswd.Indigent_intake.crops_planted, mswd.Indigent_intake.area_of_location, mswd.Indigent_intake.other_sources_of_income
                            FROM mswd.Indigent_intake_logs  
                            INNER JOIN mswd.Indigent_intake ON mswd.Indigent_intake.Indigent_intake_guid = mswd.Indigent_intake_logs.main_guid  
                            INNER JOIN mswd.general_intake_disability ON mswd.general_intake_disability.id = mswd.Indigent_intake.type_of_disability_id
                            WHERE mswd.Indigent_intake_logs.main_guid = '"+ GUID +"' ORDER BY mswd.Indigent_intake.id DESC";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public bool Insert(IndigentIntakeModel model)
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
                    model.form_trans_no = generateFormNumber("Indigent-intake");
                    model.indigent_intake_guid = Guid.NewGuid().ToString();
                    model.status = "Active";

                    string sql = "insert into mswd.Indigent_intake (`Indigent_intake_guid`, `person_guid`, `owner`, `renter`, `estimated_damaged`, `if_distressed`, `physical_disability`, `type_of_disability_id`, " +
                        "`sources_of_income`, `total_family_income`, `no_of_hectares`, `crops_planted`, `area_of_location`, `other_sources_of_income`, `status`, `form_trans_no`, `member_count`) " +
                        "VALUES(@Indigent_intake_guid, @person_guid, @owner, @renter, @estimated_damaged, @if_distressed, @physical_disability, @type_of_disability_id, @sources_of_income, " +
                        "@total_family_income, @no_of_hectares, @crops_planted, @area_of_location, @other_sources_of_income, @status, @form_trans_no, @member_count)";
                    int res = (int)QueryModule.DataObject<int>(sql, model);

                    if (model.details.Count > 0)
                    {
                        foreach (var dt in model.details)
                        {
                            dt.main_guid = model.indigent_intake_guid;
                            sql = "insert into mswd.Indigent_intake_details (`main_guid`, `person_guid`, `relation`, `educational_attainment`, `occupation_income`, `occupation`) " +
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
