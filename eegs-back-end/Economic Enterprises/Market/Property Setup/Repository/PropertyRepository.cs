using eegs_back_end.DbModule;
using eegs_back_end.Economic_Enterprises.Market.Property_Setup.Model;
using eegs_back_end.GlobalHandler.Error.ErrorException;
using eegs_back_end.Helpers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using static eegs_back_end.Economic_Enterprises.Market.Property_Setup.Repository.PropertyRepository;

namespace eegs_back_end.Economic_Enterprises.Market.Property_Setup.Repository
{
    public interface IPropertyRepository: IGlobalInterface
    {
        List<object> GetList(int? status_id = 0);
        PropertyModel GetByID(string ID);
        bool Insert(PropertyModel model);
        bool Edit(string ID, PropertyModel model);
        bool Delete(string ID, string remarks);
        List<object> GetDropDown();

    }
    public class PropertyRepository : IPropertyRepository
    {
        public bool Delete(string ID, string remarks)
        {
            string sql = "update market.property_status SET end_time = CURRENT_TIMESTAMP, remarks = '" + remarks + "' " +
                "       where property_id = '" + ID + "' and end_time is null";
            QueryModule.Execute<int>(sql, new { property_id = ID });

            //sql = "update market.rental_application SET status = 'Cancelled' where rental_application_id = '" + ID + "' ";
            //QueryModule.Execute<int>(sql, new { tenant_profile_id = ID });

            PropertyStatus status = new PropertyStatus();
            status.property_id = ID;
            status.status_id = 0;
            status.user_id = GlobalObject.user_id;

            sql = "insert into market.property_status (" + ObjectSqlMapping.MapInsert<PropertyStatus>() + ")";
            QueryModule.Execute<int>(sql, status);

            return true;
        }

        public bool Edit(string ID, PropertyModel model)
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

                    model.property_type_id = 4;
                    string sql = "update market.property SET " + ObjectSqlMapping.MapUpdate<PropertyModel>() + " where property_id = @property_id";
                    QueryModule.Execute<int>(sql, model);

                    sql = "update market.property_amount SET " + ObjectSqlMapping.MapUpdate<PropertyAmount>() + " where property_id = @property_id";
                    QueryModule.Execute<int>(sql, model.property_amount);

                    sql = "update market.property_status SET end_time = CURRENT_TIMESTAMP where property_id = @property_id and end_time is null";
                    QueryModule.Execute<int>(sql, new { property_id = ID });

                    model.property_status = new PropertyStatus();
                    model.property_status.property_id = ID;
                    model.property_status.status_id = 1;
                    model.property_status.user_id = GlobalObject.user_id;

                    sql = "insert into market.property_status (" + ObjectSqlMapping.MapInsert<PropertyStatus>() + ")";
                    QueryModule.Execute<int>(sql, model.property_status);


