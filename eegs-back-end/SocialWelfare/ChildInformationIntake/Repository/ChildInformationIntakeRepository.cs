using eegs_back_end.Admin.ChildInformationIntakeSetup.Model;
using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.ChildInformationIntakeSetup.Repository
{
    public interface IChildInformationIntakeRepository :IGlobalInterface
    {
        bool Insert(ChildInformationIntakeModel model);
        List<object> GetListOfDisability();
        List<object> GetList();
        List<object> GetEducationalType();
        List<object> GetDetails(string GUID);
        object GetPersonAdd(string GUID);
        List<object> GetHistoryLogs(string GUID);
        bool Edit(string GUID, ChildInformationIntakeModel model);
        bool Delete(string GUID);
        object GetChildInformationIntake(string ChildInformationIntake_guid);
    }
    public class ChildInformationIntakeRepository : FormNumberGenerator, IChildInformationIntakeRepository
    {
        public bool Edit(string GUID, ChildInformationIntakeModel model)
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
                    model.child_intake_guid = GUID;
                    model.status = "Active";

                    var sql = "UPDATE mswd.child_info_intake SET status='Trash' " +
                        "WHERE mswd.child_info_intake.child_intake_guid = '" + GUID + "'   ";
                    int res = (int)QueryModule.DataObject<int>(sql, model);

                    sql = "insert into mswd.child_info_intake (`child_intake_guid`, `person_guid`, `owner`, `renter`, `estimated_damaged`, `if_distressed`, `physical_disability`, `type_of_disability_id`, " +
                        "`sources_of_income`, `total_family_income`, `no_of_hectares`, `crops_planted`, `area_of_location`, `other_sources_of_income`, `status`, `form_trans_no`, `member_count`) " +
                        "VALUES(@child_intake_guid, @person_guid, @owner, @renter, @estimated_damaged, @if_distressed, @physical_disability, @type_of_disability_id, @sources_of_income, " +
                        "@total_family_income, @no_of_hectares, @crops_planted, @area_of_location, @other_sources_of_income, @status, @form_trans_no, @member_count)";
                    QueryModule.Execute<int>(sql, model);

                    sql = "DELETE FROM mswd.child_info_intake_details WHERE mswd.child_info_intake_details.main_guid = '" + GUID + "' ";
                    QueryModule.Execute<int>(sql, model);

                    sql = "DELETE FROM mswd.child_info_intake_logs WHERE mswd.child_info_intake_logs.main_guid = '" + GUID + "' ";
                    QueryModule.Execute<int>(sql, model);

                    sql = "INSERT INTO mswd.child_info_intake_logs (`main_guid`, `user_id`) VALUES('" + model.child_intake_guid + "', '" + GlobalObject.user_id + "')";
                    QueryModule.Execute<int>(sql, model);

                    if (model.details.Count > 0)
                    {
                        foreach (var dt in model.details)
                        {
                            dt.main_guid = model.child_intake_guid;
                            sql = "insert into mswd.child_info_intake_details (`main_guid`, `person_guid`, `relation`, `educational_attainment`, `occupation_income`, `occupation`) " +
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
            string sql = "UPDATE mswd.child_info_intake SET status = 'Deleted' where mswd.child_info_intake.child_intake_guid = @child_intake_guid";
            if (InsertUpdate.ExecuteSql(sql, new { child_intake_guid = GUID }))
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
            string sql = @"SELECT mswd.child_info_intake.child_intake_guid, mswd.child_info_intake.status, mswd.child_info_intake.person_guid, mswd.child_info_intake.form_trans_no, mswd.child_info_intake.member_count, mswd.child_info_intake.application_date, general.person.first_name,  
                            general.person.middle_name, general.person.last_name, general.person.prefix, general.person.suffix, general.person.gender_id, general.gender.gender_name, general.person.birth_date, general.person.place_of_birth,  
                            general.person.province_id, general.person.age, general_address.lgu_province_setup_temp.province_name,  
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.civil_status_id, general.civil_status.civil_status_name, general.person.religion, general.person.profession 
                            FROM mswd.child_info_intake  
                            INNER JOIN general.person ON general.person.person_guid = mswd.child_info_intake.person_guid  
                            INNER JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id  
                            INNER JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id  
                            INNER JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id  
                            INNER JOIN general.gender ON general.gender.gender_id = general.person.gender_id  
                            INNER JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id  
                            INNER JOIN mswd.child_info ON mswd.child_info.person_guid = mswd.child_info_intake.person_guid  
                            WHERE mswd.child_info_intake.status = 'Active' AND mswd.child_info.status = 'Active' ORDER BY mswd.child_info_intake.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("child-info-intake");

            List<object> list = new List<object>();
            list.Add(obj);
            list.Add(form);


            return list;
        }

        public List<object> GetDetails(string GUID)
        {
            string sql = @"SELECT mswd.child_info_intake_details.person_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix,  
                            general.person.gender_id, general.gender.gender_name, general.person.birth_date, general.person.place_of_birth,  
                            general.person.province_id, general_address.lgu_province_setup_temp.province_name,  
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.civil_status_id, general.civil_status.civil_status_name,  
                            general.person.religion, general.person.profession, general.person.age,  
                            mswd.child_info_intake_details.main_guid, mswd.child_info_intake_details.relation, mswd.child_info_intake_details.educational_attainment, mswd.child_info_intake_details.occupation_income, mswd.child_info_intake_details.occupation
                            FROM mswd.child_info_intake_details
                            INNER JOIN general.person ON general.person.person_guid = mswd.child_info_intake_details.person_guid
                            INNER JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            INNER JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            INNER JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            INNER JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            INNER JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            WHERE mswd.child_info_intake_details.main_guid = '" + GUID + "' ";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public object GetChildInformationIntake(string child_intake_guid)
        {
            string sql = @"SELECT mswd.child_info_intake.child_intake_guid, mswd.child_info_intake.person_guid, mswd.child_info_intake.owner, mswd.child_info_intake.renter,
                            mswd.child_info_intake.estimated_damaged, mswd.child_info_intake.if_distressed, mswd.child_info_intake.physical_disability, general.person.educational_attainment,
                            mswd.child_info_intake.type_of_disability_id, mswd.general_intake_disability.disability_name, mswd.child_info_intake.sources_of_income,
                            mswd.child_info_intake.total_family_income, mswd.child_info_intake.no_of_hectares, mswd.child_info_intake.crops_planted, mswd.child_info_intake.area_of_location,
                            mswd.child_info_intake.other_sources_of_income, mswd.child_info_intake.form_trans_no, general.person.profession,
                            general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix,
                            general.person.gender_id, general.gender.gender_name, general.person.civil_status_id, general.civil_status.civil_status_name,
                            general.person.birth_date, general.person.place_of_birth, general.person.province_id, general_address.lgu_province_setup_temp.province_name,
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name,
                            general.person.religion, general.person.age, general.person.street, mswd.child_info_intake.application_date, general.educational_type.educational_name
                            FROM mswd.child_info_intake
                            LEFT JOIN mswd.general_intake_disability ON mswd.general_intake_disability.id = mswd.child_info_intake.type_of_disability_id
                            INNER JOIN general.person ON general.person.person_guid = mswd.child_info_intake.person_guid
                            INNER JOIN mswd.child_info ON mswd.child_info.person_guid = mswd.child_info_intake.person_guid
                            INNER JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            INNER JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            INNER JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            INNER JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            INNER JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            INNER JOIN general.educational_type ON general.educational_type.id = general.person.educational_attainment
                            WHERE mswd.child_info.status = 'Active' AND mswd.child_info_intake.status = 'Active' AND mswd.child_info_intake.child_intake_guid = @child_intake_guid ";
            var obj = (ChildInformationIntakeModel)QueryModule.DataObject<ChildInformationIntakeModel>(sql, new { child_intake_guid = child_intake_guid });

            sql = @"SELECT mswd.child_info_intake_details.main_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix,  
                    general.person.age, general.person.civil_status_id, general.civil_status.civil_status_name, mswd.child_info_intake_details.person_guid, mswd.child_info_intake_details.educational_attainment,  
                    mswd.child_info_intake_details.relation, mswd.child_info_intake_details.occupation, mswd.child_info_intake_details.occupation_income, general.educational_type.educational_name
                    FROM mswd.child_info_intake_details  
                    INNER JOIN general.person ON general.person.person_guid = mswd.child_info_intake_details.person_guid  
                    INNER JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id  
                    INNER JOIN general.educational_type ON general.educational_type.id = mswd.child_info_intake_details.educational_attainment
                    WHERE mswd.child_info_intake_details.main_guid = '" + obj.child_intake_guid + "' ";
            obj.details = (List<ChildInformationIntakeDetails>)QueryModule.DataSource<ChildInformationIntakeDetails>(sql);

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
                            INNER JOIN general.person ON general.person.person_guid = mswd.solo_parent.person_guid
                            INNER JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            INNER JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            INNER JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            INNER JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            INNER JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            WHERE mswd.solo_parent.status = 'Active' AND mswd.solo_parent.person_guid = '" + GUID + "' ";
            //string sql = @"SELECT mswd.ChildInformation.person_guid, mswd.ChildInformation.ChildInformation_guid, mswd.ChildInformation.annual_income, mswd.ChildInformation.fourps_beneficiary,
            //                mswd.ChildInformation.ips, mswd.ChildInformation.house_ownership, mswd.ChildInformation.educational_attainment, mswd.ChildInformation.monthly_income,
            //                mswd.ChildInformation.occupation, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix,
            //                general.person.gender_id, general.gender.gender_name, general.person.civil_status_id, general.civil_status.civil_status_name,
            //                general.person.birth_date, general.person.place_of_birth, general.person.province_id, general_address.lgu_province_setup_temp.province_name,
            //                general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name,
            //                general.person.religion, general.person.age, general.person.person_image, general.person.street
            //                FROM mswd.ChildInformation 
            //                INNER JOIN general.person ON general.person.person_guid = mswd.ChildInformation.person_guid
            //                INNER JOIN general.gender ON general.gender.gender_id = general.person.gender_id
            //                INNER JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
            //                INNER JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
            //                INNER JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
            //                INNER JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
            //                WHERE mswd.ChildInformation.status = 'Active' AND mswd.ChildInformation.person_guid = '" + GUID + "' ";
            var obj = (ChildInformationIntakeModel)QueryModule.DataObject<ChildInformationIntakeModel>(sql);

            if (obj == null)
                return null;

            sql = @"SELECT mswd.solo_parent_details.main_guid, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.prefix,  
                    general.person.age, general.person.civil_status_id, general.civil_status.civil_status_name, mswd.solo_parent_details.person_guid, mswd.solo_parent_details.educational_attainment,  
                    mswd.solo_parent_details.relation, mswd.solo_parent_details.occupational_skills, mswd.solo_parent_details.occupation_income
                    from mswd.solo_parent_details
                    INNER JOIN general.person ON general.person.person_guid = mswd.solo_parent_details.person_guid
                    INNER JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                    WHERE mswd.solo_parent_details.main_guid = '" + obj.solo_parent_guid + "' ";
            obj.details = (List<ChildInformationIntakeDetails>)QueryModule.DataSource<ChildInformationIntakeDetails>(sql);

            return obj;
        }

        public List<object> GetHistoryLogs(string GUID)
        {
            string sql = @"SELECT mswd.child_info_intake_logs.main_guid, mswd.child_info_intake.application_date, mswd.child_info_intake_logs.user_id, mswd.child_info_intake.form_trans_no, mswd.child_info_intake.status,
	                        mswd.child_info_intake.owner, mswd.child_info_intake.renter, mswd.child_info_intake.estimated_damaged, mswd.child_info_intake.if_distressed, mswd.child_info_intake.physical_disability,
	                        mswd.child_info_intake.type_of_disability_id, mswd.general_intake_disability.disability_name, mswd.child_info_intake.sources_of_income, mswd.child_info_intake.total_family_income,
	                        mswd.child_info_intake.no_of_hectares, mswd.child_info_intake.crops_planted, mswd.child_info_intake.area_of_location, mswd.child_info_intake.other_sources_of_income
	                        FROM mswd.child_info_intake_logs  
	                        INNER JOIN mswd.child_info_intake ON mswd.child_info_intake.child_intake_guid = mswd.child_info_intake_logs.main_guid  
	                        INNER JOIN mswd.general_intake_disability ON mswd.general_intake_disability.id = mswd.child_info_intake.type_of_disability_id
	                        WHERE mswd.child_info_intake_logs.main_guid = '" + GUID + "' ORDER BY mswd.child_info_intake.id DESC ";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public bool Insert(ChildInformationIntakeModel model)
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
                    model.form_trans_no = generateFormNumber("child-info-intake");
                    model.child_intake_guid = Guid.NewGuid().ToString();
                    model.status = "Active";

                    string sql = "insert into mswd.child_info_intake (`child_intake_guid`, `person_guid`, `owner`, `renter`, `estimated_damaged`, `if_distressed`, `physical_disability`, `type_of_disability_id`, " +
                        "`sources_of_income`, `total_family_income`, `no_of_hectares`, `crops_planted`, `area_of_location`, `other_sources_of_income`, `status`, `form_trans_no`, `member_count`) " +
                        "VALUES(@child_intake_guid, @person_guid, @owner, @renter, @estimated_damaged, @if_distressed, @physical_disability, @type_of_disability_id, @sources_of_income, " +
                        "@total_family_income, @no_of_hectares, @crops_planted, @area_of_location, @other_sources_of_income, @status, @form_trans_no, @member_count)";
                    int res = (int)QueryModule.DataObject<int>(sql, model);

                    if (model.details.Count > 0)
                    {
                        foreach (var dt in model.details)
                        {
                            dt.main_guid = model.child_intake_guid;
                            sql = "insert into mswd.child_info_intake_details (`main_guid`, `person_guid`, `relation`, `educational_attainment`, `occupation_income`, `occupation`) " +
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
