using eegs_back_end.Admin.PersonSetup.Model;
using eegs_back_end.DbModule;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using eegs_back_end.Shell.Form.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Admin.PersonSetup.Repository
{
    public interface IPersonRepository :IGlobalInterface
    {
        bool Insert(PersonModel model);
        List<object> GetList();
        List<object> GetListActive();
        List<object> GetListAboveSixty();
        List<object> GetListDeleted();
        List<object> GetListGenerated(int? filter_type_status_id = 0, int? status_id = 0, int? status_deleted_id = 0, string? this_month = "", string? this_year = "", string? monthly = "", string? monthlyYear = "", string? year_quarterly = "", int? quarter = 0, string? yearly = "", string? from = "", string? to = "");
        bool Edit(string GUID, PersonModel model);
        bool Delete(string GUID);
        bool Activate(string GUID, PersonModel model);
        public List<object> GetBloodType();
        public List<object> GetRegion();
        List<object> GetEducationalType();
        List<object> GetDropDown();
        List<object> GetProvince();
        List<object> GetPrefix();
        List<object> GetCityMun(int province_id);
        List<object> GetBarangay(int? city_mun_id = 0);
        List<object> GetPerson(string person_guid);
    }
    public class PersonRepository : FormNumberGenerator, IPersonRepository
    {
        public bool Edit(string GUID, PersonModel model)
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
                    model.person_guid = GUID;
                    model.status = "Active";

                    if (model.suffix == null)
                    {
                        model.suffix = "";
                    }


                    var sql = "UPDATE general.person SET " + ObjectSqlMapping.MapUpdate<PersonModel>() + " WHERE general.person.person_guid = '" + GUID + "'   ";
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
            string sql = "UPDATE general.person SET status = 'Deleted' where general.person.person_guid = @person_guid";
            if (InsertUpdate.ExecuteSql(sql, new { person_guid = GUID }))
                return true;
            return false;
        }

        public bool Activate(string GUID, PersonModel model)
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
                    var sql = "UPDATE general.person SET status = 'Active' where general.person.person_guid = @person_guid";
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

        public List<object> GetDropDown()
        {
            string sql = "select * from general.gender";
            List<object> gender = (List<object>)QueryModule.DataSource<object>(sql);

            sql = "select * from general.civil_status";
            List<object> civil_status = (List<object>)QueryModule.DataSource<object>(sql);

            sql = "select * from general.blood_type";
            List<object> blood_type = (List<object>)QueryModule.DataSource<object>(sql);

            List<object> common_object = new List<object>();

            common_object.Add(new { gender = gender });
            common_object.Add(new { civil_status = civil_status });
            common_object.Add(new { blood_type = blood_type });

            return common_object;
        }

        public List<object> GetEducationalType()
        {
            string sql = "select * from general.educational_type";
            List<object> province = (List<object>)QueryModule.DataSource<object>(sql);

            return province;
        }

        public List<object> GetProvince()
        {
            string sql = "select * from general_address.lgu_province_setup_temp";
            List<object> province = (List<object>)QueryModule.DataSource<object>(sql);

            return province;
        }

        public List<object> GetCityMun(int province_id)
        {
            string sql = "select * from general_address.lgu_city_mun_setup_temp WHERE general_address.lgu_city_mun_setup_temp.province_id = '" + province_id + "'";
            List<object> cityMun = (List<object>)QueryModule.DataSource<object>(sql);

            return cityMun;
        }

        public List<object> GetBarangay(int? city_mun_id)
        {
            string sql = "SELECT * FROM general_address.lgu_brgy_setup_temp";
            List<object> barangay = (List<object>)QueryModule.DataSource<object>(sql);
            return barangay;
        }

        public List<object> GetBloodType()
        {
            string sql = "SELECT * FROM general.blood_type";
            List<object> blood = (List<object>)QueryModule.DataSource<object>(sql);
            return blood;
        }

        public List<object> GetRegion()
        {
            string sql = "SELECT * FROM general_address.lgu_region_setup_temp";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }

        public List<object> GetPrefix()
        {
            string sql = "SELECT * FROM general.prefix";
            var obj = (List<object>)QueryModule.DataSource<object>(sql);

            if (obj == null)
                return null;

            return obj;
        }


        public List<object> GetList()
        {

            string sql = "SELECT general.person.id, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.gender_id, general.gender.gender_name," +
                "general.person.civil_status_id, general.civil_status.civil_status_name, general.person.citizenship, general.person.blood_type_id, general.blood_type.blood_type_name," +
                "general.person.birth_date, general.person.place_of_birth, general.person.region_id, general.person.province_id, general_address.lgu_province_setup_temp.province_name," +
                "general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.zipcode_id," +
                "general_address.lgu_zipcode_setup_temp.zipcode_name, general.person.person_guid, general.person.application_date, general.person.status, general.person.age, general.person.default_checked, " +
                "general.person.prefix, general.person.tin, general.person.street, general.person.height, general.person.weight, general.person.profession, general.person.religion, general.person.person_image, " +
                "general.person.educational_attainment, general_address.lgu_region_setup_temp.reg_name, general_address.lgu_region_setup_temp.reg_code, general.person.phone_no, general.person.telephone_no, general.person.full_name, (SELECT COUNT(*) FROM general.person) as 'count' " +
                "FROM general.person " +
                "LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id " +
                "LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id " +
                "LEFT JOIN general.blood_type ON general.blood_type.blood_type_id = general.person.blood_type_id " +
                "LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id " +
                "LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id " +
                "LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id " +
                "LEFT JOIN general_address.lgu_zipcode_setup_temp ON general_address.lgu_zipcode_setup_temp.zipcode_id = general.person.zipcode_id " +
                "LEFT JOIN general_address.lgu_region_setup_temp ON general_address.lgu_region_setup_temp.SysPK_region = general.person.region_id " +
                "ORDER BY general.person.id DESC";
            
            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("person");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT per.full_name as q, 'fn' as tag
                    FROM general.`person` per

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM general.`person` per
                    LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = per.barangay_id

                    ) z";
            List<object> searches = (List<object>)QueryModule.DataSource<object>(sql);

            List<object> list = new List<object>();
            list.Add(obj);
            list.Add(form);
            list.Add(searches);


            return list;
        }

        public List<object> GetListActive()
        {
            string sql = @"SELECT general.person.id, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.gender_id, general.gender.gender_name, 
                            general.person.civil_status_id, general.civil_status.civil_status_name, general.person.citizenship, general.person.blood_type_id, general.blood_type.blood_type_name, 
                            general.person.birth_date, general.person.place_of_birth, general.person.region_id, general.person.province_id, general_address.lgu_province_setup_temp.province_name, 
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.zipcode_id, 
                            general_address.lgu_zipcode_setup_temp.zipcode_name, general.person.person_guid, general.person.application_date, general.person.status, general.person.age, general.person.default_checked,  
                            general.person.prefix, general.person.tin, general.person.street, general.person.height, general.person.weight, general.person.profession, general.person.religion, general.person.person_image,  
                            general.person.educational_attainment,  general_address.lgu_region_setup_temp.reg_name, general_address.lgu_region_setup_temp.reg_code, general.person.phone_no, general.person.telephone_no, general.person.full_name, (SELECT COUNT(*) FROM general.person WHERE general.person.status = 'Active') as 'count'
                            FROM general.person  
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id  
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id  
                            LEFT JOIN general.blood_type ON general.blood_type.blood_type_id = general.person.blood_type_id  
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id  
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id  
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id  
                            LEFT JOIN general_address.lgu_zipcode_setup_temp ON general_address.lgu_zipcode_setup_temp.zipcode_id = general.person.zipcode_id  
                            LEFT JOIN general_address.lgu_region_setup_temp ON general_address.lgu_region_setup_temp.SysPK_region = general.person.region_id  
                            WHERE general.person.status = 'Active' ORDER BY general.person.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);



            ExpandoObject form = Forms.getForm("person");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT per.full_name as q, 'fn' as tag
                    FROM general.`person` per

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM general.`person` per
                    LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = per.barangay_id

                    ) z";
            List<object> searches = (List<object>)QueryModule.DataSource<object>(sql);

            List<object> list = new List<object>();

            list.Add(obj);
            list.Add(form);
            list.Add(searches);


            return list;
        }

        public List<object> GetListAboveSixty()
        {
            string sql = @"SELECT general.person.id, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.gender_id, general.gender.gender_name, 
                            general.person.civil_status_id, general.civil_status.civil_status_name, general.person.citizenship, general.person.blood_type_id, general.blood_type.blood_type_name, 
                            general.person.birth_date, general.person.place_of_birth, general.person.region_id, general.person.province_id, general_address.lgu_province_setup_temp.province_name, 
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.zipcode_id, 
                            general_address.lgu_zipcode_setup_temp.zipcode_name, general.person.person_guid, general.person.application_date, general.person.status, general.person.age, general.person.default_checked,  
                            general.person.prefix, general.person.tin, general.person.street, general.person.height, general.person.weight, general.person.profession, general.person.religion, general.person.person_image,  
                            general.person.educational_attainment,  general_address.lgu_region_setup_temp.reg_name, general_address.lgu_region_setup_temp.reg_code, general.person.phone_no, general.person.telephone_no, general.person.full_name,
                            FLOOR((DATEDIFF(CURRENT_DATE(),general.person.birth_date)/365)) as 'age_count', (SELECT COUNT(*) FROM general.person WHERE general.person.status = 'Active' AND (DATEDIFF(CURRENT_DATE(),general.person.birth_date)/365)>= 60) as 'count'
                            FROM general.person  
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id  
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id  
                            LEFT JOIN general.blood_type ON general.blood_type.blood_type_id = general.person.blood_type_id  
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id  
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id  
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id  
                            LEFT JOIN general_address.lgu_zipcode_setup_temp ON general_address.lgu_zipcode_setup_temp.zipcode_id = general.person.zipcode_id  
                            LEFT JOIN general_address.lgu_region_setup_temp ON general_address.lgu_region_setup_temp.SysPK_region = general.person.region_id  
                            WHERE general.person.status = 'Active' AND (DATEDIFF(CURRENT_DATE(),general.person.birth_date)/365)>= 60 ORDER BY general.person.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);



            ExpandoObject form = Forms.getForm("person");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT per.full_name as q, 'fn' as tag
                    FROM general.`person` per

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM general.`person` per
                    LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = per.barangay_id

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

            string sql = "SELECT general.person.id, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.gender_id, general.gender.gender_name," +
                "general.person.civil_status_id, general.civil_status.civil_status_name, general.person.citizenship, general.person.blood_type_id, general.blood_type.blood_type_name," +
                "general.person.birth_date, general.person.place_of_birth, general.person.region_id, general.person.province_id, general_address.lgu_province_setup_temp.province_name," +
                "general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.zipcode_id," +
                "general_address.lgu_zipcode_setup_temp.zipcode_name, general.person.person_guid, general.person.application_date, general.person.status, general.person.age, general.person.default_checked, " +
                "general.person.prefix, general.person.tin, general.person.street, general.person.height, general.person.weight, general.person.profession, general.person.religion, general.person.person_image, " +
                "general.person.educational_attainment, general_address.lgu_region_setup_temp.reg_name, general_address.lgu_region_setup_temp.reg_code, general.person.phone_no, general.person.telephone_no, general.person.full_name, (SELECT COUNT(*) FROM general.person WHERE general.person.status = 'Deleted') as 'count' " +
                "FROM general.person " +
                "LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id " +
                "LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id " +
                "LEFT JOIN general.blood_type ON general.blood_type.blood_type_id = general.person.blood_type_id " +
                "LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id " +
                "LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id " +
                "LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id " +
                "LEFT JOIN general_address.lgu_zipcode_setup_temp ON general_address.lgu_zipcode_setup_temp.zipcode_id = general.person.zipcode_id " +
                "LEFT JOIN general_address.lgu_region_setup_temp ON general_address.lgu_region_setup_temp.SysPK_region = general.person.region_id " +
                "WHERE general.person.status = 'Deleted' ORDER BY general.person.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("person");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT per.full_name as q, 'fn' as tag
                    FROM general.`person` per

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM general.`person` per
                    LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = per.barangay_id

                    ) z";
            List<object> searches = (List<object>)QueryModule.DataSource<object>(sql);

            List<object> list = new List<object>();
            list.Add(obj);
            list.Add(form);
            list.Add(searches);


            return list;
        }

        public List<object> GetListGenerated(int? filter_type_status_id = 0, int? status_id = 0, int? status_deleted_id = 0, string? this_month = "", string? this_year = "", string? monthly= "", string? monthlyYear = "", string? year_quarterly = "", int? quarter = 0, string? yearly = "", string? from = "", string? to = "")
        {
            string filterStatus = "";
            string dateFilter = "";
            if (status_id != 0)
            {
                filterStatus = "general.person.status = 'Active'";
            } else if (status_deleted_id != 0)
            {
                filterStatus = "general.person.status = 'Deleted'";
            }

            if (filter_type_status_id == 1)
            {
                dateFilter = "AND MONTH(general.person.application_date) = '"+ this_month + "'";
            } else if (filter_type_status_id == 2)
            {
                dateFilter = "AND YEAR(general.person.application_date) = '"+ this_year + "'";
            } else if (filter_type_status_id == 3)
            {
                dateFilter = "AND MONTH(general.person.application_date) = '"+ monthly + "' AND YEAR(general.person.application_date) = '"+ monthlyYear + "'";
            } else if (filter_type_status_id == 4)
            {
                dateFilter = "AND YEAR(general.person.application_date) = '"+ year_quarterly +"' AND QUARTER(general.person.application_date) = '"+ quarter +"'";
            } else if (filter_type_status_id == 5)
            {
                dateFilter = "AND YEAR(general.person.application_date) = '"+ yearly + "'";
            } else if (filter_type_status_id == 6)
            {
                dateFilter = "AND general.person.application_date >= '"+ from +"' AND general.person.application_date <= '"+ to +"'";
            }

            string sql = @"SELECT general.person.id, general.person.first_name, general.person.middle_name, general.person.last_name, general.person.suffix, general.person.gender_id, general.gender.gender_name, 
                            general.person.civil_status_id, general.civil_status.civil_status_name, general.person.citizenship, general.person.blood_type_id, general.blood_type.blood_type_name, 
                            general.person.birth_date, general.person.place_of_birth, general.person.region_id, general.person.province_id, general_address.lgu_province_setup_temp.province_name, 
                            general.person.citmun_id, general_address.lgu_city_mun_setup_temp.city_mun_name, general.person.barangay_id, general_address.lgu_brgy_setup_temp.brgy_name, general.person.zipcode_id, 
                            general_address.lgu_zipcode_setup_temp.zipcode_name, general.person.person_guid, general.person.application_date, general.person.status, general.person.age, general.person.default_checked,  
                            general.person.prefix, general.person.tin, general.person.street, general.person.height, general.person.weight, general.person.profession, general.person.religion, general.person.person_image,  
                            general.person.educational_attainment, general_address.lgu_region_setup_temp.reg_name, general_address.lgu_region_setup_temp.reg_code, general.person.phone_no, general.person.telephone_no, general.person.full_name, (SELECT COUNT(*) FROM general.person WHERE " + filterStatus + @" " + dateFilter  + @") as 'count'
                            FROM general.person
                            LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id
                            LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id
                            LEFT JOIN general.blood_type ON general.blood_type.blood_type_id = general.person.blood_type_id
                            LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id
                            LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id
                            LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id
                            LEFT JOIN general_address.lgu_zipcode_setup_temp ON general_address.lgu_zipcode_setup_temp.zipcode_id = general.person.zipcode_id
                            LEFT JOIN general_address.lgu_region_setup_temp ON general_address.lgu_region_setup_temp.SysPK_region = general.person.region_id
                            WHERE " + filterStatus + @" " + dateFilter  + @" ORDER BY general.person.id DESC";

            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql);

            ExpandoObject form = Forms.getForm("person");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT per.full_name as q, 'fn' as tag
                    FROM general.`person` per

                    UNION

                    SELECT general_address.lgu_brgy_setup_temp.brgy_name as q, 'brgy' as tag
                    FROM general.`person` per
                    LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = per.barangay_id

                    ) z";
            List<object> searches = (List<object>)QueryModule.DataSource<object>(sql);

            List<object> list = new List<object>();
            list.Add(obj);
            list.Add(form);
            list.Add(searches);


            return list;
        }

        public List<object> GetPerson(string person_guid)
        {
            string sql = @"SELECT *
                        FROM general.person
                        LEFT JOIN general_address.lgu_province_setup_temp ON general_address.lgu_province_setup_temp.province_id = general.person.province_id  
                        LEFT JOIN general_address.lgu_city_mun_setup_temp ON general_address.lgu_city_mun_setup_temp.city_mun_id = general.person.citmun_id  
                        LEFT JOIN general_address.lgu_brgy_setup_temp ON general_address.lgu_brgy_setup_temp.brgy_id = general.person.barangay_id  
                        LEFT JOIN general.civil_status ON general.civil_status.civil_status_id = general.person.civil_status_id  
                        LEFT JOIN general.gender ON general.gender.gender_id = general.person.gender_id  
                        LEFT JOIN general_address.lgu_region_setup_temp ON general_address.lgu_region_setup_temp.SysPK_region = general.person.region_id  
                        LEFT JOIN general.educational_type ON general.educational_type.id = general.person.educational_attainment
                        where person_guid = @person_guid";
            List<object> obj = (List<object>)QueryModule.DataSource<object>(sql, new { person_guid = person_guid });

            return obj;
        }

        public bool Insert(PersonModel model)
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
                    model.person_guid = Guid.NewGuid().ToString();
                    model.status = "Active";

                    if (model.suffix == null)
                    {
                        model.suffix = "";
                    }

                    string sql = "insert into general.person (" + ObjectSqlMapping.MapInsert<PersonModel>() + ")";
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

    }
}