                    if (model.property_floor.Count > 0)
                    {
                        foreach(PropertyFloor floor in model.property_floor)
                        {

                            floor.property_type_id = 5;
                            sql = "update market.property SET " + ObjectSqlMapping.MapUpdate<PropertyFloor>() + " where property_id = @property_id";
                            QueryModule.Execute<int>(sql, floor);

                            sql = "update market.property_amount SET " + ObjectSqlMapping.MapUpdate<PropertyAmount>() + " where property_id = @property_id";
                            QueryModule.Execute<int>(sql, floor.property_amount);

                            sql = "delete from market.property_section where market.property_section.property_id = '" + floor.property_id + "'";
                            QueryModule.Execute<int>(sql);

                            if (floor.property_section.Count > 0)
                            {
                                foreach (PropertySection propSec in floor.property_section)
                                {
                                    propSec.property_id = floor.property_id;
                                    sql = "insert into market.property_section (" + ObjectSqlMapping.MapInsert<PropertySection>() + ")";
                                    QueryModule.Execute<int>(sql, propSec);
                                }

                            }
                            
                            if(floor.unit_stall.Count > 0)
                            {
                                foreach (UnitStall us in floor.unit_stall)
                                {
                                    us.property_type_id = 1;
                                    if (us.property_id == "")
                                    {
                                        us.property_id = Guid.NewGuid().ToString();
                                        us.property_bldg_id = model.property_id;
                                        us.parent_id = floor.property_id;
                                        sql = "insert into market.property (" + ObjectSqlMapping.MapInsert<UnitStall>() + ")";
                                        QueryModule.Execute<int>(sql, us);

                                        us.property_amount.property_id = us.property_id;
                                        sql = "insert into market.property_amount (" + ObjectSqlMapping.MapInsert<PropertyAmount>() + ")";
                                        QueryModule.Execute<int>(sql, us.property_amount);


                                        us.property_section.property_id = us.property_id;
                                        sql = "insert into market.property_section (" + ObjectSqlMapping.MapInsert<PropertySection>() + ")";
                                        QueryModule.Execute<int>(sql, us.property_section);

                                    }
                                    else
                                    {
                                        sql = "update market.property SET " + ObjectSqlMapping.MapUpdate<UnitStall>() + " where property_id = @property_id";
                                        QueryModule.Execute<int>(sql, us);

                                        sql = "update market.property_amount SET " + ObjectSqlMapping.MapUpdate<PropertyAmount>() + " where property_id = @property_id";
                                        QueryModule.Execute<int>(sql, us.property_amount);

                                        sql = "update market.property_section SET " + ObjectSqlMapping.MapUpdate<PropertySection>() + " where property_id = @property_id";
                                        QueryModule.Execute<int>(sql, us.property_section);

                                    }


                                }
                            }
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

        public PropertyModel GetByID(string ID)
        {
            string sql = "select * from market.property prop where prop.property_id = '" + ID + "'";
            PropertyModel property = (PropertyModel)QueryModule.DataObject<PropertyModel>(sql);

            sql = "select * from market.property_amount prop where prop.property_id = '" + ID + "'  ";
            property.property_amount = (PropertyAmount)QueryModule.DataObject<PropertyAmount>(sql);

            sql = "select * from market.property prop where prop.parent_id = '" + ID + "'  ";
            property.property_floor = (List<PropertyFloor>)QueryModule.DataSource<PropertyFloor>(sql);

            if (property.property_floor != null && property.property_floor.Count > 0)
            {
                foreach (PropertyFloor flr in property.property_floor)
                {

                    sql = "select * from market.property_amount prop where prop.property_id = '" + flr.property_id + "'  ";
                    flr.property_amount = (PropertyAmount)QueryModule.DataObject<PropertyAmount>(sql);

                    sql = "select * from market.property_section prop where prop.property_id = '" + flr.property_id + "'";
                    flr.property_section = (List<PropertySection>)QueryModule.DataSource<PropertySection>(sql);


                    sql = "select * from market.property prop where prop.parent_id = '" + flr.property_id + "'  ";
                    flr.unit_stall = (List<UnitStall>)QueryModule.DataSource<UnitStall>(sql);

                    if (flr.unit_stall != null && flr.unit_stall.Count > 0)
                    {
                        foreach (UnitStall us in flr.unit_stall)
                        {

                            sql = "select * from market.property_amount prop where prop.property_id = '" + us.property_id + "'  ";
                            us.property_amount = (PropertyAmount)QueryModule.DataObject<PropertyAmount>(sql);

                            sql = "select * from market.property_section prop where prop.property_id = '" + us.property_id + "'";
                            us.property_section = (PropertySection)QueryModule.DataObject<PropertySection>(sql);
                        }
                    }
                }
            }


            return property;
        }

        public List<object> GetDropDown()
        {
            string sql = "select * from general_address.lgu_brgy_setup_temp";
            List<object> brgy = (List<object>)QueryModule.DataSource<object>(sql);

            sql = "select * from market.section";
            List<object> section = (List<object>)QueryModule.DataSource<object>(sql);


            List<object> list = new List<object>();
            list.Add(new { brgy = brgy, section = section });

            return list;
        }
        public class ExpandableList { 
            public string key { get; set; }
            public string name { get; set; }
            public string area { get; set; }
            public string amt { get; set; }
            public string section { get; set; }
            public bool expand { get; set; }
            public string property_status { get; set; }
            public List<ExpandableList> children { get; set; }

        }


        public List<object> GetList(int? status_id = 0)
        {
            string typeFilter = "";
            if (status_id != 0)
            {
                //typeFilter = typeFilter + " AND stat.status_id = " + status_id;
                typeFilter = " AND market.property_status.status_id = " + status_id;
            }

            string sql = @"SELECT prop.property_id as `key`, prop.property_name as `name`, prop.property_area as `area`, true as `expand`, prop_amt.initial_amount as `amt`,
                            stat.`name` as `property_status`
                            FROM market.`property` prop 
                            INNER JOIN market.property_amount prop_amt on prop_amt.property_id = prop.property_id
                            INNER JOIN market.property_status ON market.property_status.property_id = prop.property_id
                            INNER JOIN market.status stat ON stat.status_id = market.property_status.status_id
                            WHERE prop.property_type_id = 4 
                            AND market.property_status.end_time IS NULL
                            " + typeFilter + @"
                            order by `name` asc";

            List<ExpandableList> list = (List<ExpandableList>)QueryModule.DataSource<ExpandableList>(sql);

            foreach(ExpandableList obj in list)
            {
                obj.children = new List<ExpandableList>();
                sql = @"SELECT
	                        prop.property_id AS `key`,
	                        prop.property_name AS `name`,
	                        prop.property_area AS `area`,
	                        TRUE AS `expand`,
	                        prop_amt.initial_amount AS `amt`,
                            (SELECT
	                                GROUP_CONCAT(sec.section_name)
                                FROM
	                                market.property_section propsec
                                INNER JOIN market.section sec ON sec.section_id = propsec.section_id
                                WHERE
	                                propsec.property_id = prop.property_id) as `section`
                        FROM
	                        market.`property` prop
                        INNER JOIN market.property_amount prop_amt ON prop_amt.property_id = prop.property_id
                        WHERE
	                        prop.parent_id = '" + obj.key +@"' 
                        order by `name` asc";
                obj.children = (List<ExpandableList>)QueryModule.DataSource<ExpandableList>(sql);


                if(obj.children != null && obj.children.Count > 0)
                {
                    foreach(ExpandableList floor in obj.children)
                    {
                        sql = @"SELECT
	                                prop.property_id AS `key`,
	                                prop.property_name AS `name`,
	                                prop.property_area AS `area`,
	                                TRUE AS `expand`,
	                                prop_amt.initial_amount AS `amt`,
                                    (SELECT
	                                    GROUP_CONCAT(sec.section_name)
                                    FROM
	                                    market.property_section propsec
                                    INNER JOIN market.section sec ON sec.section_id = propsec.section_id
                                    WHERE
	                                    propsec.property_id = prop.property_id) as `section`
                                FROM
	                                market.`property` prop
                                INNER JOIN market.property_amount prop_amt ON prop_amt.property_id = prop.property_id
                                WHERE
	                                prop.parent_id = '" + floor.key + @"'
                                order by `name` asc";
                        floor.children = (List<ExpandableList>)QueryModule.DataSource<ExpandableList>(sql);
                    }
                }
            }

            ExpandoObject form = Forms.getForm("property-setup");

            sql = @"SELECT z.q as val, z.tag  FROM (

                    SELECT market.property.property_name as q, 'pn' as tag
                    FROM market.property
                    INNER JOIN market.property_status ON market.property_status.property_id = market.property.property_id
                    WHERE market.property_status.end_time IS NULL
                    AND market.property_status.status_id = 1
                    ) z";
            List<object> searches = (List<object>)QueryModule.DataSource<object>(sql);


            List<object> listObj = new List<object>();
            listObj.Add(list);
            listObj.Add(form);
            listObj.Add(searches);


            return listObj;
        }

        public bool Insert(PropertyModel model)
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

                    model.property_id = Guid.NewGuid().ToString();
                    model.property_type_id = 4;
                    string sql = "insert into market.property (" + ObjectSqlMapping.MapInsert<PropertyModel>() + ")";
                    int res = (int)QueryModule.Execute<int>(sql, model);

                    model.property_amount.property_id = model.property_id;
                    sql = "insert into market.property_amount (" + ObjectSqlMapping.MapInsert<PropertyAmount>() + ")";
                    QueryModule.Execute<int>(sql, model.property_amount);

                    model.property_status = new PropertyStatus();
                    model.property_status.property_id = model.property_id;
                    model.property_status.status_id = 1;
                    sql = "insert into market.property_status (" + ObjectSqlMapping.MapInsert<PropertyStatus>() + ")";
                    QueryModule.Execute<int>(sql, model.property_status);

                    if (model.property_floor.Count > 0)
                    {
                        foreach (PropertyFloor obj in model.property_floor) //FLOOR
                        {
                            obj.property_id = Guid.NewGuid().ToString();
                            obj.property_bldg_id = model.property_id;
                            obj.parent_id = model.property_id;
                            obj.property_type_id = 5;
                            sql = "insert into market.property (" + ObjectSqlMapping.MapInsert<PropertyFloor>() + ")";
                            QueryModule.Execute<int>(sql, obj);

                            obj.property_amount.property_id = obj.property_id;
                            sql = "insert into market.property_amount (" + ObjectSqlMapping.MapInsert<PropertyAmount>() + ")";
                            QueryModule.Execute<int>(sql, obj.property_amount);

                            if(obj.property_section.Count > 0)
                            {
                                foreach(PropertySection propSec in obj.property_section)
                                {
                                    propSec.property_id = obj.property_id;
                                    sql = "insert into market.property_section (" + ObjectSqlMapping.MapInsert<PropertySection>() + ")";
                                    QueryModule.Execute<int>(sql, propSec);
                                }
                            }

                            if(obj.unit_stall.Count > 0)
                            {
                                foreach (UnitStall x in obj.unit_stall) //Unit Stall
                                {
                                    x.property_id = Guid.NewGuid().ToString();
                                    x.property_bldg_id = model.property_id;
                                    x.parent_id = obj.property_id;
                                    x.property_type_id = 1;
                                    sql = "insert into market.property (" + ObjectSqlMapping.MapInsert<UnitStall>() + ")";
                                    QueryModule.Execute<int>(sql, x);

                                    x.property_amount.property_id = x.property_id;
                                    sql = "insert into market.property_amount (" + ObjectSqlMapping.MapInsert<PropertyAmount>() + ")";
                                    QueryModule.Execute<int>(sql, x.property_amount);


                                    x.property_section.property_id = x.property_id;
                                    sql = "insert into market.property_section (" + ObjectSqlMapping.MapInsert<PropertySection>() + ")";
                                    QueryModule.Execute<int>(sql, x.property_section);
                                }
                            }
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
